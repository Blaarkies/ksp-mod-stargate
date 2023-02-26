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

        private readonly Dictionary<int, Action<Vector3, Vessel>> _teleportMethodMap;
        private bool _isActive;
        private Action _onCloseCallback;
        private Action _onTeleportedCallback;
        private Func<double, bool> _onTransferOfMassCallback;

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

            _teleportMethodMap = new Dictionary<int, Action<Vector3, Vessel>>
            {
                { 0b00, HandleToOrbitWormhole }, // orbit -> orbit
                { 0b01, HandleToOrbitWormhole }, // surface -> orbit
                { 0b10, HandleOrbitToSurfaceWormhole }, // orbit -> surface
                // { 2, HandleOrbitToSurfaceWormhole },
                // { 3, HandleSurfaceToSurfaceWormhole }, // surface -> surface
                { 0b11, HandleOrbitToSurfaceWormhole }, // surface -> surface
            };
        }

        private void TeleportSubject(Vessel subject)
        {
            var hasSuppliedTransferCost = _onTransferOfMassCallback(subject.totalMass);
            if (!hasSuppliedTransferCost)
            {
                new SoundEffect(Sounds.GateDialFail, _originGatePart.gameObject)
                        { volume = GameSettings.SHIP_VOLUME * .5f }
                    .Play();
                BlaarkiesLog.OnScreen($"Not enough Naquadah to handle {subject.name}'s mass");
                return;
            }

            var distance = Vector3.Distance(_originGate.CoM, _destinationGate.CoM);
            var ratioToPlanet = distance / _originGate.mainBody.Radius;
            BlaarkiesLog.DebugThrottled($"Teleporting {ratioToPlanet.Round(2)}-way around {_originGate.mainBody.name}",
                "distanceToTravel");

            var soundWormholeStep = new SoundEffect(Sounds.WormholeStep, subject.gameObject)
                { volume = GameSettings.SHIP_VOLUME * .3f };
            soundWormholeStep .Play();

            var gateLocation = _originGate.CoM;
            var relativeOffset = gateLocation - subject.CoM;

            var methodBitmask = subject.LandedOrSplashed.ToBinary()
                                + 2 * _destinationGate.LandedOrSplashed.ToBinary();

            _teleportMethodMap[methodBitmask].Invoke(relativeOffset, subject);

            _onTeleportedCallback?.Invoke();
        }

        private void HandleToOrbitWormhole(Vector3 offset, Vessel subject)
        {
            var newOrbit = Orbit.OrbitFromStateVectors(
                _destinationGate.CoM - offset,
                _destinationGate.obt_velocity.Clone(),
                _destinationGate.orbit.referenceBody,
                Planetarium.GetUniversalTime()
            );

            SetNewOrbit(newOrbit, subject);
        }

        private void HandleOrbitToSurfaceWormhole(Vector3 offsetOrigin, Vessel subject)
        {
            subject.Splashed = false;
            subject.Landed = false;
            subject.landedAt = null;

            var originGateLocation = _originGatePart.CoMOffset + _originGate.CoM;

            var originGateRotation = (_originGatePart.orgRot * _originGate.srfRelRotation).eulerAngles.normalized;
            var relativeLocation = originGateLocation - subject.CoM;
            var originOffsetLocation = originGateRotation - relativeLocation;

            var destinationGateRotation = (/*_destinationGate.parts.findStargatePart.orgRot **/
                _destinationGate.srfRelRotation).eulerAngles.normalized;
            var destinationOffsetLocation = destinationGateRotation + originOffsetLocation;
            var destinationGateLocation = _destinationGate.CoM;

            var originGateVelocity = _originGate.velocityD;

            var relativeVelocity = originGateVelocity - subject.velocityD;
            var originOffsetVelocityDirection = originGateRotation - relativeVelocity.normalized;

            var destinationGateVelocity = _destinationGate.velocityD;
            var destinationOffsetVelocityDirection = destinationGateRotation + originOffsetVelocityDirection;
            var reflectedVelocityDirection =
                Vector3d.Reflect(destinationOffsetVelocityDirection, destinationGateRotation);
            var destinationOffsetVelocity = reflectedVelocityDirection * relativeVelocity.magnitude;

            subject.SetPosition(destinationGateLocation + destinationOffsetLocation);
            subject.SetWorldVelocity(destinationGateVelocity + destinationOffsetVelocity);

            // TODO: fix parts-jitter after teleport
            // TODO: fix destinationGate bumping up after teleport
            // TODO: fix aero effects 1 frame during teleportation
            subject.UpdatePosVel();
            subject.orbitDriver.UpdateOrbit();
        }

        private void SetNewOrbit(Orbit newOrbit, Vessel subject)
        {
            subject.Splashed = false;
            subject.Landed = false;
            subject.landedAt = null;

            try
            {
                OrbitPhysicsManager.HoldVesselUnpack(2); // TODO: what effect does releaseAfter have?
            }
            catch (NullReferenceException)
            {
                BlaarkiesLog.DebugThrottled("Could not hold vessel unpacked");
                BlaarkiesLog.OnScreen("Wormhole malfunctioned");
                return;
            }

            FlightGlobals.Vessels.ForEach(v => v.GoOnRails());

            subject.orbitDriver.orbit.UpdateFromOrbitAtUT(
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

            TryToTeleport();
        }

        public void TryToTeleport()
        {
            var subject = GetSubjectToBeTeleported();
            if (subject != null)
            {
                TeleportSubject(subject);
                Close();
                BlaarkiesLog.OnScreen($"Teleported subject");
            }
        }

        private Vessel GetSubjectToBeTeleported()
        {
            var gateQuaternion = _originGatePart.orgRot * _originGate.srfRelRotation;
            var gateVector = gateQuaternion.eulerAngles.normalized;

            var gateLocation = _originGatePart.CoMOffset + _originGate.CoM;
            var plane = new Plane(gateVector, gateLocation);

            // prefabSize is diameter, *.5 to radius, *.75 to event horizon size
            var eventHorizonRadius = _originGatePart.prefabSize.magnitude * .5f * .75f;

            return FlightGlobals.FindNearestVesselWhere(gateLocation,
                    v => v != _originGate
                         && plane.GetDistanceToPoint(v.CoM).Absolute() < .1f
                         && Vector3.Distance(gateLocation, v.CoM) < eventHorizonRadius)
                .FirstOrDefault();
        }

        public void Open(Action onCloseCallback,
            Action onTeleportedCallback,
            Func<double, bool> onTransferOfMassCallback)
        {
            if (_isActive)
            {
                return;
            }

            _isActive = true;
            _onCloseCallback = onCloseCallback;
            _onTeleportedCallback = onTeleportedCallback;
            _onTransferOfMassCallback = onTransferOfMassCallback;
        }

        public void Close()
        {
            _isActive = false;

            _destinationGate.Load();
            var stargatePart = _destinationGate.parts.First(part => part.Modules.Contains(nameof(ModulePortal)));
            var animatorEventHorizon = new BlaarkiesAnimator(stargatePart, Animations.EventHorizon, 3f);
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
