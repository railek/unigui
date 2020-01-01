using System;
using UnityEngine;

namespace Railek.Unigui.Editor
{
    public static class Utilities
    {
        private static Vector3 AdjustToRoundValues(Vector3 v3, int maximumAllowedDecimals = 3)
        {
            return new Vector3(RoundToIntIfNeeded(v3.x, maximumAllowedDecimals),
                RoundToIntIfNeeded(v3.y, maximumAllowedDecimals),
                RoundToIntIfNeeded(v3.z, maximumAllowedDecimals));
        }

        public static void AdjustPositionRotationAndScaleToRoundValues(RectTransform rectTransform)
        {
            rectTransform.anchoredPosition3D = AdjustToRoundValues(rectTransform.anchoredPosition3D);
            rectTransform.localEulerAngles = AdjustToRoundValues(rectTransform.localEulerAngles);
            rectTransform.localScale = AdjustToRoundValues(rectTransform.localScale);
        }

        private static float RoundToIntIfNeeded(float value, int maximumAllowedDecimals = 3)
        {
            int numberOfDecimals = BitConverter.GetBytes(decimal.GetBits((decimal) value)[3])[2];
            return numberOfDecimals >= maximumAllowedDecimals ? Mathf.Round(value) : value;
        }
    }
}
