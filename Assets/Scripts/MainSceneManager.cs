using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Leap.Unity;
using Udar.SceneManager;
using System.Linq;
using TMPro;

public class MainSceneManager : MonoBehaviour
{
    private int currentSceneIndex = -1;
    public GameObject _15FingeredHands;
    public GameObject handsScene7;
    public AudioSource mainSound;
    [Tooltip("Ultraleap tracked hand prefabs. Hidden before scene 3")]
    public GameObject mainHands;
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
    private Coroutine currentSceneTimer;
   

    [System.Serializable]
    public class PicoHeadsetCameraConfig
    {
        public string cameraMountName;
        public float deviceTiltXAxis;
        public float deviceOffsetYAxis;
        public float deviceOffsetZAxis;
        public string deviceId;
    }
    [SerializeField]
    [Tooltip("Headsets have different ultraleap camera mounts with different adjustments. So we have to set these adjustments at runtime")]
    public PicoHeadsetCameraConfig pico3dPrintCameraConfig = new PicoHeadsetCameraConfig();
    [SerializeField]
    [Tooltip("Headsets have different ultraleap camera mounts with different adjustments. So we have to set these adjustments at runtime")]
    private PicoHeadsetCameraConfig hammerheadCameraConfig = new PicoHeadsetCameraConfig();
    private IDictionary<string, PicoHeadsetCameraConfig> ourPicoHeadsets = new Dictionary<string, PicoHeadsetCameraConfig>();
    [Tooltip("Headsets have different ultraleap camera mounts with different adjustments. In play mode we don't have access to the headset device ID, so have to set manually here")]
    public bool useHammerheadCameraConfigInPlayMode;

    private void Start()
    {
        devices = new List<UnityEngine.XR.InputDevice>();

        sceneStartDeltas = handuScenes.Select(GetSecondsDeltaBetweenScenes).ToArray();
        StartSceneTimer(currentSceneIndex);
        // Add camera configs that differ per headset
        ourPicoHeadsets.Add(pico3dPrintCameraConfig.deviceId, pico3dPrintCameraConfig);
        ourPicoHeadsets.Add(hammerheadCameraConfig.deviceId, hammerheadCameraConfig);

        mainHands.SetActive(false);
    }

    /* Timer moves to next scene after set amount of time */
    private void StartSceneTimer(int sceneIndex)
    {
        if (currentSceneTimer != null)
        {
            StopCoroutine(currentSceneTimer);
        }
        if (sceneIndex == handuScenes.Count - 1)
        {
            return;
        }
        currentSceneTimer = StartCoroutine(SceneTimer(sceneStartDeltas[sceneIndex + 1]));
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
        SetLeapCameraPosition();

    }

    private void SetLeapCameraPosition()
    {
        var deviceId = SystemInfo.deviceUniqueIdentifier;
        ourPicoHeadsets.TryGetValue(deviceId, out PicoHeadsetCameraConfig headset);

        // In play mode, default to option set in inspector
        if (headset == null)
        {
            headset = useHammerheadCameraConfigInPlayMode ? hammerheadCameraConfig : pico3dPrintCameraConfig;
        }


        if (headset != null)
        {
            ultraleapService.deviceOffsetYAxis = headset.deviceOffsetYAxis;
            ultraleapService.deviceOffsetZAxis = headset.deviceOffsetZAxis;
            ultraleapService.deviceTiltXAxis = headset.deviceTiltXAxis;
        }
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
        mainHands.SetActive(true);
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
        if (mainSound != null)
        {
            mainSound.time = GetSecondsFromTime(scene.mainAudioPosition);
        }
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
