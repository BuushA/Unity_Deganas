using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics.Contracts;
using System;
using System.Runtime.Serialization;
using Microsoft.Win32.SafeHandles;
using System.Linq;

public class Sell_options : MonoBehaviour
{

    //script dependencies
    [SerializeField] customer_script Customers;
    private Global_values GB_script;
    private Upgrades upgrade;
    private LabelMan Money_manager;
    private customer_Panel Panel_script;




    //Text labels
    private TMP_Text Increase;
    private TMP_Text Neutral;
    private TMP_Text Lower;



    private long price;
    private long sell_amount;
    private string item_name;
    private int score;
    private long current_amount;
    private int customer_id;

    [SerializeField] int minus_score = 5;
    [SerializeField] int plus_score = 3;

    bool Quit = new();
    private GameObject Panel;
    [SerializeField] private GameObject[] Sell_buttons = new GameObject[3];



    void Awake()
    {
        //get children refrences
        TMP_Text[] Children = new TMP_Text[3];
        Children = GetComponentsInChildren<TMP_Text>();
        Increase = Children[0];
        Neutral = Children[1];
        Lower = Children[2];
    }

    //get global_values reference

    public void InitGlobal()
    {
        GB_script = Global_values.reference;
        Money_manager = LabelMan.reference;
        upgrade = Upgrades.reference;
    }

    public void New_Customer(string name_ref, long amount_req, int SC, long curr, int id, customer_Panel script, GameObject reference)
    {
        //init labels;
        item_name = name_ref;
        sell_amount = amount_req;
        score = SC;
        current_amount = curr;
        Quit = false;
        customer_id = id;
        Panel_script = script;
        Panel = reference;

        string upgName = "Quality";
        int quality_mod = upgrade.Modifier(upgrade.Dic_upgrades[upgName].method_id, upgrade.Dic_upgrades[upgName].tier);
        
        //If there is nothing
        if(current_amount == 0)
        {
            Quit = true;
            Neutral.text = "Can't find " + item_name;
            Increase.text = "";
            Lower.text = "";
        }
        else if(current_amount < amount_req)
        {   
            
            Quit = true;
            Neutral.text = "Stock up \n next time";
            Increase.text = "";
            Lower.text = "";
        }
        else
        {
        
            int product_price = GB_script.Dic_item_sell[item_name];
            price = (long)(sell_amount * product_price * (1 + score/10) * quality_mod);
            long og_price = price;



            //pass the value to button_money to update the money;
            Neutral.text = Money_manager.Format_number(price);
            //increase by 20%
            price = (long)(og_price * 1.2);
            Increase.text = Money_manager.Format_number(price) + "\n +20% / - " + minus_score.ToString() + " score";
            //lower by 20%
            price = (long)(og_price * 0.8);
            Lower.text = Money_manager.Format_number(price) + "\n -20% / + " + plus_score.ToString() + " score";
        }
    }

    //Time_spent() tracks the time and stops selling when it reaches the limit
    public void Neutral_sell()
    {
        //Nothing to buy, so the customer quits
        if(Quit)
            end_selling();

        //sell product normally
        else
        {
            Global_values.money += price; //+money
            MonoBehaviour.print("Items before selling: " + $"{GB_script.Dic_item_amount[item_name]}");
            GB_script.add_amount_to_dic(item_name, (-1) * sell_amount); //-amount
            MonoBehaviour.print("Items left: " + $"{GB_script.Dic_item_amount[item_name]}");
        
            //update cash
            Money_manager.update_money_label(2);
            //add a coroutine for animation later
            end_selling();
        }
    }

    public void Risky_increase()
    {
        //Nothing to buy, so the customer quits
        if(Quit)
            end_selling();

        //sell product normally
        else
        {
            Global_values.money += (long)(price * 1.2); //+money
            MonoBehaviour.print("Items before selling: " + $"{GB_script.Dic_item_amount[item_name]}");
            GB_script.add_amount_to_dic(item_name, (-1) * sell_amount); //-amount
            MonoBehaviour.print("Items left: " + $"{GB_script.Dic_item_amount[item_name]}");
            //Lower score to the customer
            int new_score = Customers.Penalty(customer_id, minus_score);
            //update cash
            Money_manager.update_money_label(2);
            //add a coroutine for animation later

            StartCoroutine(close_Panel(new_score));
        }
    }

    public void Risky_lower()
    {
        //Nothing to buy, so the customer quits
        if(Quit)
            end_selling();

        //sell product normally
        else
        {
            Global_values.money += (long)(price*0.8); //+money
            MonoBehaviour.print("Items before selling: " + $"{GB_script.Dic_item_amount[item_name]}");
            GB_script.add_amount_to_dic(item_name, (-1) * sell_amount); //-amount
            MonoBehaviour.print("Items left: " + $"{GB_script.Dic_item_amount[item_name]}");
            //add score to the customer
            int new_score = Customers.Grace(customer_id, plus_score);
            //update cash
            Money_manager.update_money_label(2);
            //add a coroutine for animation later
            StartCoroutine(close_Panel(new_score));
        }
    }

    private void end_selling()
    {
        //Penalize the player
        // for now instantly switches customers
        //restart
        GameObject.Destroy(Panel);
        Customers.Time_spent();
        Customers.forget_customer();
        Customers.Start_buying();
    }

    IEnumerator close_Panel(int s)
    {
        Panel_script.updateScore(s);
        //hide buttons
        for(int i = 0; i < 3; i++)
            Sell_buttons[i].SetActive(false);

        yield return new WaitForSecondsRealtime(1);

        //show buttons
        for(int i = 0; i < 3; i++)
            Sell_buttons[i].SetActive(true);

        end_selling();
    }
}
