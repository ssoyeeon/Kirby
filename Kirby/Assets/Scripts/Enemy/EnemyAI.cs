using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public int attackDamage = 10;      //���� �����

    public Transform target;        //�÷��̾�

    NavMeshAgent nav;               //������̼�

    public PlayerController playerController;

    float attackTimer;
    Vector3 dis;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
           
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>(); 
    }

    void Update()
    {
        Move();
        PlayerAttack();
    }

    void Move()
    {
        dis = transform.position - target.position;
        if (dis.magnitude > 5.0f)
        {
            nav.SetDestination(target.position);        //������ ��ǥ ��ġ ���� �Լ���׿�? 
        }
    }

    void PlayerAttack()
    {
        if (dis.magnitude < 2.0f)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= 2)
            {
                playerController.DamagedAttack(attackDamage);
                attackTimer = 0;
            }
        }
    }
}
