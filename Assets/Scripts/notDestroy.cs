using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class notDestroy : MonoBehaviour
{



    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

    }
    
}
