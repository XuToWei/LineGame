using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree {

    public class BlockMap : BaseMap {
        [SerializeField] private GridLayoutGroup gridLayoutGroup;

        protected override void Awake () {
            base.Awake ();
            Messenger<MapData>.AddListener (MessageDefines.BlockMapInit, InitMap);
        }

        public override void InitMap (MapData mapData) {
            this.mapData = mapData;
            float cellWidth = rectTransform.sizeDelta.x / mapData.horizontalNum;
            float cellHeight = rectTransform.sizeDelta.y / mapData.verticalNum;
            gridLayoutGroup.cellSize = new Vector2 (cellWidth, cellHeight);
            gridLayoutGroup.enabled = false;
            ClearCells ();
            Vector2 origin = new Vector2(rectTransform.anchoredPosition.x - rectTransform.sizeDelta.x * 0.5f + cellWidth * 0.5f, 
                rectTransform.anchoredPosition.y + rectTransform.sizeDelta.y * 0.5f - cellHeight * 0.5f);
            RectTransform cellTransform = cellTemplate.gameObject.GetComponent<RectTransform>();
            float widthScale = cellWidth / cellTransform.sizeDelta.x;
            float heightScale = cellHeight / cellTransform.sizeDelta.y;
            for (int i = 0; i < mapData.horizontalNum * mapData.verticalNum; i++) {
                int h = i % mapData.horizontalNum;
                int v = i / mapData.verticalNum;
                GameObject obj = GameObject.Instantiate (cellTemplate.gameObject, transform);
                obj.name = i.ToString ();
                BaseCell cell = obj.GetComponent<BaseCell>();
                RectTransform cellTrans = obj.GetComponent<RectTransform>();
                cellTrans.localScale = new Vector3(widthScale, heightScale, 1);
                cellTrans.localPosition = new Vector2(origin.x + h * cellWidth, origin.y - v * cellHeight);
                cell.SetIndex (i);
                cellList[i] = cell;
            }
            for (int i = 0; i < mapData.points.Length; i++) {
                if (mapData.points[i] < 0) {
                    Debug.LogWarning ("关卡数据不能为负数！");
                    return;
                }
                if (mapData.points[i] > (cellList.Count - 1)) {
                    Debug.LogWarning ("关卡数据超过了格子数！");
                    return;
                }
                BaseCell cell = cellList[mapData.points[i]];
                int target = i;
                if(i % 2 == 0)
                    target = i + 1;
                else
                    target = i - 1;
                cell.SetCellContent (GetCellContentTyp (i), mapData.points[target]);
            }
        }

        private CellContentType GetCellContentTyp (int index) {
            if (index < 0)
                return CellContentType.None;
            switch (index / 2) {
                case 0:
                    return CellContentType.Red;
                case 1:
                    return CellContentType.Blue;
                case 2:
                    return CellContentType.Green;
                case 3:
                    return CellContentType.Yellow;
                case 4:
                    return CellContentType.LittleBule;
                case 5:
                    return CellContentType.Brown;
            }
            return CellContentType.None;
        }

        public override bool IsAroundCell (int cell1, int cell2) {
            if (cell1 == cell2) {
                Debug.Log ("相同的方块不能检查！");
                return false;
            }
            int totalNum = mapData.horizontalNum * mapData.verticalNum;
            if (cell1 < 0 || cell2 < 0 || cell1 > totalNum || cell2 > totalNum) {
                Debug.Log ("数据非法");
                return false;
            }
            if (Mathf.Abs (cell1 - cell2) % mapData.horizontalNum != 0 && Mathf.Abs (cell1 - cell2) != 1) {
                return false;
            }
            BaseCell block1 = GetCell (cell1);
            BaseCell block2 = GetCell (cell2);
            if (block1 == null || block2 == null) {
                Debug.Log ("数据错误，请检查！");
                return false;
            }
            if ((cell1 % mapData.horizontalNum == 0) && (cell1 - 1 == cell2)) //位于左边
            {
                return false;
            }
            if ((cell1 % mapData.horizontalNum == mapData.horizontalNum - 1) && (cell1 + 1 == cell2)) //位于右边
            {
                return false;
            }
            int offset = Mathf.Abs(cell1 - cell2);
            if (offset == 1 || offset == mapData.horizontalNum)
                return true;
            return false;
        }

        public override CellDirection GetNextCellDirection (int lastCell, int nextCell) {
            if (!IsAroundCell (lastCell, nextCell))
                return CellDirection.none; //没有相邻
            if (lastCell - nextCell == mapData.horizontalNum)
                return CellDirection.up; //上
            if (nextCell - lastCell == mapData.horizontalNum)
                return CellDirection.down; //下
            if (lastCell - nextCell == 1)
                return CellDirection.left; //左
            if (nextCell - lastCell == 1)
                return CellDirection.right; //右
            return CellDirection.none;
        }

    }
}