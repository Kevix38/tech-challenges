using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    [SerializeField]
    private Camera usedCamera;

    private Vector2 previousScreenSize;
    private bool hasFinishTransiton;
    private GameObject currentFocusTarget;
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
        previousScreenSize = new Vector2(Screen.width,Screen.height);
        usedCamera = GetComponent<Camera>();
        hasFinishTransiton = false;
        if(usedCamera == null)
            usedCamera = Camera.main;
    }

    //smooth position to target
    public void SmoothTransition(GameObject focusTarget, Action<string> onFinish, float targetTime = 1.0f, float speedFactor = 1.0f)
    {
        StopAllCoroutines();
        currentFocusTarget = focusTarget;
        StartCoroutine(ISmoothTransition(usedCamera.FocusDistance(focusTarget), onFinish, targetTime, speedFactor));
    }

    //force position to target
    public void FocusOnWithoutTransition(GameObject focusTarget)
    {
        StopAllCoroutines();
        usedCamera.transform.position = usedCamera.FocusDistance(focusTarget);
    }

    //recalculate correct focus distance after first finishing transition when screen size is changing 
    private void FixedUpdate() 
    {    
        if(!hasFinishTransiton)
            return;

        if((Screen.width == (int)previousScreenSize.x) && (Screen.height == (int)previousScreenSize.y))
            return;

        previousScreenSize = Vector2.right*Screen.width + Vector2.up*Screen.height;
    
        FocusOnWithoutTransition(currentFocusTarget);
    }

    private IEnumerator ISmoothTransition(Vector3 targetPosition, Action<string> onFinish, float targetTime, float speedFactor)
    {
        hasFinishTransiton = false;
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
        hasFinishTransiton = true;
        onFinish.Invoke("SmoothTransitionFinish");
    }
}
