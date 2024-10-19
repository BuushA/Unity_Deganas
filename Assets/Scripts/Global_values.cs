using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Global_values : MonoBehaviour
{
    public static long money;
    public static int time;

    
    [SerializeField] private long starting_cash;
    [SerializeField] private int starting_hour; //has to be [0;24] otherwise it's illogical
    [SerializeField] private float base_demand;

    //Names of all the items
    public static string[] Items = {"Gasolline", "Soap"};

    //dictonary of all item prices per 1 unit
    public Dictionary<string, long> Dic_item_price = new Dictionary<string, long>();
    //dictionary of how many items are in possesion
    public Dictionary<string, long> Dic_item_amount = new Dictionary<string, long>();
    //dictionary of total demand for the product
    public Dictionary<string, float> Dic_item_demand = new Dictionary<string, float>();


    // Awake is called before the application starts
    //Load values FIRST
    void Awake()
    {
            money = starting_cash*100;
            //time counted by hours
            time = starting_hour;  
    }


    public void create_price_dic(long[] unit_price)
    {
        int item_len = Items.Length;
        for(int i = 0; i < item_len; i++)
        {
            Dic_item_price.Add(Items[i], unit_price[i]);
        }
    }


    //every item will have the same demand at the start
    public void Base_Demand_dic(float base_demand)
    {
        int item_len = Items.Length;
        for(int i = 0; i < item_len; i++)
        {
            Dic_item_demand.Add(Items[i], base_demand);
        }
    }


    public void add_amount_to_dic(string item_name, long amount)
    {
        if(Dic_item_amount.ContainsKey(item_name) == false)
        {
            Dic_item_amount.Add(item_name, amount);
        }
        else //if key already exists
        {
            Dic_item_amount[item_name] += amount;
            
        }
        Debug.Log(Dic_item_amount[item_name]);
    }

}
