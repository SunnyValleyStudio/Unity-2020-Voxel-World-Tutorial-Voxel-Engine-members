using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLayerHandler : BlockLayerHandler
{
    public float terrainHeightLimit = 25;

    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (chunkData.worldPosition.y < 0)
            return false;
        if (surfaceHeightNoise < terrainHeightLimit && chunkData.treeData.treePositions.Contains(new Vector2Int(x, z)))
        {
            Vector3Int chunkCoordinates = Chunk.GetBlockInChunkCoordinates(chunkData, new Vector3Int(x, surfaceHeightNoise, z));
            BlockType type = Chunk.GetBlockFromChunkCoordinates(chunkData, chunkCoordinates);
            if(type == BlockType.Grass_Dirt)
            {
                Vector3Int localPos = Chunk.GetBlockInChunkCoordinates(chunkData, new Vector3Int(x, surfaceHeightNoise, z));
                Chunk.SetBlock(chunkData, localPos, BlockType.Dirt);
                for (int i = 1; i < 5; i++)
                {
                    localPos.y = surfaceHeightNoise + i;
                    Chunk.SetBlock(chunkData, localPos, BlockType.TreeTrunk);
                }
            }
        }
        return false;
    }
}
