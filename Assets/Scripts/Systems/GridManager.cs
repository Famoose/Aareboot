using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public float speed = 1.0f;
    public GameObject top;
    public GameObject mid;
    public GameObject bot;

    public TileMapGenerator tileMapGenerator;

    // Start is called before the first frame update
    void Start()
    {
        tileMapGenerator.CreateTileMap(
            new TileConfig[]{
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.WaterEdgeLeft),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Water),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.WaterEdgeRight)
            });
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
            tileMapGenerator.CreateTileMap(
            new TileConfig[]{
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Floor),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.WaterEdgeLeft),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.Water),
                tileMapGenerator.tileConfiguration.GetTileConfig(TileType.WaterEdgeRight)
            });
        }
    }
}
