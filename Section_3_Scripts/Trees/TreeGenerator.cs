using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public NoiseSettings treeNoiseSettings;
    public DomainWarping domainWrapping;

    public TreeData GenerateTreeData(ChunkData chunkData, Vector2Int mapSeedOffset)
    {
        treeNoiseSettings.worldOffset = mapSeedOffset;
        TreeData treeData = new TreeData();
        float[,] noiseData = GenerateTreeNoise(chunkData, treeNoiseSettings);
        treeData.treePositions = DataPorocessing.FindLocalMaxima(noiseData, chunkData.worldPosition.x, chunkData.worldPosition.z);
        return treeData;
    }

    private float[,] GenerateTreeNoise(ChunkData chunkData, NoiseSettings treeNoiseSettings)
    {
        float[,] noiseMax = new float[chunkData.chunkSize, chunkData.chunkSize];
        int xMax = chunkData.worldPosition.x + chunkData.chunkSize;
        int xMin = chunkData.worldPosition.x;
        int zMax = chunkData.worldPosition.z + chunkData.chunkSize;
        int zMin = chunkData.worldPosition.z;
        int xIndex = 0, zIndex = 0;
        for (int x = xMin; x < xMax; x++)
        {
            for (int z = zMin; z < zMax; z++)
            {
                noiseMax[xIndex, zIndex] = domainWrapping.GenerateDomainNoise(x, z, treeNoiseSettings);
                zIndex++;
            }
            xIndex++;
            zIndex = 0;
        }
        return noiseMax;
    }
}
