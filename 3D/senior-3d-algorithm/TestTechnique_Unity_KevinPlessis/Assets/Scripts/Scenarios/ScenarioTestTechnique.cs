using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScenarioTestTechnique : MonoBehaviour
{
    public bool UseOrthographicCamera = false;
    // Start is called before the first frame update
    [SerializeField]
    private RandomObjectGenerator childsRoot;

    [SerializeField]
    private CameraTransition cameraTransition;

    [SerializeField]
    private Vector2 MinMaxCameraFOV = new Vector2(30.0f,90.0f);
    [SerializeField]
    private Vector3 MaxPositionOffset = new Vector3(5.0f,10.0f,3.0f);
    [SerializeField]
    private Vector3 MaxRotationOffset = new Vector3(30.0f,60.0f,0.0f);
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        childsRoot.onfinish.AddListener(() => StartCoroutine(IBeginScenario()));
        childsRoot.GenerateRandomObject();      
    }
    
    //We set random FOV, rotation, position to camera, and made camera smooth transition to focused object
    private IEnumerator IBeginScenario()
    {
        cameraTransition.UsedCamera.fieldOfView = Random.Range(MinMaxCameraFOV.x,MinMaxCameraFOV.y);      
        cameraTransition.UsedCamera.transform.position =
        Vector3.right * Random.Range(0.0f,MaxPositionOffset.x) + 
        Vector3.up * Random.Range(0.0f,MaxPositionOffset.y) + 
        Vector3.forward * Random.Range(0.0f,MaxPositionOffset.z);

        cameraTransition.UsedCamera.transform.rotation = Quaternion.Euler(
        Random.Range(0.0f,MaxRotationOffset.x),
        Random.Range(0.0f,MaxRotationOffset.y),
        Random.Range(0.0f,MaxRotationOffset.z));

        yield return new WaitForSeconds(1.0f);

        cameraTransition.SmoothTransition(childsRoot.gameObject,onfinish => 
        {
            Debug.Log(onfinish + "");
        });
    }

    //This method can be execute in Editor
    [CanExecuteInEditor]
    public void ReloadBehaviour()
    {
        StartCoroutine(Start());
    }

    //Use to automaticaly change camera orthographic parameter when "UseOrthographicCamera" bool is changed
    private void OnValidate()
    {
        if(cameraTransition.UsedCamera == null)
            return;
            
        cameraTransition.UsedCamera.orthographic = UseOrthographicCamera;
        ReloadBehaviour();
    }
}
