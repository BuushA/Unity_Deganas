using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global_values : MonoBehaviour
{

    [SerializeField]
    public static float money;
    [SerializeField]
    public static int hour;



    // Awake is called before the application starts
    //Load values FIRST
    void Awake()
    {
        money = 1000f;
        hour = 8;

    }
}
