using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SquareLevelManager : MonoBehaviour
{
    [SerializeField] string fileName;
    [SerializeField] SquareBlockManager blockManager;
    Dictionary<int, Dictionary<int, SquareLevelModel>> levelModels = new Dictionary<int, Dictionary<int, SquareLevelModel>>();
    int currentLevel = 7;
    int currentSide = 5;

    public void Start()
    {
        Init();
        NextLevel();
    }

    public void Init()
    {
#if UNITY_ANDROID
        //string file = "jar:file://" + Application.dataPath + "!/assets/levelpack_0.txt";
        //string filePath = Application.dataPath + "/assets/";
        //BuildLevelFromFile("jar:file://" + Application.dataPath + "!/assets/levelpack_0.txt");
        //if (!Directory.Exists(Application.dataPath))
        //{
        //    test.text = "No" + Application.dataPath;
        //}
        //else
        //{
        //    test.text = "Yes";
        //}
        BuildLevelFromFile2("levelpack_0");
#else
        BuildLevelFromFile(Path.Combine(Application.streamingAssetsPath, fileName));
#endif
    }

    public void OnEnable()
    {
        Messenger.AddListener(MessageDefines.OnClickNextLevelButton, NextLevel);
    }

    public void OnDisable()
    {
        Messenger.RemoveListener(MessageDefines.OnClickNextLevelButton, NextLevel);
    }

    private void NextLevel()
    {
        currentLevel += 1;
        Dictionary<int, SquareLevelModel> levels = levelModels[currentSide];
        //if (!levels.ContainsKey(currentLevel))
        //{
        //    return;
        //}
        SquareLevelModel levelModel = levels[currentLevel];
        Messenger<SquareLevelModel>.Broadcast(MessageDefines.OnStartNewLevel, levelModel);
    }


    public void BuildLevelFromFile2(string fileName)
    {
        //Application.targetFrameRate = 60;
        TextAsset text = Resources.Load<TextAsset>(fileName);
        string[] allString = text.text.Split(Environment.NewLine.ToCharArray());
        foreach (string str in allString)
        {
            if (string.IsNullOrEmpty(str))
                continue;
            string[] totalStr = str.Split(';');
            string[] str1 = totalStr[0].Split(',');
            int pair = int.Parse(str1[3]);
            List<List<int>> dataList = new List<List<int>>();
            for (int i = 1; i <= pair; i++)
            {
                string[] tempStr = totalStr[i].Split(',');
                List<int> list = new List<int>();
                for (int j = 0; j < tempStr.Length; j++)
                {
                    list.Add(int.Parse(tempStr[j]));
                }
                dataList.Add(list);
            }
            SquareLevelModel levelModel = new SquareLevelModel()
            {
                level = int.Parse(str1[2]),
                side = int.Parse(str1[0]),
                linePair = pair,
                data = dataList,
            };
            if (!levelModels.ContainsKey(levelModel.side))
            {
                levelModels.Add(levelModel.side, new Dictionary<int, SquareLevelModel>());
            }
            levelModels[levelModel.side].Add(levelModel.level, levelModel);
        }
    }

    public void BuildLevelFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Debug.Log(fileName + " not exist!");
            return;
        }
        FileStream fileStream = File.OpenRead(fileName);
        foreach (string str in File.ReadAllLines(fileName))
        {
            string[] totalStr = str.Split(';');
            string[] str1 = totalStr[0].Split(',');
            int pair = int.Parse(str1[3]);
            List<List<int>> dataList = new List<List<int>>();
            for (int i = 1; i <= pair; i++)
            {
                string[] tempStr = totalStr[i].Split(',');
                List<int> list = new List<int>();
                for (int j = 0; j < tempStr.Length; j++)
                {
                    list.Add(int.Parse(tempStr[j]));
                }
                dataList.Add(list);
            }
            SquareLevelModel levelModel = new SquareLevelModel()
            {
                level = int.Parse(str1[2]),
                side = int.Parse(str1[0]),
                linePair = pair,
                data = dataList,
            };
            if (!levelModels.ContainsKey(levelModel.side))
            {
                levelModels.Add(levelModel.side, new Dictionary<int, SquareLevelModel>());
            }
            levelModels[levelModel.side].Add(levelModel.level, levelModel);
        }
    }


}
