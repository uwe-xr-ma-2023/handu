using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity.HandsModule;
using Leap.Unity;

public class ElementsSceneManager : MonoBehaviour
{
    public GameObject gestureGuidePrefab;
    public GameObject gameObjectContainer;
    public GameObject handAnimationPrefab;
    public bool seatedMode;
    private float seatedRange = 0.01f;
    private float standingRange = 0.5f;
    private float timeToHideHands = 5f;
    private MainSceneManager mainSceneManager;
    private float gestureGuideStartPosRange;
    private int gestureGuideChildCount;
    private int gestureGuideChildCollidedCount;
    private Leap.Unity.HandModelBase rightHandModel;
    private bool handAnimationTrackHands = false;
    private GameObject currentHandAnimationGameObjects;

    void Start()
    {
        mainSceneManager = GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>();
        gestureGuideStartPosRange = seatedMode ? seatedRange : standingRange;
        CreateGestureGuide();
    }

    private void Update()
    {
        // Track hand animation against user hands
        if (handAnimationTrackHands && currentHandAnimationGameObjects)
        {
            (Quaternion, Vector3) handPositionRotation = GetHandPositionRotation();
            currentHandAnimationGameObjects.transform.rotation = handPositionRotation.Item1;
            currentHandAnimationGameObjects.transform.position = handPositionRotation.Item2;
        }
    }


    private (Quaternion, Vector3) GetHandPositionRotation()
    {
        var rightHand = mainSceneManager.ultraleapService.GetHand(Chirality.Right);
        var rightHandIndexFinger = rightHand.Fingers.Find((finger) => finger.Type == Leap.Finger.FingerType.TYPE_INDEX);
        return (rightHand.Rotation, rightHandIndexFinger.TipPosition);
    }

    private void CreateGestureGuide()
    {
        gestureGuideChildCollidedCount = 0;
        float randomX = Random.Range(-gestureGuideStartPosRange, gestureGuideStartPosRange);
        float randomY = Random.Range(-gestureGuideStartPosRange, gestureGuideStartPosRange);
        Vector3 randomOffset = new Vector3(randomX, randomY, 0);
        Vector3 randomPosition = gestureGuidePrefab.transform.position + randomOffset;
        // instatiated inside container game object so that it is deleted on scene change
        GameObject gestureGuide = Instantiate(gestureGuidePrefab, randomPosition, gestureGuidePrefab.transform.rotation, gameObjectContainer.transform);
        gestureGuideChildCount = gestureGuide.transform.childCount;
    }

    public void IncreaseGestureGuideChildCollidedCount()
    {
        gestureGuideChildCollidedCount++;
        if (gestureGuideChildCollidedCount == gestureGuideChildCount)
        {
            CreateGestureGuide();
            CreateHandAnimation();
        }
    }

    private void CreateHandAnimation()
    {
        (Quaternion, Vector3) handPositionRotation = GetHandPositionRotation();
        var rotation = handPositionRotation.Item1;
        var position = handPositionRotation.Item2;
        // instatiated inside container game object so that it is deleted on scene change
        currentHandAnimationGameObjects = Instantiate(handAnimationPrefab, position, rotation, gameObjectContainer.transform);
        handAnimationTrackHands = true;
        StartCoroutine(WaitForHandPrefabHide());
    }

    IEnumerator WaitForHandPrefabHide()
    {
        yield return new WaitForSeconds(timeToHideHands);
        handAnimationTrackHands = false;
    }
}
