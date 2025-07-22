using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;




//Used together with a button
//Adds classes which receive bonuses
//Resets every week or a set amount of time
public class TemporaryTypeBonus : MonoBehaviour
{

    //List<BuyTypeBonus> BuyActiveBonus = new List<BuyTypeBonus>();
    //List<PopularTypeBonus> PopularActiveBonus = new List<PopularTypeBonus>();
    List<string> Selection = new List<string>();

    List<string> AllTypes;

    const int Generate_amount = 4;

    //Replace by 1 class for maintanability
    //    [System.Serializable]
    //    public class BuyTypeBonus
    //    {
    //        public string name;
    //        public List<string> types;
    //        public List<string> Products;
    //        public int bonus;
    //        public int tier; //1-5
    //    }
    //
    //    [System.Serializable]
    //    public class PopularTypeBonus
    //    {
    //        public string name;
    //        public List<string> types;
    //        public int bonus;
    //        public int tier; //1-5
    //    }



    //List<BuyTypeBonus> buyType = new List<BuyTypeBonus>();
    //List<PopularTypeBonus> popularityType = new List<PopularTypeBonus>();

    List<TypeBonuses1> typeBonus = new List<TypeBonuses1>();
    List<TypeBonuses1> ActiveTypeBonus = new List<TypeBonuses1>();
    Dictionary<string, TypeBonuses1> refrenceType = new Dictionary<string, TypeBonuses1>();


    Dictionary<string, int> WeightTypes = new Dictionary<string, int>();

    int totalV = 0;
    Chance_rng chan;
    GameObject[] Panels;
    GameObject[] Buttons;


    void CreateRNGWeights(List<TypeBonuses1> T)
    {
        foreach (var x in T)
        {
            totalV += x.tier;
            WeightTypes[x.name] = x.tier;
        }
    }

    void CreateTypeDictionary(List<TypeBonuses1> T)
    {
        int n = 0;
        foreach (var x in T)
        {
            refrenceType[x.name] = x;
        }
    }

    void CreateSelection(int number)
    {
        chan = Chance_rng.reference;
        for (int i = 0; i < number; i++)
        {
            string rezult;
            chan.strPlace_weights(totalV, WeightTypes, out rezult);
            Selection.Add(rezult);
            totalV -= WeightTypes[rezult];
            GameLog.Message(rezult);
            if (WeightTypes.Remove(rezult) == false) ;
            GameLog.Message("failed to remove " + rezult + "from dictionary");
        }
    }

    void init_panels()
    {
        GameLog.Message("init P99");
        foreach (var x in Panels)
            GameLog.Message($"{x} was found - P99");
        for (int i = 0; i < Generate_amount; i++)
        {
            var Tbonus = refrenceType[Selection[i]];
            TypePanelVariables panel_script = Panels[i].GetComponent<TypePanelVariables>();
            GameLog.Message(Tbonus.name);
            panel_script.initialize_variables(Tbonus);
        }
    }

    public void AddToActiveList(int id)
    {
        GameLog.Message("adding modifier - " + $"{typeBonus[id]} - {id}");
        ActiveTypeBonus.Add(typeBonus[id]);
    }

    public void VisualizeSelection(GameObject chosenPanel)
    {
        for (int i = 0; i < Generate_amount; i++)
        {
            var panel_graphics = Panels[i].GetComponent<Image>();
            if (Panels[i] != chosenPanel)
            {
                //Gray
                GameLog.Message($"{panel_graphics}");
            }
            else
            {
                //Add green borders
                panel_graphics.color = Color.red;
            }
        }

        //Disable buttons
        foreach (var obj in Buttons)
        {
            obj.SetActive(false);
        }
    }

    public void RestoreAndUpdateButtons()
    {
        return;
    }

    public static TemporaryTypeBonus reference;

    void Awake()
    {
        reference = this;
        Panels = GameObject.FindGameObjectsWithTag("TypeBonusPanel");
        Buttons = GameObject.FindGameObjectsWithTag("PanelModifierButton");
    }

    void Start()
    {
        //Read from one json file
        //        string buyData = JSON_operations.Read_file("buy-type.json");
        //        string popData = JSON_operations.Read_file("popularity-type.json");
        //        buyType = JSON_operations.From<BuyTypeBonus>(buyData);
        //        popularityType = JSON_operations.From<PopularTypeBonus>(popData);

        string typeData = JSON_operations.Read_file("type-declaration.json");
        typeBonus = JSON_operations.From<TypeBonuses1>(typeData);
        for (int i = 0; i < typeBonus.Count; i++)
            typeBonus[i].index = i;

        Chance_rng chan = Chance_rng.reference;
        //put for loop inside the function
        //        foreach (var e in buyType)
        //            CreateRNGWeights(e.name, e.tier);
        //        foreach (var e in popularityType)
        //            CreateRNGWeights(e.name, e.tier);
        //        GameLog.Message($"{totalV}");
        CreateRNGWeights(typeBonus);
        CreateSelection(Generate_amount);
        foreach (var e in Selection)
            GameLog.Message(e);

        //TO DO
        //Figure out a way to make everything easily expandable in the future ++
        CreateTypeDictionary(typeBonus);
        //Initialize panels
        init_panels();
        // mark which are active in the List - Pressing the button



    }

    //    public int BuyBonus(string type_1, string type_2, string type_3, string prod)
    //    {
    //        int bonus = 0;
    //        foreach (var bon in typeBonus)
    //        {
    //            List<string> T = bon.types;
    //            if (stringExists(bon.Products, prod) == false)
    //                return 0;
    //            else
    //            {
    //                if (stringExists(T, type_1) == false && stringExists(T, type_2) == false && stringExists(T, type_3) == false)
    //                    return 0;
    //                else
    //                {
    //                    bonus += bon.bonus;
    //                }
    //            }
    //        }
    //        return bonus;
    //    }
    //
    //    public int PopularityBonus(string type_1, string type_2, string type_3)
    //    {
    //        int bonus = 0;
    //        foreach (var bon in typeBonus)
    //        {
    //            List<string> T = bon.types;
    //            if (stringExists(T, type_1) == false && stringExists(T, type_2) == false && stringExists(T, type_3) == false)
    //                return 0;
    //            else
    //            {
    //                bonus += bon.bonus;
    //            }
    //        }
    //        return bonus;
    //    }

    //prod might be empty
    //Might add more arguments
    public int GiveTypeBonus(string type_1, string type_2, string type_3, string prod)
    {
        int bonus = 0;
        foreach (var bon in ActiveTypeBonus)
        {
            List<string> T = bon.types;
            if (stringExists(T, type_1) == false && stringExists(T, type_2) == false && stringExists(T, type_3) == false)
                bonus += 0;
            else
            {
                if (bon.Products.Count != null)
                {
                    if (stringExists(bon.Products, prod) == false)
                        bonus += 0;
                    else
                        bonus += bon.bonus;
                }
                else
                    bonus += bon.bonus;
            }
        }
        return bonus;
    }

    bool stringExists(List<string> T, string match)
    {
        foreach (var e in T)
        {
            if (e == match)
                return true;
            else
                return false;
        }
        return false;
    }

}

[System.Serializable]
public class TypeBonuses1//stupid fix
{

    public string name;
    public List<string> types;
    public List<string> Products;
    public int bonus;
    public int tier; //1-5
    public int index; //can't figure out how to set value in editor since structs are immutable
}

