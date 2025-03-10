using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{

    public Action OnchangePerspective;

    [SerializeField]
    public CameraState _cameraState;
    [SerializeField]
    private CinemachineVirtualCamera _fpsCamera;
    [SerializeField]
    private CinemachineFreeLook _tpsCamera;
    [SerializeField]
    private InputManager _inputManager;

    private void Start()
    {
        _inputManager.OnChangePOV += SwitchCamera;
    }

    private void OnDestroy()
    {
        _inputManager.OnChangePOV -= SwitchCamera;
    }

    public void SetTPSFieldOfView(float FieldOfView)
    {
        _tpsCamera.m_Lens.FieldOfView = FieldOfView;
    }

    public void SetFPSClampedCamera(bool isClamped, Vector3 playerRotation)
    {
        CinemachinePOV pov = _fpsCamera.GetCinemachineComponent<CinemachinePOV>();
        if (isClamped)
        {
            pov.m_HorizontalAxis.m_Wrap = false;
            pov.m_HorizontalAxis.m_MinValue = playerRotation.y - 45;
            pov.m_HorizontalAxis.m_MaxValue = playerRotation.y + 45;
        }
        else
        {
            pov.m_HorizontalAxis.m_Wrap = true;
            pov.m_HorizontalAxis.m_MinValue = -180;
            pov.m_HorizontalAxis.m_MaxValue = 180;
        }
    }

    private void SwitchCamera()
    {
        OnchangePerspective();
        if(_cameraState == CameraState.FirstPerson)
        {
            _cameraState = CameraState.ThirdPerson;
            _fpsCamera.gameObject.SetActive(false);
            _tpsCamera.gameObject.SetActive(true);
        }
        else
        {
            _cameraState = CameraState.FirstPerson;
            _fpsCamera.gameObject.SetActive(true);
            _tpsCamera.gameObject.SetActive(false);
        }
    }

}
