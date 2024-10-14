using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using TMPro;
using UnityEngine.EventSystems;

public class Panel_functions : MonoBehaviour
{


   

    //dependencies for script
    [SerializeField] string item_name;
    [SerializeField] Global_values GB_script;
    public button_money Money_manager;

    //labels - UI
    [SerializeField] TMP_Text Tunit_price; //price displayed under selling item
    [SerializeField] TMP_Text Tbuy_price;  //display calculated price
    [SerializeField] TMP_Text Tb_max_amount; //for displaying max amount
    public TMP_InputField InBuy_amount; //get amount input
    [SerializeField] TMP_Text Tunit_amount; //display how much of this unit you currently have


    //variables both public and private;
  
    [HideInInspector]
    public float buy_price;

    //set price
    private float item_price;
    private int amount;

    // Start is called before the first frame update
    void Start()
    {
        //Change label prices
        //try uses system
        try{ 

            item_price = GB_script.Dic_item_price[item_name];
            Tunit_price.text = (item_price).ToString() + "$";
            Tunit_amount.text = (amount + 0).ToString() + " of" + '\n' + item_name;
        }
        catch(NullReferenceException e)
        {
            Debug.Log("Tunit_price was not set");
        }
    
        max_possible();
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
        
        //stop overflow
        if(amount < 100000)
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
        
        
        if(Global_values.money < buy_price)
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
                //update amount and money labels;
                Money_manager.update_money_label();
                Tunit_amount.text = (GB_script.Dic_item_amount[item_name] + 0).ToString() + " of" + '\n' + item_name;
            }
            catch (NullReferenceException e)
            {
                Debug.Log("Money manager is not set");
            }
        }
    }


    public void max_possible()
    {
        int amount_max = (int)(Global_values.money /  GB_script.Dic_item_price[item_name]);
        buy_price = amount_max * item_price;

        //set button text to amount
        Tb_max_amount.text = amount_max.ToString() + " - maximum";
        Tbuy_price.text = (buy_price).ToString() + "$";
    }

}




