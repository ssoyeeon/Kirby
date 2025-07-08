using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public int attackDamage = 10;      //공격 대미지

    public Transform target;        //플레이어

    NavMeshAgent nav;               //내비게이션

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
            nav.SetDestination(target.position);        //도착할 목표 위치 지정 함수라네요? 
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
