using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree{
    public abstract class BaseMap : MonoBehaviour {
        [SerializeField] protected BaseCell cellTemplate;
        [SerializeField] protected RectTransform plane;
        protected Dictionary<int, BaseCell> cellList = new Dictionary<int, BaseCell>();
        protected RectTransform rectTransform;
        protected MapData mapData;

        protected virtual void Awake()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
        }

        public virtual BaseCell GetCell(int index)
        {
            if (cellList.ContainsKey(index))
                return cellList[index];
            return null;
        }

        public virtual Dictionary<int, BaseCell> GetAllCell(){
            return cellList;
        }

        public abstract void InitMap(MapData mapData);
        public abstract bool IsAroundCell(int cell1, int cell2);
        public abstract CellDirection GetNextCellDirection(int lastCell, int nextCell);

        protected void ClearCells()
        {
            if (cellList.Count < 1)
                return;
            foreach (var cell in cellList)
            {
                cell.Value.Destroy();
            }
            cellList.Clear();
        }

    }
}
