using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        // 서버 연결을 위한 준비가 완료되 상태일 때
        if (PhotonNetwork.NetworkClientState == ClientState.PeerCreated)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        print("연결");
        PhotonNetwork.JoinOrCreateRoom("UnityChanSurvivors", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        print("현재 방에 있는 플레이어 수: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("새로운 플레이어가 입장했습니다: " + newPlayer.NickName);
        print("현재 방에 있는 플레이어 수: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print("플레이어가 퇴장했습니다: " + otherPlayer.NickName);
        print("현재 방에 있는 플레이어 수: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
}