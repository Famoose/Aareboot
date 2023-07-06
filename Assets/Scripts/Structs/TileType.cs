using System;
using JetBrains.Annotations;
using UnityEngine.Tilemaps;

namespace Structs
{
    [Serializable]
    public enum TileType
    {
        Empty,
        Void,
        Floor,
        Water,
        WaterEdgeLeft,
        WaterEdgeRight,
        WaterEdgeTop,
        WaterEdgeBottom,
        WaterEdgeTopLeft,
        WaterEdgeTopRight,
        WaterEdgeBottomLeft,
        WaterEdgeBottomRight,
        WaterEdgeTopLeftInner,
        WaterEdgeTopRightInner,
        WaterEdgeBottomLeftInner,
        WaterEdgeBottomRightInner,
    }
    
    [Serializable]
    public enum EdgeType
    {
        Any,
        Void,
        Floor,
        Water,
        RightHalf,
        LeftHalf,
        TopHalf,
        BottomHalf,
    }
    
    [Serializable]
    public struct TileEdges
    {
        public EdgeType left;
        public EdgeType right;
        public EdgeType top;
        public EdgeType bottom;
    }

    [Serializable]
    public struct TileConfig
    {
        public TileType type;
        [CanBeNull] public Tile tile;
        public TileEdges edges;
    }

}