using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Global_values : MonoBehaviour
{
    public static float money;
    public static int hour;

    
    [SerializeField] private float starting_cash;
    [SerializeField] private int starting_hour; //has to be [0;24] otherwise it's illogical



    // Awake is called before the application starts
    //Load values FIRST
    void Awake()
    {
            money = starting_cash;
            hour = starting_hour;
    }
}
