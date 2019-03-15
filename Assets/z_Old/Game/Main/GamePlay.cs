using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class GamePlay : MonoBehaviour
    {
        public BlockPlayController play;
        
        // Use this for initialization
        void Start()
        {
            play.StartPlay(new MapData());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
