using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;      //������ �ӵ�
    public float mouseSensitivity = 2f;                 //���콺 ����
    public Rigidbody rb;                                //�÷��̾� ������ٵ�
    public Transform cameraTransform;                   //ī�޶�
    public LayerMask groundMask;                        //�÷��̾ ���� �� 
    private float verticalRotation = 0f;                
    private Vector3 moveDirection;                      

    public Animator animator;       //�÷��̾� �ִϸ��̼�

    public CinemachineVirtualCamera virCamera;      //�ó׸ӽ�ī�޶�

    public GameObject boss;     //���� ĳ����

    bool isDrink;       //������ - ����
    bool isBoss;        //����

    float drinkTimer;   //���� Ÿ�̸�
    float bossTimer;    //���� Ÿ�̸�

    public int Php = 100;       //�÷��̾� HP

    bool isGrounded;
    float jumpTime;
    public float jumpForce;

    private void Awake()
    {

    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //virCamera.m_Lens.Dutch = 180;        //ī�޶� ����
        
    }

    void Update()
    {
        // ���콺 ȸ�� ó��
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // �̵� �Է� ����
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveDirection = transform.right * moveX + transform.forward * moveZ;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f); // �밢�� �̵� �ӵ� ����ȭ

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
        // Y�� �ӵ��� �����ϸ鼭 XZ����� �̵��� ����
        Vector3 targetVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);

        // ���� �ӵ��� ��ǥ �ӵ��� �ε巴�� ����
        Vector3 velocityChange = (targetVelocity - rb.velocity);
        velocityChange.y = 0; // Y�� ��ȭ�� ����

        // �ӵ� ������ �����Ͽ� ����
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
            Debug.Log("�÷��̾� �ְŶ�");
        }
    }


}
