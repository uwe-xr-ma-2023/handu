using Leap;
using Leap.Unity.HandsModule;
using Leap.Unity;
using UnityEngine;
using System.Collections.Generic;

public class LightSceneManager : MonoBehaviour
{
    public GameObject orbPrefab;
    private bool leftHandPoseDetected = false;
    private bool rightHandPoseDetected = false;
    private SceneChanger sceneChanger;


    private void Start()
    {
        sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
    }



    private Vector3 GetCenterPointBetweenHands()
    {
        Hand leftHand = sceneChanger.ultraleapService.GetHand(Chirality.Left);
        Hand rightHand = sceneChanger.ultraleapService.GetHand(Chirality.Right);
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
            orbPrefab.SetActive(true);
            Vector3 centerPoint = GetCenterPointBetweenHands();
            orbPrefab.transform.position = centerPoint;
        }
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
            orbPrefab.SetActive(false);
        }
    }

}
