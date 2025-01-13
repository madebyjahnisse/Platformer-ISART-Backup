using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tyrolienne : MonoBehaviour
{
    [SerializeField] public List<Transform> _PointList = new List<Transform>();
    [SerializeField] public List<Collider2D> _ColliderList = new List<Collider2D>();

    [SerializeField] private float _Speed = 5f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ConnectPlayer()
    {
        foreach (Collider2D lCollider in _ColliderList)
        {
            //lCollider.OverlapCollider()
        }
    }

}
