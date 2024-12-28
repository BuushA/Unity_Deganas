using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouse_event : MonoBehaviour
{
    
    [SerializeField] Upgrade_functions Father;



    private void OnMouseEnter()
    {
        Father.ShowTooltip();
    }

    private void OnMouseExit()
    {
        Father.ShowBase();
    }

}
