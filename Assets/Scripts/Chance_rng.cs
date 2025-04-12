using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class Chance_rng : MonoBehaviour
{
    System.Random rand = new();

    public static Chance_rng reference;
    void Awake()
    {
        reference = this;
    }


    public void strPlace_weights(int Total, Dictionary<string, int> T, out string Selected)
    {
        string result = "";
        int RTotal = rand.Next(0, Total-1);
        int s = 0;
        foreach(var x in T)
        {
            s += x.Value;
            if(s > RTotal)
            {
                result = x.Key;
                break;
            }
        }
        Selected = result;
    }
}
