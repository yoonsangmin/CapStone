using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text StatusText;
    public InputField roomInput, NickNameInput;

    private byte maxPlayer = 2;
    List<RoomInfo> roomInfos = null;

    void Awake() => Screen.SetResolution(960, 540, false);


    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();

     /*
        //�������� ���н� �� ����
        if (isPressed == true)
        {
            if (isJoined == false)
            {
                int rand = Random.Range(0, 10);
                PhotonNetwork.CreateRoom(rand.ToString(), new RoomOptions { MaxPlayers = maxPlayer });
                print("hi");

                isPressed = false;
            }
        }
    */
    }
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        print("�������ӿϷ�");
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause) => print("�������");

    public void JoinLobby() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby() => print("�κ����ӿϷ�");


    public void CreateRoon() => PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = maxPlayer } );

    public void OneButtonJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoom() => PhotonNetwork.JoinRoom(roomInput.text);

    public void JoinOrCreateRoom() => PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 }, null);

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnCreatedRoom()
    {
        print("�游���Ϸ�");
    }
    public override void OnJoinedRoom() 
    { 
        print("�������Ϸ�");
        PhotonNetwork.LoadLevel("TestScene");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) => print("�游������");

    public override void OnJoinRoomFailed(short returnCode, string message) => print("����������");

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RandomCreate();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        print("hawi^^");
        //roomInfos.Clear();
        roomInfos = roomList;

        if (roomList != null)
        {
            foreach (RoomInfo item in roomList)
            {
                print(item.Name);
            }
        }
    }

    public void RandomCreate()
    {
        int rand;
        bool isOverlapped;
        print(PhotonNetwork.CountOfPlayersInRooms);

        do
        {
            isOverlapped = false;
            rand = Random.Range(0, int.MaxValue);
            if (roomInfos != null)
            {
                foreach (RoomInfo item in roomInfos)
                {
                    print(item.Name);
                    if (item.Name == rand.ToString())
                    {
                        isOverlapped = true;
                        break;
                    }
                }
            }
        } while (isOverlapped);

        print("�淣����������");
        PhotonNetwork.CreateRoom(rand.ToString(), new RoomOptions { MaxPlayers = maxPlayer });

        print(PhotonNetwork.CountOfPlayersInRooms);
    }

    [ContextMenu("����")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("���� �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
            print("���� �� �ο��� : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("���� �� �ִ��ο��� : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "�濡 �ִ� �÷��̾� ��� : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            }
            print(playerStr);
        }

        else
        {
            print("������ �ο� �� : " + PhotonNetwork.CountOfPlayers);
            print("�� ���� : " + PhotonNetwork.CountOfRooms);
            print("��� �濡 �ִ� �ο� �� : " + PhotonNetwork.CountOfPlayersInRooms);
            print("�κ� �ִ���? : " + PhotonNetwork.InLobby);
            print("����ƴ���? : " + PhotonNetwork.IsConnected);
        }
    }
}
