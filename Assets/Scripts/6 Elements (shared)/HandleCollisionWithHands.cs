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
        Instantiate(shatterPrefab, transform.position, transform.rotation);
        sceneManager.IncreaseGestureGuideChildCollidedCount();
        StartCoroutine(WaitForAudioEnd());
    }

    IEnumerator WaitForAudioEnd()

    {
        yield return new WaitUntil(() => audioSource.isPlaying == false);
        Destroy(gameObject);
    }
}
