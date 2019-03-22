using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public enum InputState
{
    None,
    OnBlockButtonDown,
    OnBlockButtonExit,
    OnBlockButtonEnter,
    OnInputUP,//全局的只要按钮放开就触发，处理一些按钮抬起操作
}

[Serializable]
public enum LineDirection
{
    None,
    Up,
    Down,
    Left,
    Right,
    Middle,
    RightDown,
    LeftDown,
    RightUp,
    LeftUp,
}

public static class BlockImageRotation
{
    public static Quaternion rotation0 = Quaternion.Euler(0, 0, 0);
    public static Quaternion rotation90 = Quaternion.Euler(0, 0, 90);
    public static Quaternion rotation180 = Quaternion.Euler(0, 0, 180);
    public static Quaternion rotation270 = Quaternion.Euler(0, 0, 270);
}
