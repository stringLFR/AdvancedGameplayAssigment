using System;
using System.Collections.Generic;
using System.Numerics;


namespace UniGameMaths
{
    public static class UnityMaths
    {

        #region NumericsConversions
        public static System.Numerics.Vector3 GetNumericsVecFromUnityVec(UnityEngine.Vector3 unityVec)
        {
            return new System.Numerics.Vector3(unityVec.x, unityVec.y, unityVec.z);
        }

        

        public static UnityEngine.Vector3 GetUnityVecFromNumericsVec(System.Numerics.Vector3 NumericVec)
        {
            return new UnityEngine.Vector3(NumericVec.X, NumericVec.Y, NumericVec.Z);
        }

        public static UnityEngine.Vector3[] NumericsVectorArrayToUnity(System.Numerics.Vector3[] NumericVec)
        {
            UnityEngine.Vector3[] list = new UnityEngine.Vector3[NumericVec.Length];
            int index = 0;
            foreach (System.Numerics.Vector3 vec in NumericVec)
            {
                list[index++] = new UnityEngine.Vector3(vec.X, vec.Y, vec.Z);
            }
            return list;
        }
        #endregion
    }

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

    public static class BezierCurvesMaths
    {
        public struct CubicBezierCurve
        {
            private Vector3 p0, p1, p2, p3;

            public void SetVectors(Vector3 pa, Vector3 pb, Vector3 ta, Vector3 tb)
            {
                p0 = pa;
                p1 = pa + ta;
                p2 = pb - tb;
                p3 = pb;
            }

            public Vector3 GetFirstPoint() => p0;
            public Vector3 GetLastPoint() => p3;

            public Vector3 GetCubicBezierPosition(float f)
            {
                Math.Clamp(f, 0, 1);
                float fOneMinusT = 1.0f - f;
                return p0 * fOneMinusT * fOneMinusT * fOneMinusT +
                       p1 * 3 * fOneMinusT * fOneMinusT * f +
                       p2 * 3 * fOneMinusT * f * f +
                       p3 * f * f * f;
            }
        }
    }

    public static class GraphsMaths
    {
        
    }

    public static class MarchingMaths
    {
        public static Vector3 Interpolate(Vector3 vA, float fWeightA, Vector3 vB, float fWeightB, float iso)
        {
            float fBlend = (iso - fWeightA) / (fWeightB - fWeightA);
            return vA + (vB - vA) * fBlend;
        }

        #region MarchingCubes

        public struct MarchingCube
        {
            public float ISO;
            public List<Vector3> Vertices;
            public List<int> Triangles;
            public Vector3 Size;
            public float[,,] weights;
            public float[,,] Weights
            {
                get
                {
                    if (weights == null ||
                        weights.GetLength(0) != Size.X ||
                        weights.GetLength(1) != Size.Y ||
                        weights.GetLength(2) != Size.Z)
                    {
                        weights = new float[(int)Size.X, (int)Size.Y, (int)Size.Z];
                    }

                    return weights;
                }
            }
        }

        #region MarchingCubeTables

        public static readonly Vector3[] MarchingCube_CornerOffsets = new Vector3[]
        {
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0)
        };

        public static readonly int[][] MarchingCube_Edges = new int[][]
        {
            new int[]{0,1}, new int[]{1,2}, new int[]{2,3}, new int[]{3,0},
            new int[]{4,5}, new int[]{5,6}, new int[]{6,7}, new int[]{7,4},
            new int[]{0,4}, new int[]{1,5}, new int[]{2,6}, new int[]{3,7}
        };

