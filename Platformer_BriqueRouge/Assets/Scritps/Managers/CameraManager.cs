using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] public bool _IsFollowPlayer;

    [SerializeField] public Transform target;
    [SerializeField] private float _FollowSpeed = 2f;
    [SerializeField] private float _MaxDistance = 20f;
    [SerializeField] private float _YOffSet = 5f;
    [SerializeField] private float _ZOffSet = -10f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(target!=null && _IsFollowPlayer) FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector3 lnewPos = new Vector3(target.position.x, target.position.y+_YOffSet, _ZOffSet);
        float lFollowRatio =  Mathf.Abs(lnewPos.x-transform.position.x);
        transform.position = Vector3.Slerp(transform.position, lnewPos, _FollowSpeed*(1+lFollowRatio/_MaxDistance)*Time.deltaTime);
    }
}
