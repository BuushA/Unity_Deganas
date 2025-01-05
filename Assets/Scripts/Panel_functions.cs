using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

using TMPro;
using System.Runtime.Versioning;

public class Panel_functions : MonoBehaviour
{

    //dependencies for script
    [SerializeField] string item_name;
    [SerializeField] Global_values GB_script;
    private LabelMan Money_manager;
    private Upgrades Upgrades_script;
    

    //labels - UI
    [SerializeField] private TMP_Text Tunit_price; //price displayed under selling item
    [SerializeField] private TMP_Text Tbuy_price;  //display calculated price
    public TMP_InputField InBuy_amount; //get amount input
    [SerializeField] TMP_Text Tunit_amount; //display how much of this unit you currently have
    [SerializeField] TMP_Text Tmax_amount; //for displaying max amount


    //variables both public and private;
    [HideInInspector]
    public long buy_price;

    //set price
    private long item_price;
    private long amount;
    private long max_amount;
    private int mod; //amount modifier
    long owned;

    //set the tag
    //tags will be used to update all labels
    void Awake()
    {   
        //gameObject.tag = "Product_Panel";
    }

    // Start is called before the first frame update
    void Start()
    {
        Money_manager = LabelMan.reference;
        Upgrades_script = Upgrades.reference;
        GB_script = Global_values.reference;
        update_labels();
    }

    public void update_labels()
    {   
        string text;
        //price of the item might decrease or increase
        item_price = GB_script.Dic_item_price[item_name]; //*100 nereikia (Prod_prices jau yra tiksli kaina);
        text = Money_manager.Format_number(item_price) + " $";
        max_amount = Global_values.stockAmount;
        Money_manager.to_label(Tunit_price, text);
        //amount of items switches between scenes
        
        //mod = Upgrades_script.storageMod(Upgrades_script.Dic_upgrades["Stockpile"].tier);


        if(GB_script.Dic_item_amount.ContainsKey(item_name))
        {
            text = Money_manager.Format_amount(item_name) + "/" + max_amount.ToString();
        }
        else
            text = "0" + "/" + max_amount.ToString();


        Money_manager.to_label(Tunit_amount, text);
    }

    //display and calculate the price
    public void New_amount(string M)
    {
        //get the value from input
        //M is always a number;
        M = InBuy_amount.text;
        max_amount = Global_values.stockAmount;

        if(GB_script.Dic_item_amount.ContainsKey(item_name))
            owned = GB_script.Dic_item_amount[item_name];
        else owned = 0;

        try {
            amount = long.Parse(M);



            if(amount >= 0  && amount + owned <= max_amount)
            {
            buy_price = amount * item_price; 

            //TMP_Text, string-> updates the label
            Money_manager.to_label(Tbuy_price, Money_manager.Format_number(buy_price) + " $");
            }
        }
        catch (FormatException e)
        {
            //Checks for errors
            //Implement more variaty and Price reset
            Debug.Log($"Unable to parse '{M}'"); 
        }
        catch (OverflowException e)
        {
            Debug.Log("This nubmer cannot fit in an Int32");
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
        max_amount = Global_values.stockAmount;

        if(GB_script.Dic_item_amount.ContainsKey(item_name))
            owned = GB_script.Dic_item_amount[item_name];
        else owned = 0;
        
        if(Global_values.money < buy_price)
        {
            Debug.Log("Not enough cash");

            if(active_message == false)
            {
                active_message = true;
                StartCoroutine(label_message(1.5f, "Not enough cash"));
            }
        }
       
       else if(amount == 0)
       {
            Debug.Log("Won't create an element with value 0");

            if(active_message == false)
            {
                active_message = true;
                StartCoroutine(label_message(1.5f, "Can't buy dust"));
            }
       }
       else if(amount + owned > max_amount)
       {
            Debug.Log("Over the max amount");

            if(active_message == false)
            {
                active_message = true;
                StartCoroutine(label_message(1.5f, "Too many products"));
            }
       }
        else
        {
            Global_values.money -= buy_price;
            GB_script.add_amount_to_dic(item_name, amount);
            Money_manager.update_money_label(1);
            //Update only the amount
            update_labels();
        }
    }

    public void max_possible()
    {
        max_amount = Global_values.stockAmount;
        amount = Global_values.money /  GB_script.Dic_item_price[item_name];

        if(GB_script.Dic_item_amount.ContainsKey(item_name))
            owned = GB_script.Dic_item_amount[item_name];
        else owned = 0;


        if(amount + owned > max_amount)
            amount = max_amount - owned;
        

        buy_price = amount * item_price;

        Money_manager.to_label(Tbuy_price, Money_manager.Format_number(buy_price) + " $");
    }
}
