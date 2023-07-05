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

    public float branchPossibility = 0.1f;

    public TileConfiguration tileConfiguration;

    public TileConfig[][] CreateWaterPath(TileConfig[][] grid, TileConfig[] baseLine)
    {
        for (var y = 0; y < grid.Length; y++)
        {
            if (y == 0)
            {
                SetWaterTile(grid[y], baseLine);
            }
            else
            {
                SetWaterTile(grid[y], grid[y-1]);
            }
        }
        return grid;
    }

    private void SetWaterTile(TileConfig[] currentBlockRow, TileConfig[] lastBlockRow)
    {
        //avoid edges because those cant be water
        for (var i = 1; i < lastBlockRow.Length - 1; i++)
        {
            if (lastBlockRow[i].type == TileType.Water)
            {
                //set water tile left, middle or right randomly based on index, or both based on branchPossibility
                if (i == 1)
                {
                    // set middle or right as water
                    currentBlockRow[i + Random.Range(0, 2)] = tileConfiguration.GetTileConfig(TileType.Water);
                    if (branchPossibility > Random.Range(0, 1))
                        currentBlockRow[i + Random.Range(0, 2)] = tileConfiguration.GetTileConfig(TileType.Water);
                }
                else if (i == lastBlockRow.Length - 2)
                {
                    // set middle or left as water
                    currentBlockRow[i - 1 + Random.Range(0, 2)] = tileConfiguration.GetTileConfig(TileType.Water);
                    if (branchPossibility > Random.Range(0, 1))
                        currentBlockRow[i - 1 + Random.Range(0, 2)] = tileConfiguration.GetTileConfig(TileType.Water);
                }
                else
                {
                    // set middle or left or right as water
                    currentBlockRow[i - 1 + Random.Range(0, 3)] = tileConfiguration.GetTileConfig(TileType.Water);
                    if (branchPossibility > Random.Range(0, 1))
                        currentBlockRow[i - 1 + Random.Range(0, 3)] = tileConfiguration.GetTileConfig(TileType.Water);
                }
            }
        }
    }

    public TileConfig[][] CreateTileMap(TileConfig[] lastBlockRow)
    {
        //initialize tilemap[][] with map width and height
        var grid = new TileConfig[MAP_HEIGHT][];
        for (int i = 0; i < MAP_HEIGHT; i++)
        {
            grid[i] = new TileConfig[MAP_WIDTH];
        }

        grid = CreateWaterPath(grid, lastBlockRow);

        bool blockLeft = false;
        
        //while blockLeft loop: find tile with lowest possibilities, set compatible tile, repeat
        while (blockLeft)
        {
            //find tile with lowest neighbours
            var tileWithLowestNeighbours = GetTileWithHighestNeighbours(grid);
            

        }

        return grid;
    }
    
    private Nullable<TileConfig> GetTileWithHighestNeighbours(TileConfig[][] grid)
    {
        var highestNeighbours = 0;
        TileConfig highestNeighboursTile;
        for (var y = 0; y < grid.Length; y++)
        {
            for (var x = 0; x < grid[y].Length; x++)
            {
                var tile = grid[y][x];
                if (tile.type == TileType.Empty)
                {
                    var neighbours = GetNeighbours(grid, x, y);
                    if (neighbours > highestNeighbours)
                    {
                        highestNeighbours = neighbours;
                        highestNeighboursTile = tile;
                    }
                }
            }
        }

        return highestNeighboursTile;
    }
    
    private int GetNeighbours(TileConfig[][] grid, int x, int y)
    {
        var neighbours = 0;
        if (x > 0 && grid[y][x - 1].type != TileType.Empty)
        {
            neighbours++;
        }
        if (x < MAP_WIDTH - 1 && grid[y][x + 1].type != TileType.Empty)
        {
            neighbours++;
        }
        if (y > 0 && grid[y - 1][x].type != TileType.Empty)
        {
            neighbours++;
        }
        if (y < MAP_HEIGHT - 1 && grid[y + 1][x].type != TileType.Empty)
        {
            neighbours++;
        }

        return neighbours;
    }

    private void SampleOneLine(TileConfig[] lastBlockRow, TileConfig[] sampleLine)
    {
        for (int i = 0; i < MAP_WIDTH; i++)
        {
            y = i;
            var bottom = lastBlockRow[i].type;
            var left = i == 0 ? TileType.Void : sampleLine[i - 1].type;
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
        if (matchingConfigs.Length == 0)
        {
            throw new ApplicationException(
                $"No matching tile at position: x->{x} y->{y} types found for edges, left:{tileEdges.left}, right:{tileEdges.right}, top:{tileEdges.top}, bottom:{tileEdges.bottom}");
        }

        return matchingConfigs[Random.Range(0, matchingConfigs.Length)];
    }

    private TileConfig[] GetAllMatchingTileTypes(TileEdges tileEdges)
    {
        var tileConfigs = tileConfiguration.GetTileTypes();

        var matchingTileTypes = new List<TileConfig>();
        foreach (var config in tileConfigs)
        {
            //skip empty tiles because they match everything
            if (config.type == TileType.Empty || config.type == TileType.Void) continue;

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