using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour
{
    public int mapSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    public int chunkDrawingRange = 8;

    public GameObject chunkPrefab;

    public TerrainGenerator terrainGenerator;
    public Vector2Int mapSeedOffset;

    //public Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    //public Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

    public UnityEvent OnWorldCreated, OnNewChunksGenerated;

    public WorldData worldData { get; private set; }

    private void Awake()
    {
        worldData = new WorldData
        {
            chunkHeight = this.chunkHeight,
            chunkSize = this.chunkSize,
            chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>(),
            chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>()
        };
    }

    public void GenerateWorld()
    {
        //chunkDataDictionary.Clear();
        //foreach (ChunkRenderer chunk in chunkDictionary.Values)
        //{
        //    Destroy(chunk.gameObject);
        //}
        //chunkDictionary.Clear();

        WorldGenerationData worldGenerationData = GetPosisionsThatPlayerSees(Vector3Int.zero);

        //for (int x = 0; x < mapSizeInChunks; x++)
        //{
        //    for (int z = 0; z < mapSizeInChunks; z++)
        //    {

        //        ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
        //        //GenerateVoxels(data);
        //        ChunkData newData = terrainGenerator.GenerateChunkData(data, mapSeedOffset);
        //        worldData.chunkDataDictionary.Add(newData.worldPosition, newData);
        //    }
        //}

        foreach (var pos in worldGenerationData.chunkDataPositionsToCreate)
        {
            ChunkData data = new ChunkData(chunkSize, chunkHeight, this, pos);
            ChunkData newData = terrainGenerator.GenerateChunkData(data, mapSeedOffset);
            worldData.chunkDataDictionary.Add(pos, newData);
        }

        foreach (var pos in worldGenerationData.chunkPositionsToCreate)
        {
            ChunkData data = worldData.chunkDataDictionary[pos];
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            worldData.chunkDictionary.Add(data.worldPosition, chunkRenderer);
            chunkRenderer.InitializeChunk(data);
            chunkRenderer.UpdateChunk(meshData);

        }

        //foreach (ChunkData data in worldData.chunkDataDictionary.Values)
        //{
        //    MeshData meshData = Chunk.GetChunkMeshData(data);
        //    GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
        //    ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
        //    worldData.chunkDictionary.Add(data.worldPosition, chunkRenderer);
        //    chunkRenderer.InitializeChunk(data);
        //    chunkRenderer.UpdateChunk(meshData);

        //}

        OnWorldCreated?.Invoke();
    }

    private WorldGenerationData GetPosisionsThatPlayerSees(Vector3Int playerPosition)
    {
        List<Vector3Int> allChunkPositionsNeeded = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPosition);
        List<Vector3Int> allChunkDataPositionsNeeded = WorldDataHelper.GetDataPositionsAroundPlayer(this, playerPosition);

        List<Vector3Int> chunkPositionsToCreate = WorldDataHelper.SelectPositonsToCreate(worldData, allChunkPositionsNeeded, playerPosition);
        List<Vector3Int> chunkDataPositionsToCreate = WorldDataHelper.SelectDataPositonsToCreate(worldData, allChunkDataPositionsNeeded, playerPosition);

        WorldGenerationData data = new WorldGenerationData
        {
            chunkPositionsToCreate = chunkPositionsToCreate,
            chunkDataPositionsToCreate = chunkDataPositionsToCreate,
            chunkPositionsToRemove = new List<Vector3Int>(),
            chunkDataToRemove = new List<Vector3Int>(),
            chunkPositionsToUpdate = new List<Vector3Int>()
        };
        return data;

    }

    internal void LoadAdditionalChunksRequest(GameObject player)
    {
        Debug.Log("Load more chunks");
        OnNewChunksGenerated?.Invoke();
    }

    private void GenerateVoxels(ChunkData data)
    {
        
    }

    internal BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, x, y, z);
        ChunkData containerChunk = null;

        worldData.chunkDataDictionary.TryGetValue(pos, out containerChunk);

        if (containerChunk == null)
            return BlockType.Nothing;
        Vector3Int blockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInCHunkCoordinates);
    }

    public struct WorldGenerationData
    {
        public List<Vector3Int> chunkPositionsToCreate;
        public List<Vector3Int> chunkDataPositionsToCreate;
        public List<Vector3Int> chunkPositionsToRemove;
        public List<Vector3Int> chunkDataToRemove;
        public List<Vector3Int> chunkPositionsToUpdate;
    }

    public struct WorldData
    {
        public Dictionary<Vector3Int, ChunkData> chunkDataDictionary;
        public Dictionary<Vector3Int, ChunkRenderer> chunkDictionary;
        public int chunkSize;
        public int chunkHeight;
    }
}
