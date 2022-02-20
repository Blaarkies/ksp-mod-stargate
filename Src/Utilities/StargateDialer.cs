using System;
using Stargate.Domain;
using UnityEngine;

namespace Stargate.Utilities
{
    public class StargateDialer
    {
        private readonly Part _stargate;
        private bool _isDialing;
        private readonly TickerWatch _watch = new TickerWatch();
        private Wormhole _wormhole;

        public StargateDialer(Part stargate)
        {
            _stargate = stargate;
        }

        public void StartDialingSequence(Wormhole wormhole, Action<Action> onComplete)
        {
            _wormhole = wormhole;
            _isDialing = true;

            var animatorEventHorizon = new BlaarkiesAnimator(_stargate, Animations.EventHorizonAction,
                1, 20f);

            var animatorKawoosh = new BlaarkiesAnimator(_stargate, Animations.KawooshAction,
                2, 1.5f);

            var animatorChevronEngageBottom = new BlaarkiesAnimator(_stargate, Animations.ChevronBottomEngageAction,
                3, 3f);

            var animatorChevronEngageTop = new BlaarkiesAnimator(_stargate, Animations.ChevronTopEngageAction,
                4, 3f);

            var animatorRingSpin = new BlaarkiesAnimator(_stargate, Animations.RingSpinAction,
                4, .6f, WrapMode.Loop);

            var gameObject = _stargate.gameObject;
            var soundChevron = new SoundEffect(Sounds.Chevron, gameObject){volume = GameSettings.SHIP_VOLUME *.8f};
            var soundRoll = new SoundEffect(Sounds.GateRollLong, gameObject){volume = GameSettings.SHIP_VOLUME *.5f};
            var soundChevronLock = new SoundEffect(Sounds.ChevronLock, gameObject){volume = GameSettings.SHIP_VOLUME *.8f};
            var soundGateOpen = new SoundEffect(Sounds.GateOpen, gameObject){volume = GameSettings.SHIP_VOLUME *.8f};
            var soundWormholeLoop = new SoundEffect(Sounds.WormholeEventhorizonLoop, gameObject){isLoop = true};

            _watch.DoAt(0, () =>
            {
                animatorRingSpin.Play();
                soundRoll.Play();
            });
            _watch.DoAt(2, () =>
            {
                animatorRingSpin.Pause();
                animatorChevronEngageBottom.Play();
                animatorChevronEngageTop.Play();
                soundChevron.Play();
            });

            _watch.DoAt(3, () =>
            {
                animatorRingSpin.ContinueReverse();
                soundRoll.Play();
            });
            _watch.DoAt(5.5f, () =>
            {
                animatorRingSpin.Pause();
                animatorChevronEngageBottom.Play();
                animatorChevronEngageTop.Play();
                soundChevron.Play();
            });
            _watch.DoAt(6.5f, () =>
            {
                animatorRingSpin.Continue();
                soundRoll.Play();
            });
            _watch.DoAt(8, () =>
            {
                animatorRingSpin.Pause();
                soundRoll.Stop();
                animatorChevronEngageBottom.Play();
                animatorChevronEngageTop.Play();
                soundChevronLock.Play();
                soundGateOpen.Play(2f); // TODO: check delay
            });
            _watch.DoAt(9, () =>
            {
                animatorEventHorizon.Play();
                animatorKawoosh.Play();
            });
            _watch.DoAt(9.5f, () =>
            {
                soundWormholeLoop.Play();
                // TODO: stop loop sound when gate closes

                KillPartsInsideVortex();
                CreateEventHorizon();

                onComplete?.Invoke(() => soundWormholeLoop.Stop());
                // _isDialing =  false;
                _watch.Stop();
            });

            _watch.Start();
        }

        public void Update()
        {
            if (!_isDialing)
            {
                return;
            }

            _watch.Tick();

            _wormhole.Update();
        }

        private void KillPartsInsideVortex()
        {
            // TODO: find vessels/parts in vortex area/volume. Call .explode() on them
        }

        private void CreateEventHorizon()
        {
            // _stargate.explode();
        }
    }
}
