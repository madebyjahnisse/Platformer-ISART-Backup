using UnityEngine;

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

    //Ground Detection :
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
            if (Mathf.Abs(_Velocity.y) < _MaxSpeedVertical) _Velocity += -Vector3.up * _GravityForce * Time.deltaTime; //to fall down
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

    private void Move()
    {
        transform.position += _Velocity * Time.deltaTime * TimeManager.TimeValue;
    }


}
