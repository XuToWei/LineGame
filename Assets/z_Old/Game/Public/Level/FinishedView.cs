using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public class FinishedView : MonoBehaviour
    {
        [SerializeField] GameObject finished;
        [SerializeField] Button nextLevelButton;
        // Use this for initialization
        void Awake()
        {
            nextLevelButton.onClick.AddListener(ClickNextLevelButton);
            Messenger<bool>.AddListener(MessageDefines.ShowOrHideFinishedWindow, ShowOrHiadFinishedWindow);
            finished.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void ClickNextLevelButton()
        {
            Messenger.Broadcast(MessageDefines.NextLevel);
            finished.SetActive(false);
        }

        void ShowOrHiadFinishedWindow(bool isShow)
        {
            finished.SetActive(isShow);
        }
    }
}
