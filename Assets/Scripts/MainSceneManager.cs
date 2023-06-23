using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Leap.Unity;

public class MainSceneManager : MonoBehaviour
{
    private int currentSceneIndex = -1;
    public string[] scenes;
    public GameObject handsScene4;
    public GameObject handsScene7;
    public LeapXRServiceProvider ultraleapService;
    private List<UnityEngine.XR.InputDevice> devices;
    bool changingScenes = false;

    private void Start()
    {
        devices = new List<UnityEngine.XR.InputDevice>();
    }


    private void Update()
    {
        if (devices.Count == 0)
        {
            InitialiseXrDevice();
            return;
        }
        devices.ForEach((device) =>
        {
            bool triggerValue;
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
            {
                ChangeScene(currentSceneIndex + 1);
            }
        });


    }

    private void InitialiseXrDevice()
    {
        UnityEngine.XR.InputDevices.GetDevices(devices);
    }

    public void ChangeScene(int newSceneIndex)
    {
        if (changingScenes || newSceneIndex >= scenes.Length)
        {
            return;
        }
        changingScenes = true;
        UnloadScene(currentSceneIndex);
        LoadScene(newSceneIndex);
    }

    private void LoadScene(int sceneIndex)
    {
        if (sceneIndex == 1 || sceneIndex == 2)
        {
            EnterScene4();
        }
        else if (sceneIndex == 8)
        {
            EnterScene7();
        }
        SceneManager.LoadScene(scenes[sceneIndex], LoadSceneMode.Additive);
        currentSceneIndex = sceneIndex;
        StartCoroutine(DebounceSceneChange());
    }

    private void UnloadScene(int sceneIndex)
    {
        if (sceneIndex == -1)
        {
            return;
        }
        if (sceneIndex == 1 || sceneIndex == 2)
        {
            LeaveScene4();
        }
        else if (sceneIndex == 8)
        {
            LeaveScene7();
        }
        SceneManager.UnloadSceneAsync(scenes[sceneIndex]);
    }

    private void EnterScene4()
    {
        handsScene4.SetActive(true);
    }

    private void LeaveScene4()
    {
        handsScene4.SetActive(false);
    }

    private void EnterScene7()
    {
        handsScene7.SetActive(true);
    }

    private void LeaveScene7()
    {
        handsScene7.SetActive(false);
    }

    private IEnumerator DebounceSceneChange()
    {
        yield return new WaitForSeconds(0.3f);
        changingScenes = false;
    }
}
