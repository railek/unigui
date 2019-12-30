using System;
using UnityEditor;
using UnityEngine;

namespace Railek.Unigui.Editor
{
    public class DelayedCall
    {
        private readonly Action _callback;
        private readonly float _delay;
        private readonly float _startupTime;

        public DelayedCall(float delay, Action callback)
        {
            _delay = delay;
            _callback = callback;
            _startupTime = Time.realtimeSinceStartup;
            EditorApplication.update += Update;
        }

        private void Update()
        {
            if (Time.realtimeSinceStartup - (double)_startupTime < _delay) return;
            if (EditorApplication.update != null) EditorApplication.update -= Update;
            _callback?.Invoke();
        }

        public void Cancel()
        {
            if (EditorApplication.update != null) EditorApplication.update -= Update;
        }
    }
}
