using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Scene_switch : MonoBehaviour
{
    //major scene or next scene
    [SerializeField] string default_scene = "Testing_2";

    public void switch_to_scene(string scene_name)
    {
        MonoBehaviour.print(scene_name);
        if(scene_name != "")
            SceneManager.LoadScene(scene_name, LoadSceneMode.Additive);
        else
            SceneManager.LoadScene(default_scene);
        

    }
}
