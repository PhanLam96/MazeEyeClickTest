using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void OnDestroy()
    {
        Instance = null;
    }

    public bool isGameReady;

    public MazeManager mazeManager;
    public UIManager uiManager;
    public MainCharacterController characterController;
    public MazeLeaderBoard mazeLeaderBoard;
    public SoundManager soundManager;

    public void StartGame()
    {
        characterController.OnStartGame();
        uiManager.OnStartGame();
        isGameReady = true;
    }
    public void EndGame()
    {
        characterController.OnEndGame();
        uiManager.OnEndGame();
        isGameReady = false;
        GameManager.Instance.soundManager.StopSound();
        GameManager.Instance.soundManager.PlaySound(1);
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
