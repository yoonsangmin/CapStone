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

    public float MOVE_SPEED = 7.0f; // 이동 속도.
    public float DASH_SPEED = 12.0f; // 대쉬 속도.
    public float dash_time = 0.5f; // 대쉬 타임
    public float dash_cool_time = 0.5f; // 대쉬 쿨 타임


    private struct Key
    { // 키 조작 정보 구조체.
        public bool up; // ↑. W
        public bool down; // ↓. S
        public bool right; // →. D
        public bool left; // ←. A
        public bool dash; // 대쉬, SHIFT
        public bool shoot; // 쏘기, 마우스 왼쪽 클릭, 아이템 위치 바꾸기 - 나중에 구현

        // 나중에 구현할 것
        public bool reload; // 재장전, R키
        public bool pick; // 아이템 줍기 F키?
        public bool drop; // 아이템 버리기 마우스 오른쪽 클릭?
        public bool useitem1; // 아이템 사용1, 1
        public bool useitem2; // 아이템 사용2, 2
        public bool changegun1; // 총 변경 1, Q
        public bool changegun2; // 총 변경 2, E
    };

    private Key key; // 키 조작 정보를 보관하는 변수.
    private Key afore_key; // 다음 키 조작 정보를 보관하는 변수.
    private Vector3 move_vector = Vector3.zero; // 이동용 벡터 이전 입력값을 계속 가지고 있음
    public float rotate_offset = 0.1f;
    public enum STEP
    { // 플레이어의 상태를 나타내는 열거체.
        NONE = -1, // 상태 정보 없음.
        MOVE = 0, // 이동 중.
        DASH, // 대쉬 중
        HEALING, // 회복 중
        NUM, // 상태가 몇 종류 있는지 나타낸다
    };

    public STEP step = STEP.NONE; // 현재 상태.
    public STEP next_step = STEP.NONE; // 다음 상태.
    public float step_timer = 0.0f; // 대쉬 타이머.
    public float dash_cool_timer = 0.0f; // 대쉬 쿨 타이머.

    public float shoot_cool_time = 0.2f; // 발사 쿨 타임
    public float shoot_timer = 0.0f; // 발사 타이머.

    // 아이템 관련 - 나중에 클래스 빼기
    public int bullet1 = 100;

    void Start()
    {
        this.gun_status = GetComponentInChildren<GunStatus>();
        this.camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        this.PV = GetComponent<PhotonView>();
        this.GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        this.animator = transform.GetComponentInChildren<Animator>();

        this.step = STEP.NONE; // 현 단계 상태를 초기화.
        this.next_step = STEP.MOVE; // 다음 단계 상태를 초기화.
        this.dash_cool_timer = dash_cool_time;  // 처음 들어갈 때 대쉬 가능하게 초기화.
    }

    void Update()
    {
        if(PV.IsMine && GM.isPlaying)
        {
            this.get_input(); // 입력 정보 취득.

            this.step_timer += Time.deltaTime;
            this.dash_cool_timer += Time.deltaTime;
            this.shoot_timer += Time.deltaTime;

            // 상태를 변화시킨다---------------------.
            if (this.next_step == STEP.NONE)
            { // 다음 예정이 없으면.
                switch (this.step)
                {
                    case STEP.MOVE: // '이동 중' 상태의 처리.
                        if (this.key.dash && this.dash_cool_timer > dash_cool_time && move_vector != Vector3.zero)
                        {
                            this.next_step = STEP.DASH; //대쉬로 이동
                            break;
                        }
                        //나중에 아이템 먹는 코드 짤 때 필요함
                        //do
                        //{
                        //    //아이템 먹는 코드 짤 때 필요함
                        //    if (!this.key.dash)
                        //    { // 달리기 키가 눌려있지 않다.
                        //        break; // 루프 탈출.
                        //    }
                        //} while (false);
                        break;
                    case STEP.DASH: // '달리기 중' 상태의 처리.
                        if (this.step_timer > dash_time)
                        { // 대쉬타임 대기
                            this.next_step = STEP.MOVE; // '이동' 상태로 이행.
                            this.dash_cool_timer = 0.0f;
                        }
                        break;
                }
            }
            // 상태가 변화했을 때------------.
            while (this.next_step != STEP.NONE)
            { // 상태가 NONE이외 = 상태가 변화했다.
                this.step = this.next_step;
                this.next_step = STEP.NONE;
                switch (this.step)
                {
                    case STEP.MOVE:
                        break;

                    case STEP.DASH: // '달리기 중'이 되면.
                        break;
                }
                this.step_timer = 0.0f;
            }

            // 각 상황에서 반복할 것----------.
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
                    // 달리기를 한다
                    move_vector.Normalize(); // 길이를 1로.
                    move_vector *= DASH_SPEED * Time.deltaTime; // 속도×시간＝거리.
                    this.transform.position += move_vector; // 위치를 이동.
                    break;
            }


            if(Vector3.Distance(move_vector, Vector3.zero) > 0.01f)
                this.animator.SetBool("IsMoving", true);
            else
                this.animator.SetBool("IsMoving", false);
        }
    }

    // 키 입력을 조사해 그 결과를 바탕으로 맴버 변수 key의 값을 갱신한다.
    private void get_input()
    {
        // W키가 눌렸으면 true를 대입.
        this.key.up = Input.GetKey(KeyCode.W);
        // S키가 눌렸으면 true를 대입.
        this.key.down = Input.GetKey(KeyCode.S);
        // D키가 눌렸으면 true를 대입.
        this.key.right = Input.GetKey(KeyCode.D);
        // A키가 눌렸으면 true를 대입..
        this.key.left = Input.GetKey(KeyCode.A);
        // Z 키가 눌렸으면 true를 대입.
        this.key.dash = Input.GetKey(KeyCode.LeftShift);
        // R 키가 눌렸으면 true를 대입.
        this.key.reload = Input.GetKey(KeyCode.R);

        
        // 마우스 왼쪽 키가 눌렸으면 true를 대입. 딜레이 만들어야 함
        this.afore_key.shoot |= Input.GetKeyDown(KeyCode.Mouse0);
        this.key.shoot = false;
       
        if (this.shoot_timer > this.shoot_cool_time)
        {
            this.key.shoot = this.afore_key.shoot;
            this.afore_key.shoot = false;
        }
    }

    // 키 입력에 따라 실제로 이동시키는 처리를 한다.
    private void move_control()
    {
        move_vector = Vector3.zero; // 이동용 벡터.

        if (this.key.right)
        { // →키가 눌렸으면.
            move_vector += Vector3.right; // 이동용 벡터를 오른쪽으로 향한다.
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

        move_vector.Normalize(); // 길이를 1로.
        move_vector *= MOVE_SPEED * Time.deltaTime; // 속도×시간＝거리.
        this.transform.position += move_vector; // 위치를 이동.
    }

    private void lookCamera()
    {
        //먼저 계산을 위해 마우스와 게임 오브젝트의 현재의 좌표를 임시로 저장합니다.
        Vector3 oPosition = transform.position; //게임 오브젝트 좌표 저장

        Ray inputRay = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit, 100))
        {
            this.transform.LookAt(hit.point);
            this.transform.rotation = Quaternion.Euler(0.0f, this.transform.rotation.eulerAngles.y + rotate_offset, 0.0f);
        }
    }
}
