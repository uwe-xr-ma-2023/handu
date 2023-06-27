using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Leap.Unity;
using Udar.SceneManager;
using System.Linq;

public class MainSceneManager : MonoBehaviour
{
    private int currentSceneIndex = -1;
    public GameObject _15FingeredHands;
    public GameObject handsScene7;
    public AudioSource mainSound;
    [System.Serializable]
    public struct MainAudioPosition
    {
        public int minutes;
        public int seconds;
    }
    [System.Serializable]
    public struct HanduScene
    {
        public SceneField scene;
        public MainAudioPosition mainAudioPosition;
        public bool show15FingeredHands;

    }
    [SerializeField]
    private List<HanduScene> handuScenes = new List<HanduScene>();


    public LeapXRServiceProvider ultraleapService;
    private List<UnityEngine.XR.InputDevice> devices;
    private bool changingScenes = false;
    private float[] sceneStartDeltas;

    private void Start()
    {
        devices = new List<UnityEngine.XR.InputDevice>();
        sceneStartDeltas = handuScenes.Select(GetSecondsDeltaBetweenScenes).ToArray();
        StartSceneTimer(currentSceneIndex);       
    }

    /* Timer moves to next scene after set amount of time */
    private void StartSceneTimer(int sceneIndex)
    {
        StopCoroutine("SceneTimer");
        if (sceneIndex == handuScenes.Count - 1)
        {
            return;
        }
        StartCoroutine(SceneTimer(sceneStartDeltas[sceneIndex + 1]));
    }

    private float GetSecondsDeltaBetweenScenes(HanduScene scene, int index)
    {
        var sceneStartSeconds = index == 0 ? 0 : GetSecondsFromTime(handuScenes[index - 1].mainAudioPosition);
        var sceneEndSeconds = GetSecondsFromTime(scene.mainAudioPosition);
        return sceneEndSeconds - sceneStartSeconds;
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

    private IEnumerator SceneTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (currentSceneIndex == handuScenes.Count - 1)
        {
            yield return null;
        }
        ChangeScene(currentSceneIndex + 1);
    }

    private void InitialiseXrDevice()
    {
        UnityEngine.XR.InputDevices.GetDevices(devices);
    }

    public void ChangeScene(int newSceneIndex)
    {
        if (changingScenes || newSceneIndex >= handuScenes.Count)
        {
            return;
        }
        changingScenes = true;
        UnloadScene(currentSceneIndex);
        LoadScene(newSceneIndex);
    }

    private float GetSecondsFromTime(MainAudioPosition mainAudioPosition)
    {
        return (mainAudioPosition.minutes * 60) + mainAudioPosition.seconds;
    }

    public void LoadSceneByPath(string scenePath)
    {
        int sceneIndex = handuScenes.FindIndex(s => s.scene.Path == scenePath);
        LoadScene(sceneIndex);
    }

    private void LoadScene(int sceneIndex)
    {
        var scene = handuScenes[sceneIndex];
        StartSceneTimer(sceneIndex);
        if (scene.show15FingeredHands)
        {
            Show15FingeredHands();
        }
        else if (sceneIndex == 8)
        {
            EnterScene7();
        }
        SceneManager.LoadScene(scene.scene.Path, LoadSceneMode.Additive);
        currentSceneIndex = sceneIndex;
        mainSound.time = GetSecondsFromTime(scene.mainAudioPosition);
        StartCoroutine(DebounceSceneChange());
    }

    private void UnloadScene(int sceneIndex)
    {
        if (sceneIndex == -1)
        {
            return;
        }
        else if (sceneIndex == 8)
        {
            LeaveScene7();
        }
        Hide15FingeredHands();
        SceneManager.UnloadSceneAsync(handuScenes[sceneIndex].scene.Path);
    }

    private void Show15FingeredHands()
    {
        _15FingeredHands.SetActive(true);
    }

    private void Hide15FingeredHands()
    {
        _15FingeredHands.SetActive(false);
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
