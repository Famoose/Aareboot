using System;
using System.Collections;
using Structs;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Jobs;
using Unity.Mathematics;

public class GridManager : MonoBehaviour
{
    private float SUB_PIXEL_MOVEMENT = 0.0625f;
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

    public bool gameRunning;

    private float summedDelta;

    // Start is called before the first frame update
    void Start()
    {
        _tilemaps = new Tilemap[] { midTilemap, topTilemap, botTilemap };
        var initialTileMap = tileMapGenerator.CreateTileMap(null, true);
        _tileConfigs[currentPosition] = initialTileMap;
        RenderTileMap();

        StartCoroutine(CalcNewGrid());
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
        if (!gameRunning) return;
        var delta = (speed * Time.deltaTime);
        summedDelta += delta;
        var allignedDelta = 0f;
        if (summedDelta < SUB_PIXEL_MOVEMENT)
        {
            return;
        }
        
        allignedDelta = summedDelta - summedDelta % SUB_PIXEL_MOVEMENT;
        summedDelta %= SUB_PIXEL_MOVEMENT;
        var transform = new Vector3(0, -allignedDelta, 0);
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

            StartCoroutine(CalcNewGrid());
        }
    }

    IEnumerator CalcNewGrid()
    {
        var maxItr = 10;
        var itr = 0;
        var shouldTryGeneration = true;
        while (shouldTryGeneration && itr < maxItr)
        {
            try
            {
                itr++;
                var newMap = tileMapGenerator.CreateTileMap(_tileConfigs[currentPosition][19]);
                currentPosition = (currentPosition + 1) % 3;
                _tileConfigs[currentPosition] = newMap;
                RenderTileMap();
                shouldTryGeneration = false;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        yield return null;
    }
}