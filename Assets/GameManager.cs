using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;


public class GameManager : MonoBehaviour
{
    //bool once = false;
    private GameObject sheriff;   //�÷��̾� ������Ʈ
    private GameObject gangster;   //�÷��̾� ������Ʈ

    public GameObject sheriff_start_position;  //�÷��̾� ĭ
    public GameObject gangster_start_position;   //�÷��̾� ������Ʈ

    PhotonView PV;

    public bool isPlaying = false;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        var localPleyerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;


        Debug.Log("GM called!");

        if (localPleyerIndex == 0)
        {
            Debug.Log("Index = 0");
            sheriff = PhotonNetwork.Instantiate("Sheriff", sheriff_start_position.transform.position, Quaternion.identity);
        }

        else if (localPleyerIndex == 1)
        {
            gangster = PhotonNetwork.Instantiate("Gangster", gangster_start_position.transform.position, Quaternion.identity);
        }

        //PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && isPlaying == false)
        {
            PV.RPC("PlayingCheckerRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void PlayingCheckerRPC()
    {
        isPlaying = true;
    }
}
