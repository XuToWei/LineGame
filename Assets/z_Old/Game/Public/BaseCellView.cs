using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree {
	public abstract class BaseCellView : MonoBehaviour {

        public Color backgroundColor = FlowFreeColors.None;
        public abstract void SetBackGround(Color color);
	}
}