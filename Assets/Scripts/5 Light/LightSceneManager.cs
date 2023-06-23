using Leap;
using Leap.Unity.HandsModule;
using Leap.Unity;
using UnityEngine;
using System.Collections;

public class LightSceneManager : MonoBehaviour
{
    public GameObject orbPrefab;
    public GameObject sceneContainer;
    [Tooltip("How long the pose must be held for before orb is spawned")]
    public float poseHoldDuration = 0f;
    private GameObject currentOrbPrefab;
    private bool leftHandPoseDetected = false;
    private bool rightHandPoseDetected = false;
    private MainSceneManager mainSceneManager;
    private bool trackOrbToHands = false;


    private void Start()
    {
        mainSceneManager = GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>();
    }

    private void Update()
    {
        if (!trackOrbToHands)
        {
            return;
        }
        Vector3 centerPoint = GetCenterPointBetweenHands();
        currentOrbPrefab.transform.position = centerPoint;
    }


    private Vector3 GetCenterPointBetweenHands()
    {
        Hand leftHand = mainSceneManager.ultraleapService.GetHand(Chirality.Left);
        Hand rightHand = mainSceneManager.ultraleapService.GetHand(Chirality.Right);
        Vector3 handCenterPoint = (rightHand.PalmPosition + leftHand.PalmPosition) / 2;
        return handCenterPoint;
    }

    public void OnPoseDetect(string hand)
    {
        if (hand == "left")
        {
            leftHandPoseDetected = true;
        } else if (hand == "right")
        {
            rightHandPoseDetected = true;
        }

        if (leftHandPoseDetected && rightHandPoseDetected)
        {
            StartCoroutine(DebouncePoseDetect());
        }
    }

    private IEnumerator DebouncePoseDetect()
    {
        yield return new WaitForSeconds(poseHoldDuration);
        currentOrbPrefab = Instantiate(orbPrefab, sceneContainer.transform);
        trackOrbToHands = true;
    }



    public void OnPoseLost(string hand)
    {
        if (hand == "left")
        {
            leftHandPoseDetected = false;
        }
        else if (hand == "right")
        {
            rightHandPoseDetected = false;
        }

        if (!leftHandPoseDetected || !rightHandPoseDetected)
        {
            trackOrbToHands = false;
            StopCoroutine(DebouncePoseDetect());
        }
    }

}
