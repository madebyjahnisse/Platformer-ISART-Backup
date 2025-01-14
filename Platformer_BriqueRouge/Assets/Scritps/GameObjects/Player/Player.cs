using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    // Movements
    [Header("Movements")]
    private Vector3 _AccelerationVector = Vector3.zero;
    private Vector3 _Velocity = Vector3.zero;

    // Horizontal Movements
    [Header("Horizontal Movements")]
    [SerializeField] private float _AccelerationSpeed = 10f;
    [SerializeField] private float _DeccelerationSpeed = 8f;
    [SerializeField] private float _MaxSpeedHorizontal = 100f;
    [SerializeField] private float _VelocityMin = 0.5f;

    // Vertical Movements
    [Header("Vertical Movements")]
    [SerializeField] private float _GravityForce = 10f;
    [SerializeField] private float _MaxSpeedVertical = 100f;
    [SerializeField] private float _JumpForce = 50f;
    private bool _Jump = false;

    // Dash
    [Header("Dash Settings")]
    [SerializeField] private float _DashDistance = 6f;
    [SerializeField] private float _DashDuration = 1f;
    private bool _isDashHeld = false;
    [SerializeField]  private float _dashHoldTime = 0f; 
    private float _holdStartTime;

    private Vector2 _InitMousePosition = Vector2.zero;
    [SerializeField] private float _DashClampAngle = 45f;
    [SerializeField] private float _OffSetMousePositionToDash = 10f;
    private bool _IsDashing;
    private bool _CanDash;
    private bool _canDash = true;

    private bool _DashInit = false;
    private bool _DashConfirm = false;

    //Arrow

    [Header("Arrow Settings")]
    [SerializeField] private Transform _ArrowPivot;
    [SerializeField] private float _holdDuration = 0f;
    [SerializeField] private float _maxScaleY = 0.5f; 
    [SerializeField] private float _minScaleY = 0f; 
    [SerializeField] private float _scaleSpeed = 2f;

    //Camera
    [Header("Camera Settings")]
    [SerializeField] private Camera _camera; // Cam ref
    [SerializeField] private float _zoomInFOV = 8f; // FOV dash
    [SerializeField] private float _zoomOutFOV = 12f; // Default fov
    [SerializeField] private float _zoomDuration = 0.2f;
    private float _targetFOV; 
    private float _originalFOV;

    // Ground Detection
    [Header("Ground Detection")]
    [SerializeField] private float _RayCastUpDownDistance = 1f;
    [SerializeField] private Vector2 _GroundDetectionBoxSize;
    [SerializeField] private Vector2 _HeadDetectionBoxSize;
    [SerializeField] private LayerMask _GroundLayer;
    private bool _IsGrounded = false;
    private bool _IsHeadColliding = false;

    // Side Collisions
    [Header("Side Collisions")]
    [SerializeField] private float _RayCastSideDistance = .5f;
    [SerializeField] private Vector2 _SideDetectionBoxSize;
    private bool _IsRightColliding = false;
    private bool _IsLeftColliding = false;

    // Tyrolean
    [Header("Tyrolean System")]
    public bool isTyro = false;

    //Inputs

    public PlayerMovements _Input;


    #region Singleton
    private static Player _Instance;

    public static Player Instance
    {
        get 
        {
            if( _Instance == null )
            {
                Debug.Log("Player not existing");
            }
            return _Instance;
            
        }
    }
    private void Awake()
    {
        _Instance = this;
        _Input = new PlayerMovements();
    }
    #endregion

    #region Enable/Disable

    private void OnEnable()
    {
        _Input.Player.Dash.performed += OnDashHold;
        _Input.Player.Dash.canceled += OnDashRelease;
        _Input.Enable();
    }

    private void OnDisable()
    {
        _Input.Player.Dash.performed -= OnDashHold;
        _Input.Player.Dash.canceled -= OnDashRelease;
        _Input.Disable();
    }
    #endregion

    void Start()
    {
        _camera = Camera.main;
        _originalFOV = _camera.orthographicSize; 
        _targetFOV = _originalFOV;
    }

    void Update()
    {
        _IsGrounded = IsGrounded();
        Inputs();
        Gravity();
        //DashInit();
        TopOfHeadCollision();
        Jump();
        Deccelerate();
        Accelerate();
        CollideWithWalls();
        Move();
        ArrowDir();
    }

    //DEBUG A SUPPRIMER AVANT CODEREVIEW --------------------------------------

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position-transform.up*_RayCastUpDownDistance,_GroundDetectionBoxSize); //permet de voir la boxcast de detection du sol
        Gizmos.DrawWireCube(transform.position + transform.up * _RayCastUpDownDistance, _HeadDetectionBoxSize); //celle au dessus de lui
        Gizmos.DrawWireCube(transform.position + transform.right * _RayCastSideDistance, _SideDetectionBoxSize); //celle a droite
        Gizmos.DrawWireCube(transform.position - transform.right * _RayCastSideDistance, _SideDetectionBoxSize); //celle a droite

    }

    //DEBUG A SUPPRIMER AVANT CODEREVIEW ----------------------------------------

    private void Inputs ()
    {
        if (_isDashHeld) return;

        if (!_DashInit)
        {
            _AccelerationVector = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
            _Jump = Input.GetKey(KeyCode.Space);
        }
    }

