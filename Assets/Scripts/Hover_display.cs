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
    [SerializeField] GameObject Tooltip;
    TMP_Text tip;
    Global_values GB_script;

    //data for the tooltip
    int item_id;
    string item_name;
    long buy_amount;
    int score;
    int type1, type2, type3;




    //init data from customer_script
    public void Init_data(int id, long amount, int S, int t1, int t2, int t3)
    {
        //script dependencies
        tip = Tooltip.GetComponent<TMP_Text>();
        Tooltip.SetActive(false);
        GB_script = Global_values.reference;

        item_id = id;
        item_name = Global_values.Items[item_id];
        buy_amount = amount;
        score = S;
        type1 = t1; type2 = t2; type3 = t3;
        to_Tooltip();
    }

    void to_Tooltip()
    {
        string information = "";
        information += "Score: " + score.ToString() + '\n';
        information += "Types: " + "[ " + type1.ToString() + ", " + type2.ToString()
        + ", " + type3.ToString() + " ]" + '\n';
        information += "Wants to buy: " + buy_amount.ToString() + '\n';
        information += "Of " + item_name;

        tip.text = information;
    }

    private void OnMouseEnter()
    {
        Tooltip.SetActive(true);
    }

    private void OnMouseExit()
    {
        Tooltip.SetActive(false);
    }


}
