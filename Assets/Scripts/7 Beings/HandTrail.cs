using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTrail : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    public GameObject handObject;
    public Mesh handMesh;

    // Start is called before the first frame update
    void Start()
    {
        skinnedMeshRenderer = handObject.GetComponent<SkinnedMeshRenderer>();
        CloneHands();
        Debug.Log("here");
    }

    // Update is called once per frame
    void CloneHands()
    {
        // yield return new WaitForSeconds(2);
        var wrapper = new GameObject();
        var rootBoneGameObject = skinnedMeshRenderer.rootBone.gameObject;
        var skeleton = Instantiate(skinnedMeshRenderer.rootBone.gameObject, rootBoneGameObject.transform.position, rootBoneGameObject.transform.rotation, wrapper.transform);
        var newHand = Instantiate(handObject, handObject.transform.position, handObject.transform.rotation, wrapper.transform);
        var meshRenderer = newHand.GetComponent<SkinnedMeshRenderer>();
        meshRenderer.enabled = false;
        meshRenderer.rootBone = skeleton.transform;
        meshRenderer.sharedMesh = handMesh;
        meshRenderer.enabled = true;
        
    }
}
