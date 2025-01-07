using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Global_values : MonoBehaviour
{
    public static long money;
    public static int time;
    public static long stockAmount;
    [SerializeField] private long startingStock = 1000;
    public static long Station_price = 1000;
    //pass the reference maby use a pointer (?)
    public static Global_values reference;

    
    [SerializeField] private long starting_cash;
    [SerializeField] private int starting_hour; //has to be [0;24] otherwise it's illogical
    

    //Names of all the items
    public static string[] Items = {"Gasolline", "Snacks"};

    //dictonary of all item prices per 1 unit
    public Dictionary<string, long> Dic_item_price = new Dictionary<string, long>();
    //Stores selling prices
    public Dictionary<string, int> Dic_item_sell = new Dictionary<string, int>();
    //dictionary of how many items are in possesion
    public Dictionary<string, long> Dic_item_amount = new Dictionary<string, long>();


    // Awake is called before the application starts
    //Load values FIRST
    void Awake()
    {
            money = starting_cash*100;
            Station_price *= 100;
            //time counted by hours
            time = starting_hour; 
            reference = this;
            stockAmount = startingStock;
    }



    //Could create a template function to create Dics
    public void create_price_dic(long[] unit_price)
    {
        int item_len = Items.Length;
        for(int i = 0; i < item_len; i++)
        {
            Dic_item_price.Add(Items[i], unit_price[i]);
        }
    }

    public void create_sell_dic(int[] unit_price)
    {
        int item_len = Items.Length;
        for(int i = 0; i < item_len; i++)
        {
            Dic_item_sell.Add(Items[i], unit_price[i]);
        }
    }


    public void add_amount_to_dic(string item_name, long amount)
    {   
        try
        {
            Dic_item_amount[item_name] += amount;
        }
        catch (KeyNotFoundException e)
        {
            MonoBehaviour.print("Adding key");
            Dic_item_amount.Add(item_name, amount);
        }
    }

    public void increase_stock(int mod)
    {
        stockAmount = startingStock * mod;
    }

}
