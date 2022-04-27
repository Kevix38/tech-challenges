using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    [SerializeField]
    private Camera usedCamera;

    public Camera UsedCamera
    {
        get
        {
            return usedCamera;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        usedCamera = GetComponent<Camera>();

        if(usedCamera == null)
            usedCamera = Camera.main;
    }

    public void SmoothTransition(GameObject focusTarget, Action<string> onFinish, float targetTime = 1.0f, float speedFactor = 1.0f)
    {
        StopAllCoroutines();
        StartCoroutine(ISmoothTransition(usedCamera.FocusDistance(focusTarget), onFinish, targetTime, speedFactor));
    }

    private IEnumerator ISmoothTransition(Vector3 targetPosition, Action<string> onFinish, float targetTime, float speedFactor)
    {
        float nearClipPlane = usedCamera.nearClipPlane;
        float farClipPlane = usedCamera.farClipPlane;
        usedCamera.nearClipPlane = 0.01f;
        usedCamera.farClipPlane = 10000.0f;
        float currentTime = 0.0f;
        Transform cameraTransform = usedCamera.transform;
        Vector3 initPosition = cameraTransform.position;
        while (currentTime <= targetTime)
        {
            cameraTransform.position = Vector3.Lerp(initPosition, targetPosition, currentTime / targetTime);
            currentTime += Time.deltaTime * speedFactor;
            yield return null;
        }
        
        cameraTransform.position = targetPosition;
        usedCamera.nearClipPlane = nearClipPlane;
        usedCamera.farClipPlane = farClipPlane;

        onFinish.Invoke("SmoothTransitionFinish");
    }
}
