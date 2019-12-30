using System;
using DG.DOTweenEditor;
using Railek.Unigui.Animation;
using UnityEngine;

namespace Railek.Unigui.Editor
{
    public static class EditorAnimator
    {
        private static DelayedCall _delayedCall;
        private static Vector3 _startPosition;
        private static Vector3 _startRotation;
        private static Vector3 _startScale;
        private static float _startAlpha;

        public static bool PreviewIsPlaying { get; private set; }

        private static void StopAllAnimations(RectTransform target)
        {
            foreach (AnimationType value in Enum.GetValues(typeof(AnimationType)))
            {
                UIAnimator.StopAnimations(target, value);
            }
        }

        public static void PreviewViewAnimation(UIView view, UIAnimation animation)
        {
            if (PreviewIsPlaying)
            {
                return;
            }

            _delayedCall?.Cancel();
            view.UpdateStartValues();
            StopViewPreview(view);
            StopAllAnimations(view.RectTransform);

            var moveFrom = UIAnimator.GetAnimationMoveFrom(view.RectTransform, animation, view.CurrentStartPosition);
            var moveTo = UIAnimator.GetAnimationMoveTo(view.RectTransform, animation, view.CurrentStartPosition);
            if (!animation.move.enabled)
            {
                view.ResetPosition();
            }
            else
            {
                PreviewMove(view.RectTransform, moveFrom, moveTo, animation, view.CurrentStartPosition);
            }

            var rotateFrom = UIAnimator.GetAnimationRotateFrom(animation, view.startRotation);
            var rotateTo = UIAnimator.GetAnimationRotateTo(animation, view.startRotation);
            if (!animation.rotate.enabled)
            {
                view.ResetRotation();
            }
            else
            {
                PreviewRotate(view.RectTransform, rotateFrom, rotateTo, animation, view.startRotation);
            }

            var scaleFrom = UIAnimator.GetAnimationScaleFrom(animation, view.startScale);
            var scaleTo = UIAnimator.GetAnimationScaleTo(animation, view.startScale);
            if (!animation.scale.enabled)
            {
                view.ResetScale();
            }
            else
            {
                PreviewScale(view.RectTransform, scaleFrom, scaleTo, animation, view.startScale);
            }

            var fadeFrom = UIAnimator.GetAnimationFadeFrom(animation, view.startAlpha);
            var fadeTo = UIAnimator.GetAnimationFadeTo(animation, view.startAlpha);
            if (!animation.fade.enabled)
            {
                view.ResetAlpha();
            }
            else
            {
                PreviewFade(view.RectTransform, fadeFrom, fadeTo, animation, view.startAlpha);
            }

            DOTweenEditorPreview.Start();
            PreviewIsPlaying = true;

            _delayedCall = new DelayedCall(
                animation.animationType == AnimationType.Loop
                    ? 5f
                    : animation.TotalDuration + (animation.animationType == AnimationType.Hide ? 0.5f : 0f), () =>
                {
                    StopViewPreview(view);
                    _delayedCall = null;
                });
        }

        private static void PreviewMove(RectTransform target, Vector3 from, Vector3 to, UIAnimation animation,
            Vector3 startPosition)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show:
                case AnimationType.Hide:
                {
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.MoveTween(target, animation, from, to),
                        true, true, false);
                    break;
                }
                case AnimationType.Loop:
                {
                    target.anchoredPosition3D = UIAnimator.MoveLoopPositionA(animation, startPosition);
                    DOTweenEditorPreview.PrepareTweenForPreview(
                        UIAnimator.MoveLoopTween(target, animation, startPosition),
                        true, true, false);
                    break;
                }
                case AnimationType.Punch:
                {
                    target.anchoredPosition3D = startPosition;
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.MovePunchTween(target, animation),
                        true, true, false);
                    break;
                }
                case AnimationType.State:
                {
                    DOTweenEditorPreview.PrepareTweenForPreview(
                        UIAnimator.MoveStateTween(target, animation, startPosition),
                        true, true, false);
                    break;
                }
                case AnimationType.Undefined:
                {
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void PreviewRotate(RectTransform target, Vector3 from, Vector3 to, UIAnimation animation,
            Vector3 startRotation)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show:
                case AnimationType.Hide:
                {
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.RotateTween(target, animation, from, to),
                        true, true, false);
                    break;
                }
                case AnimationType.Loop:
                {
                    target.localRotation = Quaternion.Euler(UIAnimator.RotateLoopRotationA(animation, startRotation));
                    DOTweenEditorPreview.PrepareTweenForPreview(
                        UIAnimator.RotateLoopTween(target, animation, startRotation),
                        true, true, false);
                    break;
                }
                case AnimationType.Punch:
                {
                    target.localRotation = Quaternion.Euler(startRotation);
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.RotatePunchTween(target, animation),
                        true, true, false);
                    break;
                }
                case AnimationType.State:
                {
                    DOTweenEditorPreview.PrepareTweenForPreview(
                        UIAnimator.RotateStateTween(target, animation, startRotation),
                        true, true, false);
                    break;
                }
                case AnimationType.Undefined:
                {
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void PreviewScale(RectTransform target, Vector3 from, Vector3 to, UIAnimation animation,
            Vector3 startScale)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show:
                case AnimationType.Hide:
                {
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.ScaleTween(target, animation, from, to),
                        true, true, false);
                    break;
                }
                case AnimationType.Loop:
                {
                    target.localScale = animation.scale.from;
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.ScaleLoopTween(target, animation),
                        true, true, false);
                    break;
                }
                case AnimationType.Punch:
                {
                    target.localScale = startScale;
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.ScalePunchTween(target, animation),
                        true, true, false);
                    break;
                }
                case AnimationType.State:
                {
                    DOTweenEditorPreview.PrepareTweenForPreview(
                        UIAnimator.ScaleStateTween(target, animation, startScale),
                        true, true, false);
                    break;
                }
                case AnimationType.Undefined:
                {
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void PreviewFade(RectTransform target, float from, float to, UIAnimation animation,
            float startAlpha)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show:
                case AnimationType.Hide:
                {
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.FadeTween(target, animation, from, to),
                        true, true, false);
                    break;
                }
                case AnimationType.Loop:
                {
                    var canvasGroup = target.GetComponent<CanvasGroup>() != null
                        ? target.GetComponent<CanvasGroup>()
                        : target.gameObject.AddComponent<CanvasGroup>();
                    animation.fade.from = Mathf.Clamp01(animation.fade.from);
                    canvasGroup.alpha = animation.fade.from;
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.FadeLoopTween(target, animation),
                        true, true, false);
                    break;
                }
                case AnimationType.State:
                {
                    DOTweenEditorPreview.PrepareTweenForPreview(
                        UIAnimator.FadeStateTween(target, animation, startAlpha),
                        true, true, false);
                    break;
                }
                case AnimationType.Undefined:
                {
                    break;
                }
                case AnimationType.Punch:
                {
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static void StopViewPreview(UIView view)
        {
            DOTweenEditorPreview.Stop(true);
            view.ResetToStartValues();
            PreviewIsPlaying = false;
        }
    }
}
