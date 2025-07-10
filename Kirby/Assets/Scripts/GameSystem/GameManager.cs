using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver,
    Loading
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [Header("Game Settings")]
    public GameState currentGameState = GameState.Menu;
    public bool isGamePaused = false;

    [Header("Game Stats")]
    public int currentScore = 0;
    public int currentLevel = 1;
    public float gameTime = 0f;

    private GameState previousGameState;

    protected override void Awake()
    {
        base.Awake();
        InitializeGame();
    }

    private void Start()
    {
        // �ٸ� �Ŵ������� �ʱ�ȭ�� �� ����
        StartCoroutine(InitializeManagers());
    }

    private void Update()
    {
        if (currentGameState == GameState.Playing && !isGamePaused)
        {
            gameTime += Time.deltaTime;
        }

        HandleInput();
    }

    private void InitializeGame()
    {
        // ���� ���� �� �⺻ ����
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Debug.Log("GameManager �ʱ�ȭ �Ϸ�");
    }

    private IEnumerator InitializeManagers()
    {
        yield return new WaitForEndOfFrame();

        // �Ŵ����� �ʱ�ȭ ���� �߿�
        if (SceneManager.Instance != null)
            Debug.Log("SceneManager ���� Ȯ��");
        //if (SoundManager.Instance != null)
        //    Debug.Log("SoundManager ���� Ȯ��");
        //if (UIManager.Instance != null)
        //    Debug.Log("UIManager ���� Ȯ��");
        //if (SaveManager.Instance != null)
        //    Debug.Log("SaveManager ���� Ȯ��");
    }

    private void HandleInput()
    {
        // ESC Ű�� ���� �Ͻ�����/�簳
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentGameState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentGameState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    #region Game State Management

    public void ChangeGameState(GameState newState)
    {
        if (currentGameState == newState) return;

        previousGameState = currentGameState;
        currentGameState = newState;

        OnGameStateChanged(newState);

        Debug.Log($"���� ���� ����: {previousGameState} -> {currentGameState}");
    }

    private void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Menu:
                Time.timeScale = 1f;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
            case GameState.Loading:
                Time.timeScale = 1f;
                break;
        }
    }

    public void StartGame()
    {
        ResetGameStats();
        ChangeGameState(GameState.Playing);
        GameEvents.GameResumed();
    }

    public void PauseGame()
    {
        if (currentGameState != GameState.Playing) return;

        isGamePaused = true;
        ChangeGameState(GameState.Paused);
        GameEvents.GamePaused();

        // �Ͻ����� UI ǥ��
        //UIManager.Instance?.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        if (currentGameState != GameState.Paused) return;

        isGamePaused = false;
        ChangeGameState(GameState.Playing);
        GameEvents.GameResumed();

        // �Ͻ����� UI �����
        //UIManager.Instance?.HidePauseMenu();
    }

    public void GameOver()
    {
        ChangeGameState(GameState.GameOver);

        // ���� ���� ó��
        //SaveManager.Instance?.SaveHighScore(currentScore);
        //UIManager.Instance?.ShowGameOverUI();
    }

    public void RestartGame()
    {
        ResetGameStats();
        ChangeGameState(GameState.Playing);
    }

    public void GoToMainMenu()
    {
        ChangeGameState(GameState.Menu);
        //SceneManager.Instance?.LoadScene("MainMenu");
    }

    #endregion

    #region Score & Stats Management

    public void AddScore(int points)
    {
        currentScore += points;
        GameEvents.ScoreChanged(currentScore);

        Debug.Log($"���� �߰�: +{points}, �� ����: {currentScore}");
    }

    public void SetScore(int score)
    {
        currentScore = score;
        GameEvents.ScoreChanged(currentScore);
    }

    public void NextLevel()
    {
        currentLevel++;
        Debug.Log($"������! ���� ����: {currentLevel}");
    }

    private void ResetGameStats()
    {
        currentScore = 0;
        currentLevel = 1;
        gameTime = 0f;
        GameEvents.ScoreChanged(currentScore);
    }

    #endregion

    #region Utility Methods

    public bool IsGamePlaying()
    {
        return currentGameState == GameState.Playing && !isGamePaused;
    }

    public void QuitGame()
    {
        Debug.Log("���� ����");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && currentGameState == GameState.Playing)
        {
            PauseGame();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && currentGameState == GameState.Playing)
        {
            PauseGame();
        }
    }
}