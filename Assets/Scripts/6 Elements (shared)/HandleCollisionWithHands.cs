using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCollisionWithHands : MonoBehaviour
{
    public GameObject shatterPrefab;
    private AudioSource audioSource;
    private Renderer _renderer;
    private ElementsSceneManager sceneManager;
    private bool collided = false;
    private GameObject shatterGameObject;
    private float timeToDestroyShatterGameObject = 0.5f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _renderer = GetComponent<Renderer>();
        sceneManager = GameObject.Find("Elements Scene Manager").GetComponent<ElementsSceneManager>();
    }

    public void OnContactBegin()
    {
        if (collided)
        {
            return;
        }
        collided = true;
        audioSource.Play();
        _renderer.enabled = false;
        sceneManager.IncreaseGestureGuideChildCollidedCount();
        StartCoroutine(WaitForAudioEnd());
    }

    public void OnContactBeginReplaceGameObject()
    {
        if (collided)
        {
            return;
        }
        collided = true;
        audioSource.Play();
        _renderer.enabled = false;
        shatterGameObject = Instantiate(shatterPrefab, transform.position, transform.rotation);
        sceneManager.IncreaseGestureGuideChildCollidedCount();
        StartCoroutine(WaitForAudioEnd());
        StartCoroutine(WaitForDestroyShatterGameObject());
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
