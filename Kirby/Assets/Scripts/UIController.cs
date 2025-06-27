using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public void OnNextSceneButtonClicked()
    {
        EventManager.Instance.RequestSceneChange("SampleScene");
    }

    public void OnSaveSceneButtonClicked()
    {
        EventManager.Instance.RequestSceneChange("SampleScene");
    }
}
