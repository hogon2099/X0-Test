using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public enum Opponent
	{
        AI,
        Player
	}
    public Mark Mark;
    public bool IsMarked = false;
    private static bool activateAIByNextTurn = false;
    public void SetMarkByPlayer()
    {
        SetMark();
        GameManager.Instance.EnemyForPlayer?.TakeTurn();
    }

    public void SetMarkByAI(AI computer)
    {
        SetMark();
        if (GameManager.Instance.AI1 && computer == GameManager.Instance.AI1)
        {
            StartCoroutine(GameManager.Instance.AI2.WaitAndTakeTurn());
        }
        else if (GameManager.Instance.AI2 && computer == GameManager.Instance.AI2)
        {
            StartCoroutine(GameManager.Instance.AI1.WaitAndTakeTurn());
        }
    }
    public void SetMark()
    {
        this.GetComponentInChildren<Button>().interactable = false;

        if (GameManager.Instance.currentSide == Side.O)
        {
            Mark = Mark.O;
            this.GetComponentInChildren<Text>().text = "O";
        }
        if (GameManager.Instance.currentSide == Side.X)
        {
            Mark = Mark.X;
            this.GetComponentInChildren<Text>().text = "X";
        }

        IsMarked = true;
        GameManager.Instance.ChangeSide();
        GameManager.Instance.CheckFieldForTheWin();
    }
}