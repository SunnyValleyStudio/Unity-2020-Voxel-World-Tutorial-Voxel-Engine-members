using System;
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
        BiomeGeneratorSelection biomeSelection = SelectBiomeGenerator(data.worldPosition, data, false);
        //TreeData treeData = biomeGenerator.GetTreeData(data, mapSeedOffset);
        data.treeData = biomeSelection.biomeGenerator.GetTreeData(data, mapSeedOffset);
        for (int x = 0; x < data.chunkSize; x++)
        {
            for (int z = 0; z < data.chunkSize; z++)
            {
                biomeSelection = SelectBiomeGenerator(new Vector3Int(data.worldPosition.x + x, 0, data.worldPosition.z + z), data);
                data = biomeSelection.biomeGenerator.ProcessChunkColumn(data, x, z, mapSeedOffset, biomeSelection.terrainSurfaceNoise);
            }
        }
        return data;
    }

    private BiomeGeneratorSelection SelectBiomeGenerator(Vector3Int worldPosition, ChunkData data, bool useDomainWarping = true)
    {
        if (useDomainWarping == true)
        {
            Vector2Int domainOffset = Vector2Int.RoundToInt(biomeDomainWarping.GenerateDomainOffset(worldPosition.x, worldPosition.z));
            worldPosition += new Vector3Int(domainOffset.x, 0, domainOffset.y);
        }

        List<BiomeSelectionHelper> biomeSelectionHelpers = GetBiomeGeneratorSelectionHelpers(worldPosition);
        BiomeGenerator generator_1 = SelectBiome(biomeSelectionHelpers[0].Index);
        BiomeGenerator generator_2 = SelectBiome(biomeSelectionHelpers[1].Index);

        float distance = 
            Vector3.Distance(
                biomeCenters[biomeSelectionHelpers[0].Index], 
                biomeCenters[biomeSelectionHelpers[1].Index]);
        float weight_0 = biomeSelectionHelpers[0].Distance / distance;
        float weight_1 = 1 - weight_0;
        int terrainHeightNoise_0 = generator_1.GetSurfaceHeightNoise(worldPosition.x, worldPosition.z, data.chunkHeight);
        int terrainHeightNoise_1 = generator_2.GetSurfaceHeightNoise(worldPosition.x, worldPosition.z, data.chunkHeight);
        return new BiomeGeneratorSelection(generator_1, Mathf.RoundToInt(terrainHeightNoise_0 * weight_0 + terrainHeightNoise_1 * weight_1));
    }

    private BiomeGenerator SelectBiome(int index)
    {
        float temp = biomeNoise[index];
        foreach (var data in biomeGeneratorsData)
        {
            if (temp >= data.temperatureStartThreshold && temp < data.temperatureEndThreshold)
                return data.biomeTerrainGenerator;
        }
        return biomeGeneratorsData[0].biomeTerrainGenerator;
    }

    private List<BiomeSelectionHelper> GetBiomeGeneratorSelectionHelpers(Vector3Int position)
    {
        position.y = 0;
        return GetClosestBiomeIndex(position);
    }

    private List<BiomeSelectionHelper> GetClosestBiomeIndex(Vector3Int position)
    {
        return biomeCenters.Select((center, index) =>
        new BiomeSelectionHelper
        {
            Index = index,
            Distance = Vector3.Distance(center, position)
        }).OrderBy(helper => helper.Distance).Take(4).ToList();
    }

    private struct BiomeSelectionHelper
    {
        public int Index;
        public float Distance;
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

public class BiomeGeneratorSelection
{
    public BiomeGenerator biomeGenerator = null;
    public int? terrainSurfaceNoise = null;

    public BiomeGeneratorSelection(BiomeGenerator biomeGeneror, int? terrainSurfaceNoise = null)
    {
        this.biomeGenerator = biomeGeneror;
        this.terrainSurfaceNoise = terrainSurfaceNoise;
    }
}
