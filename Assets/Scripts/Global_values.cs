using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Global_values : MonoBehaviour
{
    public static long money;
    public static int time;
    public static int turns = 0;
    public static readonly int TurnReset = 7;
    public readonly static int WorkingCharges = 4;
    public static int ServeCustomer; //Get diminished overtime
    public bool[] TurnCompleted = { false };
    public static long stockAmount;
    [SerializeField] private long startingStock = 100;
    public const long Starting_station_price = 1000;
    public static long Station_price;
    //pass the reference maby use a pointer (?)
    public static Global_values reference;


    [SerializeField] private long starting_cash;
    [SerializeField] private int starting_hour; //has to be [0;24] otherwise it's illogical


    //Names of all the items
    public static List<string> Items = new List<string>();
    public static string testVAR = "A";
    //dictonary of all item prices per 1 unit
    public Dictionary<string, long> Dic_item_price = new Dictionary<string, long>();
    //Stores selling prices
    public Dictionary<string, int> Dic_item_sell = new Dictionary<string, int>();
    //dictionary of how many items are in possesion
    public Dictionary<string, long> Dic_item_amount = new Dictionary<string, long>();
    public Dictionary<string, long> Dic_ItemSold = new Dictionary<string, long>(); //Implement

    public static int StockQuartersOwned = 0;
    


    //Collecting Gameplay statistics
    public long AllProfit = 0;
    public long ShortTermProfit = 0;

    public static int localID;
    public long OpponentStock = 0;

    private NetworkServer netServer;


    [System.Serializable]
    public class ProductDescription
    {
        public string name;
        public int SellPrice;
        public long BuyPrice;
    }

    public enum Gamephase
    {
        Managment = 1,
        Work = 2
    }


    // Awake is called before the application starts
    //Load values FIRST
    void Awake()
    {
        money = starting_cash * 100;
        Station_price *= 100;
        //time counted by hours
        time = starting_hour;
        reference = this;
        stockAmount = startingStock;
        ServeCustomer = WorkingCharges;
        Station_price = Starting_station_price;
    }


    void Start()
    {
        string product_data = JSON_operations.Read_file("products.json");
        List<ProductDescription> Products = JSON_operations.From<ProductDescription>(product_data);
        //create a name list for ease of access
        foreach (var e in Products)
            Items.Add(e.name);
        create_price_dic(Products);
        create_sell_dic(Products);
        netServer = NetworkServer.reference;
        //adjust function to fit new name array

    }


    //Could create a template function to create Dics
    public void create_price_dic(List<ProductDescription> prod)
    {
        for (int i = 0; i < prod.Count; i++)
        {
            Dic_item_price.Add(prod[i].name, prod[i].BuyPrice);
        }
    }

    public void create_sell_dic(List<ProductDescription> prod)
    {
        for (int i = 0; i < prod.Count; i++)
        {
            Dic_item_sell.Add(prod[i].name, prod[i].SellPrice);
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
            Dic_item_amount.Add(item_name, amount);
        }
    }

    public void increase_stock(int mod)
    {
        stockAmount = startingStock * mod;
    }


    public void AddTurn()
    {
        turns += 1;
        netServer.UpdateTurnsRpc();
        //Scenemanager updates the labels
    }
}
