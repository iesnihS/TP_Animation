using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speedWalk = 10;
    [SerializeField] private float _speedSprint = 20;
    [SerializeField] private float _dashSpeed = 10000;
    private float _currentTargetSpeed = 1;

    private PlayerInput _playerInput = null;
    private InputAction _sprintAction;
    private InputAction _moveAction;
    private InputAction _dashAction;
    private Vector2 _lastDirection;
    private bool _isSprinting = false;
    private Rigidbody2D _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rb = gameObject.GetComponent<Rigidbody2D>();

        _sprintAction = _playerInput.currentActionMap.FindAction("Sprint");
        _moveAction = _playerInput.currentActionMap.FindAction("Move");
        _dashAction = _playerInput.currentActionMap.FindAction("Dash");

        _moveAction.performed += Move;
        _sprintAction.performed += Sprint;
        _sprintAction.canceled += StopSprinting;
        
        _dashAction.performed += Dash;

        _currentTargetSpeed = _speedWalk;
    }
    
    

    private void FixedUpdate()
    {
        _rb.MovePosition((Vector2)transform.position + _moveAction.ReadValue<Vector2>() * _currentTargetSpeed * Time.fixedDeltaTime);
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        _lastDirection = _moveAction.ReadValue<Vector2>();
    }


    private void Dash(InputAction.CallbackContext ctx)
    {
        // Add force
        _rb.AddForce(_lastDirection * _dashSpeed);
    }
    
    private void Sprint(InputAction.CallbackContext ctx)
    {
        _isSprinting = true;
        _currentTargetSpeed = _speedSprint;
    }

    private void StopSprinting(InputAction.CallbackContext ctx)
    {
        _isSprinting = false;
        _currentTargetSpeed = _speedWalk;
    }

    private void OnDestroy()
    {
        _sprintAction.performed -= Sprint;
        _sprintAction.canceled -= StopSprinting;
    }
}
