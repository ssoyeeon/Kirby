using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("스킬 설정")]
    public SkillTimer[] skills = new SkillTimer[4];

    [Header("키 설정")]
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
        // 초기 UI 설정 (모든 스킬 사용 가능 상태)
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
        // 타이머 업데이트
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].UpdateTimer(Time.deltaTime);
        }

        // 키 입력 처리
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
            Debug.Log($"{skill.skillName} 스킬 사용!");

            // 여기에 실제 스킬 로직 추가
            ExecuteSkill(skillIndex);

            // 쿨타임 시작
            skill.StartCooldown();
        }
        else
        {
            Debug.Log($"{skill.skillName} 아직 쿨타임 중 ({skill.currentCooldown:F1}초 남음)");
        }
    }

    // 실제 스킬 실행 로직
    void ExecuteSkill(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:     //기본 공격 마우스 좌클릭
                Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (Physics.Raycast(ray, out RaycastHit hit, attackRange, enemyLayer))
                {
                    EnemyHealth enemey = hit.collider.GetComponent<EnemyHealth>();
                    Instantiate(Prefab, this.gameObject.transform.position + new Vector3(0,1,0), Quaternion.LookRotation(this.transform.forward));
                    enemey.TakeDamage(10, true);
                }
                break;
            case 1:     //차징 공격 Q 꾹 
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
            case 2:     //리듬 공격 E 
                this.GetComponent<RhythmAttackSystem>().enabled = true;
                break; 
            case 3:     //궁극기  R
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
            case 4:     //디폴트 
                this.GetComponent<RhythmAttackSystem>().enabled = false;
                break;
        }
    }

    // 외부에서 스킬 상태 확인용
    public bool IsSkillReady(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Length) return false;
        return skills[skillIndex].IsReady;
    }

    // 남은 쿨타임 확인용
    public float GetRemainingCooldown(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Length) return 0f;
        return skills[skillIndex].currentCooldown;
    }

    public enum States
    {
        SonicRoar,      //기본 공격 -> 입에서 직전 음파 발사
        PowerRoar,      //차지 공격 (스킬) -> 길게 눌러 충전 후 넓은 부채꼴 범위 공격
        BeatShot,       //리듬 대응 공격 (스킬) -> BGM 타이밍에 맞춰 누르면 데미지 배율 증가
        GuitarFinisher,  //궁극기 (스킬) -> 전방 광역 공격 + 버프
        Default,
    }

}
