using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Vector3 savePoint;

    public bool isSaved = false;
    public bool isClick = false;

    void Awake()
    {
        // ½Ì±ÛÅæ ±¸Çö
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    public void SaveVector()
    {
        PlayerPrefs.SetFloat("PosX", savePoint.x);
        PlayerPrefs.SetFloat("PosY", savePoint.y);
        PlayerPrefs.SetFloat("PosZ", savePoint.z);
        PlayerPrefs.Save();
    }
    public void LoadVector()
    {
        savePoint.x = PlayerPrefs.GetFloat("PosX", 0);
        savePoint.y = PlayerPrefs.GetFloat("PosY", 0);
        savePoint.z = PlayerPrefs.GetFloat("PosZ", 0);
    }
}
