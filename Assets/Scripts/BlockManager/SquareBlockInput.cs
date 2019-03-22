using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquareBlockInput : MonoBehaviour {
    [SerializeField] Image inputImage;
    [SerializeField] RectTransform inputImageParent;
    bool isTouched = false;

    void Start() {

    }

    void Update()
    {
#if UNITY_IPHONE || UNITY_ANDROID
        if (Input.touches.Length < 1)
        {
            return;
        }
        else if ((Input.touches[0].phase == TouchPhase.Ended) && isTouched)
        {
            OnInputUp();
            isTouched = false;
        }
        if (isTouched)
        {
            inputImage.gameObject.SetActive(true);
            Vector2 pos = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(inputImageParent, Input.touches[0].position, Camera.main, out pos))
            {
                inputImage.rectTransform.localPosition = pos;
            }
        }
        else
        {
            inputImage.gameObject.SetActive(false);
        }
#else
        if (isTouched && Input.GetMouseButtonUp(0))
        {
            OnInputUp();
            isTouched = false;
        }
         if (isTouched)
        {
            inputImage.gameObject.SetActive(true);
            Vector2 pos = Vector2.zero;
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(inputImageParent, Input.mousePosition, Camera.main, out pos))
            {
                inputImage.rectTransform.localPosition = pos;
            }
        }
        else
        {
            inputImage.gameObject.SetActive(false);
        }
#endif  
    }

    private void OnEnable()
    {
        Messenger<int>.RemoveAllListener(MessageDefines.OnBlockButtonDown);
        Messenger<int>.RemoveAllListener(MessageDefines.OnBlockPointEnter);
        Messenger<int>.RemoveAllListener(MessageDefines.OnBlockPointExit);
        Messenger<int>.AddListener(MessageDefines.OnBlockButtonDown, OnBlockButtonDown);
        Messenger<int>.AddListener(MessageDefines.OnBlockPointEnter, OnBlockButtonEnter);
        Messenger<int>.AddListener(MessageDefines.OnBlockPointExit, OnBlockButtonExit);
    }

    private void OnDisable()
    {
        Messenger<int>.RemoveAllListener(MessageDefines.OnBlockButtonDown);
        Messenger<int>.RemoveAllListener(MessageDefines.OnBlockPointEnter);
        Messenger<int>.RemoveAllListener(MessageDefines.OnBlockPointExit);
    }

    void OnBlockButtonDown(int id)
    {
        isTouched = true;
        Messenger<InputState, int>.Broadcast(MessageDefines.OnPostSquareInput, InputState.OnBlockButtonDown, id);
    }

    void OnBlockButtonEnter(int id)
    {
        if (!isTouched)
            return;
        Messenger<InputState, int>.Broadcast(MessageDefines.OnPostSquareInput, InputState.OnBlockButtonEnter, id);
    }

    void OnBlockButtonExit(int id)
    {
        if (!isTouched)
            return;
        Messenger<InputState, int>.Broadcast(MessageDefines.OnPostSquareInput, InputState.OnBlockButtonExit, id);
    }

    void OnInputUp()
    {
        Messenger<InputState, int>.Broadcast(MessageDefines.OnPostSquareInput, InputState.OnInputUP, -1);
    }
}
