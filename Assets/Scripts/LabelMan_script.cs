using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using TMPro;


public class LabelMan : MonoBehaviour
{



    //References to labels that are global
    [SerializeField] TMP_Text Cash_label_1;
    [SerializeField] TMP_Text Cash_label_2;
    [SerializeField] TMP_Text Time_label_1;
    [SerializeField] TMP_Text Time_label_2;

    //Object make calls to update their label
    public static LabelMan reference;
    
    
    void Awake()
    {
        reference = this;
    }

    //update a label
    //use this for better debugging 
    public void to_label(TMP_Text label, string text)
    {
        label.text = text;
    }
    //call to update scenes money label
    public void update_money_label(int Part)
    {
        string str_money;
        str_money = Format_number(Global_values.money);
        if(Part == 1)
            Cash_label_1.text = str_money + " $";
        else if(Part == 2)
            Cash_label_2.text = str_money + " $";  
    }

    //call to update scenes time label
    public void update_time_label(int Part)
    {
        //time label
        int h = Global_values.time % 24;
        int d = Global_values.time / 24;
        string day_and_hours;
        if(d <= 0)
            day_and_hours = h.ToString() + " h";
        else
           day_and_hours = d.ToString() + " d " + h.ToString() + " h";

    if(Part == 1)
        Time_label_1.text = day_and_hours;
    else if (Part == 2)
        Time_label_2.text = day_and_hours; 
    }

    public string Format_number(long number)
    {
       string[] trump ={"","K","M","B","T","Qa","Qi","Sx","Se","Oc","No","De","Un"};//n nustato kuri trumpini naudos
       int n=(((number).ToString().Length)-3)/3;
       int m=(((number).ToString().Length)-3)%3+1;//kiek reikia skaitmenu pries kableli
     
       string p="0";
         if(m<0){
               p="0";
             m=0;
         }
         
          if (m>0){
                   p=(number).ToString().Substring(0,m);}
        int k=2;
        if((number).ToString().Length<2){
            k=(number).ToString().Length;
        }
       
      return (p+","+(number).ToString().Substring(m,k)+" " +trump[n]);
    }
}
