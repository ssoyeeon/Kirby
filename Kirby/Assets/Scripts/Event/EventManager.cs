using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    //�� ��ȯ ��û �̺�Ʈ : ���� ���ڴ� �� �̸� 
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

    //�ܺο��� �� ��ȯ ��û�� �� �� ȣ���ϴ� �޼��� 
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
