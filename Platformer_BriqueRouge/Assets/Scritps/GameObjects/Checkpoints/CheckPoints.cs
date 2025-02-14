using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CheckPoints : MonoBehaviour
{
    
    public static List<Vector3> CheckPointList = new List<Vector3>();
    
    private GameObject playerToDestroy;
    private bool _CheckPointEnabled = false;
    
    void Start()
    {
        CheckPointList.Clear();
        
    }

    private void OnTriggerEnter2D(Collider2D pCollision)
    {
        if (pCollision.gameObject.CompareTag(Utils.PLAYER_TAG))
        {
            if (_CheckPointEnabled) return;
            CheckPointList.Add(pCollision.transform.position);
            _CheckPointEnabled = true;
            Player.SavePlayer();
        }
    }

}
