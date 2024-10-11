using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prod_Prices : MonoBehaviour
{

    //Naming: Object name; 
    //Object name + _price
    
    //add a static object
    public static float Gasoline;

    //add a configurable price for 1 unit
    [SerializeField] private float Gasoline_price;


    //Serialize before Start()
    void Awake()
    {
        //load at initialization
        Gasoline = Gasoline_price;





    }


}
