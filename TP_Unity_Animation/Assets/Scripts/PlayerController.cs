using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

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
    private Animator _animator;
    private System.Collections.Generic.List<VisualEffect> _effects = new System.Collections.Generic.List<VisualEffect>();

    [SerializeField] private GameObject _vfxPrefab;
    [SerializeField] private Transform _dashPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponentInChildren<Animator>();

        _sprintAction = _playerInput.currentActionMap.FindAction("Sprint");
        _moveAction = _playerInput.currentActionMap.FindAction("Move");
        _dashAction = _playerInput.currentActionMap.FindAction("Dash");

        _moveAction.performed += Move;
        _sprintAction.performed += Sprint;
        _sprintAction.canceled += StopSprinting;
        _moveAction.canceled += StopMoving;

        _dashAction.performed += Dash;

        _currentTargetSpeed = _speedWalk;
    }


    private void FixedUpdate()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();
        input.y = 0;
        _rb.MovePosition((Vector2)transform.position + input * _currentTargetSpeed * Time.fixedDeltaTime);
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        _lastDirection = _moveAction.ReadValue<Vector2>();
        _lastDirection.y = 0;

        if (transform.localScale.x > 0)
        {
            
        }
        transform.localScale = Mathf.Abs(_lastDirection.x) > 0 ? new Vector3(1 * _lastDirection.x * Mathf.Abs(transform.lossyScale.x) , transform.lossyScale.y, transform.lossyScale.z) : transform.localScale;
        _animator.SetBool("IsWalking", true);
        //transform.localScale = new Vector3((_lastDirection.x > 0 ? 1 : -1) * transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
    }

    private void StopMoving(InputAction.CallbackContext ctx)
    {
        _animator.SetBool("IsWalking", false);
    }

    private void Dash(InputAction.CallbackContext ctx)
    {
        // Add force
        _rb.AddForce(_lastDirection * _dashSpeed);
        _animator.SetTrigger("Dash");
        CreatParticlesDash();
    }
    
    private void Sprint(InputAction.CallbackContext ctx)
    {
        _isSprinting = true;
        _currentTargetSpeed = _speedSprint;
        _animator.SetBool("isSprinting", true);
    }

    private void StopSprinting(InputAction.CallbackContext ctx)
    {
        _isSprinting = false;
        _currentTargetSpeed = _speedWalk;
        _animator.SetBool("isSprinting", false);
    }

    private void OnDestroy()
    {
        _sprintAction.performed -= Sprint;
        _sprintAction.canceled -= StopSprinting;
    }

    private void CreatParticlesDash() 
    {
        bool createNew = true;
        VisualEffect effect = null;
        for (int i =0; i < _effects.Count; i++) 
        {
            VisualEffect tempEffect = _effects[i];
            if(tempEffect.aliveParticleCount <= 0)
            {
                effect = tempEffect;
                createNew = false;
                break;
            }
        }

        if(createNew) 
        {
            GameObject obj = Instantiate(_vfxPrefab, _dashPos.position, Quaternion.identity);
            effect = obj.GetComponent<VisualEffect>();
        }
        else
        
        effect.transform.position = _dashPos.position;
        effect.SetVector3("DirectionPlayer", new Vector3(_moveAction.ReadValue<Vector2>().x, 0, 0));
        effect.Play();
        _effects.Add(effect);
    }
}
