using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : SingletonMonoBehaviour<SceneManager>
{
    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "GameScene";
    public string loadingSceneName = "Loading";

    [Header("Loading Settings")]
    public float minimumLoadingTime = 2f;
    public bool useLoadingScreen = true;
    public bool useFadeEffect = true;

    [Header("Fade Settings")]
    public float fadeSpeed = 1f;
    public Color fadeColor = Color.black;

    private string currentSceneName;
    private string targetSceneName;
    private bool isLoading = false;
    private CanvasGroup fadeCanvasGroup;
    private GameObject fadeObject;

    // �� �ε� ����� ����
    public float LoadingProgress { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        // ���� �� �̸� ����
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // ���̵� UI ����
        if (useFadeEffect)
        {
            CreateFadeUI();
        }

        Debug.Log("SceneManager �ʱ�ȭ �Ϸ�");
    }

    private void Start()
    {
        // �� �ε� �̺�Ʈ ����
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    #region Fade UI ����

    private void CreateFadeUI()
    {
        // ���̵�� Canvas ����
        fadeObject = new GameObject("FadeCanvas");
        fadeObject.transform.SetParent(transform);

        Canvas canvas = fadeObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // ���� ���� ������

        // CanvasGroup �߰�
        fadeCanvasGroup = fadeObject.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;

        // ���̵� �̹��� ����
        GameObject fadeImage = new GameObject("FadeImage");
        fadeImage.transform.SetParent(fadeObject.transform);

        UnityEngine.UI.Image image = fadeImage.AddComponent<UnityEngine.UI.Image>();
        image.color = fadeColor;

        // ��ü ȭ�� ũ��� ����
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        DontDestroyOnLoad(fadeObject);
    }

    #endregion

    #region Scene Loading Methods

    /// <summary>
    /// �⺻ �� �ε� (�ε� ȭ�� ����)
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (isLoading)
        {
            Debug.LogWarning("�̹� �� �ε� ���Դϴ�.");
            return;
        }

        targetSceneName = sceneName;

        if (useFadeEffect)
        {
            StartCoroutine(LoadSceneWithFade(sceneName));
        }
        else
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    /// <summary>
    /// �ε� ȭ���� ����� �� �ε�
    /// </summary>
    public void LoadSceneWithLoadingScreen(string sceneName)
    {
        if (isLoading)
        {
            Debug.LogWarning("�̹� �� �ε� ���Դϴ�.");
            return;
        }

        targetSceneName = sceneName;
        StartCoroutine(LoadSceneWithLoading(sceneName));
    }

    /// <summary>
    /// ���̵� ȿ���� �Բ� �� �ε�
    /// </summary>
    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        isLoading = true;
        GameManager.Instance?.ChangeGameState(GameState.Loading);

        // ���̵� ��
        yield return StartCoroutine(FadeIn());

        // �� �ε�
        yield return StartCoroutine(LoadSceneAsync(sceneName));

        // ���̵� �ƿ�
        yield return StartCoroutine(FadeOut());

        isLoading = false;
    }

    /// <summary>
    /// �ε� ȭ���� ����� �� �ε�
    /// </summary>
    private IEnumerator LoadSceneWithLoading(string sceneName)
    {
        isLoading = true;
        GameManager.Instance?.ChangeGameState(GameState.Loading);

        // �ε� ȭ�� �ε�
        if (useLoadingScreen && !string.IsNullOrEmpty(loadingSceneName))
        {
            yield return StartCoroutine(LoadSceneAsync(loadingSceneName));

            // �ּ� �ε� �ð� ���
            float startTime = Time.time;

            // ���� �� �ε�
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                LoadingProgress = asyncLoad.progress;

                // �ε��� 90% �Ϸ�Ǹ� ���
                if (asyncLoad.progress >= 0.9f)
                {
                    LoadingProgress = 1f;

                    // �ּ� �ε� �ð� üũ
                    if (Time.time - startTime >= minimumLoadingTime)
                    {
                        asyncLoad.allowSceneActivation = true;
                    }
                }

                yield return null;
            }
        }
        else
        {
            // �ε� ȭ�� ���� ���� �ε�
            yield return StartCoroutine(LoadSceneAsync(sceneName));
        }

        isLoading = false;
    }

    /// <summary>
    /// �񵿱� �� �ε�
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            LoadingProgress = asyncLoad.progress;
            yield return null;
        }

        LoadingProgress = 1f;
    }

    #endregion

    #region Fade Effects

    private IEnumerator FadeIn()
    {
        if (fadeCanvasGroup == null) yield break;

        fadeCanvasGroup.blocksRaycasts = true;

        while (fadeCanvasGroup.alpha < 1f)
        {
            fadeCanvasGroup.alpha += fadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        if (fadeCanvasGroup == null) yield break;

        while (fadeCanvasGroup.alpha > 0f)
        {
            fadeCanvasGroup.alpha -= fadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    #endregion

    #region Quick Load Methods

    public void LoadMainMenu()
    {
        LoadScene(mainMenuSceneName);
    }

    public void LoadGameScene()
    {
        LoadScene(gameSceneName);
    }

    public void ReloadCurrentScene()
    {
        LoadScene(currentSceneName);
    }

    public void LoadNextScene()
    {
        int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int nextIndex = (currentIndex + 1) % UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

        UnityEngine.SceneManagement.SceneManager.LoadScene(nextIndex);
    }

    public void LoadPreviousScene()
    {
        int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int prevIndex = currentIndex - 1;

        if (prevIndex < 0)
            prevIndex = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1;

        UnityEngine.SceneManagement.SceneManager.LoadScene(prevIndex);
    }

    #endregion

    #region Scene Events

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        LoadingProgress = 0f;

        // �̺�Ʈ �˸�
        GameEvents.SceneChanged(currentSceneName);

        Debug.Log($"�� �ε� �Ϸ�: {scene.name}");

        // ���� ���� ����
        if (currentSceneName == mainMenuSceneName)
        {
            GameManager.Instance?.ChangeGameState(GameState.Menu);
        }
        else if (currentSceneName == gameSceneName)
        {
            GameManager.Instance?.ChangeGameState(GameState.Playing);
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"�� ��ε� �Ϸ�: {scene.name}");
    }

    #endregion

    #region Utility Methods

    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }

    public bool IsLoading()
    {
        return isLoading;
    }

    public bool IsSceneLoaded(string sceneName)
    {
        return currentSceneName == sceneName;
    }

    public void SetFadeColor(Color color)
    {
        fadeColor = color;

        if (fadeObject != null)
        {
            UnityEngine.UI.Image fadeImage = fadeObject.GetComponentInChildren<UnityEngine.UI.Image>();
            if (fadeImage != null)
            {
                fadeImage.color = color;
            }
        }
    }

    #endregion
}