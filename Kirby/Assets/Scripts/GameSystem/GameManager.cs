using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public bool isClick;
    public bool isSaved;
    protected override void Awake()
    {
        base.Awake();
    }
}