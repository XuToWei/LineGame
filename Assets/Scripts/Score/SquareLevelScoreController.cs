using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SquareLevelScoreData
{
    public int totalStep = 0;
    public int totalBlock = 0;
    public int moves = 0;
    public int best = 0;
    public int paint = 0;
}

public class SquareLevelScoreController : MonoBehaviour {
    [SerializeField] protected Text Flows;
    [SerializeField] protected Text Moves;
    [SerializeField] protected Text Best;
    [SerializeField] protected Text Paint;


    public void OnEnable()
    {
        Messenger<SquareLevelScoreData>.AddListener(MessageDefines.OnScoreChange, OnScoreChange);
    }

    public void OnDisable()
    {
        Messenger<SquareLevelScoreData>.RemoveListener(MessageDefines.OnScoreChange, OnScoreChange);
    }

    private void OnScoreChange(SquareLevelScoreData data)
    {
        Flows.text = data.moves + "/" + data.totalStep;
        Moves.text = data.moves.ToString();
        Best.text = data.best.ToString();
        Paint.text = (data.paint * 100 / data.totalBlock).ToString();
    }
}
