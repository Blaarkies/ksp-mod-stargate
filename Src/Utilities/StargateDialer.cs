using System;
using System.Collections.Generic;
using Stargate.Domain;
using UniLinq;
using UnityEngine;
using Random = UnityEngine.Random;

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

            var animatorEventHorizon = new BlaarkiesAnimator(_stargate, Animations.EventHorizon, 20f);
            var animatorKawoosh = new BlaarkiesAnimator(_stargate, Animations.Kawoosh, 1.5f);
            var animatorRingSpin = new BlaarkiesAnimator(_stargate, Animations.RingSpin, .4f, WrapMode.Loop);

            var animatorChevronOriginPick = new BlaarkiesAnimator(_stargate, Animations.ChevronOriginPick, 3f);
            var animatorChevronOriginBlock = new BlaarkiesAnimator(_stargate, Animations.ChevronOriginBlock, 3f);
            var animatorChevronOriginBlockLight =
                new BlaarkiesAnimator(_stargate, Animations.ChevronOriginBlockLight, 100f);
            var animatorChevronOriginPickLight =
                new BlaarkiesAnimator(_stargate, Animations.ChevronOriginPickLight, 100f);

            void AnimateOriginChevron(bool noReset = false)
            {
                animatorChevronOriginPick.Play();
                animatorChevronOriginBlock.Play();

                animatorChevronOriginBlockLight.Play();
                animatorChevronOriginPickLight.Play();

                if (noReset)
                {
                    return;
                }

                animatorChevronOriginBlockLight.Stop(.9f);
                animatorChevronOriginPickLight.Stop(.9f);
            }

            var animatorsChevronLights = new[]
                {
                    Animations.ChevronLight1,
                    Animations.ChevronLight2,
                    Animations.ChevronLight3,
                    Animations.ChevronLight4,
                    Animations.ChevronLight5,
                    Animations.ChevronLight6,
                }
                .Select(animationName => new BlaarkiesAnimator(_stargate, animationName, 100f))
                .ToList();

            var textureTransformName = ""
            Transform target = _stargate.FindModelTransform(textureTransformName);
            var rendererMaterial = target.GetComponent<Renderer>();
            rendererMaterial.material.SetColor("_EmissiveColor", new Color(1f, 1f, 1f, 1f));

            var gameObject = _stargate.gameObject;
            var volume = GameSettings.SHIP_VOLUME;
            var soundChevron = new SoundEffect(Sounds.Chevron, gameObject) { volume = volume * .8f };
            var soundRoll = new SoundEffect(Sounds.GateRollLong, gameObject) { volume = volume * .5f };
            var soundChevronLock = new SoundEffect(Sounds.ChevronLock, gameObject) { volume = volume * .8f };
            var soundGateOpen = new SoundEffect(Sounds.GateOpen, gameObject) { volume = volume * .8f };
            var soundWormholeLoop = new SoundEffect(Sounds.EventhorizonLoop, gameObject) { volume = volume * .6f };

            soundRoll.Play();
            animatorRingSpin.Play();

            Enumerable.Range(1, 6)
                .Select(t => new
                {
                    time = t * 5f + Random.Range(0f, 2f),
                    index = t - 1
                })
                .ToList()
                .ForEach(step =>
                {
                    _watch.DoAt(step.time, () =>
                    {
                        soundRoll.Stop();
                        soundChevron.Play();
                        animatorRingSpin.Pause();
                        AnimateOriginChevron();

                        animatorsChevronLights[step.index].Play();
                    });

                    _watch.DoAt(step.time + 1.5f, () =>
                    {
                        soundRoll.Play();
                        if (step.index % 2 != 0)
                            animatorRingSpin.Continue();
                        else
                            animatorRingSpin.ContinueReverse();
                    });
                });

            _watch.DoAt(37, () =>
            {
                soundRoll.Stop();
                soundChevronLock.Play();
                animatorRingSpin.Pause();
                AnimateOriginChevron(true);
                soundGateOpen.Play();
            });

            _watch.DoAt(38, () =>
            {
                animatorEventHorizon.Play();
                animatorKawoosh.Play();
            });

            _watch.DoAt(39f, () =>
            {
                soundWormholeLoop.Play();

                KillPartsInsideVortex();
                CreateEventHorizon();

                onComplete?.Invoke(() =>
                {
                    soundWormholeLoop.Stop();
                    _isDialing = false;
                });
                _watch.Stop();
            });

            _watch.Start();
        }

        public void StartDhdSequence(Wormhole wormhole, Action<Action> onComplete)
        {
            _wormhole = wormhole;
            _isDialing = true;

            var animatorEventHorizon = new BlaarkiesAnimator(_stargate, Animations.EventHorizon, 20f);
            var animatorKawoosh = new BlaarkiesAnimator(_stargate, Animations.Kawoosh, 1.5f);
            var animatorChevronOriginBlockLight =
                new BlaarkiesAnimator(_stargate, Animations.ChevronOriginBlockLight, 100f);
            var animatorChevronOriginPickLight =
                new BlaarkiesAnimator(_stargate, Animations.ChevronOriginPickLight, 100f);
            var animatorsChevronLights = new[]
                {
                    Animations.ChevronLight1,
                    Animations.ChevronLight2,
                    Animations.ChevronLight3,
                    Animations.ChevronLight4,
                    Animations.ChevronLight5,
                    Animations.ChevronLight6,
                }
                .Select(animationName => new BlaarkiesAnimator(_stargate, animationName, 100f))
                .ToList();

            var gameObject = _stargate.gameObject;
            var volume = GameSettings.SHIP_VOLUME;
            var soundRollStart = new SoundEffect(Sounds.GateRollShort, gameObject) { volume = volume * .2f };
            var soundRollLoop = new SoundEffect(Sounds.GateRollLoop, gameObject) { volume = volume * .9f };
            var soundGateOpen = new SoundEffect(Sounds.GateOpen, gameObject) { volume = volume * .8f };
            var soundWormholeLoop = new SoundEffect(Sounds.EventhorizonLoop, gameObject) { volume = volume * .6f };

            // Use multiple instance of the same sound group to allow overlap
            var soundInstancesDhd = Enumerable.Range(1, 2)
                .Select(_ => new SoundEffect(Sounds.Dhd, gameObject) { volume = volume * .2f })
                .ToList();
            var dhdInstanceCycler = new Cycler(1);

            _watch.DoAt(0, () =>
            {
                soundRollStart.Play();
                soundRollLoop.Play(1f);
            });

            Enumerable.Range(0, 5)
                .Select(t => new { time = .5f * t + Random.Range(0, .3f), index = t })
                .ToList()
                .ForEach(step => _watch.DoAt(step.time, () =>
                {
                    soundInstancesDhd[dhdInstanceCycler.Index].Play();
                    dhdInstanceCycler.Next();
                    animatorsChevronLights[step.index].Play();
                }));

            _watch.DoAt(3f, () =>
            {
                animatorChevronOriginBlockLight.Play();
                animatorChevronOriginPickLight.Play();
                soundInstancesDhd[dhdInstanceCycler.Index].Play();
                soundGateOpen.Play();
                soundRollStart.Stop();
                soundRollLoop.Stop();
            });
            _watch.DoAt(3.5f, () =>
            {
                animatorEventHorizon.Play();
                animatorKawoosh.Play();
            });
            _watch.DoAt(3.5f, () =>
            {
                soundWormholeLoop.Play();

                KillPartsInsideVortex();
                CreateEventHorizon();

                onComplete?.Invoke(() =>
                {
                    soundWormholeLoop.Stop();
                    _isDialing = false;
                });
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
