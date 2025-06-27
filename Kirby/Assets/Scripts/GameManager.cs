using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private PlayerController playerController;
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
    void Start()
    {
        
    }

    void Update()
    {

    }
}
