using System;
using Railek.Unigui.Animation;
using UnityEngine;

namespace Railek.Unigui
{
    [Serializable]
    public class UIButtonLoopAnimation
    {
        public UIAnimation animation;
        public bool enabled;
        public bool isPlaying;

        public UIButtonLoopAnimation()
        {
            Reset();
        }

        private void Reset()
        {
            animation = new UIAnimation(AnimationType.Loop);
            isPlaying = false;
        }

        public void Start(RectTransform target, Vector3 startPosition, Vector3 startRotation)
        {
            if (!enabled)
            {
                return;
            }

            if (animation == null)
            {
                return;
            }

            if (isPlaying)
            {
                return;
            }

            UIAnimator.MoveLoop(target, animation, startPosition);
            UIAnimator.RotateLoop(target, animation, startRotation);
            UIAnimator.ScaleLoop(target, animation);
            UIAnimator.FadeLoop(target, animation);
            isPlaying = true;
        }

        public void Stop(RectTransform target)
        {
            if (animation == null)
            {
                return;
            }

            if (!isPlaying)
            {
                return;
            }

            UIAnimator.StopAnimations(target, AnimationType.Loop);
            isPlaying = false;
        }
    }
}
