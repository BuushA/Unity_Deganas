using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelPopUp : MonoBehaviour
{
    GameObject panel;

    void Awake()
    {
        panel = gameObject;
        HidePanel();
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
    }


}
