namespace RSLib.Maths
{
    using UnityEngine;

    public static class AimPrediction
    {
        /// <summary>
        /// Tries to predict the aim of a projectile with a linear velocity so that it will hit a target with a linear velocity.
        /// Returns true or false depending on the aim predictability.
        /// Explanation video : https://www.youtube.com/watch?v=2zVwug_agr0&t=1683s
        /// </summary>
        /// <param name="a">Target initial position.</param>
        /// <param name="b">Projectile initial position.</param>
        /// <param name="vA">Target velocity.</param>
        /// <param name="sB">Projectile velocity.</param>
        /// <param name="result">Predicted aim for the projectile.</param>
        /// <returns>True if prediction could be computed, else false.</returns>
        public static bool TryPredictAim(Vector2 a, Vector2 b, Vector2 vA, float sB, out Vector2 result)
        {
            Vector2 aToB = b - a;
            float dC = aToB.magnitude;
            float alpha = Vector2.Angle(aToB, vA) * Mathf.Deg2Rad;
            float sA = vA.magnitude;
            float r = sA / sB;

            int roots = QuadraticEquation(1 - r * r,
                                          2f * r * dC * Mathf.Cos(alpha),
                                          -(dC * dC),
                                          out float r1,
                                          out float r2);

            if (roots == 0)
            {
                result = Vector2.zero;
                return false;
            }

            float dA = Mathf.Max(r1, r2);
            float t = dA / sB;
            Vector2 c = a + vA * t;
            result = (c - b).normalized;
            
            return true;
        }
        
        private static int QuadraticEquation(float a, float b, float c, out float r1, out float r2)
        {
            float delta = b * b - 4 * a * c;

            if (delta < 0f)
            {
                r1 = UnityEngine.Mathf.Infinity;
                r2 = -r1;
                return 0;
            }
            
            r1 = (-b + UnityEngine.Mathf.Sqrt(delta)) / (2f * a);
            r2 = (-b - UnityEngine.Mathf.Sqrt(delta)) / (2f * a);

            return delta > 0f ? 2 : 1;
        }
    }
}