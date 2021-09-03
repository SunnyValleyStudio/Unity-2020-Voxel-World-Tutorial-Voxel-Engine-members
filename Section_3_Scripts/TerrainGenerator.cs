using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public BiomeGenerator biomeGenerator;

    [SerializeField]
    List<Vector3Int> biomeCenters = new List<Vector3Int>();

    public ChunkData GenerateChunkData(ChunkData data, Vector2Int mapSeedOffset)
    {
        TreeData treeData = biomeGenerator.GetTreeData(data, mapSeedOffset);
        data.treeData = treeData;
        for (int x = 0; x < data.chunkSize; x++)
        {
            for (int z = 0; z < data.chunkSize; z++)
            {
                data = biomeGenerator.ProcessChunkColumn(data, x, z, mapSeedOffset);
            }
        }
        return data;
    }

    public void GenerateBiomePoints(Vector3 playerPosition, int drawRange, int mapSize, Vector2Int mapSeedOffset)
    {
        biomeCenters = new List<Vector3Int>();
        biomeCenters = BiomeCenterFinder.CalculateBiomeCenters(playerPosition, drawRange, mapSize);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        foreach (var biomCenterPoint in biomeCenters)
        {
            Gizmos.DrawLine(biomCenterPoint, biomCenterPoint + Vector3.up * 255);
        }
    }
}
