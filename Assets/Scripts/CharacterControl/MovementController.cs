using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;

    [Header("Jump")]
    [SerializeField] float _jumpHeight = 2.5f;
    [SerializeField] float _gravity = -9.81f;
    [SerializeField] float _fallMultiplier = 2.5f;  // Makes falling feel heavier
    [SerializeField] float _lowJumpMultiplier = 2f; // Allows for variable jump height
    [SerializeField] float _moveSpeed = 5f;
    private Vector3 _currentMovement;
    private List<object> _controllerBlockers = new();
    private bool _block;
    private float verticalVelocity;


    [Header("Animations")]
    [SerializeField] private Animator _cameraAnimator;

    private void OnEnable()
    {
        PlayerInput.OnMoveActionMain += OnMove;
        PlayerInput.OnJumpActionMain += OnJump;
    }

    private void OnDisable()
    {
        PlayerInput.OnMoveActionMain -= OnMove;
        PlayerInput.OnJumpActionMain -= OnJump;
    }

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_block)
            return;

        var inputValue = context.ReadValue<Vector2>();
        _currentMovement = new Vector3(inputValue.x, 0, inputValue.y).normalized;
    }

    void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_block)
            return;

        if (context.started && _characterController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }
    }

    private void FixedUpdate()
    {
        if (_characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small downward force to keep grounded
        }

        // Better jump feel
        if (verticalVelocity > 0)
        {
            verticalVelocity += _gravity * (_lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (verticalVelocity < 0)
        {
            verticalVelocity += _gravity * (_fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        verticalVelocity += _gravity * Time.fixedDeltaTime;

        Vector3 move = transform.TransformDirection(new Vector3(_currentMovement.x, 0, _currentMovement.z));
        _characterController.Move(move * _moveSpeed * Time.fixedDeltaTime);

        Vector3 verticalMove = new Vector3(0, verticalVelocity, 0);
        _characterController.Move(verticalMove * Time.fixedDeltaTime);


        _cameraAnimator.enabled = _currentMovement.magnitude > 0 && _characterController.isGrounded;
    }



    public void BlockMovement(object origin, bool state)
    {
        if (state && !_controllerBlockers.Contains(origin))
            _controllerBlockers.Add(origin);
        else if (!state && _controllerBlockers.Contains(origin))
            _controllerBlockers.Remove(origin);

        _block = _controllerBlockers.Count > 0;
    }
}
