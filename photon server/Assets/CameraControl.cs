using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraControl : MonoBehaviour
{
    public GameObject playerObject;

    public Vector3 offsetPosition;

    void Update()
    {
        PlayerMapping();

        if (playerObject != null)
        {
            this.transform.position = playerObject.transform.position + offsetPosition;
        }
    }

    void PlayerMapping()
    {
        if (playerObject == null && GameObject.FindWithTag("Player") != null)
        {
            if (GameObject.FindWithTag("Player").GetComponent<PhotonView>().IsMine)
            {
                playerObject = GameObject.FindWithTag("Player");
                
                this.transform.position = playerObject.transform.position + offsetPosition;
                this.transform.LookAt(playerObject.transform);
            }
        }
    }
}