private bool IsGrounded()
    {
        if(Physics2D.BoxCast(transform.position - transform.up * _RayCastUpDownDistance, _GroundDetectionBoxSize,0,-transform.up, 0, layerMask:_GroundLayer)) return true; //makes a box raycast to detect the ground below player
        return false;
    }

    #region Horizontal Movements
    private void Accelerate()
    {
        if(Mathf.Abs(_Velocity.x)<_MaxSpeedHorizontal) _Velocity += _AccelerationVector * _AccelerationSpeed * Time.deltaTime;        
    }

    private void Deccelerate()
    {
        if (_Velocity.x != 0 && Input.GetAxisRaw("Horizontal")==0)
        {
            float lSpeedRatio = Mathf.Abs(_Velocity.x) / _MaxSpeedHorizontal;
            if (_Velocity.x > 0) _Velocity += Vector3.left * (_DeccelerationSpeed * (1 + lSpeedRatio))  * Time.deltaTime; //* (1 + lSpeedRatio) -> faster you go, faster you slow deccelerate
            else _Velocity+= Vector3.right * (_DeccelerationSpeed * (1 + lSpeedRatio)) * Time.deltaTime;

            if (Mathf.Abs(_Velocity.x) < _VelocityMin) _Velocity = Vector3.up * _Velocity.y; //if horizontal velocity is small, makes it = 0 and keeps vertical velocity
            
        }   
        else if(_Velocity.x * Input.GetAxisRaw("Horizontal") < 0) _Velocity = Vector3.up * _Velocity.y; //to stop and not slide when changing direction        
    }

    private void CollideWithWalls()
    {
        //Right:
        if (Physics2D.BoxCast(transform.position + transform.right * _RayCastSideDistance, _SideDetectionBoxSize, 0, transform.right, 0, layerMask: _GroundLayer))
        {
            if(_Velocity.x>0) _Velocity.x = 0;
        }
        if (Physics2D.BoxCast(transform.position - transform.right * _RayCastSideDistance, _SideDetectionBoxSize, 0, -transform.right, 0, layerMask: _GroundLayer))
        {
            if(_Velocity.x<0) _Velocity.x = 0;
        }
    }


    #endregion

    #region Vertical Movements
    private void Gravity()
    {
        if (_IsGrounded)
        {
            _Velocity = Vector3.right * _Velocity.x; //vertical velocity = 0 but keeps horiztonal velocity
            return;
        }
        else
        {
            if (_IsDashing) return;
            _Velocity += -Vector3.up * _GravityForce * Time.deltaTime; //to fall down
        }
    }

    /// <summary>
    /// To bump and fall down when jumping head first into a wall
    /// </summary>
    private void TopOfHeadCollision()
    {
        if (Physics2D.BoxCast(transform.position + transform.up * _RayCastUpDownDistance, _HeadDetectionBoxSize, 0, transform.up, 0, layerMask: _GroundLayer))
        {
            if (!_IsHeadColliding)
            {
                _Velocity = Vector3.right * _Velocity.x; //The player does not go through ground from under
                _IsHeadColliding = true;
            }
            else
            {
                _Velocity += -Vector3.up * _GravityForce * Time.deltaTime; //to fall down
            }
        }
        else _IsHeadColliding = false;
    }

    private void Jump()
    {
        if(_IsGrounded && _Jump)
        {
            _Velocity += Vector3.up * _JumpForce;
            _IsGrounded = false;
        }
    }
    #endregion

    #region Dash

    private void OnDashHold(InputAction.CallbackContext context)
    {
        if (!_isDashHeld)
        {
            _isDashHeld = true;
            _holdStartTime = Time.time;  // TimeCode to Hold
            Debug.Log("Dash Started");

            //Play Slow Motion
            Time.timeScale = 0.1f;

            // Zoom in
            _targetFOV = _zoomInFOV;
            StartCoroutine(ZoomCamera(_zoomDuration));
        }
    }

    private void OnDashRelease(InputAction.CallbackContext context)
    {
        if (_isDashHeld)
        {
            float holdDuration = Time.unscaledTime - _holdStartTime;

            if (holdDuration >= _dashHoldTime)
            {
                Debug.Log("Dash Released");
                
                //Stop SlowMotion
                Time.timeScale = 1f;

                //if InitMousePosition != MousePosition : return Clamp Angle between Mouse Position && Player
                float lAngleBetweenMouseAndPlayer = (180 / Mathf.PI) * Mathf.Atan2(Input.mousePosition.x - _InitMousePosition.x, Input.mousePosition.y - _InitMousePosition.y);
                StartCoroutine(PerformDash());
                Dash(Mathf.Round(lAngleBetweenMouseAndPlayer));

                // Reset zoom
                _targetFOV = _originalFOV;
                StartCoroutine(ZoomCamera(_zoomDuration));
            }
            else
            {
                Debug.Log("Dash Tapped (not long enough to trigger release action) " + holdDuration);
            }

            _isDashHeld = false;  // Reset Holding
        }
    }

    private System.Collections.IEnumerator PerformDash()
    {
        _canDash = false;
        _IsDashing = true;

        yield return new WaitForSeconds(_DashDuration);

        _IsDashing = false;
        yield return new WaitForSeconds(_DashDuration);
        if (_IsGrounded)
            _canDash = true;
    }

    private IEnumerator ZoomCamera(float duration)
    {
        float timeElapsed = 0f;
        float initialFOV = _camera.orthographicSize;

        while (timeElapsed < duration)
        {
            _camera.orthographicSize = Mathf.Lerp(initialFOV, _targetFOV, timeElapsed / duration);
            timeElapsed += Time.unscaledTime;
            yield return null;
        }

        // Assure-toi que le FOV final est bien celui que l'on veut
        _camera.orthographicSize = _targetFOV;
    }
    private void ArrowDir()
    {
        Vector3 direction = _AccelerationVector;  
        direction.z = 0;  // Ignore the Z for rot

        // if moving changing arrow rotation
        if (direction.magnitude > 0.1f)  
        {
            //Calculate angle for arrow
            float lAngleBetweenPlayerAndDirection = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _ArrowPivot.rotation = Quaternion.Euler(new Vector3(0, 0, lAngleBetweenPlayerAndDirection - 90));
        }
        else
        {
            //keep arrow to default pose if no input
            _ArrowPivot.rotation = Quaternion.Euler(Vector3.zero);
        }

        // if holding dash
        if (_isDashHeld)
        {
            
            _holdDuration += Time.unscaledDeltaTime * _scaleSpeed;

            // Clamp size
            _holdDuration = Mathf.Clamp(_holdDuration, 0f, 1f);

            // lerp Y scale
            float scaleY = Mathf.Lerp(_minScaleY, _maxScaleY, _holdDuration);

            // apply scale to ArrowPivot
            _ArrowPivot.localScale = new Vector3(_ArrowPivot.localScale.x, scaleY, _ArrowPivot.localScale.z);
        }
        else
        {
            // Reset
            _holdDuration = 0f;
            _ArrowPivot.localScale = new Vector3(_ArrowPivot.localScale.x, _minScaleY, _ArrowPivot.localScale.z);
        }
    }

    private void Dash(float pDashAngle)
    {
        Vector3 dashDirection = new Vector3(
            Mathf.Sin(pDashAngle * Mathf.Deg2Rad),
            Mathf.Cos(pDashAngle * Mathf.Deg2Rad), 0
        ).normalized;
        Debug.Log(_Velocity + "  B ");
        _Velocity = _AccelerationVector.normalized * _DashDistance;
        Debug.Log(_Velocity);
        Debug.Log(dashDirection);
    }

    #endregion
    private void Move()
    {
        transform.position += _Velocity * Time.deltaTime * TimeManager.TimeValue;
    }


    // A jeté ?
    #region Poubelle
    /*
private void DashInit()
{
    if (_DashInit)
    {
        //Play Slow Motion
        Time.timeScale = 0.1f;
        _InitMousePosition = Input.mousePosition;
        //Set InitMousePosition
    }
    if (_DashConfirm)
    {
        //Stop SlowMotion
        Time.timeScale = 1f;
        //if InitMousePosition != MousePosition : return Clamp Angle between Mouse Position && Player
        if (Vector2.Distance(_InitMousePosition, Input.mousePosition)> _OffSetMousePositionToDash)
        {
            float lAngleBetweenMouseAndPlayer = (180 / Mathf.PI) * Mathf.Atan2(Input.mousePosition.x - _InitMousePosition.x, Input.mousePosition.y - _InitMousePosition.y);
            Dash(Mathf.Round(lAngleBetweenMouseAndPlayer / _DashClampAngle) * _DashClampAngle);
        }
        //if ijkl key pressed get the vector : return angle vector
        //if android get angle from drag 
    }
}
*/
    #endregion
}
