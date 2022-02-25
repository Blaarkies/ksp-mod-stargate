using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stargate.Utilities
{
    public class BlaarkiesAnimator
    {
        private readonly GameObject _gameObject;
        private readonly string _name;
        private readonly int _layer;
        private readonly float _speed;

        private float _lastPlayHeadPosition = 0f;
        private readonly List<AnimationContainer> _animators;

        public BlaarkiesAnimator(
            Part part,
            string name,
            int layer,
            float speed,
            WrapMode wrapMode = WrapMode.ClampForever)
        {
            _gameObject = part.gameObject;
            _name = name;
            _layer = layer;
            _speed = speed;


            var animators = part.FindModelAnimators(name);
            _animators = GetAnimations(animators, name).ToList();
            _animators.ForEach(animator =>
            {
                var state = animator.State;
                state.enabled = true;
                state.layer = _layer;
                state.wrapMode = wrapMode;
            });
        }

        private IEnumerable<AnimationContainer> GetAnimations(IEnumerable<Animation> animations, string name)
        {
            return animations
                .Select(a => new AnimationContainer
                {
                    Animation = a,
                    State = a[name],
                });
        }

        public void Play(float speedFactor = 1f, float playHead = 0f, float delay = 0f)
        {
            var playCallback = new Action(() =>
            {
                _animators.ForEach(animator =>
                {
                    var state = animator.State;
                    state.speed = _speed * speedFactor;
                    state.normalizedTime = playHead;
                    animator.Animation.Play(_name);
                });
            });

            if (delay == 0f)
            {
                playCallback();
                return;
            }

            _gameObject.AddComponent<Waiter>()
                .DoAction(() => playCallback(), delay);
        }

        public void PlayReverse()
        {
            Play(-1f, 1f);
        }


        public void Continue(float speedFactor = 1f)
        {
            _animators.ForEach(animator =>
            {
                var state = animator.State;
                state.speed = _speed * speedFactor;
                state.normalizedTime = _lastPlayHeadPosition;
                animator.Animation.Play(_name);
            });
        }

        public void ContinueReverse()
        {
            Continue(-1f);
        }

        public void Pause()
        {
            _animators.ForEach(animator =>
            {
                _lastPlayHeadPosition = animator.State.normalizedTime;
                animator.Animation.Stop(_name);
            });
        }

        public void Stop()
        {
            _animators.ForEach(animator =>
            {
                animator.Animation.Stop(_name);
                animator.Animation.Rewind(_name);
            });
        }
    }
}
