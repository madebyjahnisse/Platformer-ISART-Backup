using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Tyrolienne : MonoBehaviour
{
    [SerializeField] public List<Transform> _PointList = new List<Transform>();
    [SerializeField] public List<Collider2D> _ColliderList = new List<Collider2D>();

    [SerializeField] private float _Speed = 5f;

    private Player _PlayerTransform;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_PlayerTransform != null)
        {
            _PlayerTransform.transform.position += Vector3.forward * _Speed * Time.deltaTime; ;
        }
    }

    void OnTruggerEnter2D(Collision2D collision)
    {
        Debug.Log("touche");
        _PlayerTransform = collision.gameObject.GetComponent<Player>();
    }

}
