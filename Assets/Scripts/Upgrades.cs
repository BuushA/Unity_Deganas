using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Upgrades : MonoBehaviour
{

    Global_values GB_script;

    int increment = 5;


    public static Upgrades reference;
    //dictionary of all the upgrades
    public List<string> upgrades = new List<string>{"Stockpile", "SellQuality"};
    public class UP
    {
        public int tier;
        public int method_id;
    }; 

    public Dictionary<string, UP> Dic_upgrades = new Dictionary<string, UP>();

    // Start is called before the first frame update
    void Awake()
    {
        reference = this;
        GB_script = Global_values.reference;
        create_upgrades_dic();
    }

    private void create_upgrades_dic()
    {
        int upgr_len = upgrades.Count;
        
        for(int i = 0 ; i < upgr_len; i++)
        {
            Dic_upgrades.Add(upgrades[i], new UP {tier = 0, method_id = i});
        }
    }

    //call to get the modifier
    public int Modifier(int id, int tier)
    {
        switch(id)
        {
            case 0:
            return storageMod(tier);

            case 1:
            return 0;

            default:
            return -1;
        }
    }


    //call on Upgrade_function buy to modify values
    public void Modify(int id, int tier)
    {
        switch(id)
        {
            case 0:
            GB_script.increase_stock(storageMod(tier));
            break;

            case 1:
            return;

            default:
            return;
        }
    }



    public void add_upgrade(string name)
    {   
        upgrades.Add(name);
        int new_id = upgrades.Count-1;
        Dic_upgrades.Add(name, new UP {tier = 0, method_id = new_id});
    }

    public long priceMod(long price, string name)
    {
        int tier = Dic_upgrades[name].tier;
        int mod = 1;
        

        for(int i = 0; i < tier; i++)
            mod *= 5;

        return price * mod;
    }

    //id - 0
    public int storageMod(int tier)
    {
        switch(tier)
        {
            case 0:
            return 1;

            case 1:
            return increment * 2;

            case 2:
            return increment * increment * increment;

            case 3:
            return increment * 1000;
        }
        return -1;
    }
}
