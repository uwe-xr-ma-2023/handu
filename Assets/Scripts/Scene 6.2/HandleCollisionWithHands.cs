using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCollisionWithHands : MonoBehaviour
{
    private AudioSource audioSource;
    private Renderer _renderer;
    private SceneManager62Water sceneManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _renderer = GetComponent<Renderer>();
        sceneManager = GameObject.Find("Scene Manager").GetComponent<SceneManager62Water>();
    }

    public void OnHoverBegin()
    {
        audioSource.Play();
        _renderer.enabled = false;
        StartCoroutine(WaitForAudioEnd());
    }

    IEnumerator WaitForAudioEnd()
    {
        yield return new WaitUntil(() => audioSource.isPlaying == false);
        sceneManager.IncreaseWaveChildCollidedCount();
        Destroy(gameObject);
    }
}
