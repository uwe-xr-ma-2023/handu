using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementsSceneManager : MonoBehaviour
{
    public GameObject gestureGuidePrefab;
    public GameObject gameObjectContainer;
    public GameObject handAnimationPrefab;
    public bool seatedMode;
    private float seatedRange = 0.01f;
    private float standingRange = 0.5f;
    private float timeToHideHands = 3f;
    private float gestureGuideStartPosRange;
    private int gestureGuideChildCount;
    private int gestureGuideChildCollidedCount;
    private Leap.Unity.HandModelBase rightHandModel;
    private Leap.Hand rightHand;
    private Leap.Finger rightHandIndexFinger;
    private bool handAnimationTrackHands = false;
    private GameObject currentHandAnimationGameObjects;

    void Start()
    {
        gestureGuideStartPosRange = seatedMode ? seatedRange : standingRange;
        rightHandModel = GameObject.Find("Generic Hand_Right").GetComponent<Leap.Unity.HandsModule.HandBinder>();
        rightHand = rightHandModel.GetLeapHand();
        rightHandIndexFinger = rightHand.Fingers.Find((finger) => finger.Type == Leap.Finger.FingerType.TYPE_INDEX);
        CreateGestureGuide();
    }

    private void Update()
    {
        // Track hand animation against user hands
        if (handAnimationTrackHands && currentHandAnimationGameObjects)
        {
            currentHandAnimationGameObjects.transform.position = rightHandIndexFinger.TipPosition;
            currentHandAnimationGameObjects.transform.rotation = rightHand.Rotation;
        }
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
        // instatiated inside container game object so that it is deleted on scene change
        currentHandAnimationGameObjects = Instantiate(handAnimationPrefab, rightHandIndexFinger.TipPosition, rightHand.Rotation, gameObjectContainer.transform);
        handAnimationTrackHands = true;
        StartCoroutine(WaitForHandPrefabHide());
    }

    IEnumerator WaitForHandPrefabHide()
    {
        yield return new WaitForSeconds(timeToHideHands);
        handAnimationTrackHands = false;
    }
}
