using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    //씬 전환 요청 이벤트 : 전달 인자는 씬 이름 
    public event Action<string> OnSceneChangeRequest;

    private void Awake()
    {
        if ( Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    //외부에서 씬 전환 요청을 할 때 호출하는 메서드 
    public void RequestSceneChange(string sceneName)
    {
        if(OnSceneChangeRequest != null)
        {
            OnSceneChangeRequest(sceneName);
        }
    }

    public void SavePosition(Vector3 position)
    {
        if(GameManager.Instance.isSaved == true)
        {

        }
    }
}
