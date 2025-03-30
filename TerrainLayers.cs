//mycode
using UnityEngine;

public class TerrainLayers : MonoBehaviour
{
    private Terrain terrain;
    public float[] thresholds;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [ContextMenu("Apply Layers Based On Height")]
    void Start()
    {
        terrain = gameObject.GetComponent<Terrain>();
        ApplyLayersBasedOnHeight();
    }

    void ApplyLayersBasedOnHeight()
    {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;
        int layers = terrainData.alphamapLayers;

        float maxHeight = GetMaxHeight();

        float[,,] splatMap = new float[height, width, layers];//3d array in c #

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //float h = terrainData.GetHeight(y, x);
                //float normalizedHeight = h / maxHeight; // normlize persentage of height
                //int layerIndex = GetLayerBasedOnHeight(normalizedHeight);


                //for (int l = 0; l < layers; l++) // loop through all layers
                //{
                //    if (l == layerIndex)
                //        splatMap[x, y, l] = 1f;// layer am on = 1
                //    else 
                //        splatMap[x, y, l] = 0f; // other layers = 0 not visabel 
                //}
                // you can add more conditions to apply transition layers

                for (int t = 0; t < thresholds; t++)
                {
                    if (y/(float)height <= thresholds[t])
                    
                        splatMap[x, y, t] = 1f;
         
                    }
                }
            }
        }// obtained the maximum height , based on my elevation 

        terrainData.SetAlphamaps(0, 0, splatMap);
    }

    int GetLayerBasedOnHeight(float height) // order layers 0 with is persentages 
    {
        TerrainData terrainData = terrain.terrainData;
        if (terrainData.alphamapLayers == 1)// 1 layer dont do anything 
            return 0;

        int l;
        for (l = 0; l < thresholds.Length; l++)//values of thresholds of number of layer , to be able to adapt with any number of scripts 
        {
            if (height <= thresholds[l])
                return l;
        }

        return l;

    }
    float GetMaxHeight()
    {
        TerrainData data = terrain.terrainData;
        int width = data.heightmapResolution;
        int height = data.heightmapResolution;

        float maxHeight = float.MinValue;

        for (int y = 0; y < height; y++)// 2d array , 2 loops for x and y
        {
            for (int x = 0; x < width; x++)
            {
                float heightValue = data.GetHeight(y, x);
                if (heightValue > maxHeight)
                    maxHeight = heightValue;
            }
        }

        return maxHeight;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
