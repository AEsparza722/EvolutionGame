using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    float zoomSpeed = 1.5f;
    float zoomMin = 11;
    float zoomMax = 7;
    CinemachineVirtualCamera virtualCamera;

    float moveSpeed;
    [SerializeField] GameObject cameraControl;
    Vector3 originOfMovement;
    Vector3 differenceOfMovement;
    Vector3 tempPosition;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void LateUpdate()
    {
        ZoomCamera();
        MoveCamera();
    }

    void ZoomCamera() 
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && virtualCamera.m_Lens.OrthographicSize > zoomMax)
        {
            virtualCamera.m_Lens.OrthographicSize -= zoomSpeed;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && virtualCamera.m_Lens.OrthographicSize < zoomMin)
        {
            virtualCamera.m_Lens.OrthographicSize += zoomSpeed;
        }        
    }

    void MoveCamera()
    {        
        if (Input.GetMouseButtonDown(1))
        {
            originOfMovement = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            differenceOfMovement = originOfMovement - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tempPosition = cameraControl.transform.position + differenceOfMovement;

            tempPosition.x = Mathf.Clamp(tempPosition.x, -45, 45);
            tempPosition.y = Mathf.Clamp(tempPosition.y, -45, 45);

            cameraControl.transform.position = tempPosition;
        
        }



    }

}