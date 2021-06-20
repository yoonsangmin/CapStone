using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerControl : MonoBehaviour
{
    private PhotonView PV = null;
    private GameManager GM = null;
    private Camera camera = null;
    private GunStatus gun_status = null;
    private Animator animator = null;

    public float MOVE_SPEED = 7.0f; // �̵� �ӵ�.
    public float DASH_SPEED = 12.0f; // �뽬 �ӵ�.
    public float dash_time = 0.5f; // �뽬 Ÿ��
    public float dash_cool_time = 0.5f; // �뽬 �� Ÿ��


    private struct Key
    { // Ű ���� ���� ����ü.
        public bool up; // ��. W
        public bool down; // ��. S
        public bool right; // ��. D
        public bool left; // ��. A
        public bool dash; // �뽬, SHIFT
        public bool shoot; // ���, ���콺 ���� Ŭ��, ������ ��ġ �ٲٱ� - ���߿� ����

        // ���߿� ������ ��
        public bool reload; // ������, RŰ
        public bool pick; // ������ �ݱ� FŰ?
        public bool drop; // ������ ������ ���콺 ������ Ŭ��?
        public bool useitem1; // ������ ���1, 1
        public bool useitem2; // ������ ���2, 2
        public bool changegun1; // �� ���� 1, Q
        public bool changegun2; // �� ���� 2, E
    };

    private Key key; // Ű ���� ������ �����ϴ� ����.
    private Key afore_key; // ���� Ű ���� ������ �����ϴ� ����.
    private Vector3 move_vector = Vector3.zero; // �̵��� ���� ���� �Է°��� ��� ������ ����
    public float rotate_offset = 0.1f;
    public enum STEP
    { // �÷��̾��� ���¸� ��Ÿ���� ����ü.
        NONE = -1, // ���� ���� ����.
        MOVE = 0, // �̵� ��.
        DASH, // �뽬 ��
        HEALING, // ȸ�� ��
        NUM, // ���°� �� ���� �ִ��� ��Ÿ����
    };

    public STEP step = STEP.NONE; // ���� ����.
    public STEP next_step = STEP.NONE; // ���� ����.
    public float step_timer = 0.0f; // �뽬 Ÿ�̸�.
    public float dash_cool_timer = 0.0f; // �뽬 �� Ÿ�̸�.

    public float shoot_cool_time = 0.2f; // �߻� �� Ÿ��
    public float shoot_timer = 0.0f; // �߻� Ÿ�̸�.

    // ������ ���� - ���߿� Ŭ���� ����
    public int bullet1 = 100;

    void Start()
    {
        this.gun_status = GetComponentInChildren<GunStatus>();
        this.camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        this.PV = GetComponent<PhotonView>();
        this.GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        this.animator = transform.GetComponentInChildren<Animator>();

        this.step = STEP.NONE; // �� �ܰ� ���¸� �ʱ�ȭ.
        this.next_step = STEP.MOVE; // ���� �ܰ� ���¸� �ʱ�ȭ.
        this.dash_cool_timer = dash_cool_time;  // ó�� �� �� �뽬 �����ϰ� �ʱ�ȭ.
    }

    void Update()
    {
        if(PV.IsMine && GM.isPlaying)
        {
            this.get_input(); // �Է� ���� ���.

            this.step_timer += Time.deltaTime;
            this.dash_cool_timer += Time.deltaTime;
            this.shoot_timer += Time.deltaTime;

            // ���¸� ��ȭ��Ų��---------------------.
            if (this.next_step == STEP.NONE)
            { // ���� ������ ������.
                switch (this.step)
                {
                    case STEP.MOVE: // '�̵� ��' ������ ó��.
                        if (this.key.dash && this.dash_cool_timer > dash_cool_time && move_vector != Vector3.zero)
                        {
                            this.next_step = STEP.DASH; //�뽬�� �̵�
                            break;
                        }
                        //���߿� ������ �Դ� �ڵ� © �� �ʿ���
                        //do
                        //{
                        //    //������ �Դ� �ڵ� © �� �ʿ���
                        //    if (!this.key.dash)
                        //    { // �޸��� Ű�� �������� �ʴ�.
                        //        break; // ���� Ż��.
                        //    }
                        //} while (false);
                        break;
                    case STEP.DASH: // '�޸��� ��' ������ ó��.
                        if (this.step_timer > dash_time)
                        { // �뽬Ÿ�� ���
                            this.next_step = STEP.MOVE; // '�̵�' ���·� ����.
                            this.dash_cool_timer = 0.0f;
                        }
                        break;
                }
            }
            // ���°� ��ȭ���� ��------------.
            while (this.next_step != STEP.NONE)
            { // ���°� NONE�̿� = ���°� ��ȭ�ߴ�.
                this.step = this.next_step;
                this.next_step = STEP.NONE;
                switch (this.step)
                {
                    case STEP.MOVE:
                        break;

                    case STEP.DASH: // '�޸��� ��'�� �Ǹ�.
                        break;
                }
                this.step_timer = 0.0f;
            }

            // �� ��Ȳ���� �ݺ��� ��----------.
            switch (this.step)
            {
                case STEP.MOVE:
                    this.move_control();
                    this.lookCamera();

                    
                    if (this.key.shoot)
                    {
                        this.gun_status.shoot_bullet();
                        this.animator.SetTrigger("IsShooting");
                    }
                    if (this.key.reload)
                        this.gun_status.reload_bullet();
                    
                    break;
                case STEP.DASH:
                    // �޸��⸦ �Ѵ�
                    move_vector.Normalize(); // ���̸� 1��.
                    move_vector *= DASH_SPEED * Time.deltaTime; // �ӵ����ð����Ÿ�.
                    this.transform.position += move_vector; // ��ġ�� �̵�.
                    break;
            }


            if(Vector3.Distance(move_vector, Vector3.zero) > 0.01f)
                this.animator.SetBool("IsMoving", true);
            else
                this.animator.SetBool("IsMoving", false);
        }
    }

    // Ű �Է��� ������ �� ����� �������� �ɹ� ���� key�� ���� �����Ѵ�.
    private void get_input()
    {
        // WŰ�� �������� true�� ����.
        this.key.up = Input.GetKey(KeyCode.W);
        // SŰ�� �������� true�� ����.
        this.key.down = Input.GetKey(KeyCode.S);
        // DŰ�� �������� true�� ����.
        this.key.right = Input.GetKey(KeyCode.D);
        // AŰ�� �������� true�� ����..
        this.key.left = Input.GetKey(KeyCode.A);
        // Z Ű�� �������� true�� ����.
        this.key.dash = Input.GetKey(KeyCode.LeftShift);
        // R Ű�� �������� true�� ����.
        this.key.reload = Input.GetKey(KeyCode.R);

        
        // ���콺 ���� Ű�� �������� true�� ����. ������ ������ ��
        this.afore_key.shoot |= Input.GetKeyDown(KeyCode.Mouse0);
        this.key.shoot = false;
       
        if (this.shoot_timer > this.shoot_cool_time)
        {
            this.key.shoot = this.afore_key.shoot;
            this.afore_key.shoot = false;
        }
    }

    // Ű �Է¿� ���� ������ �̵���Ű�� ó���� �Ѵ�.
    private void move_control()
    {
        move_vector = Vector3.zero; // �̵��� ����.

        if (this.key.right)
        { // ��Ű�� ��������.
            move_vector += Vector3.right; // �̵��� ���͸� ���������� ���Ѵ�.
        }
        if (this.key.left)
        {
            move_vector += Vector3.left;
        }
        if (this.key.up)
        {
            move_vector += Vector3.forward;
        }
        if (this.key.down)
        {
            move_vector += Vector3.back;
        }

        move_vector.Normalize(); // ���̸� 1��.
        move_vector *= MOVE_SPEED * Time.deltaTime; // �ӵ����ð����Ÿ�.
        this.transform.position += move_vector; // ��ġ�� �̵�.
    }

    private void lookCamera()
    {
        //���� ����� ���� ���콺�� ���� ������Ʈ�� ������ ��ǥ�� �ӽ÷� �����մϴ�.
        Vector3 oPosition = transform.position; //���� ������Ʈ ��ǥ ����

        Ray inputRay = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit, 100))
        {
            this.transform.LookAt(hit.point);
            this.transform.rotation = Quaternion.Euler(0.0f, this.transform.rotation.eulerAngles.y + rotate_offset, 0.0f);
        }
    }
}
