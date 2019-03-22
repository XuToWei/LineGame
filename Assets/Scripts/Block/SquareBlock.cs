using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SquareBlock : BaseBlock {

    //[SerializeField] protected Image halfLine;
    //[SerializeField] protected RectTransform halfLineTransform;
    //[SerializeField] protected Image straightLine;
    //[SerializeField] protected RectTransform straightLineTransform;
    //[SerializeField] protected Image bendLine;
    //[SerializeField] protected RectTransform bendLineTransform;
    //[SerializeField] protected Image terminalLine;
    //[SerializeField] protected RectTransform terminalLineTransform;

    [SerializeField] Image[] halfLines;
    [SerializeField] Image[] straightLines;
    [SerializeField] Image[] bendLines;
    [SerializeField] Image[] terminalLines;

    public override void Init(int index, Vector2 position, Vector3 scale, int imageIndex, Dictionary<LineDirection,int> aroundBlock)
    {
        base.Init(index, position, scale, imageIndex, aroundBlock);
        if (imageIndex >= 0)
        {
            //terminalLine.sprite = terminalLines[imageIndex];
            ShowImage(terminalLines, imageIndex, Quaternion.identity);
        }
        ClearBlock();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        Messenger<int>.Broadcast(MessageDefines.OnBlockButtonDown, id);
    }

    //记录下周围的方块ID，方便提前进入下一个方块，解决突兀感
    public override void OnPointerEnter(PointerEventData eventData)
    {
        Messenger<int>.Broadcast(MessageDefines.OnBlockPointEnter, id);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Messenger<int>.Broadcast(MessageDefines.OnBlockPointExit, id);
    }


    public override void ClearBlock()
    {
        HideImage(halfLines);
        HideImage(straightLines);
        HideImage(bendLines);
        HideImage(terminalLines);
        //halfLine.gameObject.SetActive(false);
        //straightLine.gameObject.SetActive(false);
        //bendLine.gameObject.SetActive(false);
        //terminalLine.gameObject.SetActive(false);
    }

    public override void SetBlock(LineDirection inLine, LineDirection outLine, int imageIndex)
    {
        if (inLine == outLine || imageIndex < 0)
            return;
        if (outLine == LineDirection.Middle)
        {
            ShowImage(halfLines, imageIndex, GetHalfLineRotation(inLine));
            HideImage(straightLines);
            HideImage(bendLines);
            //halfLine.gameObject.SetActive(true);
            //halfLine.sprite = halfLines[imageIndex];
            //halfLineTransform.localRotation = GetHalfLineRotation(inLine);
            //straightLine.gameObject.SetActive(false);
            //bendLine.gameObject.SetActive(false);
        }
        else if ((outLine == LineDirection.Up && inLine == LineDirection.Down) || (outLine == LineDirection.Down && inLine == LineDirection.Up))
        {
            ShowImage(straightLines, imageIndex, BlockImageRotation.rotation0);
            HideImage(bendLines);
            HideImage(halfLines);
            //straightLine.gameObject.SetActive(true);
            //straightLine.sprite = straightLines[imageIndex];
            //straightLineTransform.localRotation = BlockImageRotation.rotation0;
            //bendLine.gameObject.SetActive(false);
            //halfLine.gameObject.SetActive(false);
        }
        else if ((outLine == LineDirection.Right && inLine == LineDirection.Left) || (outLine == LineDirection.Left && inLine == LineDirection.Right))
        {
            ShowImage(straightLines, imageIndex, BlockImageRotation.rotation90);
            HideImage(bendLines);
            HideImage(halfLines);
            //straightLine.gameObject.SetActive(true);
            //straightLine.sprite = straightLines[imageIndex];
            //straightLineTransform.localRotation = BlockImageRotation.rotation90;
            //bendLine.gameObject.SetActive(false);
            //halfLine.gameObject.SetActive(false);
        }
        else if ((outLine == LineDirection.Right && inLine == LineDirection.Up) || (outLine == LineDirection.Up && inLine == LineDirection.Right))
        {
            ShowImage(bendLines, imageIndex, BlockImageRotation.rotation0);
            HideImage(straightLines);
            HideImage(halfLines);
            //bendLine.gameObject.SetActive(true);
            //bendLine.sprite = bendLines[imageIndex];
            //bendLineTransform.localRotation = BlockImageRotation.rotation0;
            //straightLine.gameObject.SetActive(false);
            //halfLine.gameObject.SetActive(false);
        }
        else if ((outLine == LineDirection.Right && inLine == LineDirection.Down) || (outLine == LineDirection.Down && inLine == LineDirection.Right))
        {
            ShowImage(bendLines, imageIndex, BlockImageRotation.rotation270);
            HideImage(straightLines);
            HideImage(halfLines);
            //bendLine.gameObject.SetActive(true);
            //bendLine.sprite = bendLines[imageIndex];
            //bendLineTransform.localRotation = BlockImageRotation.rotation270;
            //straightLine.gameObject.SetActive(false);
            //halfLine.gameObject.SetActive(false);
        }
        else if ((outLine == LineDirection.Left && inLine == LineDirection.Down) || (outLine == LineDirection.Down && inLine == LineDirection.Left))
        {
            ShowImage(bendLines, imageIndex, BlockImageRotation.rotation180);
            HideImage(straightLines);
            HideImage(halfLines);
            //bendLine.gameObject.SetActive(true);
            //bendLine.sprite = bendLines[imageIndex];
            //bendLineTransform.localRotation = BlockImageRotation.rotation180;
            //straightLine.gameObject.SetActive(false);
            //halfLine.gameObject.SetActive(false);
        }
        else if ((outLine == LineDirection.Left && inLine == LineDirection.Up) || (outLine == LineDirection.Up && inLine == LineDirection.Left))
        {
            ShowImage(bendLines, imageIndex, BlockImageRotation.rotation90);
            HideImage(straightLines);
            HideImage(halfLines);
            //bendLine.gameObject.SetActive(true);
            //bendLine.sprite = bendLines[imageIndex];
            //bendLineTransform.localRotation = BlockImageRotation.rotation90;
            //straightLine.gameObject.SetActive(false);
            //halfLine.gameObject.SetActive(false);
        }
        else if(inLine == LineDirection.None && outLine != LineDirection.None)
        {
            Quaternion q = GetTerminalRotation(outLine);
            ShowImage(terminals, this.imageIndex, q);
            ShowImage(terminalLines, this.imageIndex, q);
            //terminalTransform.localRotation = GetTerminalRotation(outLine);
            //terminalLineTransform.localRotation = terminalTransform.localRotation;
            //terminalLine.gameObject.SetActive(true);
        }
        else if(outLine == LineDirection.None && inLine != LineDirection.None)
        {
            Quaternion q = GetTerminalRotation(inLine);
            ShowImage(terminals, this.imageIndex, q);
            ShowImage(terminalLines, this.imageIndex, q);
            //terminalTransform.localRotation = GetTerminalRotation(inLine);
            //terminalLineTransform.localRotation = terminalTransform.localRotation;
            //terminalLine.gameObject.SetActive(true);
        }
    }

    private Quaternion GetTerminalRotation(LineDirection inLine)
    {
        switch (inLine)
        {
            case LineDirection.Down:
                return BlockImageRotation.rotation0;
            case LineDirection.Up:
                return BlockImageRotation.rotation180;
            case LineDirection.Left:
                return BlockImageRotation.rotation270;
            case LineDirection.Right:
                return BlockImageRotation.rotation90;
        }
        return BlockImageRotation.rotation0;
    }

    private Quaternion GetHalfLineRotation(LineDirection inLine)
    {
        switch (inLine)
        {
            case LineDirection.Down:
                return BlockImageRotation.rotation0;
            case LineDirection.Up:
                return BlockImageRotation.rotation180;
            case LineDirection.Left:
                return BlockImageRotation.rotation270;
            case LineDirection.Right:
                return BlockImageRotation.rotation90; 
        }
        return BlockImageRotation.rotation0;
    }
}
