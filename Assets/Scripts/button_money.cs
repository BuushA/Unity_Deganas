using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using TMPro;


public class button_money : MonoBehaviour
{
    //for cash
    [SerializeField]
    private int a = 1;
    [SerializeField]
    private TMP_Text money_count;
    //for time
    [SerializeField] private TMP_Text Time_label;
    
    public Panel_functions panel_update; //update or change label updating entirely - automate it


    private void to_label(string text, TMP_Text label)
    {
        label.text = text;
    }

    public void update_money_label()
    {
       string[] trump ={"","K","M","B","T","Qa","Qi","Sx","Se","Oc","No","De","Un"};//n nustato kuri trumpini naudos
       int n=(((Global_values.money).ToString().Length)-3)/3;
       int m=(((Global_values.money).ToString().Length)-3)%3+1;//kiek reikia skaitmenu pries kableli
     
       string p="0";
         if(m<0){
               p="0";
             m=0;
         }
         
          if (m>0){
                   p=(Global_values.money).ToString().Substring(0,m);}
        int k=2;
        if((Global_values.money).ToString().Length<2){
            k=(Global_values.money).ToString().Length;
        }
       
      string str_money = "Cash: " + p+","+(Global_values.money).ToString().Substring(m,k)+" " +trump[n] +"$";
        money_count.text = str_money;  
    }

    void Start()
    {
        //money label
        update_money_label();

        //time label
        int h = Global_values.time % 24;
        int d = Global_values.time / 24;
        string day_and_hours;
        if(d <= 0)
            day_and_hours = h.ToString() + " h";
        else
           day_and_hours = d.ToString() + " d " + h.ToString() + " h";
        to_label(day_and_hours, Time_label);
    }

    public void add_to_money()
    {
        Global_values.money += a*100;
        //add to a label
        update_money_label();
        //MonoBehaviour.print(Global_values.money);

        //update max_amount
        try
        {
            panel_update.max_possible(false); //false = update only TEXT w\o buy_button
        }
        catch (NullReferenceException e)
        {
            Debug.Log("");
        }
        
    }
}
