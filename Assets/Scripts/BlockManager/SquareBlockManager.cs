using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class SquareBlockManager : BaseBlockManager
{
    private int currentTargetBlock;
    private int previousBlock;
    private int sideNum = 0;

    private Dictionary<int, List<int>> paths = new Dictionary<int, List<int>>();
    private Dictionary<int, List<int>> backPaths = new Dictionary<int, List<int>>();
    private Queue<GameObject> blockPoolEnable = new Queue<GameObject>();
    private Queue<GameObject> blockPoolDisable = new Queue<GameObject>();
    private SquareLevelScoreData scoreData = new SquareLevelScoreData();


    public void Awake()
    {
        Messenger<SquareLevelModel>.AddListener(MessageDefines.OnStartNewLevel, InitBlock);
    }

    public void OnDestroy()
    {
        Messenger<SquareLevelModel>.RemoveListener(MessageDefines.OnStartNewLevel, InitBlock);
    }

    public override void InitBlock(SquareLevelModel levelModel)
    {
        blockDictionary.Clear();
        backPaths.Clear();
        targetImage.Clear();
        targetPair.Clear();
        paths.Clear();
        sideNum = levelModel.side;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        float blockWidth = (rectTransform.sizeDelta.x) / levelModel.side;
        float blockHeight = (rectTransform.sizeDelta.y) / levelModel.side;//图片有底边 不是正方形1025*1045
        float widthScale = blockWidth / block.rectTransform.sizeDelta.x;
        float heightScale = blockHeight / block.rectTransform.sizeDelta.y;
        Vector2 origin = new Vector2(-rectTransform.sizeDelta.x * 0.5f + blockWidth * 0.5f, rectTransform.sizeDelta.y * 0.5f - blockHeight * 0.5f);
        for (int i = 0; i < levelModel.data.Count; i++)
        {
            List<int> points = levelModel.data[i];
            int length = points.Count;
            if (length < 2)
                continue;
            int start = points[0];
            int last = points[length - 1];
            targetPair.Add(start, last);
            targetPair.Add(last, start);
            targetImage.Add(start, i);
            targetImage.Add(last, i);
        }
        int childCount = rectTransform.childCount;
        int needCount = levelModel.side * levelModel.side;
        if (childCount < needCount)
        {
            for (int i = 0; i < (needCount - childCount); i++)
            {
                if (blockPoolDisable.Count > 0)
                {
                    GameObject obj = blockPoolDisable.Dequeue();
                    obj.SetActive(true);
                    blockPoolEnable.Enqueue(obj);
                }
                else
                {
                    GameObject obj = GameObject.Instantiate<GameObject>(block.gameObject, transform);
                    obj.SetActive(true);
                    blockPoolEnable.Enqueue(obj);
                }
            }
        }
        else if (childCount > needCount)
        {
            List<GameObject> deleteList = new List<GameObject>();
            for (int i = 0; i < (needCount - childCount); i++)
            {
                GameObject obj = blockPoolDisable.Dequeue();
                obj.SetActive(false);
                blockPoolDisable.Enqueue(obj);
            }
        }
        GameObject[] objArray = blockPoolEnable.ToArray();
        for (int i = 0; i < needCount; i++)
        {
            int h = i % levelModel.side;
            int v = i / levelModel.side;
            GameObject obj = objArray[i];
            obj.name = i.ToString();
            RectTransform rect = obj.GetComponent<RectTransform>();
            BaseBlock blockComponent = obj.GetComponent<BaseBlock>();
            Vector3 localScale = new Vector3(widthScale, heightScale, 1);
            Vector2 localPosition = new Vector2(origin.x + h * blockWidth, origin.y - v * blockHeight);
            if (!targetImage.ContainsKey(i))
            {
                blockComponent.Init(i, localPosition, localScale, -1, GetAroundBlock(i, sideNum));
            }
            else
            {
                blockComponent.Init(i, localPosition, localScale, targetImage[i], GetAroundBlock(i, sideNum));
            }
            blockDictionary.Add(i, blockComponent);
        }
        scoreData.totalStep = levelModel.linePair;
        scoreData.totalBlock = sideNum * sideNum;
        scoreData.best = levelModel.linePair;
        scoreData.moves = 0;
        scoreData.paint = 0;
        Messenger<SquareLevelScoreData>.Broadcast(MessageDefines.OnScoreChange, scoreData);
        Messenger<InputState>.RemoveAllListener(MessageDefines.OnPostSquareInput);
        Messenger<InputState, int>.AddListener(MessageDefines.OnPostSquareInput, OnPostSquareInput);
        paths.Clear();
    }

    public void OnPostSquareInput(InputState input, int blockID)
    {
        switch (input)
        {
            case InputState.OnBlockButtonDown:
                OnBlockButtonDown(blockID);
                UpdateAllBlock();
                break;
            case InputState.OnBlockButtonEnter:
                if (OnBlockButtonEnter(blockID))
                {
                    UpdateAllBlock();
                    //DebugPaths();
                    int moves = 0;
                    int paint = 0;
                    GetFinishedData(out moves, out paint);
                    scoreData.moves = moves;
                    scoreData.paint = paint;
                    Messenger<SquareLevelScoreData>.Broadcast(MessageDefines.OnScoreChange, scoreData);
                    if (blockID == targetPair[currentTargetBlock] && moves == (targetPair.Count / 2))
                    {
                        OnGameFinished();
                    }
                }
                break;
            case InputState.OnBlockButtonExit:
                OnBlockButtonExit(blockID);
                break;
            case InputState.OnInputUP:
                backPaths.Clear();
                break;
        }
    }

    void OnBlockButtonDown(int blockID)
    {
        BaseBlock block = GetBlock(blockID);
        if (block == null)
            return;
        if(targetPair.ContainsKey(blockID))
        {
            currentTargetBlock = blockID;
            previousBlock = blockID;
            if (!paths.ContainsKey(blockID))
            {
                paths[blockID] = new List<int>();
            }
            paths[blockID].Clear();
            paths[blockID].Add(blockID);
            if (targetPair.ContainsKey(blockID))
            {
                paths.Remove(targetPair[blockID]);
            }
        }
        else
        {
            foreach (KeyValuePair<int, List<int>> pair in paths)
            {
                if (!pair.Value.Contains(blockID))
                    continue;
                currentTargetBlock = pair.Key;
                previousBlock = blockID;
                int length = pair.Value.Count;
                int listIndex = pair.Value.IndexOf(blockID);
                pair.Value.RemoveRange(listIndex, length - listIndex);
                pair.Value.Add(blockID);
            }
        }
    }

    private bool IsLinedBlock(int id)
    {
        foreach(KeyValuePair<int,List<int>> pair in paths)
        {
            if (pair.Key == currentTargetBlock)
                continue;
            if (pair.Value.Contains(id))
                return true;
        }
        return false;
    }

    public int GetVaildCornerID(int blockID, LineDirection dir)
    {
        int cornerID = GetCornerID(blockID, dir);
        if ((targetPair.ContainsKey(cornerID) && targetPair[cornerID] != targetPair[currentTargetBlock]) || IsLinedBlock(cornerID))  //拐角为端点，去掉
        {
            cornerID = GetCornerID2(blockID, dir);
            if ((targetPair.ContainsKey(cornerID) && targetPair[cornerID] != targetPair[currentTargetBlock]) || IsLinedBlock(cornerID))
                return -1;
        }
        if (cornerID < 0 || cornerID > sideNum * sideNum - 1)
            return -1;
        return cornerID;
    }

    private List<int> Clone(List<int> list)
    {
        List<int> temp = new List<int>();
        foreach(int i in list)
        {
            temp.Add(i);
        }
        return temp;
    }

    bool OnBlockButtonEnter(int blockID)
    {
        if (previousBlock == blockID)
            return false;
        //滑到了非当前颜色的刷子
        if (targetPair.ContainsKey(blockID) && blockID != currentTargetBlock && targetPair[blockID] != currentTargetBlock)
            return false;
        List<int> path = paths[currentTargetBlock];
        if (previousBlock == targetPair[currentTargetBlock] && !path.Contains(blockID))//已经完成的线不需要
            return false;
        LineDirection dir = GetDirection(previousBlock, blockID);
        if(dir != LineDirection.None)//上下左右相邻
        {
            foreach (KeyValuePair<int, List<int>> pair in paths)
            {
                List<int> temp = pair.Value;
                if (temp.Contains(blockID))
                {
                    if (pair.Key != currentTargetBlock && !backPaths.ContainsKey(blockID))//拆坏了别的连线
                        backPaths.Add(blockID, Clone(pair.Value));
                    if (pair.Key == currentTargetBlock && backPaths.ContainsKey(previousBlock))//返回了拆坏的点
                    {
                        List<int> back = backPaths[previousBlock];
                        paths[back[0]] = Clone(back);
                    }
                    int length = temp.Count;
                    int listIndex = temp.IndexOf(blockID);
                    temp.RemoveRange(listIndex, length - listIndex);
                    break;
                }
            }
            path.Add(blockID);
            previousBlock = blockID;
            //DebugPaths();
            return true;
        }
        LineDirection dir2 = GetDirection2(previousBlock, blockID);
        if(dir2 != LineDirection.None)//在周围四角
        {
            Debug.Log("previousBlock:" + previousBlock);
            if (previousBlock == targetPair[currentTargetBlock])//已经连到了终点就不让拐角补齐
                return false;
            int cornerID = GetVaildCornerID(blockID, dir2);
            if (cornerID < 0)
                return false;
            foreach (KeyValuePair<int, List<int>> pair in paths)
            {
                if (pair.Key == currentTargetBlock)
                    continue;
                if (pair.Value.Contains(cornerID))
                    return false;//四角在别的线中就不做自动补齐
            }
            if(path.Contains(cornerID))//当前颜色有拐角点
            {
                int length = path.Count;
                int listIndex = path.IndexOf(cornerID);
                path.RemoveRange(listIndex, length - listIndex);
            }
            path.Add(cornerID);
            path.Add(blockID);
            previousBlock = blockID;
            //DebugPaths();
            return true;
        }
        return false;
    }

    void OnBlockButtonExit(int blockID)
    {
        //pass
    }

    private bool CheckGameFinished()
    {
        if (paths.Count * 2 != targetPair.Count)
            return false;
        foreach(KeyValuePair<int,List<int>> pair in paths)
        {
            List<int> path = pair.Value;
            if (path.Count < 2)
                return false;
            if(!targetPair.ContainsKey(path[0]) || !targetPair.ContainsKey(path[path.Count - 1]))
            {
                return false;
            }
            if (targetPair[path[0]] != path[path.Count - 1])
            {
                return false;
            }
        }
        return true;
    }

    private void OnGameFinished()
    {
        Debug.Log("恭喜通关！！！！");
        Messenger.Broadcast(MessageDefines.OnFinishLevel);
    }

    private void UpdateAllBlock()
    {
        foreach (KeyValuePair<int, BaseBlock> pair in blockDictionary)
        {
            pair.Value.ClearBlock();
        }
        foreach (KeyValuePair<int,List<int>> pair in paths)
        {
            List<int> path = pair.Value;
            int last = -1;
            LineDirection preDir = LineDirection.None;
            for (int i = 0; i < path.Count; i++)
            {
                if (i == 0)
                {
                    last = path[i];
                    continue;
                }
                LineDirection dir = GetDirection(last, path[i]);
                BaseBlock block = GetBlock(last);
                block.SetBlock(preDir, ReverseDirection(dir), targetImage[pair.Key]);
                preDir = dir;
                if(i == (path.Count - 1))
                {
                    int id = path[i];
                    if (id == targetPair[pair.Key])
                    {
                        block = GetBlock(id);
                        block.SetBlock(preDir, LineDirection.None, targetImage[pair.Key]);
                        //Debug.Log("block:" + block.id + "  preDir:" + preDir);
                    }
                    else
                    {
                        block = GetBlock(id);
                        block.SetBlock(preDir, LineDirection.Middle, targetImage[pair.Key]);
                        //Debug.Log("block:" + block.id + "  preDir:" + preDir);
                    }
                }
                last = path[i];
            }
        }
    }

    public void GetFinishedData(out int finishedPair, out int paint)
    {
        finishedPair = 0;
        paint = 0;
        foreach (KeyValuePair<int, List<int>> pair in paths)
        {
            int length = pair.Value.Count;
            paint += length;
            if (length < 2)
                continue;
            if (targetPair.ContainsKey(pair.Value[0]) && targetPair.ContainsKey(pair.Value[length - 1]))
            {
                finishedPair += 1;
            }
        }
    }

    public LineDirection ReverseDirection(LineDirection dir)
    {
        switch (dir)
        {
            case LineDirection.Up:
                return LineDirection.Down;
            case LineDirection.Down:
                return LineDirection.Up;
            case LineDirection.Right:
                return LineDirection.Left;
            case LineDirection.Left:
                return LineDirection.Right;
            case LineDirection.Middle:
                return LineDirection.Middle;
        }
        return LineDirection.None;
    }

    private LineDirection GetDirection2(int lastCell, int nextCell)
    {
        if (nextCell - lastCell == sideNum - 1 && nextCell % sideNum != sideNum - 1 && nextCell > sideNum - 1)
        {
            return LineDirection.RightUp;
        }
        if (nextCell - lastCell == sideNum + 1 && nextCell >= sideNum && nextCell % sideNum != 0)
        {
            return LineDirection.LeftUp;
        }
        if (lastCell - nextCell == sideNum - 1 && nextCell < sideNum * sideNum - sideNum && nextCell % sideNum != 0)
        {
            return LineDirection.LeftDown;
        }
        if (lastCell - nextCell == sideNum + 1 && nextCell < sideNum * sideNum - sideNum && nextCell % sideNum != sideNum - 1)
        {
            return LineDirection.RightDown;
        }
        return LineDirection.None;
    }

    private LineDirection GetDirection(int lastCell, int nextCell)
    {
        if (Mathf.Abs(lastCell - nextCell) % sideNum != 0 && Mathf.Abs(lastCell - nextCell) != 1)
        {
            return LineDirection.None;
        }
        if ((lastCell % sideNum == 0) && (lastCell - 1 == nextCell)) //位于左边
        {
            return LineDirection.None;
        }
        if ((lastCell % sideNum == sideNum - 1) && (lastCell + 1 == nextCell)) //位于右边
        {
            return LineDirection.None;
        }
        if (lastCell - nextCell == sideNum)
            return LineDirection.Down; //上
        if (nextCell - lastCell == sideNum)
            return LineDirection.Up; //下
        if (lastCell - nextCell == 1)
            return LineDirection.Right; //左
        if (nextCell - lastCell == 1)
            return LineDirection.Left; //右
        return LineDirection.None;//不相邻
    }

    public override void Line(int index)
    {
        throw new System.NotImplementedException();
    }

    public Dictionary<LineDirection, int> GetAroundBlock(int blockID, int side)
    {
        int allBlock = side * side;
        Dictionary<LineDirection, int> aroundBlock = new Dictionary<LineDirection, int>();
        int up = blockID - side;
        if (up >= 0)
        {
            aroundBlock.Add(LineDirection.Up, up);
        }
        int down = blockID + side;
        if (down < allBlock)
        {
            aroundBlock.Add(LineDirection.Down, down);
        }
        int left = blockID - 1;
        if (left >= 0 && (left % side != side - 1))
        {
            aroundBlock.Add(LineDirection.Left, left);
        }
        int right = blockID + 1;
        if (right < allBlock && (right % side != 0))
        {
            aroundBlock.Add(LineDirection.Right, right);
        }
        return aroundBlock;
    }

    private int GetCornerID(int blockID, LineDirection direction)
    {
        switch (direction)
        {
            case LineDirection.RightUp:
                return blockID - sideNum;
            case LineDirection.RightDown:
                return blockID + sideNum;
            case LineDirection.LeftUp:
                return blockID - sideNum;
            case LineDirection.LeftDown:
                return blockID + sideNum;
        }
        return -1;
    }

    private int GetCornerID2(int blockID, LineDirection direction)
    {
        switch (direction)
        {
            case LineDirection.RightUp:
                return blockID + 1;
            case LineDirection.RightDown:
                return blockID + 1;
            case LineDirection.LeftUp:
                return blockID - 1;
            case LineDirection.LeftDown:
                return blockID - 1;
        }
        return -1;
    }

    private void DebugPaths()
    {
#if UNITY_EDITOR
        foreach (KeyValuePair<int, List<int>> pair in paths)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(pair.Key);
            strBuilder.Append(" :");
            foreach (int i in pair.Value)
            {
                strBuilder.Append(i);
                strBuilder.Append(",");
            }
            Debug.Log(strBuilder.ToString());
        }
#endif
    }
}
