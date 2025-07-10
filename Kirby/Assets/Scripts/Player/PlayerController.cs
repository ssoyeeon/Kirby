using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;      //움직일 속도
    public float mouseSensitivity = 2f;                 //마우스 감도
    public Rigidbody rb;                                //플레이어 리지드바디
    public Transform cameraTransform;                   //카메라
    public LayerMask groundMask;                        //플레이어가 밟을 땅 
    private float verticalRotation = 0f;                
    private Vector3 moveDirection;                      

    public Animator animator;       //플레이어 애니메이션

    public CinemachineVirtualCamera virCamera;      //시네머신카메라

    public GameObject boss;     //보스 캐릭터

    bool isDrink;       //아이템 - 음료
    bool isBoss;        //보스

    float drinkTimer;   //음료 타이머
    float bossTimer;    //보스 타이머

    public int Php = 100;       //플레이어 HP

    bool isGrounded;
    float jumpTime;
    public float jumpForce;

    private void Awake()
    {

    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //virCamera.m_Lens.Dutch = 180;        //카메라 반전
        
    }

    void Update()
    {
        // 마우스 회전 처리
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 이동 입력 저장
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveDirection = transform.right * moveX + transform.forward * moveZ;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f); // 대각선 이동 속도 정규화

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true && jumpTime <= 0)
        {
            rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
            jumpTime = 1f;
            isGrounded = false;
            Debug.Log("Jump");
        }
        else if (isGrounded == false)
        {
            jumpTime -= Time.deltaTime;
            if (jumpTime < 0)
            {
                jumpTime = 0;
                isGrounded = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //SceneManager.LoadScene("New Scene");
        }

        if(isDrink == true)
        {
            drinkTimer -= Time.deltaTime;
            moveSpeed = 7;
            if (drinkTimer <= 0)
            {
                moveSpeed = 5;
                drinkTimer = 0;
                isDrink = false;
            }
        }
        else moveSpeed = 5;

        if (isBoss == true)
        {
            bossTimer -= Time.deltaTime;
            virCamera.m_LookAt.position = boss.transform.position;
            if (bossTimer <= 0)
            {
                bossTimer = 0;
                virCamera.m_LookAt.position = this.transform.position;
                isBoss = false;
            }
        }
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        // Y축 속도는 유지하면서 XZ평면의 이동만 조정
        Vector3 targetVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);

        // 현재 속도와 목표 속도를 부드럽게 보간
        Vector3 velocityChange = (targetVelocity - rb.velocity);
        velocityChange.y = 0; // Y축 변화는 무시

        // 속도 변경을 제한하여 적용
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Tomato"))
        {
            isDrink = true;
            Destroy(other.gameObject);
            drinkTimer = 1f;
        }
        if(other.CompareTag("Save1"))
        {

        }
        if(other.CompareTag("Boss"))
        {
            isBoss = true;
            bossTimer = 3;
        }
    }

    public void DamagedAttack(int Damage)
    {
        Php -= Damage;
        if(Php <= 0)
        {
            Debug.Log("플레이어 주거땅");
        }
    }


}
