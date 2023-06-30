using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class HandleCollisionWithHands : MonoBehaviour
{
    public GameObject shatterPrefab;
    private AudioSource audioSource;
    private Renderer _renderer;
    private ElementsSceneManager sceneManager;
    private bool collided = false;
    private GameObject shatterGameObject;
    private float timeToDestroyShatterGameObject = 0.5f;
    private InteractionBehaviour interactionBehaviour;

    private void Awake()
    {
        interactionBehaviour = GetComponent<InteractionBehaviour>();
        // Have to add events that differ per hand via code not UI
        // https://discord.com/channels/994213697490800670/1123598350164426872
        interactionBehaviour.OnPerControllerContactBegin -= OnContactBegin;
        interactionBehaviour.OnPerControllerContactBegin += OnContactBegin;
    }

    private void OnDestroy()
    {
        interactionBehaviour.OnPerControllerContactBegin -= OnContactBegin;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _renderer = GetComponent<Renderer>();
        sceneManager = GameObject.Find("Elements Scene Manager").GetComponent<ElementsSceneManager>();
    }

    public void OnContactBegin(InteractionController controller)
    {
        if (collided)
        {
            return;
        }
        collided = true;
        audioSource.Play();
        _renderer.enabled = false;
        sceneManager.IncreaseGestureGuideChildCollidedCount(controller.isLeft);
        StartCoroutine(WaitForAudioEnd());

        if (shatterPrefab != null) {
            shatterGameObject = Instantiate(shatterPrefab, transform.position, transform.rotation);
            StartCoroutine(WaitForDestroyShatterGameObject());
        }
    }

    IEnumerator WaitForAudioEnd()

    {
        yield return new WaitUntil(() => audioSource.isPlaying == false);
        Destroy(gameObject);
    }

    IEnumerator WaitForDestroyShatterGameObject()
    {
        yield return new WaitForSeconds(timeToDestroyShatterGameObject);
        Destroy(shatterGameObject);
    }
}
