using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Camera Distance")]
    [SerializeField] private bool canChangeCameraDistance;

    [SerializeField] private float distanceChangeRate;
    private CinemachinePositionComposer _composer;
    private float _targetCameraDistance;

    private CinemachineVirtualCameraBase _virtualCamera;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        _virtualCamera = GetComponentInChildren<CinemachineVirtualCameraBase>();
        _composer = GetComponentInChildren<CinemachinePositionComposer>();
    }

    private void Update()
    { 
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (!canChangeCameraDistance) return;
        
        float currentDistance = _composer.CameraDistance;

        if(Mathf.Abs(currentDistance - _targetCameraDistance) < 0.1f) return;
        
        _composer.CameraDistance = Mathf.Lerp(_composer.CameraDistance, _targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance) => _targetCameraDistance = distance;
}