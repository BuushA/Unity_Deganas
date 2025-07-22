using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelPopUp : MonoBehaviour
{
    GameObject panel;
    [SerializeField] GameObject vertical_panel;

    void Awake()
    {
        panel = gameObject;
    }


    //Important not to hide before all the other variables initialized
    void Start()
    {
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
