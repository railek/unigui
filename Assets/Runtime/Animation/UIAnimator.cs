using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Railek.Unigui.Animation
{
    public static class UIAnimator
    {
        public const bool DefaultAnimationEnabledState = false;
        public const Direction DefaultDirection = Direction.Left;
        public const RotateMode DefaultRotateMode = RotateMode.FastBeyond360;
        public const LoopType DefaultLoopType = LoopType.Yoyo;
        public const EaseType DefaultEaseType = EaseType.Ease;
        public const Ease DefaultEase = Ease.Linear;
        public const float DefaultDuration = 1f;
        public const float DefaultStartDelay = 0f;
        public const int DefaultNumberOfLoops = -1;
        private const float DefaultDurationOnComplete = 0.05f;
        public const int DefaultVibrato = 10;
        public const float DefaultElasticity = 1;
        public static readonly Vector3 DefaultStartPosition = Vector3.zero;
        public static readonly Vector3 DefaultStartRotation = Vector3.zero;
        public static readonly Vector3 DefaultStartScale = Vector3.one;
        public const float DefaultStartAlpha = 1f;

        public static Tween MoveTween(RectTransform target, UIAnimation animation, Vector3 startValue, Vector3 endValue)
        {
            target.anchoredPosition3D = startValue;
            Tweener tween = target.DOAnchorPos3D(endValue, animation.move.duration)
                .SetDelay(animation.move.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false);
            tween.SetEase(animation.move);
            return tween;
        }

        public static Vector3 MoveLoopPositionA(UIAnimation animation, Vector3 startValue)
        {
            return startValue - animation.move.by;
        }

        private static Vector3 MoveLoopPositionB(UIAnimation animation, Vector3 startValue)
        {
            return startValue + animation.move.by;
        }

        public static Tween MoveLoopTween(RectTransform target, UIAnimation animation, Vector3 startValue)
        {
            Tween loopTween = target.DOAnchorPos(MoveLoopPositionB(animation, startValue), animation.move.duration)
                .SetUpdate(true)
                .SetSpeedBased(false)
                .SetLoops(animation.move.numberOfLoops, animation.move.loopType);
            loopTween.SetEase(animation.move);
            return loopTween;
        }

        public static Tween MovePunchTween(RectTransform target, UIAnimation animation)
        {
            return target.DOPunchAnchorPos(animation.move.by, animation.move.duration, animation.move.vibrato,
                    animation.move.elasticity)
                .SetDelay(animation.move.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false);
        }

        public static Tween MoveStateTween(RectTransform target, UIAnimation animation, Vector3 startValue)
        {
            Tween tween = target.DOAnchorPos(startValue + animation.move.by, animation.move.duration)
                .SetDelay(animation.move.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false);
            tween.SetEase(animation.move);
            return tween;
        }


        public static Tween RotateTween(RectTransform target, UIAnimation animation, Vector3 startValue,
            Vector3 endValue)
        {
            target.localRotation = Quaternion.Euler(startValue);
            Tweener tween = target.DOLocalRotate(endValue, animation.rotate.duration, animation.rotate.rotateMode)
                .SetDelay(animation.rotate.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false);
            tween.SetEase(animation.rotate);
            return tween;
        }

        public static Vector3 RotateLoopRotationA(UIAnimation animation, Vector3 startValue)
        {
            return startValue - animation.rotate.by;
        }

        private static Vector3 RotateLoopRotationB(UIAnimation animation, Vector3 startValue)
        {
            return startValue + animation.rotate.by;
        }

        public static Tween RotateLoopTween(RectTransform target, UIAnimation animation, Vector3 startValue)
        {
            Tween loopTween = target.DOLocalRotate(RotateLoopRotationB(animation, startValue),
                    animation.rotate.duration, animation.rotate.rotateMode)
                .SetUpdate(true)
                .SetSpeedBased(false)
                .SetLoops(animation.rotate.numberOfLoops, animation.rotate.loopType);
            loopTween.SetEase(animation.rotate);
            return loopTween;
        }

        public static Tween RotatePunchTween(RectTransform target, UIAnimation animation)
        {
            return target.DOPunchRotation(animation.rotate.by, animation.rotate.duration, animation.rotate.vibrato,
                    animation.rotate.elasticity)
                .SetDelay(animation.rotate.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false);
        }

        public static Tween RotateStateTween(RectTransform target, UIAnimation animation, Vector3 startValue)
        {
            Tween tween = target.DOLocalRotate(startValue + animation.rotate.by, animation.rotate.duration,
                    animation.rotate.rotateMode)
                .SetDelay(animation.rotate.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false);
            tween.SetEase(animation.rotate);
            return tween;
        }

        public static Tween ScaleTween(RectTransform target, UIAnimation animation, Vector3 startValue,
            Vector3 endValue)
        {
            startValue.z = 1f;
            endValue.z = 1f;
            target.localScale = startValue;
            Tweener tween = target.DOScale(endValue, animation.scale.duration)
                .SetDelay(animation.scale.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false);

            tween.SetEase(animation.scale);

            return tween;
        }

        public static Tween ScaleLoopTween(RectTransform target, UIAnimation animation)
        {
            animation.scale.from.z = 1f;
            animation.scale.to.z = 1f;
            Tweener loopTween = target.DOScale(animation.scale.to, animation.scale.duration)
                .SetUpdate(true)
                .SetSpeedBased(false)
                .SetLoops(animation.scale.numberOfLoops, animation.scale.loopType);
            loopTween.SetEase(animation.scale);
            return loopTween;
        }

        public static Tween ScalePunchTween(RectTransform target, UIAnimation animation)
        {
            animation.scale.by.z = 0f;
            return target.DOPunchScale(animation.scale.by, animation.scale.duration, animation.scale.vibrato,
                    animation.scale.elasticity)
                .SetDelay(animation.scale.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false);
        }

        public static Tween ScaleStateTween(RectTransform target, UIAnimation animation, Vector3 startValue)
        {
            animation.scale.by.z = 0f;
            Tween tween = target.DOScale(startValue + animation.scale.by, animation.scale.duration)
                .SetDelay(animation.scale.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false);
            tween.SetEase(animation.scale);
            return tween;
        }

        public static Tween FadeTween(RectTransform target, UIAnimation animation, float startValue, float endValue)
        {
            startValue = Mathf.Clamp01(startValue);
            endValue = Mathf.Clamp01(endValue);
            var canvasGroup = target.GetComponent<CanvasGroup>() ?? target.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = startValue;
            Tweener tween = canvasGroup.DOFade(endValue, animation.fade.duration)
                .SetDelay(animation.fade.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false);

            tween.SetEase(animation.fade);
            return tween;
        }

        public static Tween FadeLoopTween(RectTransform target, UIAnimation animation)
        {
            animation.fade.from = Mathf.Clamp01(animation.fade.from);
            animation.fade.to = Mathf.Clamp01(animation.fade.to);
            var canvasGroup = target.GetComponent<CanvasGroup>() != null
                ? target.GetComponent<CanvasGroup>()
                : target.gameObject.AddComponent<CanvasGroup>();
            Tweener loopTween = canvasGroup.DOFade(animation.fade.to, animation.fade.duration)
                .SetUpdate(true)
                .SetSpeedBased(false)
                .SetLoops(animation.fade.numberOfLoops, animation.fade.loopType);
            loopTween.SetEase(animation.fade);
            return loopTween;
        }

        public static Tween FadeStateTween(RectTransform target, UIAnimation animation, float startValue)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>() != null
                ? target.GetComponent<CanvasGroup>()
                : target.gameObject.AddComponent<CanvasGroup>();
            var targetAlpha = startValue + animation.fade.by;
            targetAlpha = Mathf.Clamp01(targetAlpha);
            Tween tween = canvasGroup.DOFade(targetAlpha, animation.fade.duration)
                .SetDelay(animation.fade.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false);
            tween.SetEase(animation.fade);
            return tween;
        }


        public static void Move(RectTransform target, UIAnimation animation, Vector3 startValue, Vector3 endValue,
            bool instantAction = false, UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.move.enabled && !instantAction)
            {
                return;
            }

            if (instantAction)
            {
                target.anchoredPosition3D = endValue;
                onStartCallback?.Invoke();
                onCompleteCallback?.Invoke();
                return;
            }

            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Move))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .Append(MoveTween(target, animation, startValue, endValue))
                .Play();
        }

        public static void Rotate(RectTransform target, UIAnimation animation, Vector3 startValue, Vector3 endValue,
            bool instantAction = false, UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.rotate.enabled && !instantAction)
            {
                return;
            }

            if (instantAction)
            {
                target.localRotation = Quaternion.Euler(endValue);
                onStartCallback?.Invoke();
                onCompleteCallback?.Invoke();
                return;
            }

            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Rotate))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .Append(RotateTween(target, animation, startValue, endValue))
                .Play();
        }

        public static void Scale(RectTransform target, UIAnimation animation, Vector3 startValue, Vector3 endValue,
            bool instantAction = false, UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.scale.enabled && !instantAction)
            {
                return;
            }

            startValue.z = 1;
            endValue.z = 1;
            if (instantAction)
            {
                target.localScale = endValue;
                onStartCallback?.Invoke();
                onCompleteCallback?.Invoke();
                return;
            }

            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Scale))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .Append(ScaleTween(target, animation, startValue, endValue))
                .Play();
        }

        public static void Fade(RectTransform target, UIAnimation animation, float startValue, float endValue,
            bool instantAction = false, UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.fade.enabled && !instantAction)
            {
                return;
            }

            var canvasGroup = target.GetComponent<CanvasGroup>() ?? target.gameObject.AddComponent<CanvasGroup>();
            if (instantAction)
            {
                canvasGroup.alpha = endValue;
                onStartCallback?.Invoke();
                onCompleteCallback?.Invoke();
                return;
            }

            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Fade))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .Append(FadeTween(target, animation, startValue, endValue))
                .Play();
        }

        public static void MoveLoop(RectTransform target, UIAnimation animation, Vector3 startValue,
            UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.move.enabled)
            {
                return;
            }

            if (animation.animationType != AnimationType.Loop)
            {
                return;
            }

            var positionA = MoveLoopPositionA(animation, startValue);

            var loopSequence = DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Move))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Append(MoveLoopTween(target, animation, startValue))
                .SetLoops(animation.move.numberOfLoops, animation.move.loopType)
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .OnKill(() => { onCompleteCallback?.Invoke(); })
                .Pause();


            Tween startTween = target.DOAnchorPos(positionA, animation.move.duration / 2f)
                .SetDelay(animation.move.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Pause();


            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Move))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Append(startTween)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { loopSequence.Play(); });
        }

        public static void RotateLoop(RectTransform target, UIAnimation animation, Vector3 startValue,
            UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.rotate.enabled)
            {
                return;
            }

            if (animation.animationType != AnimationType.Loop)
            {
                return;
            }

            var rotationA = RotateLoopRotationA(animation, startValue);

            var loopSequence = DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Rotate))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Append(RotateLoopTween(target, animation, startValue))
                .SetLoops(animation.rotate.numberOfLoops, animation.rotate.loopType)
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .OnKill(() => { onCompleteCallback?.Invoke(); })
                .Pause();

            Tween startTween = target
                .DOLocalRotate(rotationA, animation.rotate.duration / 2f, animation.rotate.rotateMode)
                .SetDelay(animation.rotate.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Pause();


            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Rotate))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Append(startTween)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { loopSequence.Play(); });
        }

        public static void ScaleLoop(RectTransform target, UIAnimation animation, UnityAction onStartCallback = null,
            UnityAction onCompleteCallback = null)
        {
            if (!animation.scale.enabled)
            {
                return;
            }

            if (animation.animationType != AnimationType.Loop)
            {
                return;
            }

            var loopSequence = DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Scale))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Append(ScaleLoopTween(target, animation))
                .SetLoops(animation.scale.numberOfLoops, animation.scale.loopType)
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .OnKill(() => { onCompleteCallback?.Invoke(); })
                .Pause();


            Tween startTween = target.DOScale(animation.scale.from, animation.scale.duration / 2f)
                .SetDelay(animation.scale.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Pause();


            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Scale))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Append(startTween)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { loopSequence.Play(); });
        }

        public static void FadeLoop(RectTransform target, UIAnimation animation, UnityAction onStartCallback = null,
            UnityAction onCompleteCallback = null)
        {
            if (!animation.fade.enabled)
            {
                return;
            }

            if (animation.animationType != AnimationType.Loop)
            {
                return;
            }

            var canvasGroup = target.GetComponent<CanvasGroup>() != null
                ? target.GetComponent<CanvasGroup>()
                : target.gameObject.AddComponent<CanvasGroup>();

            var loopSequence = DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Fade))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Append(FadeLoopTween(target, animation))
                .SetLoops(animation.fade.numberOfLoops, animation.fade.loopType)
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .OnKill(() => { onCompleteCallback?.Invoke(); })
                .Pause();


            Tween startTween = canvasGroup.DOFade(animation.fade.from, animation.fade.duration / 2f)
                .SetDelay(animation.fade.startDelay)
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Pause();


            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Fade))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .Append(startTween)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { loopSequence.Play(); });
        }

        public static void MovePunch(RectTransform target, UIAnimation animation, Vector3 startValue,
            UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.move.enabled)
            {
                return;
            }

            if (animation.animationType != AnimationType.Punch)
            {
                return;
            }

            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Move))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() =>
                {
                    target.DOAnchorPos(startValue, DefaultDurationOnComplete)
                        .OnComplete(() => { onCompleteCallback?.Invoke(); }).Play();
                })
                .Append(MovePunchTween(target, animation))
                .Play();
        }

        public static void RotatePunch(RectTransform target, UIAnimation animation, Vector3 startValue,
            UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.rotate.enabled)
            {
                return;
            }

            if (animation.animationType != AnimationType.Punch)
            {
                return;
            }

            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Rotate))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() =>
                {
                    target.DOLocalRotate(startValue, DefaultDurationOnComplete).OnComplete(() =>
                    {
                        onCompleteCallback?.Invoke();
                    }).Play();
                })
                .Append(RotatePunchTween(target, animation))
                .Play();
        }

        public static void ScalePunch(RectTransform target, UIAnimation animation, Vector3 startValue,
            UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.scale.enabled)
            {
                return;
            }

            if (animation.animationType != AnimationType.Punch)
            {
                return;
            }

            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Scale))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() =>
                {
                    target.DOScale(startValue, DefaultDurationOnComplete).OnComplete(() =>
                    {
                        onCompleteCallback?.Invoke();
                    }).Play();
                })
                .Append(ScalePunchTween(target, animation))
                .Play();
        }

        public static void MoveState(RectTransform target, UIAnimation animation, Vector3 startValue,
            UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.move.enabled)
            {
                return;
            }

            if (animation.animationType != AnimationType.State)
            {
                return;
            }

            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Move))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .Append(MoveStateTween(target, animation, startValue))
                .Play();
        }

        public static void RotateState(RectTransform target, UIAnimation animation, Vector3 startValue,
            UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.rotate.enabled)
            {
                return;
            }

            if (animation.animationType != AnimationType.State)
            {
                return;
            }

            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Rotate))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .Append(RotateStateTween(target, animation, startValue))
                .Play();
        }

        public static void ScaleState(RectTransform target, UIAnimation animation, Vector3 startValue,
            UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.scale.enabled)
            {
                return;
            }

            if (animation.animationType != AnimationType.State)
            {
                return;
            }

            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Scale))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .Append(ScaleStateTween(target, animation, startValue))
                .Play();
        }

        public static void FadeState(RectTransform target, UIAnimation animation, float startValue,
            UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            if (!animation.fade.enabled)
            {
                return;
            }

            if (animation.animationType != AnimationType.State)
            {
                return;
            }

            DOTween.Sequence()
                .SetId(GetTweenId(target, animation.animationType, AnimationAction.Fade))
                .SetUpdate(true)
                .SetSpeedBased(false)
                .OnStart(() => { onStartCallback?.Invoke(); })
                .OnComplete(() => { onCompleteCallback?.Invoke(); })
                .Append(FadeStateTween(target, animation, startValue))
                .Play();
        }

        public static Vector3 GetAnimationMoveFrom(RectTransform target, UIAnimation animation, Vector3 startValue)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show:
                    return animation.move.useCustomFromAndTo
                        ? animation.move.from
                        : GetToPositionByDirection(target, animation,
                            animation.move.useCustomFromAndTo ? animation.move.customPosition : startValue);
                case AnimationType.Hide: return animation.move.useCustomFromAndTo ? animation.move.from : startValue;
                default: return DefaultStartPosition;
            }
        }

        public static Vector3 GetAnimationMoveTo(RectTransform target, UIAnimation animation, Vector3 startValue)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show: return animation.move.useCustomFromAndTo ? animation.move.to : startValue;
                case AnimationType.Hide:
                    return animation.move.useCustomFromAndTo
                        ? animation.move.to
                        : GetToPositionByDirection(target, animation,
                            animation.move.useCustomFromAndTo ? animation.move.customPosition : startValue);
                default: return DefaultStartPosition;
            }
        }

        public static Vector3 GetAnimationRotateFrom(UIAnimation animation, Vector3 startValue)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show: return animation.rotate.from;
                case AnimationType.Hide:
                    return animation.rotate.useCustomFromAndTo ? animation.rotate.from : startValue;
                default: return DefaultStartRotation;
            }
        }

        public static Vector3 GetAnimationRotateTo(UIAnimation animation, Vector3 startValue)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show: return animation.rotate.useCustomFromAndTo ? animation.rotate.to : startValue;
                case AnimationType.Hide: return animation.rotate.to;
                default: return DefaultStartRotation;
            }
        }

        public static Vector3 GetAnimationScaleFrom(UIAnimation animation, Vector3 startValue)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show: return animation.scale.from;
                case AnimationType.Hide: return animation.scale.useCustomFromAndTo ? animation.scale.from : startValue;
                default: return DefaultStartScale;
            }
        }

        public static Vector3 GetAnimationScaleTo(UIAnimation animation, Vector3 startValue)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show: return animation.scale.useCustomFromAndTo ? animation.scale.to : startValue;
                case AnimationType.Hide: return animation.scale.to;
                default: return DefaultStartScale;
            }
        }

        public static float GetAnimationFadeFrom(UIAnimation animation, float startValue)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show: return animation.fade.from;
                case AnimationType.Hide: return animation.fade.useCustomFromAndTo ? animation.fade.from : startValue;
                default: return DefaultStartAlpha;
            }
        }

        public static float GetAnimationFadeTo(UIAnimation animation, float startValue)
        {
            switch (animation.animationType)
            {
                case AnimationType.Show: return animation.fade.useCustomFromAndTo ? animation.fade.to : startValue;
                case AnimationType.Hide: return animation.fade.to;
                default: return DefaultStartAlpha;
            }
        }


        private static Vector3 GetToPositionByDirection(RectTransform target, UIAnimation animation, Vector3 startValue)
        {
            var rootCanvas = target.GetComponent<Canvas>().rootCanvas;
            var rootCanvasRect = rootCanvas.GetComponent<RectTransform>().rect;
            var xOffset = rootCanvasRect.width / 2 + target.rect.width * target.pivot.x;
            var yOffset = rootCanvasRect.height / 2 + target.rect.height * target.pivot.y;

            switch (animation.move.direction)
            {
                case Direction.Left: return new Vector3(-xOffset, startValue.y, startValue.z);
                case Direction.Right: return new Vector3(xOffset, startValue.y, startValue.z);
                case Direction.Top: return new Vector3(startValue.x, yOffset, startValue.z);
                case Direction.Bottom: return new Vector3(startValue.x, -yOffset, startValue.z);
                case Direction.TopLeft: return new Vector3(-xOffset, yOffset, startValue.z);
                case Direction.TopRight: return new Vector3(xOffset, yOffset, startValue.z);
                case Direction.Center: return new Vector3(0, 0, startValue.z);
                case Direction.BottomLeft: return new Vector3(-xOffset, -yOffset, startValue.z);
                case Direction.BottomRight: return new Vector3(xOffset, -yOffset, startValue.z);
                case Direction.CustomPosition: return animation.move.customPosition;
                default: return Vector3.zero;
            }
        }

        private static string GetTweenId(Object target, AnimationType animationType, AnimationAction animationAction)
        {
            return target.GetInstanceID() + "-" + animationType + "-" + animationAction;
        }

        public static void ResetCanvasGroup(RectTransform target, bool interactable = true, bool blocksRaycasts = true)
        {
            if (target == null)
            {
                return;
            }

            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = blocksRaycasts;
        }

        public static void StopAnimations(RectTransform target, AnimationType animationType, bool complete = true)
        {
            if (target == null)
            {
                return;
            }

            DOTween.Kill(GetTweenId(target, animationType, AnimationAction.Move), complete);
            DOTween.Kill(GetTweenId(target, animationType, AnimationAction.Rotate), complete);
            DOTween.Kill(GetTweenId(target, animationType, AnimationAction.Scale), complete);
            DOTween.Kill(GetTweenId(target, animationType, AnimationAction.Fade), complete);
        }

        private static void SetEase(this Tween tween, Move move)
        {
            switch (move.easeType)
            {
                case EaseType.Ease:
                {
                    tween.SetEase(move.ease);
                    break;
                }
                case EaseType.AnimationCurve:
                {
                    tween.SetEase(move.animationCurve);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void SetEase(this Tween tween, Rotate rotate)
        {
            switch (rotate.easeType)
            {
                case EaseType.Ease:
                {
                    tween.SetEase(rotate.ease);
                    break;
                }
                case EaseType.AnimationCurve:
                {
                    tween.SetEase(rotate.animationCurve);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void SetEase(this Tween tween, Scale scale)
        {
            switch (scale.easeType)
            {
                case EaseType.Ease:
                {
                    tween.SetEase(scale.ease);
                    break;
                }
                case EaseType.AnimationCurve:
                {
                    tween.SetEase(scale.animationCurve);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void SetEase(this Tween tween, Fade fade)
        {
            switch (fade.easeType)
            {
                case EaseType.Ease:
                {
                    tween.SetEase(fade.ease);
                    break;
                }
                case EaseType.AnimationCurve:
                {
                    tween.SetEase(fade.animationCurve);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
