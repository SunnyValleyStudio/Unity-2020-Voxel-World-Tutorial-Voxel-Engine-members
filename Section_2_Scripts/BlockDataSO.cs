using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Block Data" ,menuName ="Data/Block Data")]
public class BlockDataSO : ScriptableObject
{
    public float textureSizeX, textureSizeY;
    public List<TextureData> textureDataList;
}

[Serializable]
public class TextureData
{
    public BlockType blockType;
    public Vector2Int up, down, side;
    public bool isSolid = true;
    public bool generatesCollider = true;
}

