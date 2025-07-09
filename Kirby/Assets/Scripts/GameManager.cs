using System;
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

    public GameState CurrentState {  get; private set; }

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

        LoadVector();
        ChangeState(GameState.Menu);
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

    public void ChangeState(GameState state)
    {
        CurrentState = state;

        switch(state)
        {
            case GameState.Menu:
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 1;
                break;
            case GameState.Playing :
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
                break;
            case GameState.Paused :
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                break;
        }
    }

    public enum GameState
    {
        Menu,
        Playing,
        Paused
    }
}
