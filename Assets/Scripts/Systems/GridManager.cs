using System;
using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public float speed = 1.0f;
    public GameObject top;
    public Tilemap topTilemap;
    public GameObject mid;
    public Tilemap midTilemap;
    public GameObject bot;
    public Tilemap botTilemap;

    public TileMapGenerator tileMapGenerator;

    // refactor
    private TileConfig[][][] _tileConfigs = new TileConfig[3][][];
    private int currentPosition = 0;
    private Tilemap[] _tilemaps = new Tilemap[3];
    
    // Start is called before the first frame update
    void Start()
    {
        var initialTileMap = tileMapGenerator.CreateTileMap(
            new TileConfig[]{
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.WaterEdgeLeft),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Water),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Water),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.WaterEdgeRight),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
            });
        
        _tilemaps = new Tilemap[]{midTilemap, topTilemap, botTilemap};
        _tileConfigs[currentPosition] = initialTileMap;
        RenderTileMap();
        
        var newMap = tileMapGenerator.CreateTileMap(_tileConfigs[currentPosition][19]);
        currentPosition = (currentPosition + 1) % 3;
        _tileConfigs[currentPosition] = newMap;
        RenderTileMap();
    }

    private void RenderTileMap()
    {
        var initialTileMap = _tileConfigs[currentPosition];
        var targetTilemap = _tilemaps[currentPosition];
        //set mid tilemap to initial tilemaps generated tiles
        for (int i = 0; i < initialTileMap.Length; i++)
        {
            for (int j = 0; j < initialTileMap[i].Length; j++)
            {
                if (initialTileMap[i][j].type == TileType.Empty || initialTileMap[i][j].type == TileType.Void ||
                    initialTileMap[i][j].type == TileType.Water)
                {
                    targetTilemap.SetTile(new Vector3Int(j - 5, i - 10, 0), null);
                }

                targetTilemap.SetTile(new Vector3Int(j - 5, i - 10, 0), initialTileMap[i][j].tile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        var transform = new Vector3(0, speed * Time.deltaTime * -1, 0);
        top.transform.position += transform;
        mid.transform.position += transform;
        bot.transform.position += transform;

        if (bot.transform.position.y <= -30)
        {
            //magic number 20 is the height of the tile
            bot.transform.position = new Vector3(0, top.transform.position.y + 20, 0);
            var temp = bot;
            bot = mid;
            mid = top;
            top = temp;
            
            //TODO: update bottom tilemap
            var newMap = tileMapGenerator.CreateTileMap(_tileConfigs[currentPosition][19]);
            currentPosition = (currentPosition + 1) % 3;
            _tileConfigs[currentPosition] = newMap;
            RenderTileMap();
        }
    }
}
