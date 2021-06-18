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
        //랜덤참가 실패시 방 생성
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
        print("서버접속완료");
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause) => print("연결끊김");

    public void JoinLobby() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby() => print("로비접속완료");


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
        print("방만들기완료");
    }
    public override void OnJoinedRoom() 
    { 
        print("방참가완료");
        PhotonNetwork.LoadLevel("TestScene");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) => print("방만들기실패");

    public override void OnJoinRoomFailed(short returnCode, string message) => print("방참가실패");

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

        print("방랜덤참가실패");
        PhotonNetwork.CreateRoom(rand.ToString(), new RoomOptions { MaxPlayers = maxPlayer });

        print(PhotonNetwork.CountOfPlayersInRooms);
    }

    [ContextMenu("정보")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("현재 방 최대인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "방에 있는 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            }
            print(playerStr);
        }

        else
        {
            print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            print("방 갯수 : " + PhotonNetwork.CountOfRooms);
            print("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            print("로비에 있는지? : " + PhotonNetwork.InLobby);
            print("연결됐는지? : " + PhotonNetwork.IsConnected);
        }
    }
}
