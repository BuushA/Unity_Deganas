using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using TMPro;

public class Panel_functions : MonoBehaviour
{


   

    //dependencies for script
    [SerializeField] string item_name;
    [SerializeField] Global_values GB_script;
    public button_money Money_manager;

    //labels - UI
    [SerializeField] private TMP_Text Tunit_price; //price displayed under selling item
    [SerializeField] private TMP_Text Tbuy_price;  //display calculated price
    public TMP_InputField InBuy_amount; //get amount input


    //variables both public and private;
  
    [HideInInspector]
    public float buy_price;

    //set price
    private float item_price;
    private float current_money;
    private int amount;

    // Start is called before the first frame update
    void Start()
    {
        //Change label prices
        //try uses system
        try{ 
            item_price = GB_script.Dic_item_price[item_name];
            Tunit_price.text = (item_price).ToString() + "$";
        }
        catch(NullReferenceException e)
        {
            Debug.Log("Tunit_price was not set");
        }
        
    }


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
        
        //stop overflow and negative values
        if(amount < 100000 &&amount > 0)
        {
        buy_price = amount * item_price;
        Tbuy_price.text = (buy_price).ToString() + "$";
        }

    }

    

    //pop up a message
    public bool active_message = false;
    //Coroutine for delaying
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
            Global_values.money -= buy_price;
            GB_script.add_amount_to_dic(item_name, amount);
            try {
                Money_manager.update_money_label();
            }
            catch (NullReferenceException e)
            {
                Debug.Log("Money manager is not set");
            }
        }
    }
}
