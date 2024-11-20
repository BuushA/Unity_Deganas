using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Region_grid : MonoBehaviour
{
    [SerializeField] customer_script Customers;
    [SerializeField] GameObject Tile;
    [SerializeField] GameObject House;

    [SerializeField] GameObject customerCard;
    private TMP_Text LabCustomer;

    void Start()
    {
        LabCustomer = customerCard.GetComponent<TMP_Text>();
        customerCard.SetActive(false);
    }

    public void Generate_map()
    {
        Debug.Log("Grid is initialized");
        Customers.Create_customer_grid(this, Tile, House);
    }



    public void Clean()
    {
        foreach(Transform child in this.gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


    public void Show_tooltip(string information)
    {
        LabCustomer.text = information;
        customerCard.SetActive(true);
    }

    public void Hide_tooltip()
    {
        customerCard.SetActive(false);
    }

}
