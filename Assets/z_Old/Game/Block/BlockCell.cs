using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlowFree {
    public class BlockCell : BaseCell {
        public override void Start () {

        }

        public override void SetCellState (int state) {
            this.cellState = state;
        }

        public override int GetCellState () {
            return cellState;
        }

        public override void OnPointerClick (PointerEventData eventData) {
            
        }
        public override void OnPointerEnter (PointerEventData eventData) {
            Messenger<int>.Broadcast (MessageDefines.OnCellPointEnter, index);
        }
        public override void OnPointerExit (PointerEventData eventData) {
            Messenger<int>.Broadcast (MessageDefines.OnCellPointExit, index);
        }
        public override void OnPointerDown(PointerEventData eventData){
            Messenger<int>.Broadcast (MessageDefines.OnCellButtonDown, index);
        }
        public override void SetCellContent (CellContentType cellContentType, int target) {
            cellView.ClearCircle ();
            cellView.Clear ();
            contentType = cellContentType;
            this.target = target;
            backgroundColor = FlowFreeColors.None;
            switch (cellContentType) {
                case CellContentType.None:
                    break;
                case CellContentType.Green:
                    cellView.SetCircle(FlowFreeColors.Green);
                    backgroundColor = FlowFreeColors.Green_BackGround;
                    break;
                case CellContentType.Yellow:
                    cellView.SetCircle(FlowFreeColors.Yellow);
                    backgroundColor = FlowFreeColors.Yellow_BackGround;
                    break;
                case CellContentType.Red:
                    cellView.SetCircle(FlowFreeColors.Red);
                    backgroundColor = FlowFreeColors.Red_BackGround;
                    break;
                case CellContentType.Blue:
                    cellView.SetCircle(FlowFreeColors.Blue);
                    backgroundColor = FlowFreeColors.Blue_BackGround;
                    break;
                case CellContentType.LittleBule:
                    cellView.SetCircle(FlowFreeColors.LittleBule);
                    backgroundColor = FlowFreeColors.LittleBule_BackGround;
                    break;
                case CellContentType.Brown:
                    cellView.SetCircle(FlowFreeColors.Brown);
                    backgroundColor = FlowFreeColors.Brown_BackGround;
                    break;
            }
        }

    }
}