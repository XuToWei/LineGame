using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class UIAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string aniName;

        public void Play()
        {
            animator.CrossFadeInFixedTime(aniName, 0, 0, 0);
        }
    }
}
