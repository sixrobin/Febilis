namespace RSLib.Extensions
{
    using UnityEngine;

    public static class VectorExtensions
    {
        #region VECTOR2

        #region ABS

        /// <summary>Gets a vector copy with all components absolute values.</summary>
        public static Vector2 AbsAll(this Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        /// <summary>Gets a vector copy with all components absolute values.</summary>
        public static Vector2Int AbsAll(this Vector2Int v)
        {
            return new Vector2Int(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        /// <summary>Gets a vector copy with x component absolute value.</summary>
        public static Vector2 AbsX(this Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), v.y);
        }

        /// <summary>Gets a vector copy with x component absolute value.</summary>
        public static Vector2Int AbsX(this Vector2Int v)
        {
            return new Vector2Int(Mathf.Abs(v.x), v.y);
        }

        /// <summary>Gets a vector copy with y component absolute value.</summary>
        public static Vector2 AbsY(this Vector2 v)
        {
            return new Vector2(v.x, Mathf.Abs(v.y));
        }

        /// <summary>Gets a vector copy with y component absolute value.</summary>
        public static Vector2Int AbsY(this Vector2Int v)
        {
            return new Vector2Int(v.x, Mathf.Abs(v.y));
        }

        #endregion ABS

        #region ADD

        /// <summary>Gets a vector's copy with all components incremented.</summary>
        /// <param name="incr">Incrementation amount.</param>
        public static Vector2 AddAll(this Vector2 v, float value)
        {
            return new Vector2(v.x + value, v.y + value);
        }

        /// <summary>Gets a vector's copy with all components incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector2Int AddAll(this Vector2Int v, int value)
        {
            return new Vector2Int(v.x + value, v.y + value);
        }

        /// <summary>Gets a vector's copy with x component incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector2 AddX(this Vector2 v, float value)
        {
            return new Vector2(v.x + value, v.y);
        }

        /// <summary>Gets a vector's copy with x component incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector2Int AddX(this Vector2Int v, int value)
        {
            return new Vector2Int(v.x + value, v.y);
        }

        /// <summary>Gets a vector's copy with y component incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector2 AddY(this Vector2 v, float value)
        {
            return new Vector2(v.x, v.y + value);
        }

        /// <summary>Gets a vector's copy with y component incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector2Int AddY(this Vector2Int v, int value)
        {
            return new Vector2Int(v.x, v.y + value);
        }

        #endregion ADD

        #region CLAMP

        /// <summary>Gets a vector copy with all components clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector2 ClampAll(this Vector2 v, float min, float max)
        {
            return new Vector2(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
        }

        /// <summary>Gets a vector copy with all components clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector2Int ClampAll(this Vector2Int v, int min, int max)
        {
            return new Vector2Int(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
        }

        /// <summary>Gets a vector copy with all components clamped between two values.</summary>
        /// <param name="min">Minimum values.</param>
        /// <param name="max">Maximum values.</param>
        public static Vector2 ClampAll(this Vector2 v, Vector2 min, Vector2 max)
        {
            return new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
        }

        /// <summary>Gets a vector copy with all components clamped between two values.</summary>
        /// <param name="min">Minimum values.</param>
        /// <param name="max">Maximum values.</param>
        public static Vector2Int ClampAll(this Vector2Int v, Vector2Int min, Vector2Int max)
        {
            return new Vector2Int(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
        }

        /// <summary>Gets a vector copy with x component clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector2 ClampX(this Vector2 v, float min, float max)
        {
            return new Vector2(Mathf.Clamp(v.x, min, max), v.y);
        }

        /// <summary>Gets a vector copy with x component clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector2Int ClampX(this Vector2Int v, int min, int max)
        {
            return new Vector2Int(Mathf.Clamp(v.x, min, max), v.y);
        }

        /// <summary>Gets a vector copy with y component clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector2 ClampY(this Vector2 v, float min, float max)
        {
            return new Vector2(v.x, Mathf.Clamp(v.y, min, max));
        }

        /// <summary>Gets a vector copy with y component clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector2Int ClampY(this Vector2Int v, int min, int max)
        {
            return new Vector2Int(v.x, Mathf.Clamp(v.y, min, max));
        }

        /// <summary>Gets a vector copy with all components clamped between 0 and 1.</summary>
        public static Vector2 ClampAll01(this Vector2 v)
        {
            return new Vector2(Mathf.Clamp01(v.x), Mathf.Clamp01(v.y));
        }

        /// <summary>Gets a vector copy with x component clamped between 0 and 1.</summary>
        public static Vector2 ClampX01(this Vector2 v)
        {
            return new Vector2(Mathf.Clamp01(v.x), v.y);
        }

        /// <summary>Gets a vector copy with y component clamped between 0 and 1.</summary>
        public static Vector2 ClampY01(this Vector2 v)
        {
            return new Vector2(v.x, Mathf.Clamp01(v.y));
        }

        #endregion CLAMP

        #region CONVERSION

        /// <summary>Converts a Vector2 to a Vector2Int.</summary>
        /// <returns>New Vector2Int.</returns>
        public static Vector2Int ToVector2Int(this Vector2 v)
        {
            return new Vector2Int((int)v.x, (int)v.y);
        }

        /// <summary>Converts a Vector2Int to a Vector2.</summary>
        /// <returns>New Vector2.</returns>
        public static Vector2 ToVector2(this Vector2Int v)
        {
            return new Vector2(v.x, v.y);
        }

        #endregion CONVERSION

        #region NORMAL

        /// <summary>Computes the vector normal. Use NormalNormalized to get it normalized.</summary>
        /// <param name="clockwise">Normal is rotated clockwise.</param>
        /// <returns>Normal as a new vector.</returns>
        public static Vector2 Normal(this Vector2 v, bool clockwise = true)
        {
            Vector2 normal = v;

            normal.x += normal.y;
            normal.y = normal.x - normal.y;
            normal.x -= normal.y;
            normal.y *= -1;

            return clockwise ? normal : -normal;
        }

        /// <summary>Computes the vector normal normalized.</summary>
        /// <param name="clockwise">Normal is rotated clockwise.</param>
        /// <returns>Normalized normal as a new vector.</returns>
        public static Vector2 NormalNormalized(this Vector2 v, bool clockwise = true)
        {
            return v.Normal(clockwise).normalized;
        }

        #endregion NORMAL

        #region ROUND

        /// <summary>Gets a vector's copy with all components rounded.</summary>
        public static Vector2 RoundAll(this Vector2 v)
        {
            return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
        }

        /// <summary>Gets a vector's copy with x component rounded.</summary>
        public static Vector2 RoundX(this Vector2 v)
        {
            return new Vector2(Mathf.Round(v.x), v.y);
        }

        /// <summary>Gets a vector's copy with y component rounded.</summary>
        public static Vector2 RoundY(this Vector2 v)
        {
            return new Vector2(v.x, Mathf.Round(v.y));
        }

        #endregion ROUND

        #region SWAP

        /// <summary>Swaps x and y components of a vector.</summary>
        public static Vector2 Swap(this Vector2 v)
        {
            return new Vector2(v.y, v.x);
        }

        #endregion SWAP

        #region WITH

        /// <summary>Gets a vector's copy with new x value.</summary>
        /// <param name="value">New x value.</param>
        public static Vector2 WithX(this Vector2 v, float value)
        {
            return new Vector2(value, v.y);
        }

        /// <summary>Gets a vector's copy with new x value.</summary>
        /// /// <param name="value">New x value.</param>
        public static Vector2Int WithX(this Vector2Int v, int value)
        {
            return new Vector2Int(value, v.y);
        }

        /// <summary>Gets a vector's copy with new y value.</summary>
        /// <param name="value">New y value.</param>
        public static Vector2 WithY(this Vector2 v, float value)
        {
            return new Vector2(v.x, value);
        }

        /// <summary>Gets a vector's copy with new y value.</summary>
        /// <param name="value">New y value.</param>
        public static Vector2Int WithY(this Vector2Int v, int value)
        {
            return new Vector2Int(v.x, value);
        }

        #endregion WITH

        #endregion VECTOR2

        #region VECTOR3

        #region ABS

        /// <summary>Gets a vector copy with all components absolute values.</summary>
        public static Vector3 AbsAll(this Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        /// <summary>Gets a vector copy with all components absolute values.</summary>
        public static Vector3Int AbsAll(this Vector3Int v)
        {
            return new Vector3Int(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        /// <summary>Gets a vector copy with x component absolute value.</summary>
        public static Vector3 AbsX(this Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), v.y, v.z);
        }

        /// <summary>Gets a vector copy with x component absolute value.</summary>
        public static Vector3Int AbsX(this Vector3Int v)
        {
            return new Vector3Int(Mathf.Abs(v.x), v.y, v.z);
        }

        /// <summary>Gets a vector copy with y component absolute value.</summary>
        public static Vector3 AbsY(this Vector3 v)
        {
            return new Vector3(v.x, Mathf.Abs(v.y), v.z);
        }

        /// <summary>Gets a vector copy with y component absolute value.</summary>
        public static Vector3Int AbsY(this Vector3Int v)
        {
            return new Vector3Int(v.x, Mathf.Abs(v.y), v.z);
        }

        /// <summary>Gets a vector copy with z component absolute value.</summary>
        public static Vector3 AbsZ(this Vector3 v)
        {
            return new Vector3(v.x, v.y, Mathf.Abs(v.z));
        }

        /// <summary>Gets a vector copy with z component absolute value.</summary>
        public static Vector3Int AbsZ(this Vector3Int v)
        {
            return new Vector3Int(v.x, v.y, Mathf.Abs(v.z));
        }

        #endregion ABS

        #region ADD

        /// <summary>Gets a vector's copy with all components incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector3 AddAll(this Vector3 v, float value)
        {
            return new Vector3(v.x + value, v.y + value, v.z + value);
        }

        /// <summary>Gets a vector's copy with all components incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector3Int AddAll(this Vector3Int v, int value)
        {
            return new Vector3Int(v.x + value, v.y + value, v.z + value);
        }

        /// <summary>Gets a vector's copy with x component incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector3 AddX(this Vector3 v, float value)
        {
            return new Vector3(v.x + value, v.y, v.z);
        }

        /// <summary>Gets a vector's copy with x component incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector3Int AddX(this Vector3Int v, int value)
        {
            return new Vector3Int(v.x + value, v.y, v.z);
        }

        /// <summary>Gets a vector's copy with y component incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector3 AddY(this Vector3 v, float value)
        {
            return new Vector3(v.x, v.y + value, v.z);
        }

        /// <summary>Gets a vector's copy with y component incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector3Int AddY(this Vector3Int v, int value)
        {
            return new Vector3Int(v.x, v.y + value, v.z);
        }

        /// <summary>Gets a vector's copy with z component incremented.</summary>
        /// <param name="value">Incrementation amount.</param>
        public static Vector3 AddZ(this Vector3 v, float value)
        {
            return new Vector3(v.x, v.y, v.z + value);
        }

        /// <summary>Gets a vector's copy with z component incremented.</summary>
        /// <param name="incr">Incrementation amount.</param>
        public static Vector3Int AddZ(this Vector3Int v, int value)
        {
            return new Vector3Int(v.x, v.y, v.z + value);
        }

        #endregion ADD

        #region CLAMP

        /// <summary>Gets a vector copy with all components clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector3 ClampAll(this Vector3 v, float min, float max)
        {
            return new Vector3(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
        }

        /// <summary>Gets a vector copy with all components clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector3Int ClampAll(this Vector3Int v, int min, int max)
        {
            return new Vector3Int(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
        }

        /// <summary>Gets a vector copy with all components clamped between two values.</summary>
        /// <param name="min">Minimum values.</param>
        /// <param name="max">Maximum values.</param>
        public static Vector3 ClampAll(this Vector3 v, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y), Mathf.Clamp(v.z, min.z, max.z));
        }

        /// <summary>Gets a vector copy with all components clamped between two values.</summary>
        /// <param name="min">Minimum values.</param>
        /// <param name="max">Maximum values.</param>
        public static Vector3Int ClampAll(this Vector3Int v, Vector3Int min, Vector3Int max)
        {
            return new Vector3Int(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y), Mathf.Clamp(v.z, min.z, max.z));
        }

        /// <summary>Gets a vector copy with x component clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector3 ClampX(this Vector3 v, float min, float max)
        {
            return new Vector3(Mathf.Clamp(v.x, min, max), v.y, v.z);
        }

        /// <summary>Gets a vector copy with x component clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector3Int ClampX(this Vector3Int v, int min, int max)
        {
            return new Vector3Int(Mathf.Clamp(v.x, min, max), v.y, v.z);
        }

        /// <summary>Gets a vector copy with y component clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector3 ClampY(this Vector3 v, float min, float max)
        {
            return new Vector3(v.x, Mathf.Clamp(v.y, min, max), v.z);
        }

        /// <summary>Gets a vector copy with y component clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector3Int ClampY(this Vector3Int v, int min, int max)
        {
            return new Vector3Int(v.x, Mathf.Clamp(v.y, min, max), v.z);
        }

        /// <summary>Gets a vector copy with z component clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector3 ClampZ(this Vector3 v, float min, float max)
        {
            return new Vector3(v.x, v.y, Mathf.Clamp(v.z, min, max));
        }

        /// <summary>Gets a vector copy with z component clamped between two values.</summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static Vector3Int ClampZ(this Vector3Int v, int min, int max)
        {
            return new Vector3Int(v.x, v.y, Mathf.Clamp(v.z, min, max));
        }

        /// <summary>Gets a vector copy with all components clamped between 0 and 1.</summary>
        public static Vector3 ClampAll01(this Vector3 v)
        {
            return new Vector3(Mathf.Clamp01(v.x), Mathf.Clamp01(v.y), Mathf.Clamp01(v.z));
        }

        /// <summary>Gets a vector copy with x component clamped between 0 and 1.</summary>
        public static Vector3 ClampX01(this Vector3 v)
        {
            return new Vector3(Mathf.Clamp01(v.x), v.y, v.z);
        }

        /// <summary>Gets a vector copy with y component clamped between 0 and 1.</summary>
        public static Vector3 ClampY01(this Vector3 v)
        {
            return new Vector3(v.x, Mathf.Clamp01(v.y), v.z);
        }

        /// <summary>Gets a vector copy with z component clamped between 0 and 1.</summary>
        public static Vector3 ClampZ01(this Vector3 v)
        {
            return new Vector3(v.x, v.y, Mathf.Clamp01(v.z));
        }

        #endregion CLAMP

        #region CONVERSION

        /// <summary>Converts a Vector3 to a Vector3Int.</summary>
        /// <returns>New Vector3Int.</returns>
        public static Vector3Int ToVector3Int(this Vector3 v)
        {
            return new Vector3Int((int)v.x, (int)v.y, (int)v.z);
        }

        /// <summary>Converts a Vector3Int to a Vector3.</summary>
        /// <returns>New Vector3.</returns>
        public static Vector3 ToVector3(this Vector3Int v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        #endregion CONVERSION

        #region NORMAL

        public enum Axis
        {
            XY,
            YZ,
            XZ
        }

        /// <summary>Computes the vector normal. Use NormalNormalized to get it normalized.</summary>
        /// <param name="axis">Rotation axis.</param>
        /// <param name="clockwise">Normal is rotated clockwise.</param>
        /// <returns>Normal as a new vector.</returns>
        public static Vector3 Normal(this Vector3 v, Axis axis, bool clockwise)
        {
            Vector3 normal = v;

            switch (axis)
            {
                case Axis.XY:
                    normal.x += normal.y;
                    normal.y = normal.x - normal.y;
                    normal.x -= normal.y;
                    normal.y *= -1;
                    break;

                case Axis.YZ:
                    normal.y += normal.z;
                    normal.z = normal.y - normal.z;
                    normal.y -= normal.z;
                    normal.z *= -1;
                    break;

                case Axis.XZ:
                    normal.x += normal.z;
                    normal.z = normal.x - normal.z;
                    normal.x -= normal.z;
                    normal.z *= -1;
                    break;
            }

            return clockwise ? normal : -normal;
        }

        /// <summary>Computes the vector normal normalized.</summary>
        /// <param name="axis">Rotation axis.</param>
        /// <param name="clockwise">Normal is rotated clockwise.</param>
        /// <returns>Normalized normal as a new vector.</returns>
        public static Vector3 NormalNormalized(this Vector3 v, Axis axis, bool clockwise)
        {
            return v.Normal(axis, clockwise).normalized;
        }

        #endregion NORMAL

        #region ROUND

        /// <summary>Gets a vector's copy with all components rounded.</summary>
        public static Vector3 RoundAll(this Vector3 v)
        {
            return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
        }

        /// <summary>Gets a vector's copy with x component rounded.</summary>
        public static Vector3 RoundX(this Vector3 v)
        {
            return new Vector3(Mathf.Round(v.x), v.y, v.z);
        }

        /// <summary>Gets a vector's copy with y component rounded.</summary>
        public static Vector3 RoundY(this Vector3 v)
        {
            return new Vector3(v.x, Mathf.Round(v.y), v.z);
        }

        /// <summary>Gets a vector's copy with z component rounded.</summary>
        public static Vector3 RoundZ(this Vector3 v)
        {
            return new Vector3(v.x, v.y, Mathf.Round(v.z));
        }

        #endregion ROUND

        #region WITH

        /// <summary>Gets a vector's copy with new x value.</summary>
        /// <param name="value">New x value.</param>
        public static Vector3 WithX(this Vector3 v, float value)
        {
            return new Vector3(value, v.y, v.z);
        }

        /// <summary>Gets a vector's copy with new x value.</summary>
        /// <param name="value">New x value.</param>
        public static Vector3Int WithX(this Vector3Int v, int value)
        {
            return new Vector3Int(value, v.y, v.z);
        }

        /// <summary>Gets a vector's copy with new y value.</summary>
        /// <param name="value">New y value.</param>
        public static Vector3 WithY(this Vector3 v, float value)
        {
            return new Vector3(v.x, value, v.z);
        }

        /// <summary>Gets a vector's copy with new y value.</summary>
        /// <param name="value">New y value.</param>
        public static Vector3Int WithY(this Vector3Int v, int value)
        {
            return new Vector3Int(v.x, value, v.z);
        }

        /// <summary>Gets a vector's copy with new z value.</summary>
        /// <param name="value">New z value.</param>
        public static Vector3 WithZ(this Vector3 v, float value)
        {
            return new Vector3(v.x, v.y, value);
        }

        /// <summary>Gets a vector's copy with new z value.</summary>
        /// <param name="value">New z value.</param>
        public static Vector3Int WithZ(this Vector3Int v, int value)
        {
            return new Vector3Int(v.x, v.y, value);
        }

        #endregion WITH

        #endregion VECTOR3
    }
}