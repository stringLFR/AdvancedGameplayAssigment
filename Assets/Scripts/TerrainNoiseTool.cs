using UnityEngine;
using UnityEditor;

public class TerrainNoiseTool : MonoBehaviour
{
    [SerializeField]
    public Terrain terrain;

    [SerializeField]
    public int depth = 20, width = 256, height = 256;

    [SerializeField]
    public float scale = 20f;

    public float offsetX = 100f, offsetY = 100f;
}

[CustomEditor(typeof(TerrainNoiseTool))]
public class TerrainNoise : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainNoiseTool terrainNoiseTool = (TerrainNoiseTool)target;

        if (GUILayout.Button("Generate Noise"))
        {
            terrainNoiseTool.offsetX = UnityEngine.Random.Range(0f, 9999f);
            terrainNoiseTool.offsetY = UnityEngine.Random.Range(0f, 9999f);

            terrainNoiseTool.terrain.terrainData = GenerateNoiseTerrain(terrainNoiseTool);
        }
    }

    private TerrainData GenerateNoiseTerrain(TerrainNoiseTool data)
    {
        data.terrain.terrainData.heightmapResolution = data.width + 1;

        data.terrain.terrainData.size = new Vector3(data.width, data.depth, data.height);

        data.terrain.terrainData.SetHeights(0, 0, GenerateHeights(data));

        return data.terrain.terrainData;
    }

    private float[,] GenerateHeights(TerrainNoiseTool data)
    {
        float[,] heights = new float[data.width,data.height];

        for (int x = 0; x < data.width; x++)
        {
            for (int y = 0; y < data.height; y++)
            {
                heights[x, y] = CalculateHeight(x, y,data);
            }
        }

        return heights;
    }

    private float CalculateHeight(int x, int y, TerrainNoiseTool data)
    {
        float xCood = (float)x / data.width * data.scale + data.offsetX;
        float yCood = (float)y / data.height * data.scale + data.offsetY;

        return Mathf.PerlinNoise(xCood, yCood);
    }
}
