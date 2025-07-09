using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("��ų ����")]
    public SkillTimer[] skills = new SkillTimer[4];

    [Header("Ű ����")]
    public KeyCode[] skillKeys = { KeyCode.Mouse0, KeyCode.W, KeyCode.E, KeyCode.R };

    PlayerController controller;
    public float powerHoldTimer;
    public float guitarFinisherEndTimer;
    public EnemyHealth enemy;
    PlayerController PC;
    public Camera playerCamera;
    public float attackRange = 3f;
    public float Etimer;
    public LayerMask enemyLayer;

    public GameObject Prefab;
    public GameObject Prefab2;
    
    void Start()
    {
        // �ʱ� UI ���� (��� ��ų ��� ���� ����)
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].cooldownImage != null)
                skills[i].cooldownImage.fillAmount = 1f;
            if (skills[i].cooldownText != null)
                skills[i].cooldownText.text = "";
        }
        ExecuteSkill(4);
    }

    void Update()
    {
        // Ÿ�̸� ������Ʈ
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].UpdateTimer(Time.deltaTime);
        }

        // Ű �Է� ó��
        for (int i = 0; i < skillKeys.Length && i < skills.Length; i++)
        {
            if (Input.GetKey(skillKeys[1]))
            {
                powerHoldTimer -= Time.deltaTime;
                if (powerHoldTimer <= 0)
                {
                    UseSkill(1);
                    powerHoldTimer = 3;
                }
                
            }
            if (Input.GetKeyUp(skillKeys[1]))
            {
                powerHoldTimer = 3;
            }
            if (Input.GetKeyDown(skillKeys[2]))
            {
                UseSkill(2);
                Etimer = 10;
                
            }

            if (Input.GetKeyDown(skillKeys[0]))
            {
                UseSkill(0);
            }
            if (Input.GetKeyDown(skillKeys[3]))
            {
                UseSkill(3);
            }
        }
        if(Etimer >= 0)
        {
            Etimer -= Time.deltaTime; 
            if (Etimer <= 0)
            {
                ExecuteSkill(4);
            }
        }
    }

    public void UseSkill(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Length) return;

        SkillTimer skill = skills[skillIndex];

        if (skill.IsReady)
        {
            Debug.Log($"{skill.skillName} ��ų ���!");

            // ���⿡ ���� ��ų ���� �߰�
            ExecuteSkill(skillIndex);

            // ��Ÿ�� ����
            skill.StartCooldown();
        }
        else
        {
            Debug.Log($"{skill.skillName} ���� ��Ÿ�� �� ({skill.currentCooldown:F1}�� ����)");
        }
    }

    // ���� ��ų ���� ����
    void ExecuteSkill(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:     //�⺻ ���� ���콺 ��Ŭ��
                Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (Physics.Raycast(ray, out RaycastHit hit, attackRange, enemyLayer))
                {
                    EnemyHealth enemey = hit.collider.GetComponent<EnemyHealth>();
                    Instantiate(Prefab, this.gameObject.transform.position + new Vector3(0,1,0), Quaternion.LookRotation(this.transform.forward));
                    enemey.TakeDamage(10, true);
                }
                break;
            case 1:     //��¡ ���� Q �� 
                Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

                foreach (Collider enemys in enemies)
                {
                    Vector3 target = (enemys.transform.position - transform.position).normalized;
                    float angle = Vector3.Angle(transform.forward, target);

                    if (angle < 90f / 2)
                    {
                        EnemyHealth enemyHealth = enemys.GetComponent<EnemyHealth>();
                        if (enemyHealth != null)
                        {
                            Instantiate(Prefab2, this.gameObject.transform.position , Quaternion.LookRotation(this.transform.forward));
                            Destroy(Prefab2);
                            enemyHealth.TakeDamage(10, true);
                        }
                    }
                }
                break;
            case 2:     //���� ���� E 
                this.GetComponent<RhythmAttackSystem>().enabled = true;
                break; 
            case 3:     //�ñر�  R
                Collider[] enemiess = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
                foreach (Collider enemy in enemiess)
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(10, true);
                    }
                }
                break;
            case 4:     //����Ʈ 
                this.GetComponent<RhythmAttackSystem>().enabled = false;
                break;
        }
    }

    // �ܺο��� ��ų ���� Ȯ�ο�
    public bool IsSkillReady(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Length) return false;
        return skills[skillIndex].IsReady;
    }

    // ���� ��Ÿ�� Ȯ�ο�
    public float GetRemainingCooldown(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Length) return 0f;
        return skills[skillIndex].currentCooldown;
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
