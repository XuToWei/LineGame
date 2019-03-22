using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBlockManager : MonoBehaviour {
    [SerializeField] protected BaseBlock block;
    protected Dictionary<int, BaseBlock> blockDictionary = new Dictionary<int, BaseBlock>();
    protected Dictionary<int, int> targetPair = new Dictionary<int, int>();
    protected Dictionary<int, int> targetImage = new Dictionary<int, int>();
    public abstract void InitBlock(SquareLevelModel levelModel);
    public abstract void Line(int index);

    public virtual BaseBlock GetBlock(int id)
    {
        if (!blockDictionary.ContainsKey(id))
            return null;
        return blockDictionary[id];
    }
}
