using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //first person camera controller
    [SerializeField] private Vector2 _rotationSpeed;
    [SerializeField] private float _slerpSpeed;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _camera;
    [SerializeField] private Vector2 _clampAngles;
    [SerializeField] private Vector3 _offset;
    private Vector3 _currentRotation;

    private List<object> _controllerBlockers = new();
    private bool _block;
    private bool _focusing;
    private Vector3 _previousFocusRotation;
    private Tween _focusTween;

    void OnEnable()
    {
        PlayerInput.OnLookActionMain += HandleLook;
    }

    private void OnDisable()
    {
        PlayerInput.OnLookActionMain -= HandleLook;
    }

    private void HandleLook(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_block)
            return;


        var input = context.ReadValue<Vector2>();
        _currentRotation.x += input.y * _rotationSpeed.y;
        _currentRotation.x = Mathf.Clamp(_currentRotation.x, -_clampAngles.x, _clampAngles.x);
        _currentRotation.y += input.x * _rotationSpeed.x;
    }

    private void LateUpdate()
    {
        if (_focusing)
            return;

        Vector3 targetPlayerRotation = new Vector3(0, _currentRotation.y, 0);
        Vector3 targetCameraRotation = new Vector3(_currentRotation.x, _currentRotation.y, 0);

        _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, Quaternion.Euler(targetPlayerRotation), _slerpSpeed * Time.deltaTime);
        _camera.transform.localRotation = Quaternion.Slerp(_camera.transform.localRotation, Quaternion.Euler(targetCameraRotation), _slerpSpeed * Time.deltaTime);


        _camera.transform.position = _player.transform.position + _offset;
    }

    public void BlockCamera(object origin, bool state)
    {
        if (state && !_controllerBlockers.Contains(origin))
            _controllerBlockers.Add(origin);
        else if (!state && _controllerBlockers.Contains(origin))
            _controllerBlockers.Remove(origin);

        _block = _controllerBlockers.Count > 0;
    }

    public void FocusOnTarget(GameObject target)
    {
        BlockCamera(this, true);
        _previousFocusRotation = _camera.transform.eulerAngles;
        _focusTween = _camera.transform.DOLookAt(target.transform.position, .2f);
        _focusing = true;
    }

    public void RevertFocus()
    {
        if (_focusing)
        {
            _focusTween = _camera.transform.DORotate(_previousFocusRotation, .2f).OnComplete(() => BlockCamera(this, false));
            _focusing = false;
        }
    }
}
