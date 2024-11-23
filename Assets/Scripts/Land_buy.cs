using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Land_buy : MonoBehaviour
{
    private GameObject Grid;

    private Region_grid script_grid;





    public void getData(Region_grid script)
    {
        Grid = script.gameObject;
        script_grid = script;
    }



    private void OnMouseDown() 
    {

        float grid_x = Grid.transform.position.x / 24;
        float grid_y = Grid.transform.position.y / 24;
        Vector2 mouse_pos = mouse_pos = this.transform.position / 24;
        int x_pos, y_pos;

        x_pos = ((int)mouse_pos.x - (int)grid_x);
        y_pos = ((int)mouse_pos.y - (int)grid_y);



        //MonoBehaviour.print("grid coords: " + $"{grid_x}, " + $"{grid_y} &" + $"{mouse_pos.x}," + $"{mouse_pos.y}");
        //MonoBehaviour.print("Coordinates: " + $"{x_pos}, " + $"{y_pos}");

        script_grid.Buy_Deganas(x_pos, y_pos);


    }



}
