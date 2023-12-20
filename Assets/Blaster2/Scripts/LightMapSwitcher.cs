using UnityEngine;

using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class CustomLightmapData
{
    public string name;
    public Texture2D[] dir;
    public Texture2D[] light;
    public Texture2D[] shadowmask;
}

public class LightMapSwitcher : MonoBehaviour
{
    public CustomLightmapData[] levelLightmapDir;

    private Dictionary<string, LightmapData[]> levelLightData = new Dictionary<string, LightmapData[]>();

    void Start()
    {
        for (int i = 0; i < levelLightmapDir.Length; i++)
        {
            CustomLightmapData customLightmapData = levelLightmapDir[i];
            var nightLightMaps = new LightmapData[customLightmapData.dir.Length];


            for (int dirIndex = 0; dirIndex < customLightmapData.dir.Length; dirIndex++)
            {
                nightLightMaps[dirIndex] = new LightmapData();
          
                if (customLightmapData.dir.Length > 0)
                {
                    nightLightMaps[dirIndex].lightmapDir = customLightmapData.dir[dirIndex];
                }

                if (customLightmapData.light.Length > 0)
                {
                    nightLightMaps[dirIndex].lightmapColor = customLightmapData.light[dirIndex];
                }

                if (customLightmapData.shadowmask.Length > 0)
                {
                    nightLightMaps[dirIndex].shadowMask = customLightmapData.shadowmask[dirIndex];
                }
            }
            levelLightData.Add(customLightmapData.name, nightLightMaps);
        }
    }

    public void SetLevelLightmap(string name)
    {
        if (levelLightData.ContainsKey(name))
        {
            LightmapSettings.lightmaps = levelLightData[name];
        }
    }
}