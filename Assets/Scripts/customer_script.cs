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
using System.Linq;
using System.Reflection;


public class customer_script : MonoBehaviour
{

    [SerializeField] private GameObject request_UI;
    [SerializeField] private Sell_options Button_options;
    [SerializeField] private GameObject ClientPrefab;
    [SerializeField] private GameObject placeholder;
    private customer_Panel client_Panel;
    private LabelMan Money_manager;
    private Global_values GB_script;
    private Main_Scene_manager Scene_manager;
    private TMP_Text request_label;
    private Upgrades upgrades;
    public bool Stop = false;
    public long amount_req;
    private int time_efficiency;
    private Chance_rng Chan;
    Upgrades.UP EfficiencyKey;

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
    private int Client_number = 0;
    //limits for generating Clients
    const int max_clients = 48;
    public const int x_max = 8;
    public const int y_max = 8;
    //Occupied tile slots
    bool[,] occupied = new bool[x_max+1, y_max+1];
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

    const int work_hours = 12;
    int _worked_hours = 0;

    [System.Serializable]
    public class Customer_types
    {
        public string name;
        public int bonus;
    }

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
        Chan = Chance_rng.reference;
        upgrades = Upgrades.reference;
        
        Button_options.InitGlobal();
        EfficiencyKey = upgrades.Dic_upgrades["Efficiency"];
        //Init data from a json file

