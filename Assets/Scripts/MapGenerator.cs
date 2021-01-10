﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Vector2 cellSize = new Vector2(1, 1);
    [SerializeField] Vector2Int mapSize;
    [SerializeField] GameObject cell;

    private void Start()
    {
        GenerateMap(mapSize.x, mapSize.y, cellSize.x, cellSize.y);
    }

    void GenerateMap(int mapHeight, int mapWidth, float cellHeight, float cellWidth)
    {
        for (float i = 0; i < mapHeight; i++)
        {
            for (float j = 0; j < mapWidth; j++)
            {
                float x = -((mapHeight - 1) / 2.0f) + i;
                x *= cellHeight;
                float z = -((mapWidth - 1) / 2.0f) + j;
                z *= cellWidth;

                GameObject go = Instantiate(cell, new Vector3(x, 0, z), Quaternion.identity);
                go.transform.localScale = new Vector3(cellHeight, 1, cellWidth);
                go.transform.SetParent(transform);
            }
        }

        gameObject.AddComponent<BoxCollider>().size = new Vector3(mapHeight * cellHeight, 0.1f, mapWidth * cellWidth);
    }

    bool IsOdd(float value)
    {
        return value % 2 == 1;
    }
}
