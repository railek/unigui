using System;
using Railek.Unibase;
using Railek.Unigui.Animation;
using UnityEngine;

namespace Railek.Unigui
{
    [Serializable]
    public class UIButtonBehavior
    {
        private const ButtonAnimationType DefaultButtonAnimationType = ButtonAnimationType.Punch;
        private const bool DefaultDeselectButton = false;
        private const bool DefaultEnabled = false;
        private const bool DefaultReady = true;
        [SerializeField] private UIButtonBehaviorType behaviorType;

        public ButtonAnimationType buttonAnimationType;
        public bool deselectButton;
        public bool enabled;
        public bool triggerEventAfterAnimation;
        public VoidEvent onTrigger;
        public UIAnimation punchAnimation;
        public bool ready;
        public UIAnimation stateAnimation;

        public UIButtonBehavior(UIButtonBehaviorType behaviorType, bool enabled = false)
        {
            Reset(behaviorType);
            this.enabled = enabled;
        }

        public UIButtonBehaviorType BehaviorType => behaviorType;

        public float GetAnimationTotalDuration()
        {
            switch (buttonAnimationType)
            {
                case ButtonAnimationType.Punch: return punchAnimation.TotalDuration;
                case ButtonAnimationType.State: return stateAnimation.TotalDuration;
                default:                        return 0f;
            }
        }

        public void PlayAnimation(UIButton button)
        {
            switch (buttonAnimationType)
            {
                case ButtonAnimationType.Punch:
                {
                    if (punchAnimation == null)
                    {
                        return;
                    }

                    UIAnimator.StopAnimations(button.RectTransform, AnimationType.Punch);
                    if (punchAnimation.move.enabled)
                    {
                        button.ResetPosition();
                    }

                    if (punchAnimation.rotate.enabled)
                    {
                        button.ResetRotation();
                    }

                    if (punchAnimation.scale.enabled)
                    {
                        button.ResetScale();
                    }

                    UIAnimator.MovePunch(button.RectTransform, punchAnimation, button.startPosition);
                    UIAnimator.RotatePunch(button.RectTransform, punchAnimation, button.startRotation);
                    UIAnimator.ScalePunch(button.RectTransform, punchAnimation, button.startScale);
                    break;
                }
                case ButtonAnimationType.State:
                {
                    if (stateAnimation == null)
                    {
                        return;
                    }

                    UIAnimator.StopAnimations(button.RectTransform, AnimationType.State);
                    UIAnimator.MoveState(button.RectTransform, stateAnimation, button.startPosition);
                    UIAnimator.RotateState(button.RectTransform, stateAnimation, button.startRotation);
                    UIAnimator.ScaleState(button.RectTransform, stateAnimation, button.startScale);
                    UIAnimator.FadeState(button.RectTransform, stateAnimation, button.startAlpha);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Reset(UIButtonBehaviorType buttonBehaviorType)
        {
            behaviorType = buttonBehaviorType;
            enabled = DefaultEnabled;
            ready = DefaultReady;
            deselectButton = DefaultDeselectButton;
            buttonAnimationType = DefaultButtonAnimationType;
            punchAnimation = new UIAnimation(AnimationType.Punch);
            stateAnimation = new UIAnimation(AnimationType.State);
        }
    }
}
