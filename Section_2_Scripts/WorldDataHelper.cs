using System;
using System.Collections.Generic;
using System.Linq;
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

    internal static List<Vector3Int> GetChunkPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - (world.chunkDrawingRange) * world.chunkSize;
        int startZ = playerPosition.z - (world.chunkDrawingRange) * world.chunkSize;
        int endX = playerPosition.x + (world.chunkDrawingRange) * world.chunkSize;
        int endZ = playerPosition.z + (world.chunkDrawingRange) * world.chunkSize;

        List<Vector3Int> chunkPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, 0, z));
                chunkPositionsToCreate.Add(chunkPos);
                //if (x >= playerPosition.x - world.chunkSize
                //    && x <= playerPosition.x + world.chunkSize
                //    && z >= playerPosition.z - world.chunkSize
                //    && z <= playerPosition.z + world.chunkSize)
                //{
                //    for (int y = -world.chunkHeight; y >= playerPosition.y - world.chunkHeight * 2; y -= world.chunkHeight)
                //    {
                //        chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, y, z));
                //        chunkPositionsToCreate.Add(chunkPos);
                //    }
                //}
            }
        }

        return chunkPositionsToCreate;
    }

    internal static List<Vector3Int> GetDataPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - (world.chunkDrawingRange + 1) * world.chunkSize;
        int startZ = playerPosition.z - (world.chunkDrawingRange + 1) * world.chunkSize;
        int endX = playerPosition.x + (world.chunkDrawingRange + 1) * world.chunkSize;
        int endZ = playerPosition.z + (world.chunkDrawingRange + 1) * world.chunkSize;

        List<Vector3Int> chunkDataPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, 0, z));
                chunkDataPositionsToCreate.Add(chunkPos);
                //if (x >= playerPosition.x - world.chunkSize
                //    && x <= playerPosition.x + world.chunkSize
                //    && z >= playerPosition.z - world.chunkSize
                //    && z <= playerPosition.z + world.chunkSize)
                //{
                //    for (int y = -world.chunkHeight; y >= playerPosition.y - world.chunkHeight * 2; y -= world.chunkHeight)
                //    {
                //        chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, y, z));
                //        chunkDataPositionsToCreate.Add(chunkPos);
                //    }
                //}
            }
        }

        return chunkDataPositionsToCreate;
    }

    internal static List<Vector3Int> SelectPositonsToCreate(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded, Vector3Int playerPosition)
    {
        return allChunkPositionsNeeded
            .Where(pos => worldData.chunkDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPosition, pos))
            .ToList();
    }

    internal static List<Vector3Int> SelectDataPositonsToCreate(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded, Vector3Int playerPosition)
    {
        return allChunkDataPositionsNeeded
            .Where(pos => worldData.chunkDataDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPosition, pos))
            .ToList();
    }
}
