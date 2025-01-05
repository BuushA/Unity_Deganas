using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;

using TMPro;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Globalization;
using System.Security;
using System.Numerics;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;


public class customer_script : MonoBehaviour
{

    [SerializeField] private GameObject request_UI;
    [SerializeField] private Sell_options Button_options;
    private LabelMan Money_manager;
    private Global_values GB_script;
    private Main_Scene_manager Scene_manager;
    private TMP_Text request_label;
    public bool Stop = false;
    public long amount_req;
    [SerializeField] private int time_efficiency = 4; //4 hours
    private Chance_rng Chan;

    //For grid and score - competition
    private Region_grid Grid;

    //rng
    System.Random rand = new();

    //Data for Customers
    struct Customer {
        public int x;
        public int y;
        public int type1, type2, type3;
        public int item_id; //upgrade later to be multible items
        public long buy_amount; 
        public int score;
        public int visits;
    };

    [HideInInspector]
    public int Client_number = 0;
    //limits for generating Clients
    const int max_clients = 48;
    const int x_max = 8;
    const int y_max = 8;
    const int max_score = 100;
    //public string[] types = {"Young", "Alcoholic", "Spender", "Cheap", "Old", "Poor"}; //Set Values inside the inspector

    //physical condition or age
    public string[] type_1 = {"Young", "Old", "Kid"};
    //social position or occupation/job 
    public string[] type_2 = {"Trucker", "Poor", "Millionaire"};
    //Habit, characteristic
    public string[] type_3 = {"Alcoholic", "Cheap", "Spender"};


    private string item_name;
    [SerializeField] int gen = 5;

    //Weighted values;
    Dictionary<string, int> T = new Dictionary<string, int>();
    //RNG table, case sensitive
    Dictionary<string, int> Buy = new Dictionary<string, int>
    {
        {"10%", 30},
        {"20%", 20},
        {"25%", 15},
        {"40%", 6},
        {"50%", 5},
        {"60%", 4},
        {"75%", 3},
        {"80%", 2},
        {"100%", 1}
    };
    //Creates a list of customers with all the properties and product info;
    //Temporary values for now


    // Start is called before the first frame update
    void Start()
    {   
        request_label = request_UI.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();


        GB_script = Global_values.reference;
        Scene_manager = Main_Scene_manager.reference;
        Money_manager = LabelMan.reference;
        Button_options.InitGlobal();
        Chan = Chance_rng.reference;

        //start buying start after pressing the scene button
    }
    

    int person_max = max_clients;
    bool maxed;
    int n = 0;
    //Recreate an array
    //Add additional values if needed in the future
    Customer[] person_list = new Customer[101];
   public void Scene_init()
    {
        Stop = false;
        maxed = false;
        Generate_clients(gen);

        //make a copy of an array;
        person_max = Client_number;
        person_list = (Customer[]) Client_list.Clone();
        n = 0;
        Start_buying();
        
    }

    Customer[] Client_list = new Customer[max_clients];
    public void Generate_clients(int n)
    {
        int x_cord, y_cord = new();

        if(Client_number + n >= max_clients)
        {
             MonoBehaviour.print("Too many clients!");
             //Array index
            if(maxed == false)
            {
                for(int i = Client_number; i < max_clients; i++)
                {   
                    //add random coordinates to client
                        get_coordinates(out x_cord, out y_cord, max_clients - Client_number); //potentially add a chance
                        Client_list[i].x = x_cord;
                        Client_list[i].y = y_cord;
                    //Get the competitive score based on distance
                    //needs coordinates beforehand
                    Client_list[i].score = Get_Score(i);
                    //add random types to Client
                    int Tlen = type_1.Length; Client_list[i].type1 = rand.Next(0, Tlen);
                    
                    Tlen = type_2.Length; Client_list[i].type2 = rand.Next(0, Tlen);
                    
                    Tlen = type_3.Length; Client_list[i].type3 = rand.Next(0, Tlen);

                    //Make it compatible with multible items in the future
                    int ItemID = 0; long BuyAmount = 0;
                    //add random item and buy amount
                    Products(out ItemID, out BuyAmount);
                    Client_list[i].item_id = ItemID;
                    Client_list[i].buy_amount = BuyAmount;

                    //Get the competitive score based on distance
                    //The closer it is to max the more modifier % it adds to everything
                    }
                    maxed = true;
                    Client_number = max_clients;
            }
        }
        
        if (Client_number < max_clients) {
        //!!!!Implement CHANCE later on!!!!
        for(int i = Client_number; i < Client_number + n; i++)
        {
            
            //multiplies money; adds chance to sell options; lowers time taken in the store

            //add random coordinates to client
                get_coordinates(out x_cord, out y_cord, n); //potentially add a chance
                Client_list[i].x = x_cord;
                Client_list[i].y = y_cord;

            //Get the competitive score based on distance
            //needs coordinates beforehand
            Client_list[i].score = Get_Score(i);

            //add random types to Client
            int Tlen = type_1.Length; Client_list[i].type1 = rand.Next(0, Tlen);
            
            Tlen = type_2.Length; Client_list[i].type2 = rand.Next(0, Tlen);
            
            Tlen = type_3.Length; Client_list[i].type3 = rand.Next(0, Tlen);

            //Make it compatible with multible items in the future
            int ItemID = 0; long BuyAmount = 0;
            //add random item and buy amount
            Products(out ItemID, out BuyAmount);
            Client_list[i].item_id = ItemID;
            Client_list[i].buy_amount = BuyAmount;

            }
            //how many clients
            //Stop incrementing after max_clients
            Client_number += n;
        }

    }

