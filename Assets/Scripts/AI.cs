using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    enum CheckType
	{
        Enemy,
        AI
	}

    private int fieldSize;
	private void Start()
	{
        fieldSize = GameManager.Instance.fieldSize;
    }
  
    private Cell[,] CloneGameField()
    {
         fieldSize = GameManager.Instance.fieldSize;
        Cell[,] originalArray = GameManager.Instance.gameField;

        Cell[,] newArray = new Cell[fieldSize, fieldSize];

        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                newArray[i, j] = new Cell();
                newArray[i, j].Mark = originalArray[i, j].Mark;
                newArray[i, j].IsMarked = originalArray[i, j].IsMarked;
            }
        }
        return newArray;
    }
    private Tuple<int,int> CheckForOptimalTurn(CheckType type)
    {
        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                Cell[,] gamefield = CloneGameField();

                if (!gamefield[i, j].IsMarked)
                {
                    if (type == CheckType.Enemy)
                    {
                        if (GameManager.Instance.currentSide == Side.O)
                        {
                            gamefield[i, j].Mark = Mark.X;
                        }
                        if (GameManager.Instance.currentSide == Side.X)
                        {
                            gamefield[i, j].Mark = Mark.O;
                        }
                    }
                    if (type == CheckType.AI)
                    {
                        if (GameManager.Instance.currentSide == Side.O)
                        {
                            gamefield[i, j].Mark = Mark.O;
                        }
                        if (GameManager.Instance.currentSide == Side.X)
                        {
                            gamefield[i, j].Mark = Mark.X;
                        }
                    }
                    if (GameManager.Instance.CheckStrikes(gamefield, fieldSize))
                    {
                        return new Tuple<int, int>(i, j);
                    }
                }
            }
        }
        return null;
    }
    public IEnumerator WaitAndTakeTurn()
	{
        yield return new WaitForSeconds(1);
        TakeTurn();
    }
    public void TakeTurn()
    {

        Tuple<int, int> coords = null;
        if (CheckForOptimalTurn(CheckType.Enemy) != null)
        {
            coords = CheckForOptimalTurn(CheckType.Enemy);
        }
        else
        if (CheckForOptimalTurn(CheckType.AI) != null)
        {
            coords = CheckForOptimalTurn(CheckType.AI);
        }
        else
        {
            bool isEmptyCellFound = false;
            while (!isEmptyCellFound)
            {
                coords = new Tuple<int, int>(Random.Range(0, fieldSize), Random.Range(0, fieldSize));
                if (!GameManager.Instance.gameField[coords.Item1, coords.Item2].IsMarked)
				{
                    isEmptyCellFound = true;
                }
                if(GameManager.Instance.CheckIfFieldIsFull())
				{
                    coords = null;
                    break;
				}
            }
        }
        GameManager.Instance?.gameField[coords.Item1, coords.Item2].SetMarkByAI(this);
    }

}
