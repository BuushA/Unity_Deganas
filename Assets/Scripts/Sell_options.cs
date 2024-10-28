using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics.Contracts;
using System;

public class Sell_options : MonoBehaviour
{

    //script dependencies
    [SerializeField] customer_script Customers;
    private Global_values GB_script;
    private LabelMan Money_manager;




    //Text labels
    private TMP_Text Increase;
    private TMP_Text Neutral;
    private TMP_Text Lower;



    private long price;
    private long sell_amount;
    private string item_name;


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
    }

    public void New_Customer(string name_ref, long amount_req)
    {
        //init labels;
        item_name = name_ref;
        long product_price = GB_script.Dic_item_price[item_name];
        sell_amount = amount_req;
        price = sell_amount * product_price;

        //pass the value to button_money to update the money;
        //temporary labels
        Neutral.text = Money_manager.Format_money(price);
        //increase

        //lower
    }

    public void Neutral_sell()
    {
        bool continue_selling = true; 
        //Current amount is always above selling
        //sell product normally
        Global_values.money += price; //+money
        GB_script.add_amount_to_dic(item_name, (-1) * sell_amount); //-amount
       
        
        if(GB_script.Dic_item_amount[item_name] <= 0 && GB_script.Dic_item_amount.Count > 1)
            GB_script.Dic_item_amount.Remove(item_name);
        else if(GB_script.Dic_item_amount[item_name] <= 0 && GB_script.Dic_item_amount.Count == 1)
        {
            
            GB_script.Dic_item_amount.Clear();
            Customers.Switch_to_managment();
            continue_selling = false;
        }
            

        
        //update cash
        Money_manager.update_money_label(2);
        //add a coroutine for animation later
        

        if(continue_selling == true)
        {
            // for now instantly switches customers
            //first check if the is anything to sell
            Customers.Start_buying();
            Customers.Time_spent();
        }

    }


    public void Risky_increase()
    {
       int a = 0;
    }

    public void Risky_lower()
    {
        int a = 0;
    }
}
