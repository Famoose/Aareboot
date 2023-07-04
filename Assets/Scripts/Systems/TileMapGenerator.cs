using System;
using System.Collections.Generic;
using DefaultNamespace;
using Structs;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TileMapGenerator : MonoBehaviour
{
    private static int MAP_WIDTH = 10;
    private static int MAP_HEIGHT = 20;

    private int x = 0;
    private int y = 0;
    
    public TileConfiguration tileConfiguration;
    
    public void CreateTileMap(TileConfig[] lastBlockRow)
    {
        //initialize tilemap[][] with map width and height
        var tileMap = new TileConfig[MAP_HEIGHT][];
        for (int i = 0; i < MAP_HEIGHT; i++)
        {
            x = i;
            tileMap[i] = new TileConfig[MAP_WIDTH];
            if (i == 0)
            {
                SampleOneLine(lastBlockRow, tileMap[i]);
            }
            else
            {
                SampleOneLine(tileMap[i - 1], tileMap[i]);
            }
        }

        // log the tilemap
        for (int i = 0; i < MAP_HEIGHT; i++)
        {
            var line = "";
            for (int j = 0; j < MAP_WIDTH; j++)
            {
                line += tileMap[i][j].type + " ";
            }
            Debug.Log(line);
        }
    }
    
    private void SampleOneLine(TileConfig[] lastBlockRow, TileConfig[] sampleLine)
    {
        for (int i = 0; i < MAP_WIDTH; i++)
        {
            y = i;
            var bottom = lastBlockRow[i].type;
            var left = i == 0 ? TileType.Floor : sampleLine[i - 1].type;
            var right = i == MAP_WIDTH - 1 ? TileType.Floor : sampleLine[i + 1].type;
            var top = TileType.Empty;
            sampleLine[i] = GetRandomCompatibleTile(left, right, top, bottom);
        }
    }

    private TileConfig GetRandomCompatibleTile(TileType left, TileType right, TileType top, TileType bottom)
    {
        var edges = new TileEdges();
        edges.left = tileConfiguration.GetTileConfig(left).edges.right;
        edges.right = tileConfiguration.GetTileConfig(right).edges.left;
        edges.top = tileConfiguration.GetTileConfig(top).edges.bottom;
        edges.bottom = tileConfiguration.GetTileConfig(bottom).edges.top;
        
        return GetRandomTileFromTileEdges(edges);
    }

    private TileConfig GetRandomTileFromTileEdges(TileEdges tileEdges)
    {
        var matchingConfigs = GetAllMatchingTileTypes(tileEdges);
        return matchingConfigs[Random.Range(0, matchingConfigs.Length)];
    }
    
    private TileConfig[] GetAllMatchingTileTypes(TileEdges tileEdges)
    {
        var tileConfigs = tileConfiguration.GetTileTypes();

        var matchingTileTypes = new List<TileConfig>();
        foreach (var config in tileConfigs)
        {
            //skip empty tiles because they match everything
            if (config.type == TileType.Empty) continue;
            
            bool match = true;
            if (config.edges.left != tileEdges.left && tileEdges.left != EdgeType.Any)
            {
                match = false;
            }
            if (config.edges.right != tileEdges.right && tileEdges.right != EdgeType.Any)
            {
                match = false;
            }
            if (config.edges.top != tileEdges.top && tileEdges.top != EdgeType.Any)
            {
                match = false;
            }
            if (config.edges.bottom != tileEdges.bottom && tileEdges.bottom != EdgeType.Any)
            {
                match = false;
            }

            if (match)
            {
                matchingTileTypes.Add(config);
            }
        }

        return matchingTileTypes.ToArray();
    }
    
}
