using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Netcode;
using TMPro;


public class Main_Scene_Manager : MonoBehaviour
{
    public static Main_Scene_Manager reference;

    //dependencies / scripts
    [SerializeField] Global_values GB_script;
    [SerializeField] customer_script Customers;
    private LabelMan Label_Manager;
    private Upgrades upgrades;
    [SerializeField] OverviewFunction StockFunction;

    //UI objects
    [SerializeField] GameObject Work_UI;
    [SerializeField] GameObject Managment_UI;
    [SerializeField] GameObject Managment_Internal_UI;
    [SerializeField] GameObject Grid_Map_UI;
    [SerializeField] GameObject Overview;
    [SerializeField] TMP_Text button_label;
    [SerializeField] GameObject Operations;

    //other
    bool hasStation = false;
    Upgrades.UP TimeKey;
    bool Selling = false;

    NetworkServer netServer;

    void Awake()
    {
        reference = this;
    }

    //init dependencies
    void Start()
    {
        //Init the start
        Work_UI.SetActive(false);
        Overview.SetActive(false);
        Managment_UI.SetActive(false);
        Label_Manager = LabelMan.reference;
        Label_Manager.update_money_label((int)Global_values.Gamephase.Managment);
        Label_Manager.update_time_label((int)Global_values.Gamephase.Managment);
        Label_Manager.update_turn_label((int)Global_values.Gamephase.Managment);


        Operations.SetActive(false);
        upgrades = Upgrades.reference;
        netServer = NetworkServer.reference;
        StockFunction = OverviewFunction.reference;
        TimeKey = upgrades.Dic_upgrades["Time"];
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
        //Consider adding log here
        if (GB_script.Dic_item_amount.Count == 0)
        {
            active_message = true;
            StartCoroutine(label_message(1.5f, "Stock UP!!!"));
        }
//        else if(netServer.canStart == false)
//        {
//            active_message = true;
//
//            int player_id = Global_values.localID;
//            netServer.requestJoinedToServer(player_id);
//            string text = Label_Manager.LabelReadyCount();
//            StartCoroutine(label_message(1f, text));
//        }
        //has items bought
        else
        {
            //    //change the time to opening hours
            //    int current_time = Global_values.time % 24;
            //    int add_time = 0;
            //    int opening = upgrades.Modifier(TimeKey.method_id, TimeKey.tier);
            //    if (current_time > opening)
            //    {
            //        add_time = 24 + opening - current_time;
            //    }
            //    else if (current_time < opening)
            //    {
            //        add_time = opening - current_time;
            //    }
            //    Global_values.time += add_time;

            active_message = true;

            int player_id = Global_values.localID;
            netServer.requestJoinedToServer(player_id, (int)NetworkServer.Scenes.Work);
            string text = Label_Manager.LabelReadyCount();
            StartCoroutine(label_message(1f, text));
        }
    }

    //Server calls from SendConfirmationRpc
    public void activateWork()
    {
        //change the UI
        Work_UI.SetActive(true);
        Managment_UI.SetActive(false);
        Overview.SetActive(false);
        //update labels
        Label_Manager.update_money_label((int)Global_values.Gamephase.Work);
        Label_Manager.update_time_label((int)Global_values.Gamephase.Work);
        GB_script.AddTurn();
        Label_Manager.update_turn_label((int)Global_values.Gamephase.Work);
        OpenOverview(); //Will open on a specific turn

        //Init in customer_script.cs
        if (Selling == false)
        {
            Customers.Scene_init();
            Selling = true; //Implemented for reusability in Closing Overveiw
        }
    }

    //
    public void Revert_scenes()
    {
        //change the UI
        Managment_UI.SetActive(true);
        Work_UI.SetActive(false);
        Selling = false;
        //update labels
        Label_Manager.update_money_label((int)Global_values.Gamephase.Managment);
        Label_Manager.update_time_label((int)Global_values.Gamephase.Managment);
        Label_Manager.update_turn_label((int)Global_values.Gamephase.Managment);
        //Turns are added only after starting a working session
        //Panel labels
        Label_Manager.update_Panels();
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
        if (hasStation == false)
        {
            if (Grid.N_stations > 0)
            {
                hasStation = true;
                Operations.SetActive(true);
            }
        }
        Grid.Clean();
    }


    public void OpenOverview()
    {
        if (Global_values.turns > 0 && (Global_values.turns % Global_values.TurnReset) == 0)
        {
            Overview.SetActive(true);
            Work_UI.SetActive(false);
            StockFunction.SceneInit();
        }
        else if (Global_values.turns > 1 && (Global_values.turns % Global_values.TurnReset) == 1)
        {
            GB_script.ShortTermProfit = 0;
        }
    }

}
