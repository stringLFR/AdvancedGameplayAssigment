using UnityEditor;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.Rendering;
using UniGameMaths;
using Unity.VisualScripting;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MarchingTerrain : MonoBehaviour
{
    private Mesh m_mesh;

    [SerializeField]
    float iso = 0.5f;

    [SerializeField]
    Vector3 sizeVec = Vector3.one;

    public void Create()
    {
        MarchingMaths.MarchingCube cube = MarchingMaths.CreateMarchingCube(iso, UnityMaths.GetNumericsVecFromUnityVec(sizeVec),
            MarchingMaths.CreateWeightByRandom(UnityMaths.GetNumericsVecFromUnityVec(sizeVec)));

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
            foreach (var t in cube.Vertices)
            {
                m_mesh.vertices.AddRange(new Vector3[] { UnityMaths.GetUnityVecFromNumericsVec(t) });

                m_mesh.colors.AddRange(new Color[] { Color.red, Color.red, Color.red });
            }
            m_mesh.triangles = cube.Triangles.ToArray();
            m_mesh.RecalculateBounds();
            m_mesh.RecalculateNormals();
        }

        // assign mesh
        GetComponent<MeshFilter>().mesh = m_mesh;
    }
}

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
