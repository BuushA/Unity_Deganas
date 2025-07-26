using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using TMPro;

public class NetworkServer : NetworkBehaviour
{

    //Other scripts for easy reference
    [SerializeField] networkstartgame startGame;
    [SerializeField] Main_Scene_Manager scene_Manager;
    [SerializeField] Global_values GB_script;
    [SerializeField] LabelMan labelManager;

    public bool canStart = false;
    private List<bool> PlayerChecks = new List<bool>();

    List<long> AllProfit = new List<long>();
    List<long> ShortTermProfit = new List<long>();
    List<long> BoughtAssets = new List<long>();






    public enum Scenes
    {
        Start = 0,
        ExitOverview = 1,
        Work = 2
    }

    public static NetworkServer reference;
    public delegate void BasicDelegate();

    public const int player_count = 2;
    public int ReadyCount = 0;

    void Awake()
    {
        reference = this;
    }


    void Start()
    {
        InitilizeLongList(AllProfit);
        InitilizeLongList(ShortTermProfit);
        InitilizeLongList(BoughtAssets);
        InitilizeBoolList(PlayerChecks);

    }

    public void BeAServer()
    {
        if (!IsServer)
            NetworkManager.Singleton.StartServer();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            GameLog.Message("Not server");
        else
            GameLog.Message("Server started");

        Global_values.localID = (int)NetworkManager.Singleton.LocalClientId;
        GameLog.Message("Assigned LocalId - " + $"{Global_values.localID}");
    }

    private void InitilizeLongList(List<long> L)
    {
        for (int i = 0; i < player_count; i++)
        {
            L.Add(0);
        }
    }

    private void InitilizeBoolList(List<bool> L)
    {
        for (int i = 0; i < player_count; i++)
            L.Add(false);
    }

    public void requestPingToServer()
    {
        pingServerRpc();
    }

    public void requestPingToClients()
    {
        pingClientRpc();
    }

    public void requestJoinedToServer(int scene_id, int label_id)
    {
        int id = Global_values.localID;
        //Upgrade scene_id customization later
        playerJoinedRpc(id, scene_id, label_id);
    }

    public void requestUpdateProfit(int ClientId, long amount)
    {
        UpdateProfitRpc(ClientId, amount);
    }

    public void requestResetProfit()
    {
        ResetProfitRpc();
    }

    //Not secure, need to implement other check later
    [Rpc(SendTo.Server)]
    public void UpdateAssetsRpc(int ClientId, long sum)
    {
        BoughtAssets[ClientId] = sum;
    }

    [Rpc(SendTo.Server)]
    public void requestGetOpponentStockRpc(int ClientId)
    {
        //Get opponents ID
        int OpponentsID = new int();
        for (int i = 1; i <= player_count; i++)
        {
            if (i != ClientId) { OpponentsID = i; }
        }

        for (int i = 0; i < player_count; i++)
        {
            GameLog.Message($"All={AllProfit[i]} ; Short={ShortTermProfit[i]}; ASSETS={BoughtAssets[i]}");
        }
        
        float procentage = 0.25f;
        long Property = Global_values.Starting_station_price + BoughtAssets[OpponentsID - 1];
        long Profits = ShortTermProfit[OpponentsID - 1] + (long)(AllProfit[OpponentsID - 1] * procentage);

        GetOpponentStockPriceRpc(Property, Profits, RpcTarget.Single((ulong)ClientId, RpcTargetUse.Temp));
    }


    [Rpc(SendTo.Server)]
    private void pingServerRpc()
    {
        GameLog.Message("Ping Ping Ping!");
    }

    [Rpc(SendTo.NotServer)]
    private void pingClientRpc()
    {
        GameLog.Message("Wake up Wake up");
    }

