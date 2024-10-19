using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;

using TMPro;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;


public class customer_script : MonoBehaviour
{

    [SerializeField] private GameObject request_UI;
    private Global_values GB_script;

    private TMP_Text request_label;
    
    public bool Stop = false;
    public long amount_req;

    //rng
    System.Random rand = new();


    // Start is called before the first frame update
    void Start()
    {   
        request_label = request_UI.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();

        GB_script = FindFirstObjectByType<Global_values>();
        Start_buying();

    }


    private void Start_buying()
    {
        request_UI.SetActive(true);
        int Dcount = GB_script.Dic_item_amount.Count;
        int item_amount = 0; //int because .Next() requires one

        int item_req = rand.Next(Dcount);
        string item_name = Global_values.Items[item_req];
        item_amount = (int)GB_script.Dic_item_amount[item_name];

        amount_req = rand.Next(1, (item_amount));
        
        string request = "Buying " + amount_req.ToString() + " " + item_name;
        request_label.text = request;
    }

}
