using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Upgrades : MonoBehaviour
{

    Global_values GB_script;

    int storageX = 5;
    int QualityX = 2;


    public static Upgrades reference;
    //dictionary of all the upgrades
    //ADD UPGRADES THROUGH THE EDITOR
    //IT WILL OVERIDE THEESE
    public List<string> upgrades = new List<string>{"Stockpile", "Quality", "Time", "Efficiency"};
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
        MonoBehaviour.print(upgr_len);
        for(int i = 0 ; i < upgr_len; i++)
        {
            MonoBehaviour.print(upgrades[i]);
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
            return sellQuality(tier);

            case 2:
            return openTime(tier);

            case 3:
            return workerEfficiency(tier);

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
            
            //No modifications
            case 1:
            return;

            case 2:
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
    int storageMod(int tier)
    {
        switch(tier)
        {
            case 0:
            return 1;

            case 1:
            return storageX * 2;

            case 2:
            return storageX * storageX * storageX;

            case 3:
            return storageX * 1000;
        }
        return -1;
    }

    int sellQuality(int tier)
    {
        switch(tier)
        {
            case 0:
            return 1;

            case 1:
            return QualityX;

            case 2:
            return QualityX+1;

            case 3:
            return QualityX*2+1;
        }

        return -1;
    }

    int openTime(int tier)
    {
        switch(tier)
        {
            case 0:
            return 8;

            case 1:
            return 7;

            case 2:
            return 6;

            case 3:
            return 5;
        }
        return -1;
    }


    int workerEfficiency(int tier)
    {
        switch(tier)
        {
            case 0:
            return 5;

            case 1:
            return 4;

            case 2:
            return 3;

            //update for more effects
            case 3:
            return 2;
        }
        return -1;

    }
}
