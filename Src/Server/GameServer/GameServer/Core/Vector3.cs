using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Utils;

namespace GameServer.Core
{
    public struct Vector3 : IEquatable<Vector3>
    {
        public const float kEpsilon = 1E-05f;

        public const float kEpsilonNormalSqrt = 1E-15f;

        /// <summary>
        ///   <para>X component of the vector.</para>
        /// </summary>
        public float x;

        /// <summary>
        ///   <para>Y component of the vector.</para>
        /// </summary>
        public float y;

        /// <summary>
        ///   <para>Z component of the vector.</para>
        /// </summary>
        public float z;

        private static readonly Vector3 zeroVector = new Vector3(0f, 0f, 0f);

        private static readonly Vector3 oneVector = new Vector3(1f, 1f, 1f);

        private static readonly Vector3 upVector = new Vector3(0f, 1f, 0f);

        private static readonly Vector3 downVector = new Vector3(0f, -1f, 0f);

        private static readonly Vector3 leftVector = new Vector3(-1f, 0f, 0f);

        private static readonly Vector3 rightVector = new Vector3(1f, 0f, 0f);

        private static readonly Vector3 forwardVector = new Vector3(0f, 0f, 1f);

        private static readonly Vector3 backVector = new Vector3(0f, 0f, -1f);

        private static readonly Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        private static readonly Vector3 negativeInfinityVector = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        /// <summary>
        ///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
        /// </summary>
        public Vector3 normalized => Normalize(this);

        /// <summary>
        ///   <para>Returns the length of this vector (Read Only).</para>
        /// </summary>
        public float magnitude => (float)Math.Sqrt(x * x + y * y + z * z);

        /// <summary>
        ///   <para>Returns the squared length of this vector (Read Only).</para>
        /// </summary>
        public float sqrMagnitude => x * x + y * y + z * z;

        /// <summary>
        ///   <para>Shorthand for writing Vector3(0, 0, 0).</para>
        /// </summary>
        public static Vector3 zero => zeroVector;

        /// <summary>
        ///   <para>Shorthand for writing Vector3(1, 1, 1).</para>
        /// </summary>
        public static Vector3 one => oneVector;

        /// <summary>
        ///   <para>Shorthand for writing Vector3(0, 0, 1).</para>
        /// </summary>
        public static Vector3 forward => forwardVector;

        /// <summary>
        ///   <para>Shorthand for writing Vector3(0, 0, -1).</para>
        /// </summary>
        public static Vector3 back => backVector;

        /// <summary>
        ///   <para>Shorthand for writing Vector3(0, 1, 0).</para>
        /// </summary>
        public static Vector3 up => upVector;

        /// <summary>
        ///   <para>Shorthand for writing Vector3(0, -1, 0).</para>
        /// </summary>
        public static Vector3 down => downVector;

        /// <summary>
        ///   <para>Shorthand for writing Vector3(-1, 0, 0).</para>
        /// </summary>
        public static Vector3 left => leftVector;

        /// <summary>
        ///   <para>Shorthand for writing Vector3(1, 0, 0).</para>
        /// </summary>
        public static Vector3 right => rightVector;

        /// <summary>
        ///   <para>Shorthand for writing Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity).</para>
        /// </summary>
        public static Vector3 positiveInfinity => positiveInfinityVector;

        /// <summary>
        ///   <para>Shorthand for writing Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity).</para>
        /// </summary>
        public static Vector3 negativeInfinity => negativeInfinityVector;

        [Obsolete("Use Vector3.forward instead.")]
        public static Vector3 fwd => new Vector3(0f, 0f, 1f);



