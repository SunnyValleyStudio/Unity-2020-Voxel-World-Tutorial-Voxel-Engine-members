using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public BiomeGenerator biomeGenerator;

    [SerializeField]
    List<Vector3Int> biomeCenters = new List<Vector3Int>();
    List<float> biomeNoise = new List<float>();

    [SerializeField]
    private NoiseSettings biomeNoiseSettings;

    public DomainWarping biomeDomainWarping;

    [SerializeField]
    private List<BiomeData> biomeGeneratorsData = new List<BiomeData>();


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

        for (int i = 0; i < biomeCenters.Count; i++)
        {
            Vector2Int domainWarpingOffset
                = biomeDomainWarping.GenerateDomainOffsetInt(biomeCenters[i].x, biomeCenters[i].y);
            biomeCenters[i] += new Vector3Int(domainWarpingOffset.x, 0, domainWarpingOffset.y);
        }
        biomeNoise = CalculateBiomeNoise(biomeCenters, mapSeedOffset);
    }

    private List<float> CalculateBiomeNoise(List<Vector3Int> biomeCenters, Vector2Int mapSeedOffset)
    {
        biomeNoiseSettings.worldOffset = mapSeedOffset;
        return biomeCenters.Select(center => MyNoise.OctavePerlin(center.x, center.y, biomeNoiseSettings)).ToList();
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

[Serializable]
public struct BiomeData
{
    [Range(0f, 1f)]
    public float temperatureStartThreshold, temperatureEndThreshold;
    public BiomeGenerator biomeTerrainGenerator;
}
