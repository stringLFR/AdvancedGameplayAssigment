using System;
using System.Numerics;

namespace UniGameMaths
{
    public static class SegmentMaths
    {
        public static Vector3 ClosestPointOnSegment(Vector3 point, Vector3 VecA, Vector3 vecB)
        {
            Vector3 AP = point - VecA;
            Vector3 AB = vecB - VecA;

            float magnitudeAB = AB.LengthSquared();
            float ABAPproduct = Vector3.Dot(AP, AB);
            float distance = ABAPproduct / magnitudeAB;

            if (distance < 0) return VecA;
            else if (distance > 1) return vecB;
            else return VecA + AB * distance;
        }

        public static float SegmentSegmentGetIntersectPoints(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 p1p2, out Vector3 p3p4)
        {
            p1p2 = Vector3.Zero;
            p3p4 = Vector3.Zero;

            Vector3 p13 = p1 - p3;
            Vector3 p43 = p4 - p3;
            Vector3 p21 = p2 - p1;

            float d1343 = p13.X * p43.X + p13.Y * p43.Y + p13.Z * p43.Z;
            float d4321 = p43.X * p21.X + p43.Y * p21.Y + p43.Z * p21.Z;
            float d1321 = p13.X * p21.X + p13.Y * p21.Y + p13.Z * p21.Z;
            float d4343 = p43.X * p43.X + p43.Y * p43.Y + p43.Z * p43.Z;
            float d2121 = p21.X * p21.X + p21.Y * p21.Y + p21.Z * p21.Z;

            float denom = d2121 * d4343 - d4321 * d4321;
            float numer = d1343 * d4321 - d1321 * d4343;
            float mua = numer / denom;
            float mub = (d1343 + d4321 * mua) / d4343;

            p1p2 = p1 + Math.Clamp(mua,0,1) * p21;
            p3p4 = p3 + Math.Clamp(mub, 0, 1) * p43;
            Vector3 dist = p3p4 - p1p2;

            // Returns the distance between the closes points
            return dist.Length();
        }
    }

    public static class BaryCentricCoordMaths
    {

    }

    //Easing funcs returns a value which can be used in lerps.
    //Remmember that sometimes you need to use unclamped lerps for full effect!
    //https://easings.net/ Shows visuals of the easing functions!
    public static class EasingFunctionMaths
    {
        #region Sine 
        public static float EaseInSine(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(1 - Math.Cos((x * Math.PI) / 2));
        }

        public static float EaseOutSine(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)Math.Sin((x * Math.PI) / 2);
        }

        public static float EaseInOutSine(float x)
        {
            Math.Clamp(x, 0, 1);
            return -(float)(Math.Cos(Math.PI * x) - 1) / 2;
        }
        #endregion

        #region Quad
        public static float EaseInQuad(float x)
        {
            Math.Clamp(x, 0, 1);
            return x * x;
        }

        public static float EaseOutQuad(float x)
        {
            Math.Clamp(x, 0, 1);
            return 1 - (1 - x) * (1 - x);
        }

        public static float EaseInOutQuad(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2);
        }
        #endregion

        #region Cubic
        public static float EaseInCubic(float x)
        {
            Math.Clamp(x, 0, 1);
            return x * x * x;
        }

        public static float EaseOutCubic(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(1 - Math.Pow(1 - x, 3));
        }

        public static float EaseInOutCubic(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2);
        }
        #endregion

        #region Quart
        public static float EaseInQuart(float x)
        {
            Math.Clamp(x, 0, 1);
            return x * x * x * x;
        }

        public static float EaseOutQuart(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(1 - Math.Pow(1 - x, 4));
        }

        public static float EaseInOutQuart(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(x < 0.5 ? 8 * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 4) / 2);
        }
        #endregion

        #region Quint
        public static float EaseInQuint(float x)
        {
            Math.Clamp(x, 0, 1);
            return x * x * x * x * x;
        }

        public static float EaseOutQuint(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(1 - Math.Pow(1 - x, 5));
        }

        public static float EaseInOutQuint(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2);
        }
        #endregion

        #region Expo
        public static float EaseInExpo(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(x == 0 ? 0 : Math.Pow(2, 10 * x - 10));
        }

        public static float EaseOutExpo(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(x == 1 ? 1 : 1 - Math.Pow(2, -10 * x));
        }

        public static float EaseInOutExpo(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(x == 0 ? 0 : x == 1 ? 1 : x < 0.5 ? Math.Pow(2, 20 * x - 10) / 2 : (2 - Math.Pow(2, -20 * x + 10)) / 2);
        }
        #endregion

        #region Circ
        public static float EaseInCirc(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(1 - Math.Sqrt(1 - Math.Pow(x, 2)));
        }

        public static float EaseOutCirc(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(Math.Sqrt(1 - Math.Pow(x - 1, 2)));
        }

        public static float EaseInOutCirc(float x)
        {
            Math.Clamp(x, 0, 1);
            return (float)(x < 0.5 ? (1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2 : (Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2);
        }
        #endregion

        #region Back
        public static float EaseInBack(float x)
        {
            Math.Clamp(x, 0, 1);
            const double c1 = 1.70158f;
            const double c3 = c1 + 1;

            return (float)(c3 * x * x * x - c1 * x * x);
        }

        public static float EaseOutBack(float x)
        {
            Math.Clamp(x, 0, 1);
            const double c1 = 1.70158;
            const double c3 = c1 + 1;

            return (float)(1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2));
        }

        public static float EaseInOutBack(float x)
        {
            Math.Clamp(x, 0, 1);
            const double c1 = 1.70158;
            const double c2 = c1 * 1.525;

            return (float)(x < 0.5 ? (Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2 : (Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2);
        }
        #endregion

        #region Elastic
        public static float EaseInElastic(float x)
        {
            Math.Clamp(x, 0, 1);
            const double c4 = (2 * Math.PI) / 3;

            return (float)(x == 0 ? 0 : x == 1 ? 1 : -Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * c4));
        }

        public static float EaseOutElastic(float x)
        {
            Math.Clamp(x, 0, 1);
            const double c4 = (2 * Math.PI) / 3;

            return (float)(x == 0 ? 0 : x == 1 ? 1 : Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * c4) + 1);
        }

        public static float EaseInOutElastic(float x)
        {
            Math.Clamp(x, 0, 1);
            const double c5 = (2 * Math.PI) / 4.5;

            return (float)(x == 0 ? 0 : x == 1 ? 1 : x < 0.5 ? -(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * c5)) / 2 : (Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * c5)) / 2 + 1);
        }
        #endregion

        #region Bounce
        public static float EaseInBounce(float x)
        {
            Math.Clamp(x, 0, 1);
            return 1 - EaseOutBounce(1 - x);
        }

        public static float EaseOutBounce(float x)
        {
            Math.Clamp(x, 0, 1);
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (x < 1 / d1)
            {
                return (float)(n1 * x * x);
            }
            else if (x < 2 / d1)
            {
                return (float)(n1 * (x -= 1.5f / d1) * x + 0.75);
            }
            else if (x < 2.5 / d1)
            {
                return (float)(n1 * (x -= 2.25f / d1) * x + 0.9375);
            }
            else
            {
                return (float)(n1 * (x -= 2.625f / d1) * x + 0.984375);
            }
        }

        public static float EaseInOutBounce(float x)
        {
            Math.Clamp(x, 0, 1);
            return x < 0.5 ? (1 - EaseOutBounce(1 - 2 * x)) / 2 : (1 + EaseOutBounce(2 * x - 1)) / 2;
        }
        #endregion
    }
}


