using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayerHandler : BlockLayerHandler
{
    public int waterLevel = 1;
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if(y > surfaceHeightNoise && y <= waterLevel)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            Chunk.SetBlock(chunkData, pos, BlockType.Water);
            if(y == surfaceHeightNoise + 1)
            {
                pos.y = surfaceHeightNoise;
                Chunk.SetBlock(chunkData, pos, BlockType.Sand);
            }
            return true;
        }
        return false;
    }
}
