using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Threading.Tasks;

public class networkstartgame : MonoBehaviour
{
    Main_Scene_Manager sceneManager;
    NetworkServer netServer;
    LabelMan labelManager;


    [SerializeField] TMP_Text Button_label;
    GameObject button;
    void Start()
    {
        sceneManager = Main_Scene_Manager.reference;
        labelManager = LabelMan.reference;
        netServer = NetworkServer.reference;
        Button_label.text = "Ready: " + netServer.ReadyCount.ToString() + "/" + NetworkServer.player_count.ToString();
    }

    public void JoinClient(GameObject button)
    {
        NetworkManager.Singleton.StartClient();
        button.SetActive(false);
    }

    public void startGame(GameObject clickedButton)
    {
        button = clickedButton;
        int player_id = (int)NetworkManager.Singleton.LocalClientId;
        netServer.requestJoinedToServer((int)NetworkServer.Scenes.Start, (int)LabelMan.ReadyLabels.Start);
        Button_label.text = "Ready: " + netServer.ReadyCount.ToString() + "/" + NetworkServer.player_count.ToString();
        //    if (netServer.canStart == true)
        //    {
        //        sceneManager.Revert_scenes();
        //        button.SetActive(false);
        //        netServer.canStart = false;
        //    }
    }


    public void activateGame()
    {
        sceneManager.Revert_scenes();
        button.SetActive(false); 
    }
}
