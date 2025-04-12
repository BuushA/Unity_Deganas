using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Reflection;

public class Region_grid : MonoBehaviour
{
    [SerializeField] customer_script Customers;
    private LabelMan LabelManager;
    [SerializeField] GameObject Tile;
    [SerializeField] GameObject House;
    [SerializeField] GameObject Station;

    [SerializeField] GameObject customerCard;
    [SerializeField] private TMP_Text Station_label;
    private TMP_Text LabCustomer;

    public int[,] Owned_land = new int[15, 15];
    struct ST //store coordinates of stations
    {
        public int x;
        public int y;
    }
    const int stations_max = 25;
    public int N_stations = 0;
    public int[] Stations_x = new int[stations_max];
    public int[] Stations_y = new int[stations_max];

    [SerializeField] int increment_price = 1000;

    void Start()
    {
        LabCustomer = customerCard.GetComponent<TMP_Text>();
        customerCard.SetActive(false);
        LabelManager = LabelMan.reference;
        
    }

    public void Generate_map()
    {
        Customers.Create_customer_grid(this, Tile, House, Station, Owned_land);
        Station_cost();
    }



    public void Clean()
    {
        foreach(Transform child in this.gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        Station_label.text = "";
    }


    public void Show_tooltip(string information)
    {
        LabCustomer.text = information;
        customerCard.SetActive(true);
    }

    public void Hide_tooltip()
    {
        customerCard.SetActive(false);
    }

    public void Buy_Deganas(int x, int y)
    {
        //Land tile script activate the function on click
        //it sends approximated coordinates
        //check if there is enough money;
        if(Global_values.money < Global_values.Station_price)
            return;
        
        Global_values.money -= Global_values.Station_price;
        GameLog.Message(MethodBase.GetCurrentMethod().Name + " : Price=" + $"{Global_values.Station_price} =>" + $" {Global_values.Station_price * increment_price}");
         GameLog.Message(MethodBase.GetCurrentMethod().Name + " : Coordinates from Land_buy=" + $"{x}, " + $" {y}");
        Global_values.Station_price *= increment_price;
        LabelManager.update_money_label(1);
        Station_cost();
        //updates the tile
        GameObject tile = Instantiate(Station, this.transform, true) as GameObject;
        tile.transform.position = new UnityEngine.Vector2(this.transform.position.x + x * 24, this.transform.position.y + y * 24);
        tile.transform.SetSiblingIndex(0);
        //add its position to land taken
        Owned_land[x, y] = 1;
        Stations_x[N_stations] = x; 
        Stations_y[N_stations] = customer_script.y_max-1 - y; //it is in reverse here
        N_stations++;
    }

    private void Station_cost()
    {
        Station_label.text = "Station: costs\n" + LabelManager.Format_number(Global_values.Station_price) + " $";
    }
}