        public static int[][] MarchingCube_TriangleTable = new int[][]
        {
            new int[]{ },
            new int[]{0, 8, 3},
            new int[]{0, 1, 9},
            new int[]{1, 8, 3, 9, 8, 1},
            new int[]{1, 2, 10},
            new int[]{0, 8, 3, 1, 2, 10},
            new int[]{9, 2, 10, 0, 2, 9},
            new int[]{2, 8, 3, 2, 10, 8, 10, 9, 8},
            new int[]{3, 11, 2},
            new int[]{0, 11, 2, 8, 11, 0},
            new int[]{1, 9, 0, 2, 3, 11},
            new int[]{1, 11, 2, 1, 9, 11, 9, 8, 11},
            new int[]{3, 10, 1, 11, 10, 3},
            new int[]{0, 10, 1, 0, 8, 10, 8, 11, 10},
            new int[]{3, 9, 0, 3, 11, 9, 11, 10, 9},
            new int[]{9, 8, 10, 10, 8, 11},
            new int[]{4, 7, 8},
            new int[]{4, 3, 0, 7, 3, 4},
            new int[]{0, 1, 9, 8, 4, 7},
            new int[]{4, 1, 9, 4, 7, 1, 7, 3, 1},
            new int[]{1, 2, 10, 8, 4, 7},
            new int[]{3, 4, 7, 3, 0, 4, 1, 2, 10},
            new int[]{9, 2, 10, 9, 0, 2, 8, 4, 7},
            new int[]{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4},
            new int[]{8, 4, 7, 3, 11, 2},
            new int[]{11, 4, 7, 11, 2, 4, 2, 0, 4},
            new int[]{9, 0, 1, 8, 4, 7, 2, 3, 11},
            new int[]{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1},
            new int[]{3, 10, 1, 3, 11, 10, 7, 8, 4},
            new int[]{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4},
            new int[]{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3},
            new int[]{4, 7, 11, 4, 11, 9, 9, 11, 10},
            new int[]{9, 5, 4},
            new int[]{9, 5, 4, 0, 8, 3},
            new int[]{0, 5, 4, 1, 5, 0},
            new int[]{8, 5, 4, 8, 3, 5, 3, 1, 5},
            new int[]{1, 2, 10, 9, 5, 4},
            new int[]{3, 0, 8, 1, 2, 10, 4, 9, 5},
            new int[]{5, 2, 10, 5, 4, 2, 4, 0, 2},
            new int[]{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8},
            new int[]{9, 5, 4, 2, 3, 11},
            new int[]{0, 11, 2, 0, 8, 11, 4, 9, 5},
            new int[]{0, 5, 4, 0, 1, 5, 2, 3, 11},
            new int[]{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5},
            new int[]{10, 3, 11, 10, 1, 3, 9, 5, 4},
            new int[]{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10},
            new int[]{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3},
            new int[]{5, 4, 8, 5, 8, 10, 10, 8, 11},
            new int[]{9, 7, 8, 5, 7, 9},
            new int[]{9, 3, 0, 9, 5, 3, 5, 7, 3},
            new int[]{0, 7, 8, 0, 1, 7, 1, 5, 7},
            new int[]{1, 5, 3, 3, 5, 7},
            new int[]{9, 7, 8, 9, 5, 7, 10, 1, 2},
            new int[]{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3},
            new int[]{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2},
            new int[]{2, 10, 5, 2, 5, 3, 3, 5, 7},
            new int[]{7, 9, 5, 7, 8, 9, 3, 11, 2},
            new int[]{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11},
            new int[]{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7},
            new int[]{11, 2, 1, 11, 1, 7, 7, 1, 5},
            new int[]{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11},
            new int[]{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0 },
            new int[]{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0 },
            new int[]{11, 10, 5, 7, 11, 5},
            new int[]{10, 6, 5},
            new int[]{0, 8, 3, 5, 10, 6},
            new int[]{9, 0, 1, 5, 10, 6},
            new int[]{1, 8, 3, 1, 9, 8, 5, 10, 6},
            new int[]{1, 6, 5, 2, 6, 1},
            new int[]{1, 6, 5, 1, 2, 6, 3, 0, 8},
            new int[]{9, 6, 5, 9, 0, 6, 0, 2, 6},
            new int[]{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8},
            new int[]{2, 3, 11, 10, 6, 5},
            new int[]{11, 0, 8, 11, 2, 0, 10, 6, 5},
            new int[]{0, 1, 9, 2, 3, 11, 5, 10, 6},
            new int[]{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11},
            new int[]{6, 3, 11, 6, 5, 3, 5, 1, 3},
            new int[]{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6},
            new int[]{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9},
            new int[]{6, 5, 9, 6, 9, 11, 11, 9, 8},
            new int[]{5, 10, 6, 4, 7, 8},
            new int[]{4, 3, 0, 4, 7, 3, 6, 5, 10},
            new int[]{1, 9, 0, 5, 10, 6, 8, 4, 7},
            new int[]{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4},
            new int[]{6, 1, 2, 6, 5, 1, 4, 7, 8},
            new int[]{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7},
            new int[]{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6},
            new int[]{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9 },
            new int[]{3, 11, 2, 7, 8, 4, 10, 6, 5},
            new int[]{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11},
            new int[]{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6},
            new int[]{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6 },
            new int[]{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6},
            new int[]{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11 },
            new int[]{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7 },
            new int[]{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9},
            new int[]{10, 4, 9, 6, 4, 10},
            new int[]{4, 10, 6, 4, 9, 10, 0, 8, 3},
            new int[]{10, 0, 1, 10, 6, 0, 6, 4, 0},
            new int[]{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10},
            new int[]{1, 4, 9, 1, 2, 4, 2, 6, 4},
            new int[]{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4},
            new int[]{0, 2, 4, 4, 2, 6},
            new int[]{8, 3, 2, 8, 2, 4, 4, 2, 6},
            new int[]{10, 4, 9, 10, 6, 4, 11, 2, 3},
            new int[]{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6},
            new int[]{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10},
            new int[]{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1 },
            new int[]{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3},
            new int[]{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1 },
            new int[]{3, 11, 6, 3, 6, 0, 0, 6, 4},
            new int[]{6, 4, 8, 11, 6, 8},
            new int[]{7, 10, 6, 7, 8, 10, 8, 9, 10},
            new int[]{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10},
            new int[]{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0},
            new int[]{10, 6, 7, 10, 7, 1, 1, 7, 3},
            new int[]{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7},
            new int[]{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9 },
            new int[]{7, 8, 0, 7, 0, 6, 6, 0, 2},
            new int[]{7, 3, 2, 6, 7, 2},
            new int[]{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7},
            new int[]{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7 },
            new int[]{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11 },
            new int[]{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1},
            new int[]{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6 },
            new int[]{0, 9, 1, 11, 6, 7},
            new int[]{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0},
            new int[]{7, 11, 6},
            new int[]{7, 6, 11},
            new int[]{3, 0, 8, 11, 7, 6},
            new int[]{0, 1, 9, 11, 7, 6},
            new int[]{8, 1, 9, 8, 3, 1, 11, 7, 6},
            new int[]{10, 1, 2, 6, 11, 7},
            new int[]{1, 2, 10, 3, 0, 8, 6, 11, 7},
            new int[]{2, 9, 0, 2, 10, 9, 6, 11, 7},
            new int[]{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8},
            new int[]{7, 2, 3, 6, 2, 7},
            new int[]{7, 0, 8, 7, 6, 0, 6, 2, 0},
            new int[]{2, 7, 6, 2, 3, 7, 0, 1, 9},
            new int[]{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6},
            new int[]{10, 7, 6, 10, 1, 7, 1, 3, 7},
            new int[]{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8},
            new int[]{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7},
            new int[]{7, 6, 10, 7, 10, 8, 8, 10, 9},
            new int[]{6, 8, 4, 11, 8, 6},
            new int[]{3, 6, 11, 3, 0, 6, 0, 4, 6},
            new int[]{8, 6, 11, 8, 4, 6, 9, 0, 1},
            new int[]{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6},
            new int[]{6, 8, 4, 6, 11, 8, 2, 10, 1},
            new int[]{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6},
            new int[]{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9},
            new int[]{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3 },
            new int[]{8, 2, 3, 8, 4, 2, 4, 6, 2},
            new int[]{0, 4, 2, 4, 6, 2},
            new int[]{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8},
            new int[]{1, 9, 4, 1, 4, 2, 2, 4, 6},
            new int[]{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1},
            new int[]{10, 1, 0, 10, 0, 6, 6, 0, 4},
            new int[]{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3 },
            new int[]{10, 9, 4, 6, 10, 4},
            new int[]{4, 9, 5, 7, 6, 11},
            new int[]{0, 8, 3, 4, 9, 5, 11, 7, 6},
            new int[]{5, 0, 1, 5, 4, 0, 7, 6, 11},
            new int[]{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5},
            new int[]{9, 5, 4, 10, 1, 2, 7, 6, 11},
            new int[]{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5},
            new int[]{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2},
            new int[]{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6 },
            new int[]{7, 2, 3, 7, 6, 2, 5, 4, 9},
            new int[]{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7},
            new int[]{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0},
            new int[]{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8 },
            new int[]{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7},
            new int[]{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4 },
            new int[]{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10 },
            new int[]{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10},
            new int[]{6, 9, 5, 6, 11, 9, 11, 8, 9},
            new int[]{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5},
            new int[]{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11},
            new int[]{6, 11, 3, 6, 3, 5, 5, 3, 1},
            new int[]{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6},
            new int[]{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10 },
            new int[]{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5 },
            new int[]{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3},
            new int[]{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2},
            new int[]{9, 5, 6, 9, 6, 0, 0, 6, 2},
            new int[]{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8 },
            new int[]{1, 5, 6, 2, 1, 6},
            new int[]{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6 },
            new int[]{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0},
            new int[]{0, 3, 8, 5, 6, 10},
            new int[]{10, 5, 6},
            new int[]{11, 5, 10, 7, 5, 11},
            new int[]{11, 5, 10, 11, 7, 5, 8, 3, 0},
            new int[]{5, 11, 7, 5, 10, 11, 1, 9, 0},
            new int[]{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1},
            new int[]{11, 1, 2, 11, 7, 1, 7, 5, 1},
            new int[]{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11},
            new int[]{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7},
            new int[]{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2 },
            new int[]{2, 5, 10, 2, 3, 5, 3, 7, 5},
            new int[]{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5},
            new int[]{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2},
            new int[]{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2 },
            new int[]{1, 3, 5, 3, 7, 5},
            new int[]{0, 8, 7, 0, 7, 1, 1, 7, 5},
            new int[]{9, 0, 3, 9, 3, 5, 5, 3, 7},
            new int[]{9, 8, 7, 5, 9, 7},
            new int[]{5, 8, 4, 5, 10, 8, 10, 11, 8},
            new int[]{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0},
            new int[]{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5},
            new int[]{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4 },
            new int[]{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8},
            new int[]{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11 },
            new int[]{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5 },
            new int[]{9, 4, 5, 2, 11, 3},
            new int[]{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4},
            new int[]{5, 10, 2, 5, 2, 4, 4, 2, 0},
            new int[]{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9 },
            new int[]{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2},
            new int[]{8, 4, 5, 8, 5, 3, 3, 5, 1},
            new int[]{0, 4, 5, 1, 0, 5},
            new int[]{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5},
            new int[]{9, 4, 5},
            new int[]{4, 11, 7, 4, 9, 11, 9, 10, 11},
            new int[]{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11},
            new int[]{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11},
            new int[]{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4 },
            new int[]{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2},
            new int[]{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3 },
            new int[]{11, 7, 4, 11, 4, 2, 2, 4, 0},
            new int[]{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4},
            new int[]{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9},
            new int[]{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7 },
            new int[]{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10 },
            new int[]{1, 10, 2, 8, 7, 4},
            new int[]{4, 9, 1, 4, 1, 7, 7, 1, 3},
            new int[]{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1},
            new int[]{4, 0, 3, 7, 4, 3},
            new int[]{4, 8, 7},
            new int[]{9, 10, 8, 10, 11, 8},
            new int[]{3, 0, 9, 3, 9, 11, 11, 9, 10},
            new int[]{0, 1, 10, 0, 10, 8, 8, 10, 11},
            new int[]{3, 1, 10, 11, 3, 10},
            new int[]{1, 2, 11, 1, 11, 9, 9, 11, 8},
            new int[]{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9},
            new int[]{0, 2, 11, 8, 0, 11},
            new int[]{3, 2, 11},
            new int[]{2, 3, 8, 2, 8, 10, 10, 8, 9},
            new int[]{9, 10, 2, 0, 9, 2},
            new int[]{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8},
            new int[]{1, 10, 2},
            new int[]{1, 3, 8, 9, 1, 8},
            new int[]{0, 9, 1},
            new int[]{0, 3, 8},
            new int[]{ }
        };

