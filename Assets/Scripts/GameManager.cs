using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameMode
{
	AIVsAI,
	PlayerVsAI,
	PlayerVsPlayer
}
public enum Mark
{
    Empty,
    X,
    O
}
public enum Side
{
    X,
    O
}

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public AI EnemyForPlayer;
    [HideInInspector]
    public AI AI1;
    [HideInInspector]
    public AI AI2;
    public AI AIPrefab;
    public Text CurrentSideText;
    public Transform WinInfo;
    public GameObject CellPrefab;
    public GameObject GamefieldParentObject;
    public static GameManager Instance;
    public Side currentSide;
    public Cell[,] gameField;
    public int fieldSize;
    private GameMode gameMode;
    private Mark winner = Mark.Empty;
    public GameManager()
    {
        Instance = this;
    }
    private void Start()
    {
        MainMenuManager.Instance.GameStarted += InitializeGameManager;
    }
    private void InitializeGameManager(GameMode gameMode, int fieldSize)
    {
        this.gameMode = gameMode;
        this.fieldSize = fieldSize;
        CreateGameField();

        if (gameMode == GameMode.PlayerVsAI)
        {
            EnemyForPlayer = Instantiate(AIPrefab);
            int shot = Random.Range(0, 100);
            Debug.Log(shot);
            if (shot % 2 == 0)
            {
                EnemyForPlayer.TakeTurn();
            }
        }
        if (gameMode == GameMode.AIVsAI)
        {
            AI1 = Instantiate(AIPrefab);
            AI2 = Instantiate(AIPrefab);
            AI1.TakeTurn();
        }
    }

    private void CreateGameField()
    {
        gameField = new Cell[fieldSize, fieldSize];

        float elementSize = 3;
        Vector2 origin = new Vector2(-3.5f, -3.25f); // начало координат, нижняя левая точка
        float decrCoeff = (float)(elementSize / fieldSize);
        Vector3 neededScale = new Vector3((CellPrefab.transform.localScale.x) * (float)decrCoeff, (CellPrefab.transform.localScale.y) * (float)decrCoeff, 1);

        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                GameObject newCell =
                    Instantiate(
                         CellPrefab,
                         new Vector2(
                             origin.x + (float)(elementSize * i * decrCoeff),
                             origin.y + (float)(elementSize * j * decrCoeff)),
                         Quaternion.identity);
                newCell.transform.localScale = neededScale;
                newCell.transform.SetParent(GamefieldParentObject.transform);
                gameField[i, j] = newCell.GetComponent<Cell>();
            }
        }
    }

    public void ChangeSide()
    {
        if (currentSide == Side.O)
        {
            currentSide = Side.X;
            CurrentSideText.text = "Current turn by: " + "X";
        }
        else
        {
            if (currentSide == Side.X)
            {
                currentSide = Side.O;
                CurrentSideText.text = "Current turn by: " + "O";
            }
        }
    }
   
    public bool CheckIfFieldIsFull()
    {
        bool isFieldFull = true;

        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                if (!gameField[i, j].IsMarked) isFieldFull = false;
            }
        }
        return isFieldFull;
    }
    public  bool CheckStrikes(Cell[,] a_gameField, int a_fieldSize)
    {
        bool[] potentialStrikes = new bool[4]
        {
            true, //diagonal
            true, //diagonal
            true, //vertical
            true  //horizontal
        };

        Mark[] firstMarks = new Mark[4];
        firstMarks[0] = a_gameField[0, 0].Mark;
        firstMarks[1] = a_gameField[0, a_fieldSize - 1].Mark;


        for (int i = 0; i < a_fieldSize; i++)
        {
            potentialStrikes[2] = true;
            potentialStrikes[3] = true;
            firstMarks[2] = a_gameField[i, 0].Mark;
            firstMarks[3] = a_gameField[0, i].Mark;

            for (int j = 0; j < a_fieldSize; j++)
            {
                if ((i == j) && a_gameField[i, j].Mark != firstMarks[0])
                {
                    potentialStrikes[0] = false;
                }
                if ((j == (a_fieldSize - 1) - i) && a_gameField[i, j].Mark != firstMarks[1])
                {
                    potentialStrikes[1] = false;
                }
                if (a_gameField[i, j].Mark != firstMarks[2])
                {
                    potentialStrikes[2] = false;
                }
                if (a_gameField[j, i].Mark != firstMarks[3])
                {
                    potentialStrikes[3] = false;
                }
            }
            for (int d = 0; d < 4; d++)
            {
                if (firstMarks[d] == Mark.Empty)
                {
                    potentialStrikes[d] = false;
                }
            }

            if (potentialStrikes[2])
            {
                winner = firstMarks[2];
                return true;
            }
            else if (potentialStrikes[3])
            {
                winner = firstMarks[3];
                return true;
            }
        }

        if (potentialStrikes[0])
        {
            winner = firstMarks[0];
            return true;
        }
        else
        if (potentialStrikes[1])
        {
            winner = firstMarks[1];
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckFieldForTheWin()
    {
        if (CheckStrikes(gameField, fieldSize) || CheckIfFieldIsFull())
        {
            if (CheckIfFieldIsFull()) winner = Mark.Empty;
            StartCoroutine(EndGame());
        }
    }
    private IEnumerator EndGame()
    {
        WinInfo.gameObject.SetActive(true);
        WinInfo.GetComponentInChildren<Text>().text = winner.ToString() + " won";
        if (winner == Mark.Empty)
        {
            WinInfo.GetComponentInChildren<Text>().text = "draw";
        }
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
    }
}
