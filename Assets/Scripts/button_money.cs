using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


public class button_money : MonoBehaviour
{
    [SerializeField]
    private int a = 1;

    [SerializeField]
    private TMP_Text money_count;
    

    private void add_to_label(float m)
    {
        string str_money = "Cash: " + m.ToString() + " $";
        money_count.text = str_money;
    }

    void Start()
    {
        MonoBehaviour.print(Global_values.money);
       add_to_label(Global_values.money + 0);
    }

    public void add_to_money()
    {
        Global_values.money += a;
        //add to a label
        add_to_label(Global_values.money);

        //MonoBehaviour.print(Global_values.money);
    }
}