    private void Products(out int It_id, out long amount)
    {
            //How many product are on sale
            int Dcount = GB_script.Dic_item_sell.Count;
            It_id = rand.Next(0, Dcount);

            
            //amount of products they will buy
            item_name = Global_values.Items[It_id];
            int max_amount = (int)Global_values.stockAmount / 2;
            MonoBehaviour.print($"{max_amount}" + " to buy" + $"{item_name}");

            //add a chance for amounts. Weights are place on percentage of max_buy amount {20%: 90, 100%: 10}
            int total = 0;
            string S;
            foreach(KeyValuePair<string, int> kvp in Buy) 
            {
                total += kvp.Value;
            }
            Chan.strPlace_weights(total, Buy, out S); //return a string of %
            //remove %
            if(S.Length == 3)
                S = S.Remove(2); 
            else S = S.Remove(3);
            //new max_amount
            max_amount = max_amount - (max_amount * Int32.Parse(S)/ 100);
            MonoBehaviour.print($"{max_amount}" + " to buy" + $"{item_name}");

            amount = rand.Next(1, max_amount);
            MonoBehaviour.print("BOUGHt" + $"{amount}");
    }

    private void get_coordinates(out int x_cord, out int y_cord, int n)
    {
        //repeats until found
        bool found = false;
        x_cord = 0;
        y_cord = 0;
        while(found == false)
        {
            int a = rand.Next(1, x_max-1);
            int b = rand.Next(1, y_max-1);
            bool no_match = true;
            for(int j = 0; j < Client_number + n; j++)
            {
                if(a == Client_list[j].x && b == Client_list[j].y)
                {
                    no_match = false;
                    break;
                }                 
            }

            if(no_match == true)
            {
                x_cord = a;
                y_cord = b;
                found = true;
            }
        }
    }

    //function might be restored later
    //if weights are needed
    //or other values assosiated with types
    private void get_Types()
    {
        return;
    }

    
    int CNo = new();
    public void Start_buying()
    {
        

        Customer person = new();
        long current_amount = new();
        //Client_debug();
        //MonoBehaviour.print(Client_number);

         if(person_max == 0)
        {
            Stop = true;
        }

            CNo = rand.Next(0, person_max);
            person = person_list[CNo];
            
            item_name = Global_values.Items[person.item_id];
            if(GB_script.Dic_item_amount.ContainsKey(item_name) == true)
                current_amount = GB_script.Dic_item_amount[item_name];
            else
            {
                current_amount = 0;
            }

            if(n == max_clients)
            {
                Stop = true;
            }
            n++;

        if(Stop == false)
        {

            string request = "Buying " + person.buy_amount + "\n" + item_name;
            Money_manager.to_label(request_label, request);

            
            //button_option handles the prices and other modifiers (types, score, etc.)
            //handles the button labels aswell
            Button_options.New_Customer(item_name, person.buy_amount, person.score, current_amount, CNo);

            //remove people who have already visited
            for(int i = CNo; i < person_max - 1; i++)
            {
                //swap
                person_list[i] = person_list[i+1];
                person_list[i+1] = person_list[i];
            }
            person_max--;

            //regenerate the customer values based on the amount of visits it has
            if(Client_list[CNo].visits % 2 == 0)
                ValueRegen(CNo);
            else
                Client_list[CNo].visits += 1;
        }
        

        else if(Stop == true)
        {
            Switch_to_managment();
            //add a pop up
            //switch scene
        }
    }
    //Prideti veliau laukima kliento, kad butu realistiskiau

    public void Switch_to_managment()
    {
        Scene_manager.Revert_scenes();
        MonoBehaviour.print("Over");
    }


