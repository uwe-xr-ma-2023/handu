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
    [Tooltip("How far the hand should be away from sphere to stop it following hand")]
    public float handDistanceFromSphereStopTracking = 0.5f;
    private GameObject currentOrbPrefab;
    private bool leftHandPoseDetected = false;
    private bool rightHandPoseDetected = false;
    private MainSceneManager mainSceneManager;
    private bool trackOrbToHands = false;
    private MeshRenderer currentOrbMesh;


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
        Vector3? centerPoint = GetCenterPointBetweenHands();
        if (centerPoint != null)
        {

            currentOrbPrefab.transform.position = (Vector3)centerPoint;
            CheckIfHandsRemovedFromOrb((Vector3)centerPoint);
        }
        
    }

    /* When user moves their hands away from orb, stop tracking it with hands and leave in position */
    private void CheckIfHandsRemovedFromOrb(Vector3 centerPoint)
    {
        if (currentOrbMesh == null || centerPoint == null)
        {
            return;
        }
        Hand leftHand = mainSceneManager.ultraleapService.GetHand(Chirality.Left);
        Hand rightHand = mainSceneManager.ultraleapService.GetHand(Chirality.Right);
        float leftHandDistanceFromOrbEdge = 0;
        float rightHandDistanceFromOrbEdge = 0;

        if (leftHand != null && leftHand.PalmPosition != null && centerPoint != null && currentOrbMesh != null)
        {
            leftHandDistanceFromOrbEdge = GetHandDistanceFromOrbEdge(leftHand.PalmPosition, (Vector3)centerPoint, currentOrbMesh);
            if (leftHandDistanceFromOrbEdge >= handDistanceFromSphereStopTracking)
            {
                OnPoseLost("left");
            }
        }
        if (rightHand != null && rightHand.PalmPosition != null && centerPoint != null && currentOrbMesh != null)
        {

            rightHandDistanceFromOrbEdge = GetHandDistanceFromOrbEdge(rightHand.PalmPosition, (Vector3)centerPoint, currentOrbMesh);
            if (rightHandDistanceFromOrbEdge >= handDistanceFromSphereStopTracking)
            {
                OnPoseLost("right");
            }
        }
    }

    private float GetHandDistanceFromOrbEdge(Vector3 handPosition, Vector3 centre, MeshRenderer mesh)
    {
        float distanceBetweenHandsAndOrbCentre = Vector3.Distance(handPosition, currentOrbPrefab.transform.position);
        float orbRadius = currentOrbMesh.bounds.extents.x;
        return distanceBetweenHandsAndOrbCentre - orbRadius;
    }


    private Vector3? GetCenterPointBetweenHands()
    {
        Hand leftHand = mainSceneManager.ultraleapService.GetHand(Chirality.Left);
        Hand rightHand = mainSceneManager.ultraleapService.GetHand(Chirality.Right);
        if (leftHand == null || rightHand == null)
        {
            return null;
        }
        Vector3 handCenterPoint = (rightHand.PalmPosition + leftHand.PalmPosition) / 2;
        return handCenterPoint;
    }

    public void OnPoseDetect(string hand)
    {
        if (hand == "left")
        {
            leftHandPoseDetected = true;
        }
        else if (hand == "right")
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
        currentOrbMesh = currentOrbPrefab.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
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
            currentOrbMesh = null;
            StopCoroutine(DebouncePoseDetect());
        }
    }

}
