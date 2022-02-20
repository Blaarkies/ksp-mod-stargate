using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stargate.Domain;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Stargate.Utilities
{
    /// <summary>
    /// Handles the connection and teleportation between 2 stargates.
    /// This entity is intended to live from the gate pre-dial sequence,
    /// up to:
    /// <br/>- The successful teleportation of the subject
    /// <br/>- Or cancellation of the wormhole
    /// </summary>
    public class Wormhole
    {
        private readonly Vessel _originGate;
        private readonly Vessel _destinationGate;
        private readonly Part _originGatePart;

        private readonly Dictionary<int, Action<Vector3>> _teleportMethodMap;
        private bool _isActive;
        private Vessel _subject;
        private Action _onCloseCallback;
        private Action _onTeleportedCallback;

        public readonly bool Valid;


        public Wormhole(Vessel originGate, StargateSelector stargateSelector)
        {
            _originGate = originGate;
            _originGatePart = originGate.parts.First(p => p.Modules.Contains(nameof(ModulePortal)));
            _destinationGate = FlightGlobals.FindVessel(stargateSelector.SelectedStargateId);
            if (!_destinationGate)
            {
                BlaarkiesLog.OnScreen($"No other gate selected");
                Valid = false;
                return;
            }

            Valid = true;

            _teleportMethodMap = new Dictionary<int, Action<Vector3>>
            {
                { 0, HandleToOrbitWormhole }, // orbit -> orbit
                { 1, HandleToOrbitWormhole }, // surface -> orbit
                { 2, HandleOrbitToSurfaceWormhole }, // orbit -> surface
                // { 2, HandleOrbitToSurfaceWormhole },
                // { 3, HandleSurfaceToSurfaceWormhole }, // surface -> surface
                { 3, HandleOrbitToSurfaceWormhole }, // surface -> surface
            };
        }

        public void TeleportSubject()
        {
            new SoundEffect(Sounds.WormholeStep, _subject.gameObject) { volume = GameSettings.SHIP_VOLUME * .3f }
                .Play();

            var gateLocation = _originGate.CoM;
            var relativeOffset = gateLocation - _subject.CoM;

            var methodBitmask = _subject.LandedOrSplashed.ToSign()
                                + 2 * _destinationGate.LandedOrSplashed.ToSign();

            _teleportMethodMap[methodBitmask].Invoke(relativeOffset);

            _onTeleportedCallback?.Invoke();
        }

        private void HandleToOrbitWormhole(Vector3 offset)
        {
            var newOrbit = Orbit.OrbitFromStateVectors(
                _destinationGate.CoM - offset,
                _destinationGate.obt_velocity.Clone(),
                _destinationGate.orbit.referenceBody,
                Planetarium.GetUniversalTime()
            );

            SetNewOrbit(newOrbit);
        }

        private void HandleOrbitToSurfaceWormhole(Vector3 offset)
        {
            _subject.Splashed = false;
            _subject.Landed = false;
            _subject.landedAt = null;

            _subject.SetPosition(_destinationGate.CoM - offset);
            _subject.SetWorldVelocity(_destinationGate.obt_velocity - _subject.srf_velocity);

            // TODO: fix parts-jitter after teleport
            // TODO: fix destinationGate bumping up after teleport
            // TODO: fix aero effects 1 frame during teleportation
            _subject.UpdatePosVel();
            _subject.orbitDriver.UpdateOrbit();
        }

        private void SetNewOrbit(Orbit newOrbit)
        {
            _subject.Splashed = false;
            _subject.Landed = false;
            _subject.landedAt = null;

            try
            {
                OrbitPhysicsManager.HoldVesselUnpack(2);
            }
            catch (NullReferenceException)
            {
                Debug.Log("Could not hold vessel unpacked");
                BlaarkiesLog.OnScreen("Wormhole malfunction");
                return;
            }

            FlightGlobals.Vessels.ForEach(v => v.GoOnRails());

            _subject.orbitDriver.orbit.UpdateFromOrbitAtUT(
                newOrbit,
                Planetarium.GetUniversalTime(),
                newOrbit.referenceBody);
        }

        public void Update()
        {
            if (!_isActive)
            {
                return;
            }

            var gateLocation = _originGatePart.CoMOffset + _originGate.CoM;
            var gateActiveSize = _originGatePart.prefabSize.magnitude * .7f;

            _subject = FlightGlobals.FindNearestVesselWhere(gateLocation,
                    v => v != _originGate && Vector3.Distance(v.CoM, gateLocation) < gateActiveSize)
                .FirstOrDefault();

            if (_subject != null)
            {
                TeleportSubject();
                Close();
                BlaarkiesLog.OnScreen($"Teleported subject");
            }
        }

        public void Open(Action onCloseCallback, Action onTeleportedCallback)
        {
            if (_isActive)
            {
                return;
            }

            _isActive = true;
            _onCloseCallback = onCloseCallback;
            _onTeleportedCallback = onTeleportedCallback;
        }

        public void Close()
        {
            if (!_isActive)
            {
                return;
            }

            _isActive = false;

            _destinationGate.Load();
            var stargatePart = _destinationGate.parts.First(part => part.Modules.Contains(nameof(ModulePortal)));
            var animatorEventHorizon = new BlaarkiesAnimator(
                stargatePart,
                Animations.EventHorizonAction,
                1, 3f);
            animatorEventHorizon.Play(0f, 1f);

            var timeToKeepWormholeOpen = Random.Range(3, 10);
            stargatePart.gameObject.AddComponent<Waiter>()
                .DoAction(() => new SoundEffect(Sounds.GateClose, stargatePart.gameObject)
                            { volume = GameSettings.SHIP_VOLUME * .5f }
                        .Play(),
                    timeToKeepWormholeOpen);
            stargatePart.gameObject.AddComponent<Waiter>()
                .DoAction(() =>
                    {
                        _onCloseCallback?.Invoke();
                        animatorEventHorizon.PlayReverse();
                    },
                    timeToKeepWormholeOpen + 1.3f);
        }
    }
}
