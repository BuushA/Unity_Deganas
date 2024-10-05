using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOne : MonoBehaviour
{
    // Start is called before the first frame update
    int stop;
    void Start()
    {
        stop = 0;

    }

    // Update is called once per frame
    void Update()
    {

    
     if(stop % 2 != 0)
     {
     MonoBehaviour.print("WORKS");
     }


     stop++;
    }
}
