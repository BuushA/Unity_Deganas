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

    //Global_values reference
    [SerializeField] Global_values GB_script;





    //Serialize before Start()
    void Awake()
    {
        //load at initialization
        Gasoline = Gasoline_price;

        //load prices into an array
        float[] unit_price = {Gasoline_price, 10f};


        //Initialize the dictionary inside Global_values script
        GB_script.create_price_dic(unit_price);
    }



}
