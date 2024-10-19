using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;

using TMPro;

public class customer_script : MonoBehaviour
{

    [SerializeField] private GameObject request_UI;

    private TMP_Text request_label;




    // Start is called before the first frame update
    void Start()
    {   
        request_label = request_UI.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        request_label.text = "W NIGGERZ";



    }





}
