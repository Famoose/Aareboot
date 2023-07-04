using System;
using System.Collections.Generic;
using Structs;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "TileConfiguration", menuName = "TileConfiguration", order = 0)]
    public class TileConfiguration : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private TileConfig[] tileTypes;
        
        [NonSerialized]
        private TileConfig[] tileTypesRuntime;

        private Dictionary<TileType, TileConfig> tileConfigMap;

        public void OnAfterDeserialize()
        {
            //deep copy tileTypes to tileTypesRuntime
            tileTypesRuntime = new TileConfig[tileTypes.Length];
            for (var i = 0; i < tileTypes.Length; i++)
            {
                tileTypesRuntime[i] = new TileConfig();
                tileTypesRuntime[i].type = tileTypes[i].type;
                tileTypesRuntime[i].edges = new TileEdges();
                tileTypesRuntime[i].edges.left = tileTypes[i].edges.left;
                tileTypesRuntime[i].edges.right = tileTypes[i].edges.right;
                tileTypesRuntime[i].edges.top = tileTypes[i].edges.top;
                tileTypesRuntime[i].edges.bottom = tileTypes[i].edges.bottom;
            }

            tileConfigMap = new Dictionary<TileType, TileConfig>();
            foreach (var tileConfig in tileTypesRuntime)
            {
                tileConfigMap.Add(tileConfig.type, tileConfig);
            }
            
        }
        
        public void OnBeforeSerialize()
        {
        }
        
        public TileConfig GetTileConfig(TileType tileType)
        {
            return tileConfigMap[tileType];
        }
        
        public TileConfig[] GetTileTypes()
        {
            return tileTypesRuntime;
        }

    }
}