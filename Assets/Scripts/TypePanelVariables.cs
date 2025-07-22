using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypePanelVariables : MonoBehaviour
{
    //Initilize variables in type_bonus script at the start
    int TypeIndex = -1; //-1 is not set
    [SerializeField] TMP_Text Header;
    [SerializeField] TMP_Text Description;
    //[SerializeField] GameObject ChooseButton;

    TemporaryTypeBonus type_bonus;

    private string panel_name = "";
    private string short_description = "";

    void Start()
    {
        type_bonus = TemporaryTypeBonus.reference;
    }

    public void initialize_variables(TypeBonuses1 typeClass)
    {
        TypeIndex = typeClass.index;
        panel_name = typeClass.name;
        foreach (var x in typeClass.types)
            short_description += x + "\n";

        Header.text = panel_name;
        Description.text = short_description;
    }

    public void ChooseType()
    {
        type_bonus.AddToActiveList(TypeIndex);
        type_bonus.VisualizeSelection(this.gameObject);
    }
    
}
