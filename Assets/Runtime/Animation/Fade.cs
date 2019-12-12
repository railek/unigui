using System;
using DG.Tweening;
using UnityEngine;

namespace Railek.Unigui.Animation
{
    [Serializable]
    public class Fade
    {
        public AnimationType animationType;
        public bool enabled;
        public float from;
        public float to;
        public float by;
        public bool useCustomFromAndTo;
        public int numberOfLoops;
        public LoopType loopType;
        public EaseType easeType;
        public Ease ease;
        public AnimationCurve animationCurve;
        public float startDelay;
        public float duration;

        public Fade(AnimationType animationType)
        {
            Reset(animationType);
        }

        public Fade(AnimationType animationType,
            bool enabled,
            float from, float to, float by,
            bool useCustomFromAndTo,
            int numberOfLoops, LoopType loopType,
            EaseType easeType, Ease ease, AnimationCurve animationCurve,
            float startDelay, float duration) : this(animationType)
        {
            this.animationType = animationType;
            this.enabled = enabled;
            this.from = from;
            this.to = to;
            this.by = by;
            this.useCustomFromAndTo = useCustomFromAndTo;
            this.numberOfLoops = numberOfLoops;
            this.loopType = loopType;
            this.easeType = easeType;
            this.ease = ease;
            this.animationCurve = new AnimationCurve(animationCurve.keys);
            this.startDelay = startDelay;
            this.duration = duration;
        }

        public float TotalDuration => startDelay + duration;

        private void Reset(AnimationType uiAnimationType)
        {
            animationType = uiAnimationType;
            enabled = UIAnimator.DefaultAnimationEnabledState;
            from = 0f;
            to = 0f;
            by = 0.5f;
            useCustomFromAndTo = false;
            numberOfLoops = UIAnimator.DefaultNumberOfLoops;
            loopType = UIAnimator.DefaultLoopType;
            easeType = UIAnimator.DefaultEaseType;
            ease = UIAnimator.DefaultEase;
            animationCurve = new AnimationCurve();
            startDelay = UIAnimator.DefaultStartDelay;
            duration = UIAnimator.DefaultDuration;
        }
    }
}
