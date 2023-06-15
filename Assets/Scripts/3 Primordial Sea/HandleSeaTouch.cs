using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSeaTouch : MonoBehaviour
{
    private AudioSource audioSource;
    private Renderer meshRenderer;
    public Material white;
    public Material white50;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        meshRenderer = GetComponent<Renderer>();
        meshRenderer.material = white50;
    }
    public void OnSeaTouchStart()
    {
        audioSource.Play();
        meshRenderer.material = white50;
    }



    public void OnSeaTouchEnd()
    {
        audioSource.Pause();
        meshRenderer.material = white;
    }


}
