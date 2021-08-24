using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundLayerHandler : BlockLayerHandler
{
    public BlockType undergroundBlockType;
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (y < surfaceHeightNoise)
        {
            Vector3Int pos = new Vector3Int(x, y - chunkData.worldPosition.y, z);
            Chunk.SetBlock(chunkData, pos, undergroundBlockType);
            return true;
        }
        return false;
    }
}
