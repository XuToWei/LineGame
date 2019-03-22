using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsDebug : MonoBehaviour
{
    [SerializeField] Text fps;
    private float currentTime = 0;
    private float lateTime = 0;

    private float framesNum = 0;
    private float fpsTime = 0;

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        framesNum++;

        if (currentTime - lateTime >= 0.2f)
        {
            fpsTime = framesNum / (currentTime - lateTime);

            lateTime = currentTime;

            framesNum = 0;

            fps.text = ((int)fpsTime).ToString();
        }
    }


}
