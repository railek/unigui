using System;
using DG.Tweening;
using UnityEngine;

namespace Railek.Unigui.Animation
{
    [Serializable]
    public class Move
    {
        public AnimationType animationType;
        public bool enabled;
        public Vector3 from;
        public Vector3 to;
        public Vector3 by;
        public bool useCustomFromAndTo;
        public int vibrato;
        public float elasticity;
        public int numberOfLoops;
        public LoopType loopType;
        public Direction direction;
        public Vector3 customPosition;
        public EaseType easeType;
        public Ease ease;
        public AnimationCurve animationCurve;
        public float startDelay;
        public float duration;

        public Move(AnimationType animationType)
        {
            Reset(animationType);
        }

        public Move(AnimationType animationType,
            bool enabled,
            Vector3 from, Vector3 to, Vector3 by,
            bool useCustomFromAndTo,
            int vibrato, float elasticity,
            int numberOfLoops, LoopType loopType,
            Direction direction,
            Vector3 customPosition,
            EaseType easeType, Ease ease, AnimationCurve animationCurve,
            float startDelay, float duration) : this(animationType)
        {
            this.animationType = animationType;
            this.enabled = enabled;
            this.from = from;
            this.to = to;
            this.by = by;
            this.useCustomFromAndTo = useCustomFromAndTo;
            this.vibrato = vibrato;
            this.elasticity = elasticity;
            this.numberOfLoops = numberOfLoops;
            this.loopType = loopType;
            this.direction = direction;
            this.customPosition = customPosition;
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
            from = Vector3.zero;
            to = Vector3.zero;
            by = Vector3.zero;
            useCustomFromAndTo = false;
            vibrato = UIAnimator.DefaultVibrato;
            elasticity = UIAnimator.DefaultElasticity;
            numberOfLoops = UIAnimator.DefaultNumberOfLoops;
            loopType = UIAnimator.DefaultLoopType;
            direction = UIAnimator.DefaultDirection;
            customPosition = Vector3.zero;
            easeType = UIAnimator.DefaultEaseType;
            ease = UIAnimator.DefaultEase;
            animationCurve = new AnimationCurve();
            startDelay = UIAnimator.DefaultStartDelay;
            duration = UIAnimator.DefaultDuration;
        }
    }
}
