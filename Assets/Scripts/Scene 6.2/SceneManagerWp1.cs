using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerWp1 : MonoBehaviour
{
    public AudioSource sceneAudio;
    public GameObject wavePrefab;
    private GameObject waveGameObject;
    private float timeToShowWave = 45.0f;
    private float timeToHideWave = 73.0f;
    private int waveChildCount;
    private int waveChildCollidedCount;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForShowWave());
        StartWave();
        waveGameObject.SetActive(false);
        SkipToWaveShow();
    }

    // Update is called once per frame
    IEnumerator WaitForShowWave()
    {
        yield return new WaitForSeconds(timeToShowWave);
        ShowWave();
    }
    IEnumerator WaitForHideWave()
    {
        yield return new WaitForSeconds(timeToHideWave - timeToShowWave);
        waveGameObject.SetActive(false);

    }

    private void ShowWave()
    {
        waveGameObject.SetActive(true);
        StartCoroutine(WaitForHideWave());
    }

    public void SkipToWaveShow()
    {
        if (sceneAudio != null)
        {
            sceneAudio.time = timeToShowWave;
        }
        StopAllCoroutines();
        ShowWave();
    }

    public void IncreaseWaveChildCollidedCount()
    {
        waveChildCollidedCount++;
        if (waveChildCollidedCount == waveChildCount)
        {
            StartWave();
        }
    }

    private void StartWave()
    {
        waveChildCollidedCount = 0;
        float randomX = Random.Range(-0.5f, 0.5f);
        float randomY = Random.Range(-0.5f, 0.5f);
        Vector3 randomOffset = new Vector3(randomX, randomY, 0);
        Vector3 randomPosition = wavePrefab.transform.position + randomOffset;
        waveGameObject = Instantiate(wavePrefab, randomPosition, wavePrefab.transform.rotation);
        waveChildCount = waveGameObject.transform.childCount;
    }
}
