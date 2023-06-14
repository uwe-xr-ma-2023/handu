using System.Collections;
using Unity.XR.PXR;
using UnityEngine;

public class EnableSeeThrough : MonoBehaviour
{
    private Camera mainCamera;
    private float enableSeeThroughAfter = 0.1f;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = GetComponent<Camera>();

        if (mainCamera)
        {
            Debug.Log(nameof(ToggleSeeThrough));
            StartCoroutine(ToggleSeeThrough(true));
            mainCamera.backgroundColor = new Color(0, 0, 0, 0);
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
        }
        else
        {
            Debug.LogError("A camera must be referenced or be part of this game object");
        }

    }

    private IEnumerator ToggleSeeThrough(bool enable)
    {
        yield return new WaitForSeconds(enableSeeThroughAfter);

        PXR_Boundary.EnableSeeThroughManual(enable);
        Debug.Log($"See through is set to ({enabled})");
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            PXR_Boundary.EnableSeeThroughManual(true);
        }
    }
}