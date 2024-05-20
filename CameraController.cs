using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables

    private Camera _mainCamera;
    
    [SerializeField] private List<Transform> targets;
    [SerializeField] private float edgeBuffer = 4.0f;
    [SerializeField] private float minSize = 6.0f;
    [SerializeField] private float maxSize = 18.0f;

    [SerializeField] private float smoothTime = 0.2f;
    private Vector3 _velocity;
    private float _zoomSpeed;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        transform.position = GetAveragePosition();
        _mainCamera.orthographicSize = GetDesiredSize();
    }

    private void LateUpdate()
    {
        SetPosition();
        SetSize();
    }

    #endregion

    #region Setters

    private void SetPosition()
    {
        transform.position = Vector3.SmoothDamp(transform.position, GetAveragePosition(), ref _velocity, smoothTime);
    }

    private void SetSize()
    {
        _mainCamera.orthographicSize = Mathf.SmoothDamp(_mainCamera.orthographicSize, GetDesiredSize(), ref
            _zoomSpeed, smoothTime);
    }

    #endregion

    #region Getters
    
    private Vector3 GetAveragePosition()
    {
        var averagePosition = new Vector3();

        foreach (var target in targets)
        {
            averagePosition += target.position;
        }

        averagePosition /= targets.Count;

        averagePosition.y = transform.position.y;

        return averagePosition;
    }

    private float GetDesiredSize()
    {
        var size = 0.0f;
        var desiredLocalPosition = transform.InverseTransformPoint(GetAveragePosition());

        foreach (var target in targets)
        {
            var targetLocalPos = transform.InverseTransformPoint(target.position);
            var desiredPosToTarget = targetLocalPos - desiredLocalPosition;

            size = Mathf.Max(
                size,
                Mathf.Abs(desiredPosToTarget.y),
                Mathf.Abs(desiredPosToTarget.x) / _mainCamera.aspect);
        }

        return Mathf.Clamp(size + edgeBuffer, minSize, maxSize);
    }

    #endregion
}
