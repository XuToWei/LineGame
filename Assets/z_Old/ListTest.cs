using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ListTest : MonoBehaviour {

	List<int> testList = new List<int>(){99,88,77,66,55,44,33,22,11};

    Delegate d;
    private void Awake()
    {
        Debug.Log("awake");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }
    // Use this for initialization
    void Start () {
        Debug.Log("Start");
        Debug.Log(testList.IndexOf(88));
        testList.RemoveRange(testList.IndexOf(88),3);
        foreach(int i in testList)
        {
            Debug.Log(i);
        }

        Action a = Print1;
        Action b = Print2;
        d = Delegate.Combine(a, b);
        Action[] acts = d.GetInvocationList().Cast<Action>().ToArray();
        foreach (var callback in acts)
            callback.Invoke();
        Debug.Log("=====");
        d = Delegate.RemoveAll(d, a);
        acts = d.GetInvocationList().Cast<Action>().ToArray();
        foreach (var callback in acts)
            callback.Invoke();

    }

    void Print1()
    {
        Debug.Log(1);
    }

    void Print2()
    {
        Debug.Log(2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
