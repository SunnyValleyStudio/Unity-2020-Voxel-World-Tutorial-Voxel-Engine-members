using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldDataHelper
{
    public static Vector3Int ChunkPositionFromBlockCoords(World world, Vector3Int position)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(position.x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(position.y / (float)world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(position.z / (float)world.chunkSize) * world.chunkSize
        };
    }

}
