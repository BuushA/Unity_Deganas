using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;

using TMPro;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;


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
    private int time_efficiency = 4; //4 hours

    //rng
    System.Random rand = new();

    //Creates a list of customers with all the properties and product info;

    //Temporary values for now
    public string item_name;


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

    public void Scene_init()
    {
        Stop = false;
        Start_buying();
    }
 
    public void Start_buying()
    {
        //you have nothing left
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

        }

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
