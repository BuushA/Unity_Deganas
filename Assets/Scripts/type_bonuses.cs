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
    GameObject ActivePanel;


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
        if (Panels.Length == 0)
        {
            GameLog.Error("Modifier panels were not found");
            return;
        }

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
        ActivePanel = chosenPanel; //cache for future
        for (int i = 0; i < Generate_amount; i++)
        {
            var panel_graphics = Panels[i].GetComponent<Image>();
            if (Panels[i] != chosenPanel)
            {
                GameLog.Message("not this one");
                //Add effects here later
            }
            else
            {
                //Add green borders later
                panel_graphics.color = Color.red;
            }
        }

        foreach (var obj in Buttons)
        {
            obj.SetActive(false);
        }
    }

    //Called in Scene_switch CloseOverview()
    public void RestoreAndUpdateButtons()
    {
        GameLog.Message("Restoring Modifiers");
        CreateSelection(Generate_amount);
        init_panels();
        foreach (var obj in Buttons)
        {
            obj.SetActive(true);
        }
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
        string typeData = JSON_operations.Read_file("type-declaration.json");
        typeBonus = JSON_operations.From<TypeBonuses1>(typeData);
        for (int i = 0; i < typeBonus.Count; i++)
            typeBonus[i].index = i;

        Chance_rng chan = Chance_rng.reference;
        CreateRNGWeights(typeBonus);     //Weights for creating selection
        CreateTypeDictionary(typeBonus); //Dictionary for name reference
        CreateSelection(Generate_amount);//Call everytime you need new types selected
        init_panels();                   //push information onto all panels

    }

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

