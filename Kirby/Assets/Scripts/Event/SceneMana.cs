using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMana : MonoBehaviour
{

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SaveButton()
    {
        if (GameManager.Instance.isSaved == true)
        {
            GameManager.Instance.isClick = true;
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
            GameManager.Instance.isClick = false;
        }
    }

    public void NewGameButton()
    {
        SceneManager.LoadScene("SampleScene");
        GameManager.Instance.isClick = false;
    }
    
    //    private void OnEnable()
    //    {
    //        if(EventManager.Instance != null)
    //        {
    //            EventManager.Instance.OnSceneChangeRequest += LoadScene;
    //        }
    //    }

    //    private void OnDisable()
    //    {
    //        if(EventManager.Instance != null)
    //        {
    //            EventManager.Instance.OnSceneChangeRequest -= LoadScene;
    //        }
    //    }

    //    private void LoadScene(string sceneName)
    //    {
    //        StartCoroutine(LoadSceneAsync(sceneName));
    //    }

    //    IEnumerator LoadSceneAsync(string sceneName)
    //    {
    //        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

    //        while ( !operation.isDone)
    //        {
    //            yield return null;
    //        }
    //    }
}
