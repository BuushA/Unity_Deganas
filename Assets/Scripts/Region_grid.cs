using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region_grid : MonoBehaviour
{
    [SerializeField] customer_script Customers;
    [SerializeField] GameObject Tile;
    [SerializeField] GameObject House;




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
}
