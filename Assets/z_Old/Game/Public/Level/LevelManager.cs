using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class LevelManager : MonoBehaviour
    {
        int level = 1;
        List<MapData> mapDatas = new List<MapData>();
        [SerializeField] private BlockPlayController playController;
        // Use this for initialization
        void Awake()
        {
            mapDatas.Add(new MapData { level = 0, horizontalNum = 5, verticalNum = 5, points = new int[] { 0, 21, 2, 16, 7, 22, 4, 18, 9, 23 } });
            mapDatas.Add(new MapData { level = 1, horizontalNum = 6, verticalNum = 6, points = new int[] { 0, 24, 1, 30, 2, 14, 4, 20, 5, 32, 10, 26} });
            mapDatas.Add(new MapData { level = 2, horizontalNum = 7, verticalNum = 7, points = new int[] { 7, 47, 12, 15, 13, 39, 24, 30, 25, 48, 32, 40} });
            mapDatas.Add(new MapData { level = 3, horizontalNum = 8, verticalNum = 8, points = new int[] { 4, 57, 12, 23, 14, 20, 15, 21, 27, 36, 28, 44 } });
            Messenger.AddListener(MessageDefines.NextLevel, NextLevel);
        }

        private void Start()
        {
            playController.StartPlay(mapDatas[level - 1]);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void NextLevel()
        {
            level += 1;
            if (level > mapDatas.Count)
                level = 1;
            MapData mapData = mapDatas[level - 1];
            playController.StartPlay(mapData);
        }
    }
}
