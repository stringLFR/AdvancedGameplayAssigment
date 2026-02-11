using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UniGameMaths;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MarchingTerrain : MonoBehaviour
{
    private Mesh m_mesh;

    [SerializeField]
    float iso = 0.5f, randomModifier = 3f;

    [SerializeField]
    UnityEngine.Vector3 sizeVec = UnityEngine.Vector3.one;

    [SerializeField]
    public float groundLevel, aboveGroundLevel, firstAirLevel, secondAirLevel, finalAirLevel;

    public static float[,,] CreateWeightByRandom(System.Numerics.Vector3 size, MarchingTerrain t, float modifier = 3f)
    {
        float[,,] w = new float[(int)size.X, (int)size.Y, (int)size.Z];

        float rand = UnityEngine.Random.Range(1f, 10f) + UnityEngine.Random.Range(1f, 10f);

        for (int z = 0; z < size.Z; ++z)
        {
            float fZ = ((z * rand) / (float)size.Z - 1) * modifier;

            for (int y = 0; y < size.Y; ++y)
            {
                float fY = ((y * rand) / (float)size.Y - 1) * modifier;

                for (int x = 0; x < size.X; ++x)
                {
                    float fX = ((x * rand) / (float)size.X - 1) * modifier;

                    float perlin = UnityEngine.Mathf.PerlinNoise(fX, fY) * UnityEngine.Mathf.PerlinNoise(fY, fZ);
                    
                    float final = 0;

                    if (y < t.groundLevel)
                    {
                        final = perlin * 1f;
                    }
                    else if (y > t.groundLevel && y < t.aboveGroundLevel)
                    {
                        final = perlin * 0.75f;
                    }
                    else if (y > t.groundLevel && y > t.aboveGroundLevel && y < t.firstAirLevel)
                    {
                        final = perlin * 0.5f;
                    }
                    else if (y > t.groundLevel && y > t.aboveGroundLevel && y > t.firstAirLevel && y < t.secondAirLevel)
                    {
                        final = perlin * 0.25f;
                    }
                    else if (y > t.groundLevel && y > t.aboveGroundLevel && y > t.firstAirLevel && y > t.secondAirLevel && y < t.finalAirLevel)
                    {
                        final = perlin * 0.125f;
                    }

                    w[x, y, z] = perlin;
                }
            }
        }
        return w;
    }

    public void Create()
    {
        MarchingMaths.MarchingCube cube = MarchingMaths.CreateMarchingCube(iso, UnityMaths.GetNumericsVecFromUnityVec(sizeVec),
            CreateWeightByRandom(UnityMaths.GetNumericsVecFromUnityVec(sizeVec), this,randomModifier));

        // create mesh
        if (m_mesh == null)
        {
            m_mesh = new Mesh();
            m_mesh.name = name;
            m_mesh.hideFlags = HideFlags.DontSave;
        }

        // write mesh data?
        m_mesh.Clear();
        m_mesh.indexFormat = cube.Vertices.Count > ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
        if (cube.Vertices.Count > 0 && cube.Triangles.Count > 0)
        {
            m_mesh.vertices = UnityMaths.NumericsVectorArrayToUnity(cube.Vertices.ToArray());
            m_mesh.triangles = cube.Triangles.ToArray();
            m_mesh.RecalculateBounds();
            m_mesh.RecalculateNormals();
        }

        // assign mesh
        GetComponent<MeshFilter>().mesh = m_mesh;
    }
}

/*

[CustomEditor(typeof(MarchingTerrain))]
public class MarchingTerrainTool : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MarchingTerrain Tool = (MarchingTerrain)target;

        if (GUILayout.Button("Generate Mesh"))
        {
            Tool.Create();
        }
    }
}

*/
