using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    //Movements:
    private Vector3 _AccelerationVector = Vector3.zero;
    private Vector3 _Velocity = Vector3.zero;

    //Horizontal Movements :
    [SerializeField] private float _AccelerationSpeed = 10f;
    [SerializeField] private float _DeccelerationSpeed = 8f;
    [SerializeField] private float _MaxSpeedHorizontal = 100f;
    [SerializeField] private float _VelocityMin = 0.5f;

    //Vertical Movements :
    [SerializeField] private float _GravityForce = 10f;
    [SerializeField] private float _MaxSpeedVertical = 100f;
    [SerializeField] private float _JumpForce = 50f;
    private bool _Jump = false;

    //Dash
    [SerializeField] private float _DashDistance = 6f;
    [SerializeField] private float _DashDuration = 1f;

    private Vector2 _InitMousePosition = Vector2.zero;
    [SerializeField] private float _DashClampAngle = 45f;
    [SerializeField] private float _OffSetMousePositionToDash = 10f;
    private bool _IsDashing;
    private bool _CanDash;

    private bool _DashInit = false;
    private bool _DashConfirm = false;

    //Ground Detection :
    private RaycastHit2D Tophit;
    private RaycastHit2D DownHit;
    private bool _IsGrounded = false;
    private bool _IsHeadColliding = false;
    [SerializeField] private float _RayCastUpDownDistance = 1f;
    [SerializeField] private Vector2 _GroundDetectionBoxSize;
    [SerializeField] private Vector2 _HeadDetectionBoxSize;
    [SerializeField] private LayerMask _GroundLayer;

    //Side Collisions :
    private bool _IsRightColliding = false;
    private bool _IsLeftColliding = false;
    [SerializeField] private float _RayCastSideDistance = .5f;
    [SerializeField] private Vector2 _SideDetectionBoxSize;

    //Tyrolienne :
    public bool isTyro = false;

    //Platform:
    [SerializeField] private string _TraversableTag = "Traversable";
    [SerializeField] private string _MobileTag = "Mobile";
    [SerializeField] private float _TraversableSize = 1f;
    private bool _IsOnMobilePlatform = false;
    private MobilePlatform _ActualMobilePlatform;

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
    }
    #endregion

    void Start()
    {
        
    }

    void Update()
    {
        _IsGrounded = IsGrounded();
        Inputs();
        Gravity();
        DashInit();
        TopOfHeadCollision();
        Jump();
        Deccelerate();
        Accelerate();
        CollideWithWalls();
        Move();
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
        _AccelerationVector = new Vector3(Input.GetAxisRaw("Horizontal"),0,0);
        _Jump = Input.GetKey(KeyCode.Space);
        _DashInit = Input.GetMouseButtonDown(0);
        _DashConfirm = Input.GetMouseButtonUp(0);
    }

private bool IsGrounded()
    {
        if (DownHit = Physics2D.BoxCast(transform.position - transform.up * _RayCastUpDownDistance,
            _GroundDetectionBoxSize, 0, -transform.up, 0, layerMask: _GroundLayer)) //makes a box raycast to detect the ground below player
        {
            if(DownHit.transform.gameObject.CompareTag(_TraversableTag) && 
                DownHit.transform.position.y+_TraversableSize > transform.position.y) return false;
            
            if (DownHit.transform.gameObject.CompareTag(_MobileTag))
            {
                _ActualMobilePlatform = DownHit.transform.gameObject.GetComponent<MobilePlatform>();
                _IsOnMobilePlatform =true;
            }
            return true; 
        }
        else
        {
            _IsOnMobilePlatform = false;
            _ActualMobilePlatform = null;
        }
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
            _Velocity += -Vector3.up * _GravityForce * Time.deltaTime; //to fall down
        }
    }

    /// <summary>
    /// To bump and fall down when jumping head first into a wall
    /// </summary>
    private void TopOfHeadCollision()
    {
        
        if (Tophit=Physics2D.BoxCast(transform.position + transform.up * _RayCastUpDownDistance, _HeadDetectionBoxSize, 0, transform.up, 0, layerMask: _GroundLayer))
        {
            if (!_IsHeadColliding && !Tophit.transform.gameObject.CompareTag(_TraversableTag))
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
    private void DashInit()
    {
        if (_DashInit)
        {
            //Play Slow Motion
            _InitMousePosition = Input.mousePosition;
            //Set InitMousePosition
        }
        if (_DashConfirm)
        {
            //Stop SlowMotion
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
    private void Dash(float pDashAngle)
    {
        Vector3 dashDirection = new Vector3(
            Mathf.Sin(pDashAngle * Mathf.Deg2Rad),
            Mathf.Cos(pDashAngle * Mathf.Deg2Rad), 0
        ).normalized;
        _Velocity = dashDirection * _DashDistance;
        Debug.Log(dashDirection);
    }


    #endregion
    private void Move()
    {
        transform.position += _Velocity * Time.deltaTime * TimeManager.TimeValue;
        if (_IsOnMobilePlatform && _ActualMobilePlatform!=null) transform.position += _ActualMobilePlatform.posDifference;
    }
    public static void SavePlayer()
    {
        SaveSystem.SavePlayer(_Instance);
    }
    public static void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data != null)
        {
            _Instance.transform.position = new Vector3(data._PlayerCheckPointPosition[0], data._PlayerCheckPointPosition[1], data._PlayerCheckPointPosition[2]);
        }
    }
}
