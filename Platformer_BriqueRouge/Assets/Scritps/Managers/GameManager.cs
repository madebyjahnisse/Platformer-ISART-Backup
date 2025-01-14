using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : MonoBehaviour
{
    //Test Checkpoints
    private bool _Respawn = false;
    [SerializeField] private GameObject _Player;
    [SerializeField] private GameObject _Spawn;
    void Start()
    {
        CheckPoints.CheckPointList.Add(_Spawn.transform.position);
    }

    private void Update()
    {
        Respawn();
    }
    private void Respawn()
    {
        _Respawn = Input.GetKey(KeyCode.X);
        if (_Respawn)
        {
           _Player.transform.position = CheckPoints.CheckPointList[CheckPoints.CheckPointList.Count - 1];
        }
    }
}
