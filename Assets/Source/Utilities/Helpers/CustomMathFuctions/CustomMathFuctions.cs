

namespace Assets.Source.Utilities.Helpers
{
    public static class CustomMathFuctions
    {
        public static float EaseIn(float t)
        {
            return t * t; // Quadratic easing in
        }

        // EaseOut function: fast start, decelerates
        public static float EaseOut(float t)
        {
            return t * (2f - t); // Quadratic easing out
        }

        // EaseInOut function: slow at start and end, fast in the middle
        public static float EaseInOut(float t)
        {
            return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
        }
    }
}
