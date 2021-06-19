using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class BulletControl : MonoBehaviour
{
    GameObject go;

    public float speed = 20.0f;
    public float power;
    public float range = 20.0f;

    private Vector3 fired_position;
    public Vector3 Fired_position { set { fired_position = value; } }

    void Start()
    {
  
    }

    private void OnTriggerEnter(Collider other)
    {
        go = other.gameObject;
        GetComponent<PhotonView>().RPC("PlayerDestroyerRPC", RpcTarget.AllBuffered);
        GetComponent<PhotonView>().RPC("BulletDestroyerRPC", RpcTarget.AllBuffered);
    }

    void Update()
    {
        this.transform.Translate(new Vector3(0.0f, 0.0f, this.speed * Time.deltaTime));

        if (Vector3.Distance(this.transform.position, this.fired_position) >= this.range)
        {
            //Enqueue();
            GetComponent<PhotonView>().RPC("BulletDestroyerRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void BulletDestroyerRPC()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    void PlayerDestroyerRPC()
    {
        Destroy(go);
    }
}
