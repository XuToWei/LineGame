using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    [Serializable]
    public enum MapType
    {
        Block,
    }

    [Serializable]
    public enum CellType
    {
        Block,
        Six
    }

    [Serializable]
    public enum CellContentType
    {
        None,
        Green,
        Red,
        Yellow,
        Blue,
        LittleBule,
        Brown,
    }

    public static class FlowFreeColors
    {
        public static readonly Color None = new Color(0, 0, 0, 0);
        public static readonly Color Green = new Color(0, 1, 0, 1);
        public static readonly Color Green_BackGround = new Color(0, 1, 0, 0.368f);
        public static readonly Color Red = new Color(1, 0, 0, 1);
        public static readonly Color Red_BackGround = new Color(1, 0, 0, 0.368f);
        public static readonly Color Blue = new Color(0, 0, 1, 1);
        public static readonly Color Blue_BackGround = new Color(0, 0, 1, 0.368f);
        public static readonly Color Yellow = new Color(1, 1, 0, 1);
        public static readonly Color Yellow_BackGround = new Color(1, 1, 0, 0.368f);
        public static readonly Color LittleBule = new Color(0.5f, 0.5f, 1, 1);
        public static readonly Color LittleBule_BackGround = new Color(0.5f, 0.5f, 1, 0.368f);
        public static readonly Color Brown = new Color(0.75f, 0.45f, 0.1f, 1);
        public static readonly Color Brown_BackGround = new Color(0.75f, 0.45f, 0.1f, 0.368f);
    }

    public enum OperationType
    {
        OnPointEnter,
        OnPointExit,
        OnClick
    }

    public enum CellDirection
    {
        none,
        up,
        down,
        left,
        right,

    }
}
