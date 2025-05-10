using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("연결");
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinedRoom()
    {
        print("입장");
    }
}