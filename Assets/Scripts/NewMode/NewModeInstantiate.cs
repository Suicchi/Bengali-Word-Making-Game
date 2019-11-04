using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewModeInstantiate : MonoBehaviourPun
{
    [SerializeField]
    GameObject inputPanel, playboardPanel, gridslotPrefab, gridslotTxtPrefab;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        inputPanel.GetPhotonView().RPC("GenerateInputByMaster", RpcTarget.MasterClient);
        var currentTurn = PhotonNetwork.PlayerListOthers[0];
    }

    

}
