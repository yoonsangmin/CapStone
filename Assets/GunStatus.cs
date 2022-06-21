using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class GunStatus : MonoBehaviour
{
    //public GameObject p_bullet;
 
    public int max_bullet = 30;

    public int cur_bullet;

    private float reload_time = 2.0f;
    public float reload_timer;

    bool is_reload_clicked = false;

    public Vector3 bullet_offset = new Vector3(0.0f, 0.0f, 1.0f);

    private PlayerControl player = null;

    void Start()
    {
        this.player = transform.parent.GetComponent<PlayerControl>();

        cur_bullet = max_bullet;
        reload_timer = reload_time;
    }

    void Update()
    {
        reloading();
    }

    public void shoot_bullet()
    {
        if (cur_bullet > 0)
        {
            //GameObject bullet = BulletPoolManager.Instance.pool.Dequeue();
            GameObject bullet = PhotonNetwork.Instantiate("p_bullet", this.transform.position + transform.right * bullet_offset.x + transform.up * bullet_offset.y + transform.forward * bullet_offset.z, this.transform.rotation);
            BulletControl bullet_control = bullet.GetComponent<BulletControl>();

            //bullet.transform.position = this.transform.position + transform.right * bullet_offset.x + transform.up * bullet_offset.y + transform.forward * bullet_offset.z;
            //bullet.transform.rotation = this.transform.rotation;

            bullet_control.power = 0f;
            bullet_control.speed = 20f;
            bullet_control.range = 20f;
            bullet_control.Fired_position = player.transform.position;

            cur_bullet--;

            player.shoot_timer = 0.0f;

            cancel_reload_bullet();
        }
        else if (!is_reload_clicked)
        {
            reload_bullet();
        }
    }

    public void reload_bullet()
    {
        // 장전 시작
        if (player.bullet1 != 0)
        {
            reload_timer = 0;
            is_reload_clicked = true;
        }
        // 장전 실패
        else
        {

        }
    }

    public void reloading()
    {
        if (is_reload_clicked)
        {
            if (reload_time > reload_timer)
                reload_timer += Time.deltaTime;

            else if (reload_time <= reload_timer)
            {
                if (max_bullet > player.bullet1)
                {
                    cur_bullet += player.bullet1;
                    player.bullet1 = 0;
                }
                else
                {
                    player.bullet1 -= max_bullet - cur_bullet;
                    cur_bullet = max_bullet;
                }
                is_reload_clicked = false;
            }
        }
    }

    public void cancel_reload_bullet()
    {
        reload_timer = 0;
        is_reload_clicked = false;
    }
}
