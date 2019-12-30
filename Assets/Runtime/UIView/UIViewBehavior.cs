using System;
using Railek.Unibase;
using Railek.Unigui.Animation;

namespace Railek.Unigui
{
    [Serializable]
    public class UIViewBehavior
    {
        public UIAnimation animation;
        public VoidEvent onStart;
        public VoidEvent onFinished;

        public UIViewBehavior(AnimationType animationType)
        {
            Reset(animationType);
        }

        private void Reset(AnimationType animationType)
        {
            animation = new UIAnimation(animationType);
        }
    }
}
