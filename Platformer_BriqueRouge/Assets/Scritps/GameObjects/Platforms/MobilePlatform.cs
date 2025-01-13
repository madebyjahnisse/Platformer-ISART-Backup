using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilePlatform : MonoBehaviour
{
    [SerializeField] public float _Speed = 10f;
    [SerializeField] private List<Transform> _TransformList = new List<Transform>();
    private Transform _ActualTarget;
    private Vector3 _LastPos = Vector3.zero;
    private Vector3 _NewPos = Vector3.zero;
    public Vector3 posDifference = Vector3.zero;
    private int _ActualIndice = 0;
    private bool _IsAller = true;

    // Start is called before the first frame update
    void Start()
    {
        _ActualTarget = _TransformList[0];
        transform.position = _ActualTarget.position;
        _ActualIndice = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _LastPos = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, _ActualTarget.position, _Speed * Time.deltaTime);
        _NewPos = transform.position;
        posDifference = _NewPos - _LastPos;
        if ((Vector3.Distance(transform.position, _ActualTarget.position) < 0.05f))
        {
            ActualiseTarget();
        }
    }

    private void ActualiseTarget()
    {
        if (_IsAller)
        {
            _ActualIndice++;
            if (_ActualIndice >= _TransformList.Count)
            {
                _IsAller = false;
                _ActualIndice--;
            }
        }
        else
        {
            _ActualIndice--;
            if (_ActualIndice < 0)
            {
                _IsAller = true;
                _ActualIndice++;
            }
        }
        _ActualTarget = _TransformList[_ActualIndice];
    }
}
