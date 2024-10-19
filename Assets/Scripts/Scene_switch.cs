using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Runtime.Serialization;
using TMPro;

public class Scene_switch : MonoBehaviour
{
    //major scene or next scene
    [SerializeField] string default_scene = "Testing_2";
    [SerializeField] Global_values GB_script;
    [SerializeField] TMP_Text button_label;




    bool active_message = false;
    //Coroutine for delaying
    IEnumerator label_message(float delay, string msg)
    {
        string tmp = button_label.text;
        button_label.text = msg;
        yield return new WaitForSeconds(delay);
        button_label.text = tmp;
        active_message = false;
    }

    public void switch_to_scene(string scene_name)
    {
        MonoBehaviour.print(scene_name);

        if(GB_script.Dic_item_amount.Count == 0)
        {
            active_message = true;
            StartCoroutine(label_message(1.5f, "Stock UP!!!"));
        }
        //has items bought
        else
        {
            if(scene_name != "")
                SceneManager.LoadScene(scene_name, LoadSceneMode.Additive);
            else
                SceneManager.LoadScene(default_scene);
        }
    }
}
