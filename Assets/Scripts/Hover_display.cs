using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Hover_display : MonoBehaviour
{
    
    //customization
    [SerializeField] float offset;

    //dependencies
    Global_values GB_script;
    Region_grid Grid;
    LabelMan MoneyManager;

    //data for the tooltip
    int item_id;
    string item_name;
    long buy_amount;
    int score;
    int type1, type2, type3;

    string information = "";



    //init data from customer_script
    public void Init_data(int id, long amount, int S, int t1, int t2, int t3, Region_grid GReference)
    {
        //script dependencies
        GB_script = Global_values.reference;
        Grid = GReference;
        MoneyManager = LabelMan.reference;

        item_id = id;
        item_name = Global_values.Items[item_id];
        buy_amount = amount;
        score = S;
        type1 = t1; type2 = t2; type3 = t3;
        information = to_Tooltip();
    }

    //create a text reference
    private string to_Tooltip()
    {
        string I = "";
        I += "Score: " + score.ToString() + '\n';
        I += "Types: " + "[ " + type1.ToString() + ", " + type2.ToString()
        + ", " + type3.ToString() + " ]" + '\n';
        I += "Wants to buy: " + MoneyManager.Format_amount(item_name);

        return I;
    }

    //Communication with Grid object
    private void OnMouseEnter()
    {
        Grid.Show_tooltip(information);
    }

    private void OnMouseExit()
    {
        Grid.Hide_tooltip();
    }
}
