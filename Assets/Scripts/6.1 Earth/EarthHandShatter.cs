using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthHandShatter : MonoBehaviour
{
    public GameObject shatterPrefab;
    public float waitForShatter = 0f;
    void Start()
    {
        StartCoroutine(ShowShatterPrefab());
    }

    private IEnumerator ShowShatterPrefab()
    {
        yield return new WaitForSeconds(waitForShatter);
        Instantiate(shatterPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
