using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


//Base class for panel specific scripts
//Mainly collects data about a panel
public class Panel_update : MonoBehaviour
{
    GameObject[] objectRows;
    GameObject[] panels;

    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameLog.Message($"{this.transform.GetChild(i)}");
        }
        return;
    }


}
