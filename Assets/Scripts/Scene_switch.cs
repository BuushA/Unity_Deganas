using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;


public class Main_Scene_manager : MonoBehaviour
{
    public static Main_Scene_manager reference;

    //dependencies / scripts
    [SerializeField] Global_values GB_script;
    [SerializeField] customer_script Customers;
    private LabelMan Label_Manager;

    //UI objects
    [SerializeField] GameObject Work_UI;
    [SerializeField] GameObject Managment_UI;
    [SerializeField] GameObject Managment_Internal_UI;
    [SerializeField] GameObject Grid_Map_UI;
    [SerializeField] TMP_Text button_label;
    [SerializeField] GameObject Operations;



    bool hasStation = false;

    void Awake()
    {
            //Init the start
            Work_UI.SetActive(false);
            Managment_UI.SetActive(true);
            reference = this;
    }

    //init dependencies
    void Start()
    {
        Label_Manager = LabelMan.reference;
        Label_Manager.update_money_label(1);
        Label_Manager.update_time_label(1);
        MonoBehaviour.print("ha");
        Operations.SetActive(false);
    }


    bool active_message = false;
    //Coroutine for delaying and showing a message
    IEnumerator label_message(float delay, string msg)
    {
        string tmp = button_label.text;
        button_label.text = msg;
        yield return new WaitForSeconds(delay);
        button_label.text = tmp;
        active_message = false;
    }

    public void switch_to_scene()
    {
        if(GB_script.Dic_item_amount.Count == 0)
        {
            active_message = true;
            StartCoroutine(label_message(1.5f, "Stock UP!!!"));
        }
        //has items bought
        else
        {
            //change the time
            int Starting_h = 8; //work day starts at 8
            int Current_h = Global_values.time % 24; 
            int time_difference = (Current_h - Starting_h);
            //Calculate  8:00 clock again;
            Global_values.time += 24 - time_difference;

            //change the UI
            Work_UI.SetActive(true);
            Managment_UI.SetActive(false);
            //update labels
            Label_Manager.update_money_label(2);
            Label_Manager.update_time_label(2);

            //Init in customer_script.cs
            Customers.Scene_init();
        }
    }

    //
    public void Revert_scenes() 
    {   
        //change the UI
        Managment_UI.SetActive(true);
        Work_UI.SetActive(false);
        //update labels
        Label_Manager.update_money_label(1);
        Label_Manager.update_time_label(1);
        //Panel labels
        Panel_functions[] Panel = GameObject.FindObjectsOfType<Panel_functions>(true);
        foreach(Panel_functions pan in Panel)
        {
            pan.update_labels();
        }

    }

    //Button events
    //Swaps scene visibility
    public void Map()
    {
        Grid_Map_UI.SetActive(true);
        Managment_Internal_UI.SetActive(false);

    }

    public void Managment()
    {
        Grid_Map_UI.SetActive(false);
        Managment_Internal_UI.SetActive(true);
        //deletes spawned children from Grid_Map
        Region_grid Grid = Grid_Map_UI.GetComponent<Region_grid>();

        //activate gameplay when you buy your first gas station
        if(hasStation == false)
        {
            MonoBehaviour.print(Grid.N_stations);
            if(Grid.N_stations > 0)
            {
                MonoBehaviour.print("Activated");
                hasStation = true;
                Operations.SetActive(true);
            }
        }
        Grid.Clean();
    }


}