        /// <summary>
        ///   <para>Linearly interpolates between two points.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = MathUtil.Clamp01(t);
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        /// <summary>
        ///   <para>Linearly interpolates between two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        /// <summary>
        ///   <para>Calculate a position between the points specified by current and target, moving no farther than the distance specified by maxDistanceDelta.</para>
        /// </summary>
        /// <param name="current">The position to move from.</param>
        /// <param name="target">The position to move towards.</param>
        /// <param name="maxDistanceDelta">Distance to move current per call.</param>
        /// <returns>
        ///   <para>The new position.</para>
        /// </returns>
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            float num = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = target.z - current.z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            if (num4 == 0f || (maxDistanceDelta >= 0f && num4 <= maxDistanceDelta * maxDistanceDelta))
            {
                return target;
            }
            float num5 = (float)Math.Sqrt(num4);
            return new Vector3(current.x + num / num5 * maxDistanceDelta, current.y + num2 / num5 * maxDistanceDelta, current.z + num3 / num5 * maxDistanceDelta);
        }

        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
        {
            float deltaTime = Time.deltaTime;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
        {
            float deltaTime = Time.deltaTime;
            float maxSpeed = float.PositiveInfinity;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
        {
            float num = 0f;
            float num2 = 0f;
            float num3 = 0f;
            smoothTime = Math.Max(0.0001f, smoothTime);
            float num4 = 2f / smoothTime;
            float num5 = num4 * deltaTime;
            float num6 = 1f / (1f + num5 + 0.48f * num5 * num5 + 0.235f * num5 * num5 * num5);
            float num7 = current.x - target.x;
            float num8 = current.y - target.y;
            float num9 = current.z - target.z;
            Vector3 vector = target;
            float num10 = maxSpeed * smoothTime;
            float num11 = num10 * num10;
            float num12 = num7 * num7 + num8 * num8 + num9 * num9;
            if (num12 > num11)
            {
                float num13 = (float)Math.Sqrt(num12);
                num7 = num7 / num13 * num10;
                num8 = num8 / num13 * num10;
                num9 = num9 / num13 * num10;
            }
            target.x = current.x - num7;
            target.y = current.y - num8;
            target.z = current.z - num9;
            float num14 = (currentVelocity.x + num4 * num7) * deltaTime;
            float num15 = (currentVelocity.y + num4 * num8) * deltaTime;
            float num16 = (currentVelocity.z + num4 * num9) * deltaTime;
            currentVelocity.x = (currentVelocity.x - num4 * num14) * num6;
            currentVelocity.y = (currentVelocity.y - num4 * num15) * num6;
            currentVelocity.z = (currentVelocity.z - num4 * num16) * num6;
            num = target.x + (num7 + num14) * num6;
            num2 = target.y + (num8 + num15) * num6;
            num3 = target.z + (num9 + num16) * num6;
            float num17 = vector.x - current.x;
            float num18 = vector.y - current.y;
            float num19 = vector.z - current.z;
            float num20 = num - vector.x;
            float num21 = num2 - vector.y;
            float num22 = num3 - vector.z;
            if (num17 * num20 + num18 * num21 + num19 * num22 > 0f)
            {
                num = vector.x;
                num2 = vector.y;
                num3 = vector.z;
                currentVelocity.x = (num - vector.x) / deltaTime;
                currentVelocity.y = (num2 - vector.y) / deltaTime;
                currentVelocity.z = (num3 - vector.z) / deltaTime;
            }
            return new Vector3(num, num2, num3);
        }

        /// <summary>
        ///   <para>Creates a new vector with given x, y, z components.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        ///   <para>Creates a new vector with given x, y components and sets z to zero.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0f;
        }

        /// <summary>
        ///   <para>Set x, y and z components of an existing Vector3.</para>
        /// </summary>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        /// <param name="newZ"></param>
        public void Set(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        /// <summary>
        ///   <para>Multiplies two vectors component-wise.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static Vector3 Scale(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        /// <summary>
        ///   <para>Multiplies every component of this vector by the same component of scale.</para>
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(Vector3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        /// <summary>
        ///   <para>Cross Product of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }

        /// <summary>
        ///   <para>Returns true if the given vector is exactly equal to this vector.</para>
        /// </summary>
        /// <param name="other"></param>
        public override bool Equals(object other)
        {
            if (!(other is Vector3))
            {
                return false;
            }
            return Equals((Vector3)other);
        }

        public bool Equals(Vector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        /// <summary>
        ///   <para>Reflects a vector off the plane defined by a normal.</para>
        /// </summary>
        /// <param name="inDirection"></param>
        /// <param name="inNormal"></param>
        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
        {
            float num = -2f * Dot(inNormal, inDirection);
            return new Vector3(num * inNormal.x + inDirection.x, num * inNormal.y + inDirection.y, num * inNormal.z + inDirection.z);
        }

        /// <summary>
        ///   <para>Makes this vector have a magnitude of 1.</para>
        /// </summary>
        /// <param name="value"></param>
        public static Vector3 Normalize(Vector3 value)
        {
            float num = Magnitude(value);
            if (num > 1E-05f)
            {
                return value / num;
            }
            return zero;
        }

        public void Normalize()
        {
            float num = Magnitude(this);
            if (num > 1E-05f)
            {
                this /= num;
            }
            else
            {
                this = zero;
            }
        }

        /// <summary>
        ///   <para>Dot Product of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }


        /// <summary>
        ///   <para>Returns the angle in degrees between from and to.</para>
        /// </summary>
        /// <param name="from">The vector from which the angular difference is measured.</param>
        /// <param name="to">The vector to which the angular difference is measured.</param>
        /// <returns>
        ///   <para>The angle in degrees between the two vectors.</para>
        /// </returns>
        public static float Angle(Vector3 from, Vector3 to)
        {
            float num = (float)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (num < 1E-15f)
            {
                return 0f;
            }
            float num2 = MathUtil.Clamp(Dot(from, to) / num, -1f, 1f);
            return (float)Math.Acos(num2) * 57.29578f;
        }

        /// <summary>
        ///   <para>Returns the signed angle in degrees between from and to.</para>
        /// </summary>
        /// <param name="from">The vector from which the angular difference is measured.</param>
        /// <param name="to">The vector to which the angular difference is measured.</param>
        /// <param name="axis">A vector around which the other vectors are rotated.</param>
        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            float num = Angle(from, to);
            float num2 = from.y * to.z - from.z * to.y;
            float num3 = from.z * to.x - from.x * to.z;
            float num4 = from.x * to.y - from.y * to.x;
            float num5 = (float)Math.Sign(axis.x * num2 + axis.y * num3 + axis.z * num4);
            return num * num5;
        }

        /// <summary>
        ///   <para>Returns the distance between a and b.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Distance(Vector3 a, Vector3 b)
        {
            float num = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3);
        }

        /// <summary>
        ///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="maxLength"></param>
        public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
        {
            float sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude > maxLength * maxLength)
            {
                float num = (float)Math.Sqrt(sqrMagnitude);
                float num2 = vector.x / num;
                float num3 = vector.y / num;
                float num4 = vector.z / num;
                return new Vector3(num2 * maxLength, num3 * maxLength, num4 * maxLength);
            }
            return vector;
        }

        public static float Magnitude(Vector3 vector)
        {
            return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        public static float SqrMagnitude(Vector3 vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }

        /// <summary>
        ///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector3 Min(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
        }

        /// <summary>
        ///   <para>Returns a vector that is made from the largest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector3 Max(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(0f - a.x, 0f - a.y, 0f - a.z);
        }

        public static Vector3 operator *(Vector3 a, float d)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator *(float d, Vector3 a)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            float num = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            float num3 = lhs.z - rhs.z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            return num4 < 9.99999944E-11f;
        }

        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        ///   <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1})", x, y, z);
        }

        /// <summary>
        ///   <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2})", x.ToString(format, CultureInfo.InvariantCulture.NumberFormat), y.ToString(format, CultureInfo.InvariantCulture.NumberFormat), z.ToString(format, CultureInfo.InvariantCulture.NumberFormat));
        }

        [Obsolete("Use Vector3.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
        public static float AngleBetween(Vector3 from, Vector3 to)
        {
            return (float)Math.Acos(MathUtil.Clamp(Dot(from.normalized, to.normalized), -1f, 1f));
        }

        public static implicit operator Vector3(Vector3Int v)
        {
            return new Vector3() { x = v.x, y = v.y, z = v.z };
        }
        public static implicit operator Vector3Int(Vector3 v)
        {
            return new Vector3Int((int)v.x, (int)v.y, (int)v.z);
        }

    }
}
