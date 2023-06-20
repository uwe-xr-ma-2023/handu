using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager62Water : MonoBehaviour
{
    public AudioSource sceneAudio;
    public GameObject wavePrefab;
    public GameObject waveContainer;
    public GameObject handPrefab;
    public bool seatedMode;
    private float seatedRange = 0.01f;
    private float standingRange = 0.5f;
    private float wavePrefabRange;
    private GameObject waveGameObject;
    private float timeToShowWave = 45.0f;
    private float timeToHideWave = 73.0f;
    private float timeToHideHands = 3f;
    private int waveChildCount;
    private int waveChildCollidedCount;
    private Leap.Unity.HandModelBase rightHandModel;
    private Leap.Hand rightHand;
    private Leap.Finger rightHandIndexFinger;
    private bool handPrefabsTrackHands = false;
    private GameObject currentHandGameObjects;
    // Start is called before the first frame update
    void Start()
    {
        wavePrefabRange = seatedMode ? seatedRange : standingRange;
        rightHandModel = GameObject.Find("Generic Hand_Right").GetComponent<Leap.Unity.HandsModule.HandBinder>();
        rightHand = rightHandModel.GetLeapHand();
        rightHandIndexFinger = rightHand.Fingers.Find((finger) => finger.Type == Leap.Finger.FingerType.TYPE_INDEX);
        StartCoroutine(WaitForShowWave());
        StartWave();
        waveGameObject.SetActive(false);
        SkipToWaveShow();
    }

    private void Update()
    {
        if (handPrefabsTrackHands && currentHandGameObjects)
        {
            currentHandGameObjects.transform.position = rightHandIndexFinger.TipPosition;
            currentHandGameObjects.transform.rotation = rightHand.Rotation;
        }
    }

    IEnumerator WaitForShowWave()
    {
        yield return new WaitForSeconds(timeToShowWave);
        ShowWave();
    }
    IEnumerator WaitForHideWave()
    {
        yield return new WaitForSeconds(timeToHideWave - timeToShowWave);
        waveGameObject.SetActive(false);

    }

    private void ShowWave()
    {
        waveGameObject.SetActive(true);
        StartCoroutine(WaitForHideWave());
    }

    public void SkipToWaveShow()
    {
        if (sceneAudio != null)
        {
            sceneAudio.time = timeToShowWave;
        }
        StopAllCoroutines();
        ShowWave();
    }

    public void IncreaseWaveChildCollidedCount()
    {
        waveChildCollidedCount++;
        if (waveChildCollidedCount == waveChildCount)
        {
            StartWave();
            ShowHandPrefabs();
        }
    }

    private void StartWave()
    {
        waveChildCollidedCount = 0;
        float randomX = Random.Range(-wavePrefabRange, wavePrefabRange);
        float randomY = Random.Range(-wavePrefabRange, wavePrefabRange);
        Vector3 randomOffset = new Vector3(randomX, randomY, 0);
        Vector3 randomPosition = wavePrefab.transform.position + randomOffset;
        // wave is instatiated inside waveContainer so that it is deleted on scene change
        waveGameObject = Instantiate(wavePrefab, randomPosition, wavePrefab.transform.rotation, waveContainer.transform);
        waveChildCount = waveGameObject.transform.childCount;
    }

    private void ShowHandPrefabs()
    {
        // hands are instatiated inside waveContainer so that it is deleted on scene change
        currentHandGameObjects = Instantiate(handPrefab, rightHandIndexFinger.TipPosition, rightHand.Rotation, waveContainer.transform);
        handPrefabsTrackHands = true;
        StartCoroutine(WaitForHandPrefabHide());
    }

    IEnumerator WaitForHandPrefabHide()
    {
        yield return new WaitForSeconds(timeToHideHands);
        handPrefabsTrackHands = false;
    }
}
