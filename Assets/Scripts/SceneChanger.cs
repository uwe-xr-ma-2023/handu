using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Leap.Unity.Interaction;

public class SceneChanger : MonoBehaviour
{
    private int currentSceneIndex = -1;
    public string[] scenes;
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
        if (currentSceneIndex != -1)
        {
            SceneManager.UnloadSceneAsync(scenes[currentSceneIndex]);
        }
        SceneManager.LoadScene(scenes[newSceneIndex], LoadSceneMode.Additive);
        currentSceneIndex = newSceneIndex;
        StartCoroutine(DebounceSceneChange());
    }

    private IEnumerator DebounceSceneChange()
    {
        yield return new WaitForSeconds(0.3f);
        changingScenes = false;
    }
}
