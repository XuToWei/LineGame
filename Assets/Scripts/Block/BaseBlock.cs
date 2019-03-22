using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BaseBlock : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    [SerializeField] public RectTransform rectTransform;
    //[SerializeField] protected Image terminal;
    [SerializeField] protected Image[] terminals;
    //[SerializeField] protected RectTransform terminalTransform;
    protected int imageIndex;
    public int id;
    protected Dictionary<LineDirection, int> aroundBlock = new Dictionary<LineDirection, int>();

    public abstract void OnPointerEnter(PointerEventData eventData);
    public abstract void OnPointerExit(PointerEventData eventData);
    public abstract void OnPointerDown(PointerEventData eventData);
    public abstract void ClearBlock();
    public abstract void SetBlock(LineDirection inLine, LineDirection outLine, int imgIndex);

    public virtual void Init(int index, Vector2 position, Vector3 scale, int imageIndex, Dictionary<LineDirection, int> aroundBlock)
    {
        id = index;
        this.aroundBlock = aroundBlock;
        rectTransform.anchoredPosition = position;
        rectTransform.localScale = scale;
        this.imageIndex = imageIndex;
        if (imageIndex >= 0 )
        {
            ShowImage(terminals, imageIndex, Quaternion.identity);
            //terminal.sprite = terminals[imgIndex];
            //terminal.gameObject.SetActive(true);
        }
        else
        {
            HideImage(terminals);
            //terminal.gameObject.SetActive(false);
        }
    }

    protected virtual void ShowImage(Image[] images, int imageIndex, Quaternion localRotation)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (i == imageIndex)
            {
                images[i].rectTransform.localRotation = localRotation;
                images[i].gameObject.SetActive(true);
            }
            else if(images[i].gameObject.activeSelf)
            {
                images[i].gameObject.SetActive(false);
            }
        }
    }

    protected virtual void HideImage(Image[] images)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].gameObject.activeSelf)
            {
                images[i].gameObject.SetActive(false);
            }
        }
    }

}