    [Rpc(SendTo.Server)]
    private void playerJoinedRpc(int id, int scene_id, int label_id)
    {
        id -= 1; //standardize, Only server is 0
                 //Upgrade later to allow deselecting

        //    foreach (var x in PlayerChecks)
        //    {
        //        GameLog.Message($"A: BEFORE player can press? {x}");
        //    }
        if (PlayerChecks[id] == false)
        {
            ReadyCount += 1;
            UpdateReadyCountRpc(label_id);
            if (player_count == ReadyCount)
            {
                ReadyCount = 0;
                SendConfirmationRpc(scene_id);
                for (int i = 0; i < player_count; i++)
                    PlayerChecks[i] = false;
            }
            else
                PlayerChecks[id] = true;

            //  foreach (var x in PlayerChecks)
            //  {
            //      GameLog.Message($"Z: AFTER player can press? {x}");
            //  }
        }
        else
            GameLog.Message("Already confirmed");

        GameLog.Message($"Player with id={id + 1}; Tottal count= {ReadyCount}");
    }

    [Rpc(SendTo.NotServer)]
    private void SendConfirmationRpc(int scene_id)
    {
        GameLog.Message("Everyone ready");
        ReadyCount = 0;
        switch (scene_id)
        {
            case (int)Scenes.Start:
                startGame.activateGame();
                labelManager.UpdateReadyLabel((int)LabelMan.ReadyLabels.Start, ReadyCount, player_count);
                break;
            case (int)Scenes.Work:
                scene_Manager.activateWork();
                labelManager.UpdateReadyLabel((int)LabelMan.ReadyLabels.Work, ReadyCount, player_count);
                break;
            case (int)Scenes.ExitOverview:
                scene_Manager.CloseOverview();
                labelManager.UpdateReadyLabel((int)LabelMan.ReadyLabels.ExitOverview, ReadyCount, player_count);
                break;
        }
    }

    [Rpc(SendTo.NotServer)]
    private void UpdateReadyCountRpc(int label_id)
    {
        ReadyCount += 1;
        labelManager.UpdateReadyLabel(label_id, ReadyCount, player_count);
    }

    //To make it completely server side for security
    //The whole calculation logic must be moved to the server
    //This does not protect from someone modifying (in mem.) products bought and increasing their profits
    [Rpc(SendTo.Server)]
    public void UpdateProfitRpc(int ClientId, long amount)
    {
        ClientId -= 1;
        AllProfit[ClientId] += amount;
        ShortTermProfit[ClientId] += amount;
        GameLog.Message($"Adding profit to {ClientId} with amount of {amount}");
        GameLog.Message($"Total profit {AllProfit[ClientId]} and This week {ShortTermProfit[ClientId]}");
    }

    [Rpc(SendTo.Server)]
    private void ResetProfitRpc()
    {
        for (int i = 0; i < player_count; i++)
            ShortTermProfit[i] = 0;

        GameLog.Message("Reseting Profit");
    }

    //Standardize with clients
    //Recheck if turn add should be after or before
    [Rpc(SendTo.Server)]
    public void UpdateTurnsRpc()
    {
        Global_values.turns += 1;
    }

    public void OnTurnReset()
    {
        ResetProfitRpc();
    }   


    [Rpc(SendTo.SpecifiedInParams)]
    private void GetOpponentStockPriceRpc(long Property, long Profits, RpcParams rpcParams)
    {

        Global_values.OpponentStock = Property + Profits;
        //Not secure, easy to exploit but it needs a rewrite to the server side 
        GameLog.Message($"{rpcParams}");
        GameLog.Message($"Opponents stock is worth {Global_values.OpponentStock}");
    }

    [Rpc(SendTo.Server)]
    public void StockBoughtRpc(int ClientId, long money)
    {
        UpdateProfitRpc(ClientId, money);
        SendMoneyToPlayerRpc(money, ClientId, RpcTarget.Single((ulong)ClientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void SendMoneyToPlayerRpc(long money, int ClientId, RpcParams rpcParams)
    {
        //Get opponents ID
        int OpponentsID = new int();
        for (int i = 1; i <= player_count; i++)
        {
            if (i != ClientId) { OpponentsID = i; }
        }

        Global_values.money += money;
        UpdateProfitRpc(OpponentsID, money);
    }


    
}
