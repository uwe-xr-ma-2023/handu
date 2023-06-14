using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Leap.Unity.Interaction;

public class SceneChanger : MonoBehaviour
{
    private string currentScene = "0 Home";

    public void ChangeScene(string sceneName)
    {
        if (currentScene != "0 Home") 
        {
            SceneManager.UnloadScene(currentScene);
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        currentScene = sceneName;
    }
}
