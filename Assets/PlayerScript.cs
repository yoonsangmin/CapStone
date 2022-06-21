using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerScript : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    void Update()
    {
        if (PV.IsMine)
        {
            float axis = Input.GetAxisRaw("Horizontal");
            transform.Translate(new Vector3(axis * Time.deltaTime * 7, 0, 0));

            //if (axis != 0) PV.RPC("FlipXRPC", RpcTarget.AllBuffered, axis);
        }
    }

}