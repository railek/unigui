using System;
using UnityEngine;

namespace Railek.Unigui.Animation
{
    [Serializable]
    public class UIAnimation
    {
        public AnimationType animationType;
        public Move move;
        public Rotate rotate;
        public Scale scale;
        public Fade fade;


        public UIAnimation(AnimationType animationType)
        {
            Reset(animationType);
        }

        public UIAnimation(AnimationType animationType, Move move, Rotate rotate, Scale scale, Fade fade) : this(
            animationType)
        {
            this.move = move;
            this.rotate = rotate;
            this.scale = scale;
            this.fade = fade;
        }

        public bool Enabled
        {
            get
            {
                switch (animationType)
                {
                    case AnimationType.Undefined: return false;
                    case AnimationType.Show: return move.enabled || rotate.enabled || scale.enabled || fade.enabled;
                    case AnimationType.Hide: return move.enabled || rotate.enabled || scale.enabled || fade.enabled;
                    case AnimationType.Loop: return move.enabled || rotate.enabled || scale.enabled || fade.enabled;
                    case AnimationType.Punch: return move.enabled || rotate.enabled || scale.enabled;
                    case AnimationType.State: return move.enabled || rotate.enabled || scale.enabled || fade.enabled;
                    default: return false;
                }
            }
        }

        public float StartDelay
        {
            get
            {
                if (!Enabled)
                {
                    return 0;
                }

                return Mathf.Min(move.enabled ? move.startDelay : 10000,
                    rotate.enabled ? rotate.startDelay : 10000,
                    scale.enabled ? scale.startDelay : 10000,
                    fade.enabled ? fade.startDelay : 10000);
            }
        }

        public float TotalDuration =>
            Mathf.Max(move.enabled ? move.TotalDuration : 0,
                rotate.enabled ? rotate.TotalDuration : 0,
                scale.enabled ? scale.TotalDuration : 0,
                fade.enabled ? fade.TotalDuration : 0);

        private void Reset(AnimationType uiAnimationType)
        {
            animationType = uiAnimationType;
            move = new Move(uiAnimationType);
            rotate = new Rotate(uiAnimationType);
            scale = new Scale(uiAnimationType);
            fade = new Fade(uiAnimationType);
        }
    }
}