        #endregion
        public static int MarchingCube_GetLookup(Vector3 cube, float iso, float[,,] w)
        {
            int iLookup = 0;
            for (int i = 0; i < MarchingCube_CornerOffsets.Length; ++i)
            {
                Vector3 vCorner = cube + MarchingCube_CornerOffsets[i];
                bool bActive = w[(int)vCorner.X, (int)vCorner.Y, (int)vCorner.Z] >= iso;
                if (bActive)
                {
                    iLookup |= (1 << i);
                }
            }

            return iLookup;
        }

        public static Vector3 GetCornerPosition(Vector3 vCube, int iCorner)
        {
            return vCube + MarchingCube_CornerOffsets[iCorner];
        }

        public static MarchingCube CreateMarchingCube(float iso, Vector3 size, float[,,] newWeight)
        {
            MarchingCube marchingCube = new MarchingCube();

            marchingCube.Size = size;
            marchingCube.ISO = iso;
            marchingCube.Vertices = new List<Vector3>();
            marchingCube.Triangles = new List<int>();

            for (int z = 0; z < marchingCube.Size.Z; ++z)
            {
                float fZ = (z / (float)marchingCube.Size.Z) * 3.0f;

                for (int y = 0; y < marchingCube.Size.Y; ++y)
                {
                    float fY = (y / (float)marchingCube.Size.Y) * 3.0f;

                    for (int x = 0; x < marchingCube.Size.X; ++x)
                    {
                        marchingCube.Weights[x, y, z] = newWeight[x, y, z];
                    }
                }
            }

            Dictionary<Vector3, int> vertexLookUp = new Dictionary<Vector3, int>();

            for (int z = 0; z < marchingCube.Size.Z - 1; ++z)
            {
                for (int y = 0; y < marchingCube.Size.Y - 1; ++y)
                {
                    for (int x = 0; x < marchingCube.Size.X - 1; ++x)
                    {
                        Vector3 vCube = new Vector3(x, y, z);
                        int iLookup = MarchingCube_GetLookup(vCube, marchingCube.ISO, marchingCube.Weights);

                        float[] cubeWeights = Array.ConvertAll(MarchingCube_CornerOffsets, c => marchingCube.Weights[x + (int)c.X, y + (int)c.Y, z + (int)c.Z]);

                        int[] cubeTriangles = MarchingCube_TriangleTable[iLookup];

                        for (int i = 0; i < cubeTriangles.Length; i += 3)
                        {
                            int e00 = MarchingCube_Edges[cubeTriangles[i + 0]][0];
                            int e01 = MarchingCube_Edges[cubeTriangles[i + 0]][1];

                            int e10 = MarchingCube_Edges[cubeTriangles[i + 1]][0];
                            int e11 = MarchingCube_Edges[cubeTriangles[i + 1]][1];

                            int e20 = MarchingCube_Edges[cubeTriangles[i + 2]][0];
                            int e21 = MarchingCube_Edges[cubeTriangles[i + 2]][1];

                            Vector3 vIso1 = Interpolate(GetCornerPosition(vCube, e00), cubeWeights[e00],
                                                        GetCornerPosition(vCube, e01), cubeWeights[e01], marchingCube.ISO);

                            Vector3 vIso2 = Interpolate(GetCornerPosition(vCube, e10), cubeWeights[e10],
                                                        GetCornerPosition(vCube, e11), cubeWeights[e11], marchingCube.ISO);

                            Vector3 vIso3 = Interpolate(GetCornerPosition(vCube, e20), cubeWeights[e20],
                                                        GetCornerPosition(vCube, e21), cubeWeights[e21], marchingCube.ISO);

                            int iStart = marchingCube.Vertices.Count;
                            
                            if (vertexLookUp.TryGetValue(vIso1, out int value1) == false)
                            {
                                vertexLookUp.Add(vIso1, iStart);
                                marchingCube.Vertices.Add(vIso1);
                                marchingCube.Triangles.Add(iStart);
                                iStart++;
                            }
                            else
                            {
                                marchingCube.Triangles.Add(value1);
                            }

                            if (vertexLookUp.TryGetValue(vIso3, out int value3) == false)
                            {
                                vertexLookUp.Add(vIso3, iStart);
                                marchingCube.Vertices.Add(vIso3);
                                marchingCube.Triangles.Add(iStart);
                                iStart++;
                            }
                            else
                            {
                                marchingCube.Triangles.Add(value3);
                            }

                            if (vertexLookUp.TryGetValue(vIso2, out int value2) == false)
                            {
                                vertexLookUp.Add(vIso2, iStart);
                                marchingCube.Vertices.Add(vIso2);
                                marchingCube.Triangles.Add(iStart);
                                iStart++;
                            }
                            else
                            {
                                marchingCube.Triangles.Add(value2);
                            }
                        }

                    }
                }
            }
            return marchingCube;
        }


        #endregion
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


