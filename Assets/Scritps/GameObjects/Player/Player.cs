using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public static Player GetInstance()
    {
        return Instance;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
