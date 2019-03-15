using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public enum BlockCellState
    {
        circle, //实心原
        Enter, //进入一条线
        Exit, //两条线出去
        None, //空的
    }
    public class BlockCellView : BaseCellView
    {
        [SerializeField] private Image circle;
        [SerializeField] private UIAnimationController clickCircle;
        [SerializeField] private Image middle;
        [SerializeField] private Image end; //连线终点
        [SerializeField] private UIAnimationController clickEnd;
        [SerializeField] private Image up;
        [SerializeField] private Image right;
        [SerializeField] private Image left;
        [SerializeField] private Image down;
        [SerializeField] private Image background;

        private BlockCellState state = BlockCellState.None;

        private CellDirection enterDir;
        private CellDirection exitDir;

        public override void SetBackGround(Color color)
        {
            background.color = color;
            background.gameObject.SetActive(true);
        }
        public void SetMiddle(Color color)
        {
            middle.color = color;
            middle.gameObject.SetActive(true);
        }
        public void SetCircle(Color color)
        {
            circle.color = color;
            circle.gameObject.SetActive(true);
        }
        public Color GetBackGroundColor()
        {
            return background.color;
        }
        public void ClickCircleAnimation()
        {
            clickCircle.Play();
        }
        public void ClickEndAnimation()
        {
            clickEnd.Play();
        }
        public void SetEnd(Color color)
        {
            end.color = color;
            end.gameObject.SetActive(true);
        }

        public Color GetCircleColor()
        {
            return circle.color;
        }

        public void AddLine(CellDirection dir, Color color)
        {
            switch (dir)
            {
                case CellDirection.none:
                    Clear();
                    break;
                case CellDirection.up:
                    up.color = color;
                    up.gameObject.SetActive(true);
                    UpdateMiddleWhenAdd(CellDirection.up, color);
                    break;
                case CellDirection.down:
                    down.color = color;
                    down.gameObject.SetActive(true);
                    UpdateMiddleWhenAdd(CellDirection.down, color);
                    break;
                case CellDirection.left:
                    left.color = color;
                    left.gameObject.SetActive(true);
                    UpdateMiddleWhenAdd(CellDirection.left, color);
                    break;
                case CellDirection.right:
                    right.color = color;
                    right.gameObject.SetActive(true);
                    UpdateMiddleWhenAdd(CellDirection.right, color);
                    break;
            }
        }

        public void ClearCircle()
        {
            circle.color = FlowFreeColors.None;
            circle.gameObject.SetActive(false);
        }

        public void ClearUp()
        {
            up.color = FlowFreeColors.None;
            up.gameObject.SetActive(false);
        }

        public void ClearDown()
        {
            down.color = FlowFreeColors.None;
            down.gameObject.SetActive(false);
        }

        public void ClearLeft()
        {
            left.color = FlowFreeColors.None; ;
            left.gameObject.SetActive(false);
        }

        public void ClearRight()
        {
            right.color = FlowFreeColors.None; ;
            right.gameObject.SetActive(false);
        }

        protected void ClearMiddle()
        {
            middle.color = FlowFreeColors.None; ;
            middle.gameObject.SetActive(false);
        }

        protected void ClearEnd()
        {
            end.color = FlowFreeColors.None;
            end.gameObject.SetActive(false);
        }

        protected void ClearBackground()
        {
            background.color = FlowFreeColors.None;
            background.gameObject.SetActive(false);
        }

        protected void UpdateMiddleWhenAdd(CellDirection dir, Color color)
        {
            if (dir == enterDir && enterDir != CellDirection.none)
                return;
            if (enterDir == CellDirection.none)
            {
                enterDir = dir;
            }
            else
            {
                exitDir = dir;
                SetMiddle(color);
            }
        }

        protected void UpdateMiddleWhenDel(CellDirection dir, Color color)
        {
            if (dir == enterDir && enterDir != CellDirection.none)
                return;
            if (enterDir == CellDirection.none)
            {
                enterDir = dir;
            }
            else
            {
                exitDir = dir;
                SetMiddle(color);
            }
        }

        public void Clear()
        {
            ClearUp();
            ClearDown();
            ClearLeft();
            ClearRight();
            ClearMiddle();
            ClearEnd();
            ClearBackground();
        }
    }
}