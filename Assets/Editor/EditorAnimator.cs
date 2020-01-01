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

        public static void PreviewButtonAnimation(UIAnimation animation, RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            if (PreviewIsPlaying) return;
            _delayedCall?.Cancel();
            StopButtonPreview(rectTransform, canvasGroup);

            var target = rectTransform;
            _startPosition = target.anchoredPosition3D;
            _startRotation = target.localRotation.eulerAngles;
            _startScale = target.localScale;
            _startAlpha = canvasGroup.alpha;

            StopAllAnimations(target);

            var moveFrom = UIAnimator.GetAnimationMoveFrom(rectTransform, animation, _startPosition);
            var moveTo = UIAnimator.GetAnimationMoveTo(rectTransform, animation, _startPosition);
            if (!animation.move.enabled) target.anchoredPosition3D = _startPosition;
            else PreviewMove(rectTransform, moveFrom, moveTo, animation, _startPosition);

            var rotateFrom = UIAnimator.GetAnimationRotateFrom(animation, _startRotation);
            var rotateTo = UIAnimator.GetAnimationRotateTo(animation, _startRotation);
            if (!animation.rotate.enabled) target.localRotation = Quaternion.Euler(_startRotation);
            else PreviewRotate(rectTransform, rotateFrom, rotateTo, animation, _startRotation);

            var scaleFrom = UIAnimator.GetAnimationScaleFrom(animation, _startScale);
            var scaleTo = UIAnimator.GetAnimationScaleTo(animation, _startScale);
            if (!animation.scale.enabled) target.localScale = _startScale;
            else PreviewScale(rectTransform, scaleFrom, scaleTo, animation, _startScale);

            var fadeFrom = UIAnimator.GetAnimationFadeFrom(animation, _startAlpha);
            var fadeTo = UIAnimator.GetAnimationFadeTo(animation, _startAlpha);
            if (!animation.fade.enabled) canvasGroup.alpha = _startAlpha;
            else PreviewFade(rectTransform, fadeFrom, fadeTo, animation, _startAlpha);

            DOTweenEditorPreview.Start();
            PreviewIsPlaying = true;

            _delayedCall = new DelayedCall(
                animation.animationType == AnimationType.Loop
                    ? 5f
                    : animation.TotalDuration +
                      (animation.animationType == AnimationType.Hide || animation.animationType == AnimationType.State
                          ? 0.5f
                          : 0f), () =>
                          {
                              StopButtonPreview(rectTransform, canvasGroup);
                              _delayedCall = null;
                          });
        }

        private static void ResetButtonToStartValues(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            rectTransform.anchoredPosition3D = _startPosition;
            rectTransform.localRotation = Quaternion.Euler(_startRotation);
            rectTransform.localScale = _startScale;
            canvasGroup.alpha = _startAlpha;
        }

        public static void StopButtonPreview(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            DOTweenEditorPreview.Stop(true);
            if (PreviewIsPlaying)
            {
                ResetButtonToStartValues(rectTransform, canvasGroup);
            }
            PreviewIsPlaying = false;
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
