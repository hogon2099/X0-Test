using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MainMenu;
    public Text FieldSizeText;
    public Slider FieldSizeSlider;
    public Text GameModeText;
    public static MainMenuManager Instance;

    public GameMode gameMode;
    public int fieldSize;
    public MainMenuManager()
	{
        Instance = this;
	}

	private void Start()
	{
        gameMode = GameMode.PlayerVsAI;
        fieldSize = 3;
    }

	public delegate void GameStartHandler(GameMode gameMode, int fieldSize);
    public event GameStartHandler GameStarted;

    public void StartGame()
	{
        GameStarted?.Invoke(this.gameMode, this.fieldSize);
        MainMenu.SetActive(false);
	}
    public void SetFieldSizeText()
	{
        FieldSizeText.text = FieldSizeSlider.value + "x" + FieldSizeSlider.value;
    }
    public void SetGameModeToAIVsAI()
	{
        gameMode = GameMode.AIVsAI;
        GameModeText.text = "AI vs AI";
    }
    public void SetGameModeToPlayerVsAI()
    {
        gameMode = GameMode.PlayerVsAI;
        GameModeText.text = "Player vs AI";
    }
    public void SetGameModeToPlayerVsPlayer()
    {
        gameMode = GameMode.PlayerVsPlayer;
        GameModeText.text = "Player vs Player";
    }
    public void SwitchFieldSize()
	{
        fieldSize = (int) FieldSizeSlider.value;
        SetFieldSizeText();

    }
    public void Reload()
	{
        SceneManager.LoadScene(0);
	}
    public void QuitGame()
	{
        Application.Quit();
    }
}
