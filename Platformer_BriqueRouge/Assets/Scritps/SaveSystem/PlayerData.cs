using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerData 
{
   public string _PlayerName;
   public string _PlayerPassword;
   public int _PlayerLevel;
   public int _PlayerHealth;
   public int _PlayerScore;
   public float[] _PlayerCheckPointPosition;

    public PlayerData(Player player)
    {

        //Sauvegarde de la position du joueur
        _PlayerCheckPointPosition = new float[3];
        _PlayerCheckPointPosition[0] = CheckPoints.CheckPointList[CheckPoints.CheckPointList.Count - 1].x;
        _PlayerCheckPointPosition[1] = CheckPoints.CheckPointList[CheckPoints.CheckPointList.Count - 1].y;
        _PlayerCheckPointPosition[2] = CheckPoints.CheckPointList[CheckPoints.CheckPointList.Count - 1].z;
    }
}
