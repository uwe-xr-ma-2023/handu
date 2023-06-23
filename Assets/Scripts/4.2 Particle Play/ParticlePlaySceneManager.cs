using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ParticlePlaySceneManager : MonoBehaviour
{
    public GameObject particlePrefab;
    public GameObject container;
    [Range(0, 100)]
    public int numberOfParticles;
    [Range(0, 0.1f)]
    public float particleScaleMinimum;
    [Range(0, 0.1f)]
    public float particleScaleMaximum;
    public float particleAreaXMin;
    public float particleAreaYMin;
    public float particleAreaZMin;
    public float particleAreaXMax;
    public float particleAreaYMax;
    public float particleAreaZMax;

    void Start()
    {
      GenerateParticles();
    }

    // Update is called once per frame
    void GenerateParticles()
    {
        for (int i = 0; i < numberOfParticles; i++)
        {
            float xPos = Random.Range(particleAreaXMin, particleAreaXMax);
            float yPos = Random.Range(particleAreaYMin, particleAreaYMax);
            float zPos = Random.Range(particleAreaZMin, particleAreaZMax);
            float scale = Random.Range(particleScaleMinimum, particleScaleMaximum);
            Vector3 partilcePosition = new Vector3(xPos, yPos, zPos);
            GameObject particleObject = Instantiate(particlePrefab, partilcePosition, particlePrefab.transform.rotation, container.transform);
            particleObject.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
