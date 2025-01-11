using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _AccelerationSpeed = 10f;
    [SerializeField] private float _DeccelerationSpeed = 8f;
    [SerializeField] private float _MaxSpeedHorizontal = 100f;

    private Vector3 _Acceleration = Vector3.zero;
    private Vector3 _Velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        Deccelerate();
        Accelerate();
        MoveHorizontally();
    }

    private void Inputs ()
    {
        _Acceleration = new Vector3(Input.GetAxisRaw("Horizontal")*_AccelerationSpeed,0,0);
        
    }

    private void Accelerate()
    {
        if(Mathf.Abs(_Velocity.x)<_MaxSpeedHorizontal) _Velocity += _Acceleration *Time.deltaTime;        
    }

    private void Deccelerate()
    {
        if (_Velocity.x != 0 && Input.GetAxisRaw("Horizontal")==0)
        {
            float lSpeedRatio = Mathf.Abs(_Velocity.x) / _MaxSpeedHorizontal;
            if (_Velocity.x > 0) _Velocity += new Vector3(-_DeccelerationSpeed * (1 + lSpeedRatio), 0, 0)  * Time.deltaTime; //* (1 + lSpeedRatio) -> faster you go, faster you slow deccelerate
            else _Velocity+= new Vector3(_DeccelerationSpeed * (1 + lSpeedRatio), 0, 0) * Time.deltaTime;
        }       
    }

    private void MoveHorizontally()
    {
        transform.position += _Velocity * Time.deltaTime;
    }
}
