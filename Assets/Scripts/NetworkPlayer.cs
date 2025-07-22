using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{

    NetworkObject netObj;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            return;
        else
            GameLog.Message("New client connected");

        netObj = GetComponent<NetworkObject>();
        Global_values.localID = (int)NetworkManager.Singleton.LocalClientId;
        GameLog.Message("Assigned LocalId - " + $"{Global_values.localID}");
    }

}
