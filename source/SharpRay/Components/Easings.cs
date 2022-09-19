namespace SharpRay.Components
{
    public static class Easings
    {
        public static float EaseLinearNone(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }

        public static float EaseLinearIn(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }

        public static float EaseLinearOut(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }

        public static float EaseLinearInOut(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }

        public static float EaseSineIn(float t, float b, float c, float d)
        {
            return (0f - c) * MathF.Cos(t / d * (MathF.PI / 2f)) + c + b;
        }

        public static float EaseSineOut(float t, float b, float c, float d)
        {
            return c * MathF.Sin(t / d * (MathF.PI / 2f)) + b;
        }

        public static float EaseSineInOut(float t, float b, float c, float d)
        {
            return (0f - c) / 2f * (MathF.Cos(MathF.PI * t / d) - 1f) + b;
        }

        public static float EaseCircIn(float t, float b, float c, float d)
        {
            return (0f - c) * (MathF.Sqrt(1f - (t /= d) * t) - 1f) + b;
        }

        public static float EaseCircOut(float t, float b, float c, float d)
        {
            return c * MathF.Sqrt(1f - (t = t / d - 1f) * t) + b;
        }

        public static float EaseCircInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2f) < 1f)
            {
                return (0f - c) / 2f * (MathF.Sqrt(1f - t * t) - 1f) + b;
            }

            return c / 2f * (MathF.Sqrt(1f - t * (t -= 2f)) + 1f) + b;
        }

        public static float EaseCubicIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t + b;
        }

        public static float EaseCubicOut(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1f) * t * t + 1f) + b;
        }

        public static float EaseCubicInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2f) < 1f)
            {
                return c / 2f * t * t * t + b;
            }

            return c / 2f * ((t -= 2f) * t * t + 2f) + b;
        }

        public static float EaseQuadIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t + b;
        }

        public static float EaseQuadOut(float t, float b, float c, float d)
        {
            return (0f - c) * (t /= d) * (t - 2f) + b;
        }

        public static float EaseQuadInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2f) < 1f)
            {
                return c / 2f * (t * t) + b;
            }

            return (0f - c) / 2f * ((t - 2f) * (t -= 1f) - 1f) + b;
        }

        public static float EaseExpoIn(float t, float b, float c, float d)
        {
            return (t == 0f) ? b : (c * MathF.Pow(2f, 10f * (t / d - 1f)) + b);
        }

        public static float EaseExpoOut(float t, float b, float c, float d)
        {
            return (t == d) ? (b + c) : (c * (0f - MathF.Pow(2f, -10f * t / d) + 1f) + b);
        }

        public static float EaseExpoInOut(float t, float b, float c, float d)
        {
            if (t == 0f)
            {
                return b;
            }

            if (t == d)
            {
                return b + c;
            }

            if ((t /= d / 2f) < 1f)
            {
                return c / 2f * MathF.Pow(2f, 10f * (t - 1f)) + b;
            }

            return c / 2f * (0f - MathF.Pow(2f, -10f * (t -= 1f)) + 2f) + b;
        }

        public static float EaseBackIn(float t, float b, float c, float d)
        {
            float num = 1.70158f;
            float num2 = (t /= d);
            return c * num2 * t * ((num + 1f) * t - num) + b;
        }

        public static float EaseBackOut(float t, float b, float c, float d)
        {
            float num = 1.70158f;
            return c * ((t = t / d - 1f) * t * ((num + 1f) * t + num) + 1f) + b;
        }

        public static float EaseBackInOut(float t, float b, float c, float d)
        {
            float num = 1.70158f;
            if ((t /= d / 2f) < 1f)
            {
                return c / 2f * (t * t * (((num *= 1.525f) + 1f) * t - num)) + b;
            }

            float num2 = (t -= 2f);
            return c / 2f * (num2 * t * (((num *= 1.525f) + 1f) * t + num) + 2f) + b;
        }

        public static float EaseBounceOut(float t, float b, float c, float d)
        {
            if ((t /= d) < 0.363636374f)
            {
                return c * (7.5625f * t * t) + b;
            }

            if (t < 0.727272749f)
            {
                float num = (t -= 0.545454562f);
                return c * (7.5625f * num * t + 0.75f) + b;
            }

            if ((double)t < 0.90909090909090906)
            {
                float num2 = (t -= 0.8181818f);
                return c * (7.5625f * num2 * t + 0.9375f) + b;
            }

            float num3 = (t -= 21f / 22f);
            return c * (7.5625f * num3 * t + 63f / 64f) + b;
        }

        public static float EaseBounceIn(float t, float b, float c, float d)
        {
            return c - EaseBounceOut(d - t, 0f, c, d) + b;
        }

        public static float EaseBounceInOut(float t, float b, float c, float d)
        {
            if (t < d / 2f)
            {
                return EaseBounceIn(t * 2f, 0f, c, d) * 0.5f + b;
            }

            return EaseBounceOut(t * 2f - d, 0f, c, d) * 0.5f + c * 0.5f + b;
        }

        public static float EaseElasticIn(float t, float b, float c, float d)
        {
            if (t == 0f)
            {
                return b;
            }

            if ((t /= d) == 1f)
            {
                return b + c;
            }

            float num = d * 0.3f;
            float num2 = num / 4f;
            float num3 = c * MathF.Pow(2f, 10f * (t -= 1f));
            return 0f - num3 * MathF.Sin((t * d - num2) * (MathF.PI * 2f) / num) + b;
        }

        public static float EaseElasticOut(float t, float b, float c, float d)
        {
            if (t == 0f)
            {
                return b;
            }

            if ((t /= d) == 1f)
            {
                return b + c;
            }

            float num = d * 0.3f;
            float num2 = num / 4f;
            return c * MathF.Pow(2f, -10f * t) * MathF.Sin((t * d - num2) * (MathF.PI * 2f) / num) + c + b;
        }

        public static float EaseElasticInOut(float t, float b, float c, float d)
        {
            if (t == 0f)
            {
                return b;
            }

            if ((t /= d / 2f) == 2f)
            {
                return b + c;
            }

            float num = d * 0.450000018f;
            float num2 = num / 4f;
            float num3 = 0f;
            if (t < 1f)
            {
                num3 = c * MathF.Pow(2f, 10f * (t -= 1f));
                return -0.5f * (num3 * MathF.Sin((t * d - num2) * (MathF.PI * 2f) / num)) + b;
            }

            num3 = c * MathF.Pow(2f, -10f * (t -= 1f));
            return num3 * MathF.Sin((t * d - num2) * (MathF.PI * 2f) / num) * 0.5f + c + b;
        }
    }
}
