using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlowFree
{
    public abstract class BaseCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] protected Button cellButton;
        [SerializeField] public BlockCellView cellView;
        protected Color backgroundColor;

        protected CellContentType contentType;
        protected int index;
        protected int target;
        protected int cellState = 0;//方块的状态

        public virtual CellContentType GetContentType()
        {
            return contentType;
        }
        public virtual int GetTarget(){
            return target;
        }
        public virtual Color GetBackgroundColor()
        {
            return backgroundColor;
        }
        public virtual void SetIndex(int index)
        {
            this.index = index;
        }

        public abstract void SetCellContent(CellContentType cellContentType, int target);
        public abstract void Start();
        public abstract void SetCellState(int state);
        public abstract int GetCellState();
        
        public virtual void Destroy()
        {
            Destroy(gameObject);
        }

        public abstract void OnPointerClick(PointerEventData eventData);
        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);
        public abstract void OnPointerDown(PointerEventData eventData);
    }


}
