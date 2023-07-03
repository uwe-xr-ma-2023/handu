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
    public enum ElementNames { earth, water, wind, fire };
    public ElementNames elementName;
    public Transform[] gestureGuidePositions;
    private int currentGestureGuidePositionIndex = 0;
    private float timeToHideHands = 5f;
    private MainSceneManager mainSceneManager;
    private int gestureGuideChildCount;
    private int gestureGuideChildCollidedCount;
    private bool handAnimationTrackHands = false;
    private GameObject currentHandAnimationGameObjects;
    private GameObject handAnimationContainer;
    private bool currentHandIsLeft = false;

    void Start()
    {
        mainSceneManager = GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>();
        handAnimationContainer = new GameObject($"HandAnimationContainer{elementName.ToString().Capitalize()}");
        CreateGestureGuide();
    }

    private void Update()
    {
        // Track hand animation against user hands
        if (handAnimationTrackHands && currentHandAnimationGameObjects)
        {
            var (rotation, position) = GetHandPositionRotation();
            if (rotation != null && position != null)
            {
                currentHandAnimationGameObjects.transform.rotation = (Quaternion)rotation;
                currentHandAnimationGameObjects.transform.position = (Vector3)position;
            }
            
        }
    }


    private (Quaternion?, Vector3?) GetHandPositionRotation()
    {
        var currentHand = currentHandIsLeft ? Chirality.Left : Chirality.Right;
        var hand = mainSceneManager.ultraleapService.GetHand(currentHand);
        if (hand == null)
        {
            return (null, null);
        }
        var indexFinger = hand.Fingers.Find((finger) => finger.Type == Leap.Finger.FingerType.TYPE_INDEX);
        return (hand.Rotation, indexFinger.TipPosition);
    }

    private void CreateGestureGuide()
    {
        gestureGuideChildCollidedCount = 0;
        var gestureGuidePosition = gestureGuidePositions[currentGestureGuidePositionIndex];
        // instatiated inside container game object so that it is deleted on scene change
        GameObject gestureGuide = Instantiate(gestureGuidePrefab, gestureGuidePosition.transform.position, gestureGuidePosition.transform.rotation, gameObjectContainer.transform);
        gestureGuideChildCount = gestureGuide.transform.childCount;
    }

    public void IncreaseGestureGuideChildCollidedCount(bool isLeftHand)
    {
        gestureGuideChildCollidedCount++;
        bool gestureCompleted = gestureGuideChildCollidedCount == gestureGuideChildCount;
        if (gestureCompleted)
        {
            bool isLastGesturePosition = currentGestureGuidePositionIndex == gestureGuidePositions.Length - 1;
            currentGestureGuidePositionIndex = isLastGesturePosition ? 0 : currentGestureGuidePositionIndex  + 1;
            currentHandIsLeft = isLeftHand;
            CreateGestureGuide();
            CreateHandAnimation();
        }
    }

    private void CreateHandAnimation()
    {
        var (rotation, position) = GetHandPositionRotation();
        if (rotation == null || position == null)
        {
            return;
        }
        if (currentHandAnimationGameObjects != null)
        {
            Destroy(currentHandAnimationGameObjects);
        }
        // instatiated inside hand animation container so that it remains on scene change
        currentHandAnimationGameObjects = Instantiate(handAnimationPrefab, (Vector3)position, (Quaternion)rotation, handAnimationContainer.transform);
        handAnimationTrackHands = true;
        StartCoroutine(WaitForHandPrefabHide());
    }

    IEnumerator WaitForHandPrefabHide()
    {
        yield return new WaitForSeconds(timeToHideHands);
        handAnimationTrackHands = false;
    }
}
