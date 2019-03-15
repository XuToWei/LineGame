using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    [Serializable]
    public class MapData
    {
        public MapType mapType;
        public int horizontalNum = 5;
        public int verticalNum = 5;
        public int level;
        public int[] points = new int[] { 1, 2, 3, 4 };

    }
}
