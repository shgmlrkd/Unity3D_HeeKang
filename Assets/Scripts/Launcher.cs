using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        // ���� ������ ���� �غ� �Ϸ�� ������ ��
        if (PhotonNetwork.NetworkClientState == ClientState.PeerCreated)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        print("����");
        PhotonNetwork.JoinOrCreateRoom("UnityChanSurvivors", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        print("���� �濡 �ִ� �÷��̾� ��: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("���ο� �÷��̾ �����߽��ϴ�: " + newPlayer.NickName);
        print("���� �濡 �ִ� �÷��̾� ��: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print("�÷��̾ �����߽��ϴ�: " + otherPlayer.NickName);
        print("���� �濡 �ִ� �÷��̾� ��: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
}