    const int work_hours = 18;
    int _worked_hours = 0;
    public void Time_spent(int score)
    {
        //check if the time doesn't go over the limit
        //Unfair mechanic, change accuracy later
        int time_taken = (int)(time_efficiency - (1 - score / 100));
        _worked_hours += time_taken;
        Global_values.time += time_taken;
        Money_manager.update_time_label(2);

        if(work_hours - (_worked_hours + time_taken) <= 0)
            Stop = true;
    }

    //Both functions bellow add something to a customer
    //Call to penalize the player
    public void Penalty(int id, int minus)
    {
        MonoBehaviour.print("Customer score before: " + $"{Client_list[id].score}");
        if(Client_list[id].score > minus)
        Client_list[id].score -= minus;
        MonoBehaviour.print("Customer score AFTER: " + $"{Client_list[id].score}");
    }

    //Call to reward the player
    public void Grace(int id, int plus)
    {
        MonoBehaviour.print("Customer score before: " + $"{Client_list[id].score}");
        if(Client_list[id].score < 100-plus)
        Client_list[id].score += plus;
        MonoBehaviour.print("Customer score AFTER: " + $"{Client_list[id].score}");

    }


    private void ValueRegen(int id)
    {
        //MonoBehaviour.print("Visited - " + $"{Client_list[id].visits}");
        
        //refresh item and buy amount
        int ItemId = 0; long BuyAmount = 0;
        Products(out ItemId, out BuyAmount);
        Client_list[id].item_id = ItemId;
        Client_list[id].buy_amount = BuyAmount;
    }

    public int Get_Score(int ClientID)
    {
        if(Grid == null)
            return ClientID;

        // get the distance to the nearest station
        MonoBehaviour.print("SCore of client - " + $"{ClientID}");
        int biggest = 0;
        int Cl_x = Client_list[ClientID].x;
        int Cl_y = Client_list[ClientID].y;
        int distance = 0;
        for(int i = 0; i < Grid.N_stations; i++)
        {
            int S_x = Grid.Stations_x[i];
            int S_y = Grid.Stations_y[i];

            //MonoBehaviour.print("Measuring distance from: " + $"{Cl_x}, " + $"{Cl_y}"  + " to " + $"{S_x}, " + $"{S_y}");

            distance = Math.Abs(S_x - Cl_x) + Math.Abs(S_y - Cl_y);

            //MonoBehaviour.print("Distance is: " + $"{distance}");

            if(distance > biggest)
                biggest = distance;

        }
        //MonoBehaviour.print("smallest dist: " + $"{smallest}");
        //calculate the score based on the distance;
        //Score totals to up to 20% (if the grid is 8x8)
        //Make the calculation automatic and flexible 
        int score = (int)(biggest*2.5);
        return score;
    }


    public void Create_customer_grid(Region_grid reference, GameObject TilePrefab, GameObject HousePrefab, GameObject StationPrefab ,int[,] Owned_land)
    {
        Grid = reference;
        
        float grid_x = Grid.transform.position.x;
        float grid_y = Grid.transform.position.y;

        int[,] Land_Taken = new int[15, 15];

        Hover_display Tooltip;




        for(int i = 0; i < Client_number; i++)
        {

            int x = Client_list[i].x;
            int y = Client_list[i].y;
            Customer Cl = Client_list[i];
            
            if(x != null && y != null && Owned_land[x, y] != 1)
            {
                //spawn a house object
                GameObject grid = Instantiate(HousePrefab, Grid.transform, true) as GameObject;
                grid.transform.position = new UnityEngine.Vector2(grid_x + x * 24, grid_y + y * 24);
                //Activate Tooltip display for each person
                Tooltip = grid.GetComponent<Hover_display>();
                Tooltip.Init_data(Cl.item_id, Cl.buy_amount, Cl.score, Cl.type1, Cl.type2, Cl.type3, Grid);
                //track who has land in the grid
                Land_Taken[x, y] = 1;
            }
        }

        for(int i = 0; i < x_max; i++)
        {
            for(int j = 0; j < y_max; j++)
            {
                
                //avoid duplicates
                if(Land_Taken[i, j] != 1 && Owned_land[i, j] != 1){
                    GameObject grid = Instantiate(TilePrefab, Grid.transform, true) as GameObject;
                    grid.transform.position = new UnityEngine.Vector2(grid_x + i * 24, grid_y + j * 24);
                    grid.GetComponent<Land_buy>().getData(Grid);
            }
            else if( Owned_land[i, j] == 1)
            {
                GameObject grid = Instantiate(StationPrefab, Grid.transform, true) as GameObject;
                grid.transform.position = new UnityEngine.Vector2(grid_x + i * 24, grid_y + j * 24);
            }
            }
        }


    }
}
