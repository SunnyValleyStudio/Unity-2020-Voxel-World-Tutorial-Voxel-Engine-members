using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    public int waterThreshold = 50;
    public float noiseScale = 0.03f;
    public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int mapSeedOffset)
    {
        float noiseValue = Mathf.PerlinNoise((mapSeedOffset.x + data.worldPosition.x + x) * noiseScale, (mapSeedOffset.y + data.worldPosition.z + z) * noiseScale);
        int groundPosition = Mathf.RoundToInt(noiseValue * data.chunkHeight);
        for (int y = 0; y < data.chunkHeight; y++)
        {
            BlockType voxelType = BlockType.Dirt;
            if (y > groundPosition)
            {
                if (y < waterThreshold)
                {
                    voxelType = BlockType.Water;
                }
                else
                {
                    voxelType = BlockType.Air;
                }

            }
            else if (y == groundPosition && y < waterThreshold)
            {
                voxelType = BlockType.Sand;
            }
            else if (y == groundPosition)
            {
                voxelType = BlockType.Grass_Dirt;
            }

            Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
        }
        return data;
    }
}
