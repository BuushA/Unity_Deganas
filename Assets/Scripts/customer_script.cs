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
    };

    [HideInInspector]
    public int Client_number = 0;
    //limits for generating Clients
    const int max_clients = 100;
    const int x_max = 20;
    const int y_max = 20;
    const int max_score = 100;
    public string[] types = {"Poor", "Alcoholic", "Spender"};
    private string item_name;
    [SerializeField] int gen = 5;
     

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

        //start buying start after pressing the scene button
    }
    
    Customer[] person_list = new Customer[max_clients];
    int person_max = max_clients;
    public void Scene_init()
    {
        Stop = false;
        Generate_clients(gen);

        //make a copy of an array;
        person_list = Client_list;
        person_max = Client_number;
        Start_buying();
    }

    Customer[] Client_list = new Customer[max_clients];
    public void Generate_clients(int n)
    {
        if(Client_number + n >= max_clients)
            MonoBehaviour.print("Too many clients!");

        else {
        //!!!!Implement CHANCE later on!!!!
        for(int i = Client_number; i < Client_number + n; i++)
        {   
            //add random coordinates to client
                int x_cord, y_cord = new();
                get_coordinates(out x_cord, out y_cord, n);
                Client_list[i].x = x_cord;
                Client_list[i].y = y_cord;

            //add random types to Client
            int[] T = new int[3];
            get_Types(T);
            Client_list[i].type1 = T[0];
            Client_list[i].type2 = T[1];
            Client_list[i].type3 = T[2];

            //Make it compatible with multible items in the future
            //add what product they will buy
            int Dcount = GB_script.Dic_item_amount.Count;
            int It_id = rand.Next(0, Dcount);
            Client_list[i].item_id = It_id;
            
            //amount of products they will buy
            item_name = Global_values.Items[It_id];
            int max_amount = (int)GB_script.Dic_item_amount[item_name];
            Client_list[i].buy_amount = rand.Next(1, max_amount);

            //Get the competitive score based on distance
            //The closer it is to max the more modifier % it adds to everything
            }
        }
        //how many clients
        Client_number += n;
    }

    private void get_coordinates(out int x_cord, out int y_cord, int n)
    {
        //repeats until found
        bool found = false;
        x_cord = 0;
        y_cord = 0;
        while(found == false)
        {
            int a = rand.Next(0, x_max);
            int b = rand.Next(0, y_max);
            for(int j = 0; j < Client_number+n; j++)
            {
                if(a != Client_list[j].x || b != Client_list[j].y)
                {
                    x_cord = a;
                    y_cord = b;
                    found = true;
                    break;
                }                 
            }
        }
    }

    private void get_Types(int[] T)
    {
        int type_n = types.Length;
        //check for repeats
        bool found = false;

        while (found == false)
        {
            int type = rand.Next(0, type_n);
            //assume numbers [0, 1, 1, 2]
            //each while loop will do: 1) add 0; 2) j + 1; add 1 (0, 1) and reset 3) j + 1; break; 4) j + 1; j + 1; add 2 (0, 1, 2)
            for (int j = 0; j < type_n; j++)
            {
                if (T[j] == type)
                    break;

                else if (T[j] == null)
                {
                    T[j] = type;
                    break;
                }

                else if (T[j] != type)
                {
                    /*
                    bool change = true;
                    if (j + 1 == max_clients)
                    {
                        for (int s = 0; s < max_clients; s++)
                        {
                            if (T[s] != type)
                            {
                                change = false;
                                break;
                            }
                        }
                    }

                    else
                    {
                        for (int s = j + 1; s < max_clients; s++)
                        {
                            if (T[s] != type)
                            {
                                change = false;
                                break;
                            }
                        }
                    }

                    if (change == true)
                    {
                        T[j] = type;
                        break;
                    }
                    */
                    continue;
                }
            }

            if (T.Length == 3)
                found = true;
        }
    }


    private void Client_debug()
    {
        MonoBehaviour.print("LIST OF CUSTOMERS: \n");
        int i = 0;
        foreach(Customer person in Client_list)
        {
            MonoBehaviour.print(i);
            MonoBehaviour.print(") " + Global_values.Items[person.item_id] + " has: ");
            MonoBehaviour.print(person.buy_amount);
            i++;
        }
    }
    
    int CNo = new();
    public void Start_buying()
    {
        bool searching = true;
        int n = 0;
        Customer person = new();
        //Client_debug();
        //MonoBehaviour.print(Client_number);
        

        
        while(searching == true)
        {
            bool new_person = true;
            CNo = rand.Next(0, person_max-1);
            person = person_list[CNo];
            
            //check if the person already visited
            
            item_name = Global_values.Items[person.item_id];
            long current_amount = GB_script.Dic_item_amount[item_name];


            if( (current_amount >= person.buy_amount && person.buy_amount > 0))
                searching = false;

            if(n == max_clients)
            {
                Stop = true;
                break;
            }
            n++;
        }

        if(Stop == false)
        {
            string request = "Buying " + person.buy_amount.ToString() + " " + item_name;
            Money_manager.to_label(request_label, request);

            
            
            

            //button_option handles the prices and other modifiers (types, score, etc.)
            Button_options.New_Customer(item_name, person.buy_amount);

            //Make it Regenerate item, amount 
            Client_list[CNo].buy_amount = -1; //TEMPORARY pre-emptively subtract the amount

            //remove people who have already visited
            for(int i = CNo; i < person_max - 1; i++)
            {
                //swap
                person_list[i] = person_list[i+1];
                person_list[i+1] = person_list[i];
            }
            person_max--;
        MonoBehaviour.print("NEW LIST: ");
        for(int i = 0; i < person_max; i++)
            MonoBehaviour.print(person_list[i].buy_amount);
        }

        

        //you have nothing left
        /*
        int Dcount = GB_script.Dic_item_amount.Count;
        if(Dcount == 0)
        {
            Stop = true;
        }
            
        if(Stop == false)
        {
        request_UI.SetActive(true);

        int item_req = new int();
        if(Dcount > 1)
            item_req = rand.Next(Dcount);
        else
            item_req = 0;
        
        item_name = Global_values.Items[item_req];
        int item_amount = (int)GB_script.Dic_item_amount[item_name]; //should be long

        if(item_amount <= 0)
            amount_req = 1;
        else
            amount_req = rand.Next(1, (item_amount));

        string request = "Buying " + amount_req.ToString() + " " + item_name;
        request_label.text = request;

        Button_options.New_Customer(item_name);
        */

        else if(Stop == true)
        {
            Scene_manager.Revert_scenes();
            MonoBehaviour.print("Over");
            //add a pop up
            //switch scene
        }
    }
    //Prideti veliau laukima kliento, kad butu realistiskiau



    const int work_hours = 16;
    int _worked_hours = 0;
    public void Time_spent()
    {
        //check if the time doesn't go over the limit
        _worked_hours += time_efficiency;
        Global_values.time += time_efficiency;
        Money_manager.update_time_label(2);

        if(work_hours - (_worked_hours + time_efficiency) <= 0)
            Stop = true;
    }
}
