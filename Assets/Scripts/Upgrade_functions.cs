using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class Upgrade_functions : MonoBehaviour
{
    //init values
    [SerializeField] public long buy_price = 0;
    [SerializeField] string upgrade_name;
    [SerializeField] string Tooltip_text;
    //UI dependencies
    [SerializeField] private TMP_Text Header;
    [SerializeField] private TMP_Text Tier;
    [SerializeField] private TMP_Text Price;
    [SerializeField] private TMP_Text Description;
    [SerializeField] GameObject Tooltip;
    [SerializeField] GameObject Base;

    //Script dependencies
    Global_values GB_script;
    LabelMan MoneyManager;
    Upgrades Upgrade_script;

    int tier;
    int method_id;




    // Start is called before the first frame update
    void Start()
    {
        Tooltip.SetActive(false);
        GB_script = Global_values.reference;
        MoneyManager = LabelMan.reference;
        Upgrade_script = Upgrades.reference;
        Header.text = upgrade_name;
        Price.text = MoneyManager.Format_number(buy_price) + "$";
        tier = Upgrade_script.Dic_upgrades[upgrade_name].tier;
        method_id = Upgrade_script.Dic_upgrades[upgrade_name].method_id;
        Base_UI();
        add_description();
    }




    private void buy_upgrade()
    {
        return;
    }




    private void Base_UI()
    {
        //Tiers
        string text = "";
        switch(tier)
        {
            case 0:
            text = "Buy upgrade\n";
            break;

            case 1:
            text = "Tier: I\n";
            break;

            case 2:
            text = "Tier: II\n";
            break;

            case 3:
            text = "Tier: III\n MAX";
            break;
        }

        //Modifier added
        text += "\n";
        int mod = Upgrade_script.Modifier(method_id, tier+1);

            int old_mod;
            if(tier == 0)
                old_mod  = 1;
            else
                old_mod = Upgrade_script.Modifier(method_id, tier);

            if(mod == -1 || mod == 0)
                text += "";
            else
                text += "x" + old_mod.ToString() + " -> x" + mod.ToString();

        Tier.text = text;
    }

    private void add_description()
    {
        string text = Tooltip_text;
        text += "\n";
        Description.text = text;
    }

    public void ShowBase()
    {
        Base.SetActive(true);
        Tooltip.SetActive(false);
    }

    public void ShowTooltip()
    {
        Base.SetActive(false);
        Tooltip.SetActive(true);
    }
}
