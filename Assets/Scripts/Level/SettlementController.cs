using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettlementController : MonoBehaviour {
    [SerializeField] private Button next;
    [SerializeField] private GameObject panel;
    // Use this for initialization
    void Awake()
    {
        next.onClick.AddListener(OnClickNextLevel);
        Messenger.AddListener(MessageDefines.OnFinishLevel, ShowSettlementView);
        Debug.Log("SettlementController!!!");
    }

    private void OnDestroy()
    {
        next.onClick.RemoveListener(OnClickNextLevel);
        Messenger.RemoveListener(MessageDefines.OnFinishLevel, ShowSettlementView);
    }

    void ShowSettlementView()
    {
        panel.SetActive(true);
    }

    void OnClickNextLevel()
    {
        Messenger.Broadcast(MessageDefines.OnClickNextLevelButton);
        panel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
