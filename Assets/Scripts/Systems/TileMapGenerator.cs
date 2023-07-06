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

    public TileConfiguration tileConfiguration;

    public TileConfig[][] CreateWaterPath(TileConfig[][] grid, TileConfig[] baseLine)
    {
        for (var y = 0; y < grid.Length; y++)
        {
            if (y == 0)
            {
                SetWaterTile(grid[y], baseLine, true);
            }
            else
            {
                SetWaterTile(grid[y], grid[y - 1]);
            }
        }

        return grid;
    }

    private void SetWaterTile(TileConfig[] currentBlockRow, TileConfig[] lastBlockRow, bool noCurves = false)
    {
        //avoid edges because those cant be water
        for (var i = 1; i < lastBlockRow.Length - 1; i++)
        {
            if (lastBlockRow[i].type == TileType.Water)
            {
                if (noCurves)
                {
                    currentBlockRow[i] = tileConfiguration.GetTileConfig(TileType.Water);
                    continue;
                }
                //set water tile left, middle or right randomly based on index, or both based on branchPossibility
                if (i == 1)
                {
                    // set middle or right as water
                    currentBlockRow[i + Random.Range(0, 2)] = tileConfiguration.GetTileConfig(TileType.Water);
                }
                else if (i == lastBlockRow.Length - 2)
                {
                    // set middle or left as water
                    currentBlockRow[i - 1 + Random.Range(0, 2)] = tileConfiguration.GetTileConfig(TileType.Water);
                }
                else
                {
                    // set middle or left or right as water
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

        bool blockLeft = true;

        //while blockLeft loop: find tile with lowest possibilities, set compatible tile, repeat
        while (blockLeft)
        {
            //find tile with lowest neighbours
            var tileWithLowestNeighbours = GetTileWithHighestNeighbours(grid, lastBlockRow);
            if (tileWithLowestNeighbours.HasValue)
            {
                var newTile = GetRandomTileFromTileEdges(tileWithLowestNeighbours.Value);
                grid[tileWithLowestNeighbours.Value.coords.y][tileWithLowestNeighbours.Value.coords.x] = newTile;
            }
            else
            {
                blockLeft = false;
            }
        }

        return grid;
    }

    private NeighbourResult? GetTileWithHighestNeighbours(TileConfig[][] grid, TileConfig[] lastRow)
    {
        NeighbourResult? highestNeighboursResult = null;
        for (var y = 0; y < grid.Length; y++)
        {
            for (var x = 0; x < grid[y].Length; x++)
            {
                var tile = grid[y][x];
                if (tile.type == TileType.Empty)
                {
                    var neighbours = GetNeighbours(grid, x, y, lastRow);
                    if (!highestNeighboursResult.HasValue)
                    {
                        highestNeighboursResult = neighbours;
                    }
                    else if (neighbours.amount > highestNeighboursResult.Value.amount)
                    {
                        highestNeighboursResult = neighbours;
                    }
                }
            }
        }

        return highestNeighboursResult;
    }

    private NeighbourResult GetNeighbours(TileConfig[][] grid, int x, int y, TileConfig[] lastRow)
    {
        var neighboursResult = new NeighbourResult();
        neighboursResult.coords = new Vector2Int(x, y);
        neighboursResult.config = grid[y][x];
        if (x == 0)
        {
            neighboursResult.edges.left = EdgeType.Floor;
            neighboursResult.amount++;
        }

        if (x == MAP_WIDTH - 1)
        {
            neighboursResult.edges.right = EdgeType.Floor;
            neighboursResult.amount++;
        }

        if (y == 0)
        {
            neighboursResult.edges.bottom = lastRow[x].edges.top;
            neighboursResult.amount++;
        }

        if (x > 0 && grid[y][x - 1].type != TileType.Empty)
        {
            neighboursResult.edges.left = grid[y][x - 1].edges.right;
            neighboursResult.amount++;
        }

        if (x < MAP_WIDTH - 1 && grid[y][x + 1].type != TileType.Empty)
        {
            neighboursResult.edges.right = grid[y][x + 1].edges.left;
            neighboursResult.amount++;
        }

        if (y > 0 && grid[y - 1][x].type != TileType.Empty)
        {
            neighboursResult.edges.bottom = grid[y - 1][x].edges.top;
            neighboursResult.amount++;
        }

        if (y < MAP_HEIGHT - 1 && grid[y + 1][x].type != TileType.Empty)
        {
            neighboursResult.edges.top = grid[y + 1][x].edges.bottom;
            neighboursResult.amount++;
        }

        return neighboursResult;
    }

    private TileConfig GetRandomTileFromTileEdges(NeighbourResult result)
    {
        var matchingConfigs = GetAllMatchingTileTypes(result.edges);
        if (matchingConfigs.Length == 0)
        {
            throw new ApplicationException(
                $"No matching tile at position: x->{result.coords.x} y->{result.coords.y} types found for edges, left:{result.edges.left}, right:{result.edges.right}, top:{result.edges.top}, bottom:{result.edges.bottom}");
        }

        int floorIndex = Array.FindIndex(matchingConfigs, config => config.type == TileType.Floor);

        //prefere floor
        if (floorIndex > -1)
        {
            return matchingConfigs[floorIndex];
        }

        //prefere edge left
        int rightIndex = Array.FindIndex(matchingConfigs, config => config.type == TileType.WaterEdgeLeft);
        if (rightIndex > -1)
        {
            return matchingConfigs[rightIndex];
        }

        //prefere edge right
        int leftIndex = Array.FindIndex(matchingConfigs, config => config.type == TileType.WaterEdgeRight);
        if (leftIndex > -1)
        {
            return matchingConfigs[leftIndex];
        }

        //avoid water
        if (matchingConfigs.Length > 1)
        {
            matchingConfigs = Array.FindAll(matchingConfigs, config => config.type != TileType.Water);
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

    struct NeighbourResult
    {
        public int amount;
        public TileEdges edges;
        public TileConfig config;
        public Vector2Int coords;
    }
}