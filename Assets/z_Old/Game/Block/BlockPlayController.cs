using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree {
    public class BlockPlayController : MonoBehaviour {

        [SerializeField] BlockMap map;
        [SerializeField] RectTransform touchImageTransform;
        [SerializeField] RectTransform canvasRectTransform;
        [SerializeField] Image touchImage;

        RectTransform touchImageParent;
        Vector2 touchLocalPosition = Vector2.zero;

        private BaseCell currentCircle;
        private Color currentColor = FlowFreeColors.None;
        int lastCell = -1;
        int startCell = -1;
        int pairNum = 0;
        Dictionary<int, List<int>> paths = new Dictionary<int, List<int>> ();
        Dictionary<int, Color> colors = new Dictionary<int, Color> ();
        bool isTouched = false;
        bool needShowTouchImage = false;
        Color touchPointColor = Color.clear;

        public void StartPlay(MapData mapData)
        {
            paths.Clear();
            colors.Clear();
            map.InitMap(mapData);
            pairNum = mapData.points.Length / 2;
            AddListener();
            touchImageParent = touchImageTransform.parent.GetComponent<RectTransform>();
        }

        public void AddListener () {
            Messenger<int>.RemoveAllListener(MessageDefines.OnCellButtonDown);
            Messenger<int>.RemoveAllListener(MessageDefines.OnCellPointEnter);
            Messenger<int>.RemoveAllListener(MessageDefines.OnCellPointExit);
            Messenger<int>.AddListener (MessageDefines.OnCellButtonDown, OnCellButtonDown);
            Messenger<int>.AddListener (MessageDefines.OnCellPointEnter, OnCellButtonEnter);
            Messenger<int>.AddListener (MessageDefines.OnCellPointExit, OnCellButtonEnter);
        }

        private void StartLine(int index)
        {
            BaseCell cell = map.GetCell(index);
            if (cell == null || cell.GetContentType() == CellContentType.None)
                return;
            lastCell = index;
            Debug.Log("Cut:" + lastCell);
            startCell = index;
            if (!paths.ContainsKey(index))
            {
                paths[index] = new List<int>();
            }
            paths[index].Clear();
            paths[index].Add(index);
            int target = cell.GetTarget();
            if (paths.ContainsKey(target))
                paths.Remove(target);
            colors[index] = GetColor(map.GetCell(index).GetContentType());
        }

        private bool CutLine(int index)
        {
            foreach (KeyValuePair<int, List<int>> pair in paths)
            {
                if (!pair.Value.Contains(index))
                    continue;
                startCell = pair.Key;
                lastCell = index;
                Debug.Log("Cut:" + lastCell);
                int length = pair.Value.Count;
                int listIndex = pair.Value.IndexOf(index);
                pair.Value.RemoveRange(listIndex, length - listIndex);
                pair.Value.Add(index);
                return true;
            }
            return false;
        }

        private void AddLine(int lastCell, int index)
        {
            if (!map.IsAroundCell(lastCell, index))
                return;
            if (!paths.ContainsKey(startCell))
                return;
            List<int> path = paths[startCell];
            if (!path.Contains(index))
            {
                foreach (KeyValuePair<int, List<int>> pair in paths)
                {
                    if (pair.Key == startCell)
                        continue;
                    List<int> temp = pair.Value;
                    if (temp.Contains(index))
                    {
                        int length = temp.Count;
                        int listIndex = temp.IndexOf(index);
                        temp.RemoveRange(listIndex, length - listIndex);
                    }
                }
                path.Add(index);
            }
            else
            {
                int length = path.Count;
                int listIndex = path.IndexOf(index);
                path.RemoveRange(listIndex, length - listIndex);
                path.Add(index);
            }
            this.lastCell = index;
        }

        private void DebugPaths()
        {
            foreach(KeyValuePair<int,List<int>> pair in paths)
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append(pair.Key);
                strBuilder.Append(" :");
                foreach(int i in pair.Value)
                {
                    strBuilder.Append(i);
                    strBuilder.Append(",");
                }
                Debug.Log(strBuilder.ToString());
            }
        }

        private void UpdateLineBackGround()
        {
            foreach (KeyValuePair<int, List<int>> pair in paths)
            {
                if ((startCell == pair.Key && !IsInputTouched()) || startCell != pair.Key)//当前路径按下状态    不是当前的路径
                {
                    BaseCell circle = map.GetCell(pair.Key);
                    if (circle == null)
                        continue;
                    Color background = circle.GetBackgroundColor();
                    foreach (int i in pair.Value)
                    {
                        BaseCell cell = map.GetCell(i);
                        if (cell == null)
                            continue;
                        cell.cellView.SetBackGround(background);
                        Debug.Log("background:" + background);
                    }
                }
            }
        }

        private void UpdateLineView () {
            //清除
            foreach(KeyValuePair<int,BaseCell> pair in map.GetAllCell()){
                pair.Value.cellView.Clear();
            }
            //刷新
            foreach (KeyValuePair<int, List<int>> pair in paths) {
                List<int> path = pair.Value;
                int last = -1;
                foreach (int i in path) {
                    if (last < 0)
                        last = i;
                    if (last == i)
                        continue;
                    CellDirection dir = map.GetNextCellDirection (last, i);
                    if (dir == CellDirection.none)
                        continue;
                    BaseCell lCell = map.GetCell(last);
                    BaseCell nCell = map.GetCell(i);
                    Color color = colors[pair.Key];
                    lCell.cellView.AddLine(dir, color);
                    nCell.cellView.AddLine(GetCellInverseDir(dir), color);
                    last = i;
                }
            }
            foreach (KeyValuePair<int, List<int>> pair in paths)
            {
                List<int> path = pair.Value;
                if (path.Count <= 1)
                    continue;
                int pathLast = path[path.Count - 1];
                BaseCell cell = map.GetCell(pathLast);
                cell.cellView.SetEnd(colors[pair.Key]);
            }
            UpdateLineBackGround();
            if (CheckIsFinished())
                OnGameFinished();
        }

        private bool CheckIsFinished()
        {
            int i = 0;
            foreach (KeyValuePair<int, List<int>> pair in paths)
            {
                BaseCell cell = map.GetCell(pair.Key);
                if (cell == null || pair.Value.Count < 1)
                    return false;
                if (pair.Value[pair.Value.Count - 1] != cell.GetTarget())
                    return false;
                i += 1;
            }
            if( i == pairNum)
                return true;
            return false;
        }

        public void OnCellButtonDown (int index) {
            Debug.Log ("press index:" + index);
            BaseCell cell = map.GetCell (index);
            if (cell == null)
                return;
            if (cell.GetContentType() != CellContentType.None)
            {
                StartLine(index);
                UpdateLineView();
                BaseCell target = map.GetCell(cell.GetTarget());
                if (target != null)
                {
                    cell.cellView.ClickCircleAnimation();
                    target.cellView.ClickCircleAnimation();
                }
            }
            else
            {
                if (!CutLine(index))
                    return;
                UpdateLineView();
                BaseCell target = map.GetCell(map.GetCell(startCell).GetTarget());
                if (target != null)
                {
                    target.cellView.ClickCircleAnimation();
                    cell.cellView.ClickEndAnimation();
                }
            }
            needShowTouchImage = true;
            Color color = colors[startCell];
            touchImage.color = new Color(color.r, color.g, color.b, 0.3f);
        }

        private bool IsInputTouched()
        {
            //if (Input.GetMouseButton(0)) //鼠标，todo触屏
            if(isTouched)
                return true;
            return false;
        }

        public void OnCellButtonEnter (int index) {
            if (lastCell == index || index == startCell)
                return;
            if (!IsInputTouched())
                return;
            Debug.Log("enter index:" + index + "   ;" + "lastIndex:" + lastCell);
            BaseCell start = map.GetCell (startCell);
            if (start == null || !paths.ContainsKey (startCell) || !colors.ContainsKey (startCell))
                return;
            BaseCell cell1 = map.GetCell (lastCell);
            BaseCell cell2 = map.GetCell (index);
            if (cell1 == null || cell2 == null)
                return;
            CellContentType startContent = map.GetCell(startCell).GetContentType();
            if (paths.ContainsKey(startCell))
            {
                List<int> path = paths[startCell];
                if (path.Count > 1)
                {
                    int final = path[path.Count - 1];
                    BaseCell finalCell = map.GetCell(final);
                    if(finalCell.GetContentType() == startContent)
                    {
                        return;
                    }
                }

            }
            if (cell2.GetContentType() == CellContentType.None || cell2.GetContentType() == startContent) {//未到终点 || 连线完成
                Debug.Log("lastCell:" + lastCell + "    " + "index:" + index);
                AddLine(lastCell, index);
                UpdateLineView();
                DebugPaths();
            }
            
        }

        public void OnCellButtonExit(int index)
        {

        }

        public void OnGameFinished()
        {
            Debug.Log("恭喜通关！！！！");
            Messenger<bool>.Broadcast(MessageDefines.ShowOrHideFinishedWindow, true);
        }

        private CellDirection GetCellInverseDir (CellDirection dir) {
            switch (dir) {
                case CellDirection.none:
                    return CellDirection.none;
                case CellDirection.down:
                    return CellDirection.up;
                case CellDirection.up:
                    return CellDirection.down;
                case CellDirection.left:
                    return CellDirection.right;
                case CellDirection.right:
                    return CellDirection.left;
            }
            return CellDirection.none;
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_IPHONE || UNITY_ANDROID
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                isTouched = true;
            }
            else if ((Input.touches[0].phase == TouchPhase.Ended) && isTouched)
            {
                isTouched = false;
                UpdateLineBackGround();
                if (needShowTouchImage)
                {
                    needShowTouchImage = false;
                    HideTouchImage();
                }
            }
            if (needShowTouchImage)
                ShowTouchImage();

#else

            if (Input.GetMouseButtonDown(0))
            {
                isTouched = true;
            }
            else if (Input.GetMouseButtonUp(0) && isTouched)
            {
                isTouched = false;
                UpdateLineBackGround();
                if (needShowTouchImage)
                {
                    needShowTouchImage = false;
                    HideTouchImage();
                }
            }
            if (needShowTouchImage)
                ShowTouchImage();
#endif
        }

        
        private void ShowTouchImage()
        {
#if UNITY_IPHONE || UNITY_ANDROID
            Vector2 position = Input.touches[0].position;
#else 
            Vector2 position = Input.mousePosition;
#endif
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, Camera.main, out touchLocalPosition);
            touchImageTransform.anchoredPosition = touchLocalPosition;
            touchImageTransform.gameObject.SetActive(true);

        }

        private void HideTouchImage()
        {
            touchImageTransform.gameObject.SetActive(false);
        }

        private Color GetColor (CellContentType contentType) {
            switch (contentType) {
                case CellContentType.None:
                    return FlowFreeColors.None;
                case CellContentType.Blue:
                    return FlowFreeColors.Blue;
                case CellContentType.Green:
                    return FlowFreeColors.Green;
                case CellContentType.Yellow:
                    return FlowFreeColors.Yellow;
                case CellContentType.Red:
                    return FlowFreeColors.Red;
                case CellContentType.LittleBule:
                    return FlowFreeColors.LittleBule;
                case CellContentType.Brown:
                    return FlowFreeColors.Brown;
            }
            return FlowFreeColors.None;
        }
    }
}