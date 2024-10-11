using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using TMPro;

public class Panel_functions : MonoBehaviour
{
    //initial label names
    [SerializeField] private TMP_Text Tunit_price; 

    [SerializeField] private TMP_Text Tbuy_price;

    public TMP_InputField InBuy_amount;

    public button_money Money_manager;


   
    // Start is called before the first frame update
    void Start()
    {
        //Change label prices
        //try uses system
        try{ 
            Tunit_price.text = (Prod_Prices.Gasoline).ToString();
        }
        catch(NullReferenceException e)
        {
            Debug.Log("Tunit_price was not set");
        }
        
    }

    [HideInInspector]
    public float buy_price;
    private int amount;

    //display and calculate the price
    public void New_amount(string M)
    {
        

        //get the value from input
        //M is always a number;
        M = InBuy_amount.text;

        try {
            amount = int.Parse(M);
        }

        //Checks for errors
        //Implement more variaty and Price reset
        catch (FormatException e)
        {
            Debug.Log($"Unable to parse '{M}'");
            
        }
        catch (OverflowException e)
        {
            Debug.Log("This nubmer cannot fith in an Int32");
        }
        
        //stop overflow
        if(amount < 100000)
        {
        buy_price = amount * Prod_Prices.Gasoline;
        Tbuy_price.text = (buy_price).ToString();
        }

    }

    private float current_money;

    //pop up a message
    public bool active_message = false;
    IEnumerator label_message(float delay, string msg)
    {
        string tmp = Tbuy_price.text;
        Tbuy_price.text = msg;
        yield return new WaitForSeconds(delay);
        Tbuy_price.text = tmp;
        active_message = false;
    }

    public void Buy_item()
    {
        current_money = Global_values.money;
        
        if(current_money < buy_price)
        {
            Debug.Log("Not enough cash");

            if(active_message == false)
            {
                active_message = true;
                StartCoroutine(label_message(1.5f, "Not enough cash"));
                
            }
                
        }
        else
        {
            Global_values.money -= current_money;
            try {
                Money_manager.update_label();
            }
            catch (NullReferenceException e)
            {
                Debug.Log("Money manager is not set");
            }
        }
    }
}
