using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlayerSkill : MonoBehaviour
{
    PlayerController controller;
    public float beatShotCoolTimer;
    public float powerCoolTimer;
    public float guitarFinisherCoolTimer; 
    public float SonicRoarCoolTimer; 
    public EnemyHealth enemy;
    PlayerController PC;        
    public Camera playerCamera;
    public float attackRange = 3f;
    public LayerMask enemyLayer;

    private void Awake()
    {
        beatShotCoolTimer = 0;
        powerCoolTimer = 0;
        guitarFinisherCoolTimer = 0;
        SonicRoarCoolTimer = 0;
    }

    private void Start()
    {
        enemy = FindObjectOfType<EnemyHealth>();
        PC = FindObjectOfType<PlayerController>();
    }
    void Update()
    {
        if(SonicRoarCoolTimer > 0)
        {
            SonicRoarCoolTimer -= Time.deltaTime; 
            if (SonicRoarCoolTimer <= 0)
            {
                PlayerStates(States.Default);
            }
        }

        if (beatShotCoolTimer > 0)
        {
            beatShotCoolTimer -= Time.deltaTime;
            if (beatShotCoolTimer <= 0)
            {
                PlayerStates(States.Default);
            }
        }

        if (powerCoolTimer > 0)
        {
            powerCoolTimer -= Time.deltaTime;
            if (powerCoolTimer <= 0)
            {
                PlayerStates(States.Default);
            }
        }

        if(guitarFinisherCoolTimer > 0)
        {
            guitarFinisherCoolTimer -= Time.deltaTime; 
            if (guitarFinisherCoolTimer <= 0)
            {
                PlayerStates(States.Default);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlayerStates(States.SonicRoar);
        }

        if(Input.GetKey(KeyCode.Q))
        {
            PlayerStates(States.PowerRoar);

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerStates(States.BeatShot);

        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            PlayerStates(States.GuitarFinisher);

        }
    }
    void PlayerStates(States states)
    {
        switch(states)
        {
            case States.SonicRoar:
                Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (Physics.Raycast(ray, out RaycastHit hit, attackRange, enemyLayer))
                {
                    enemy.TakeDamage(10 , true);
                }
                break;
            case States.PowerRoar:
                float powerTimer = 3f;
                powerTimer -= Time.deltaTime;
                if(powerTimer <= 0)
                {
                    //�ƴ� �̰� ������ ��� ���ؿ�?? 
                    Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
                    
                    foreach (Collider enemys in enemies)
                    {
                        Vector3 target = (enemys.transform.position - transform.position).normalized;
                        float angle = Vector3.Angle(transform.forward, target);

                        if(angle < 90f / 2)
                        {
                            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                            if (enemyHealth != null)
                            {
                                enemyHealth.TakeDamage(10, true);
                            }
                        }
                    }
                }
                break;
            case States.BeatShot:
                this.GetComponent<RhythmAttackSystem>().enabled = true;
                break;
            case States.GuitarFinisher:
                float guitarTimer = 3f;
                guitarTimer -= Time.deltaTime;
                if (guitarTimer <= 0)
                {
                    Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

                    foreach (Collider enemy in enemies)
                    {
                        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                        if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(10, true);
                        }
                    }
                }
                break;
            case States.Default:
                this.GetComponent<RhythmAttackSystem>().enabled = false;
                break;
        }
    }

    public enum States
    {
        SonicRoar,      //�⺻ ���� -> �Կ��� ���� ���� �߻�
        PowerRoar,      //���� ���� (��ų) -> ��� ���� ���� �� ���� ��ä�� ���� ����
        BeatShot,       //���� ���� ���� (��ų) -> BGM Ÿ�ֿ̹� ���� ������ ������ ���� ����
        GuitarFinisher,  //�ñر� (��ų) -> ���� ���� ���� + ����
        Default,
    }
}
