using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics.Contracts;
using System;
using System.Runtime.Serialization;
using Microsoft.Win32.SafeHandles;
using System.Linq;
using System.Reflection;

public class Sell_options : MonoBehaviour
{

    //script dependencies
    [SerializeField] customer_script Customers;
    private Global_values GB_script;
    private Upgrades upgrade;
    private LabelMan Money_manager;
    private customer_Panel Panel_script;
    private NetworkServer netServer;


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

    public enum SellMods
    {
        increase = 120,
        lower = 80
    }

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
        netServer = NetworkServer.reference;
    }

    public void New_Customer(string name_ref, long amount_req, int SC, long curr, int id, customer_Panel script,GameObject reference)
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
            price = sell_amount * product_price * quality_mod;
            long og_price = price;

            //LOG about the customer
            GameLog.Message(MethodBase.GetCurrentMethod().Name + ": Customer: " + $"{customer_id}, " + item_name + $": {sell_amount}");
            GameLog.Message(MethodBase.GetCurrentMethod().Name + ": Product: " + $"1 unit={product_price}, " + $"total={price}");
            GameLog.Message(MethodBase.GetCurrentMethod().Name + ": Score: " + $"{score}");

            //pass the value to button_money to update the money;
            Neutral.text = Money_manager.Format_number(price);
            //increase by 20%
            price = (long)(og_price * 1.2);
            Increase.text = Money_manager.Format_number(price) + "\n +20% / - " + minus_score.ToString() + " score";
            //lower by 20%
            price = (long)(og_price * 0.8);
            Lower.text = Money_manager.Format_number(price) + "\n -20% / + " + plus_score.ToString() + " score";
            price = og_price;
        }


    }

    //Time_spent() tracks the time and stops selling when it reaches the limit
    public void Sell(int price_modifier)
    {
        long profit = 0;
        long modified_price = (long)(price * price_modifier / 100);
        if (Quit)
            end_selling();
        else
        {
            Global_values.money += modified_price;
            GB_script.add_amount_to_dic(item_name, (-1) * sell_amount);
            profit = modified_price + (GB_script.Dic_item_amount[item_name] * sell_amount) * (-1);
            GameLog.Message(MethodBase.GetCurrentMethod().Name + " Price: " + $"Mod={modified_price} ; " + $"normal={(price)}");
            Money_manager.update_money_label(2);
            GameLog.Message($"Profit for F-16 {profit}");
            GB_script.AllProfit += profit;
            GB_script.ShortTermProfit += profit;
            netServer.UpdateProfitRpc(Global_values.localID, profit);
            netServer.requestGetOpponentStockRpc(Global_values.localID);
            GameLog.Message($"Opponent Stock F-15 {Global_values.OpponentStock}");
            if (price_modifier == (int)SellMods.increase)
            {
                int new_score = Customers.Penalty(customer_id, plus_score);
                GameLog.Message(MethodBase.GetCurrentMethod().Name + " updated Score: " + $"{new_score}");
                StartCoroutine(close_Panel(new_score));
            }
            else if (price_modifier == (int)SellMods.lower)
            {
                int new_score = Customers.Penalty(customer_id, minus_score);
                GameLog.Message(MethodBase.GetCurrentMethod().Name + " updated Score: " + $"{new_score}");
                StartCoroutine(close_Panel(new_score));
            }
            else
                end_selling();
        }
    }

    public void neutral()
    {
        //100 does nothing
        Sell(100);
    }

    public void JackUp()
    {
        Sell((int)SellMods.increase);
    }

    public void BringDown()
    {
        Sell((int)SellMods.lower);
    }

    private void end_selling()
    {
        //Penalize the player
        // for now instantly switches customers
        //restart
        GameObject.Destroy(Panel);
        //Customers.Time_spent();
        Customers.UseWorkCharge();
        Customers.forget_customer();
        Customers.Start_buying();
    }

    IEnumerator close_Panel(int s)
    {
        Panel_script.updateScore(s);
        for(int i = 0; i < 3; i++)
            Sell_buttons[i].SetActive(false);

        yield return new WaitForSecondsRealtime(1);

        for(int i = 0; i < 3; i++)
            Sell_buttons[i].SetActive(true);

        end_selling();
    }
}