        string type_data = JSON_operations.Read_file("types.json");
        List<Customer_types> tempor = JSON_operations.From<Customer_types>(type_data);
        GameLog.Message("JSON got: " + $"{tempor.Count}");
        //start buying start after pressing the scene button
    }
    

    int person_max = max_clients;
    bool maxed;
    int count = 0;
    //Recreate an array
    //Add additional values if needed in the future
    List<Customer> person_list = new List<Customer>(101);
    List<Customer> Client_list = new List<Customer>(48);

    void Log_client_gen(int id, Customer a)
    {
        GameLog.Message("GEN: " + "ID=" + $"{id}; " + "POS=" + $"{a.x}, " + $"{a.y}; " + "SCORE=" + $"{a.score}; " + "ITEM=" + Global_values.Items[a.item_id] 
        + "; " + "AMOUNT=" + $"{a.buy_amount}" + "TYPES: " + $"{type_1[a.type1]}, " +  $"{type_2[a.type2]}, " + $"{type_3[a.type3]}; ");
    }

   public void Scene_init()
    {
        int start_bonus = 0;
        Stop = false;
        maxed = false;
        
        if(Client_number == 0)
            start_bonus = 5;

        Generate_clients(gen + start_bonus);
        //make a copy of an array;
        person_max = Client_number;

        //unoptimized
        person_list = new List<Customer>(101);
        //person_list = (Customer[]) Client_list.Clone(); I hate c# for creating this. Shallow copy my ass
        for(int i = 0; i < Client_list.Count; i++)
        {
            person_list.Add( new Customer {x = Client_list[i].x, y = Client_list[i].y, type1 = Client_list[i].type1, type2 = Client_list[i].type2, type3 = Client_list[i].type3,
            item_id = Client_list[i].item_id, buy_amount = Client_list[i].buy_amount, score = Client_list[i].score, visits = Client_list[i].visits});
        }

        if(Client_list.Count != Client_number)
            GameLog.Error(MethodBase.GetCurrentMethod().Name + ": Wrong Client count");
        if(person_list.Count != Client_number)
            GameLog.Error(MethodBase.GetCurrentMethod().Name + ": Wrong copied Client count");

        count = 0;
        _worked_hours = 0;
        Start_buying();
    }


    public void Generate_clients(int n)
    {
        int x_cord, y_cord = new();
        int last_number = Client_number;
        //used for some functions which need a specific amount
        int gen_amount = new();

        //Choose the right parameters
        if(Client_number == max_clients)
            return;
        else if(Client_number + n >= max_clients)
        {
            Client_number = max_clients;
            gen_amount = max_clients - Client_number;
        }
        else
        {
            Client_number += n;
            gen_amount = n;
        }

        for(int i = last_number; i < Client_number; i++)
        {
            //multiplies money; adds chance to  sell options; lowers time taken in the store
            //add random coordinates to client
                get_coordinates(out x_cord, out y_cord, gen_amount); //potentially add a chance
                Customer a = new Customer();
                occupied[x_cord, y_cord] = true;

                a.x = x_cord; a.y = y_cord;

            //First element in a list must be added with a function
            Client_list.Add(a);
            //Get the competitive score based on distance
            //needs coordinates beforehand

            a.score = Get_Score(i);

            //add random types to Client
            int Tlen = type_1.Length; 
            a.type1 = rand.Next(0, Tlen);
            Tlen = type_2.Length; 
            a.type2 = rand.Next(0, Tlen);
            Tlen = type_3.Length; 
            a.type3 = rand.Next(0, Tlen);

            //Make it compatible with multible items in the future
            int ItemID = 0; long BuyAmount = 0;
            //add random item and buy amount
            Products(out ItemID, out BuyAmount);

            a.item_id = ItemID;
            a.buy_amount = BuyAmount;
            //update the current value with all the data
            Client_list[i] = a;
            //Log client Generation:
            Log_client_gen(i, Client_list[i]);
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

            amount = rand.Next(1, max_amount);
    }

    private void get_coordinates(out int x_cord, out int y_cord, int n)
    {
        //repeats until found

        x_cord = 0;
        y_cord = 0;
        int a = rand.Next(0, x_max-1);
        int b = rand.Next(0, y_max-1);
            
            for(int u = b; u < y_max; u++)
            {
                for(int z = a; z < x_max; z++)
                {
                    if(occupied[z, u] == false)
                    {
                        x_cord = z;
                        y_cord = u;
                        return;
                    }
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

         if(person_max == 0)
        {
            Stop = true;
        }

            CNo = rand.Next(0, person_max-1);
            person = person_list[CNo];
            
            item_name = Global_values.Items[person.item_id];
            if(GB_script.Dic_item_amount.ContainsKey(item_name) == true)
                current_amount = GB_script.Dic_item_amount[item_name];
            else
            {
                current_amount = 0;
            }

            if(count == max_clients)
            {
                Stop = true;
            }
            count++;


        if(Stop == false)
        {
            string request = "Buying " + person.buy_amount + "\n" + item_name;
            Money_manager.to_label(request_label, request);

            GameObject Cpanel = Instantiate(ClientPrefab, placeholder.transform) as GameObject;
            client_Panel = Cpanel.GetComponent<customer_Panel>();
            //Add type effects info here
            client_Panel.update_labels(person.score, type_1[person.type1], type_2[person.type2], type_3[person.type3], person.visits);
            //button_option handles the prices and other modifiers (types, score, etc.)
            //handles the button labels aswell
            
            Button_options.New_Customer(item_name, person.buy_amount, person.score, current_amount, CNo, client_Panel, Cpanel);
            if(Client_list[CNo].visits % 3 == 2)
                ValueRegen(CNo);
            else
            {
                //Hotfix might not be the best option
                Customer a = Client_list[CNo];
                a.visits += 1;
                Client_list[CNo] = a;
            }
                
        }
        

        else if(Stop == true)
        {
            Switch_to_managment();
            //add a pop up
            //switch scene
        }
    }

    //Prideti veliau laukima kliento, kad butu realistiskiau
    //Called at the end of buying
    //Prevents data races and other problems
    public void forget_customer()
    {
            Log_client_gen(CNo, person_list[CNo]);
            for(int i = CNo; i < person_max - 1; i++)
            {
                Customer next = person_list[i+1];
                Customer curr = person_list[i];
                person_list[i+1] = curr;
                person_list[i] = next;
            }
            GameLog.Message(MethodBase.GetCurrentMethod().Name + " AFTER");
            Log_client_gen(CNo, person_list[CNo]);
            person_max--;
    }

    public void Switch_to_managment()
    {
        Scene_manager.Revert_scenes();
    }


    public void Time_spent()
    {
        //check if the time doesn't go over the limit
        int time_taken = upgrades.Modifier(EfficiencyKey.method_id, EfficiencyKey.tier);
        _worked_hours += time_taken;
        Global_values.time += time_taken;
        Money_manager.update_time_label(2);

        if(work_hours <= (_worked_hours))
        {
            Money_manager.update_time_label(1);
            Stop = true;
        }
            
    }

    //Both functions bellow add something to a customer
    //Call to penalize the player
    public int Penalty(int id, int minus)
    {
        Customer a = Client_list[id];
        Customer b = person_list[id];

        if(Client_list[id].score > minus)
        {
            
            a.score -= minus;
            b.score -= minus;
        }
            
        else
        {
            a.score = 0;
            b.score = 0;
        }

        GameLog.Message(MethodBase.GetCurrentMethod().Name + " 1) " + $"{a.score}; " + "2) " + $"{b.score}");

        Client_list[id] = a;
        person_list[id] = b;

        return a.score;
    }

    //Call to reward the player
    public int Grace(int id, int plus)
    {
        Customer a = Client_list[id];
        Customer b = person_list[id];

        if(Client_list[id].score + plus >= 100)
        {
            a.score = 100;
            //update. Needed because the array get resized
            b.score = 100;
        }
        else
        {
            a.score += plus;
            b.score += plus;
        }

        GameLog.Message(MethodBase.GetCurrentMethod().Name + " 1) " + $"{a.score}; " + "2) " + $"{b.score}");

        Client_list[id] = a;
        person_list[id] = b;
    
        return a.score;
    }


    private void ValueRegen(int id)
    {
        //refresh item and buy amount
        int ItemId = 0; long BuyAmount = 0;
        Products(out ItemId, out BuyAmount);
        Customer a = Client_list[id];
        a.item_id = ItemId;
        a.buy_amount = BuyAmount;
        Client_list[id] = a;

        //Log customer updates
        GameLog.Message("Regenerating customer: ");
        Log_client_gen(id, Client_list[id]);
    }


    public int Get_Score(int ClientID)
    {
        if(Grid == null)
            return ClientID;

        // get the distance to the nearest station

        int biggest = 0;
        int Cl_x = Client_list[ClientID].x;
        int Cl_y = y_max-1 - Client_list[ClientID].y; //reverse it
        int distance = 0;
        for(int i = 0; i < Grid.N_stations; i++)
        {
            int S_x = Grid.Stations_x[i];
            int S_y = Grid.Stations_y[i];

            distance = Math.Abs(S_x - Cl_x) + Math.Abs(S_y - Cl_y);

            if(distance > biggest)
                biggest = distance;

        }
        
        //calculate the score based on the distance;
        //Make the calculation automatic and flexible 
        int score = (int)(biggest*2);
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
