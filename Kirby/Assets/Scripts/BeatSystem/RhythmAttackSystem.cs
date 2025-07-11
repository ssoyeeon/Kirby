using UnityEngine;
using System.Collections;

public class RhythmAttackSystem : MonoBehaviour
{
    [Header("리듬 설정")]
    public float bpm = 120f; // 분당 비트 수
    public float beatTolerancePerfect = 0.2f; // 리듬 허용 오차
    public float beatToleranceGood = 0.4f; // 리듬 허용 오차
    public float beatToleranceBad = 0.6f; // 리듬 허용 오차

    [Header("공격 설정")]
    public float attackForce = 10f;
    public float attackRange = 2f;
    public LayerMask enemyLayer = 1;

    [Header("시각 효과")]
    public GameObject attackEffect;
    public GameObject beatIndicator;

    [Header("오디오")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip beatSound;

    private float beatInterval;
    private float nextBeatTime;
    private float gameStartTime;
    private bool canAttack = true;

    // 공격 콤보
    private int comboCount = 0;
    private float comboTimer = 0f;
    private float comboWindow = 1f;
    bool isOnBeat;
    void Start()
    {
        beatInterval = 60f / bpm;
        gameStartTime = Time.time;
        nextBeatTime = gameStartTime + beatInterval;

        if (beatIndicator != null)
        {
            StartCoroutine(BeatIndicatorAnimation());
        }
    }

    void Update()
    {
        // 비트 타이밍 체크
        if (Time.time >= nextBeatTime)
        {
            OnBeat();
            nextBeatTime += beatInterval;
        }

        // 입력 처리
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }

        // 콤보 타이머 업데이트
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                comboCount = 0;
            }
        }
    }

    void OnBeat()
    {
        // 비트 사운드 재생
        if (audioSource != null && beatSound != null)
        {
            audioSource.PlayOneShot(beatSound, 0.3f);
        }

        // 비트 인디케이터 효과
        if (beatIndicator != null)
        {
            beatIndicator.transform.localScale = Vector3.one * 1.2f;
            StartCoroutine(ScaleDown(beatIndicator.transform));
        }
    }

    void TryAttack()
    {
        if (!canAttack) return;

        float currentTime = Time.time;
        float timeToBeat = Mathf.Abs((currentTime - gameStartTime) % beatInterval - beatInterval / 2);

        // 리듬에 맞는지 체크
        if(isOnBeat = timeToBeat <= beatTolerancePerfect)
        {
            PerformAttack(true, 1);
        }
        else if(isOnBeat = timeToBeat <= beatToleranceGood)
        {
            PerformAttack(true, 2);
        }
        else
        {
            PerformAttack(false, 3);
        }
    }

    void PerformAttack(bool isRhythmic , int RhythmJudgment)
    {
        StartCoroutine(AttackCooldown());

        float finalAttackForce = attackForce;

        if (isRhythmic && RhythmJudgment == 1)
        {
            // Perfect 판정이면 강한 공격
            comboCount++;
            comboTimer = comboWindow;
            finalAttackForce *= (1f + comboCount * 0.5f); // 콤보에 따라 데미지 증가

            // 화면 효과
            StartCoroutine(RhythmicAttackEffect());
        }
        if (isRhythmic && RhythmJudgment == 2)
        {
            // GOOD 판정이면 조금 약해진 공격
            comboCount++;
            comboTimer = comboWindow;
            finalAttackForce *= (0.8f + comboCount * 0.5f); // 콤보에 따라 데미지 증가

            // 화면 효과
            StartCoroutine(RhythmicAttackEffect());
        }
        else
        {
            // 리듬에 안 맞으면 약한 공격
            finalAttackForce *= 0.5f;
            comboCount = 0;
        }

        // 공격 실행
        ExecuteAttack(finalAttackForce, isRhythmic);

        // 사운드 재생
        if (audioSource != null && attackSound != null)
        {
            float pitch = isRhythmic ? 1f + (comboCount * 0.1f) : 0.8f;
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(attackSound);
            audioSource.pitch = 1f;
        }
    }

    void ExecuteAttack(float force, bool isRhythmic)
    {
        // 범위 내 적들 찾기
        Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            // 적에게 데미지 주기
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(force, isRhythmic);
            }

            // 적을 밀어내기
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                Vector3 direction = (enemy.transform.position - transform.position).normalized;
                enemyRb.AddForce(direction * force, ForceMode.Impulse);
            }
        }

        // 공격 이펙트 생성
        if (attackEffect != null)
        {
            GameObject effect = Instantiate(attackEffect, transform.position, Quaternion.identity);
            if (isRhythmic)
            {
                effect.transform.localScale *= 1.5f; // 리듬 공격시 더 큰 이펙트
            }
            Destroy(effect, 2f);
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(0.1f);
        canAttack = true;
    }

    IEnumerator RhythmicAttackEffect()
    {
        // 화면 흔들기 효과
        Camera.main.transform.position += Random.insideUnitSphere * 0.1f;
        yield return new WaitForSeconds(0.05f);
        Camera.main.transform.position -= Random.insideUnitSphere * 0.1f;

        // 시간 느리게 효과
        Time.timeScale = 0.8f;
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 1f;
    }

    IEnumerator BeatIndicatorAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(beatInterval - 0.1f);
            if (beatIndicator != null)
            {
                StartCoroutine(PulseIndicator());
            }
        }
    }

    IEnumerator PulseIndicator()
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 originalScale = beatIndicator.transform.localScale;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float scale = 1f + Mathf.Sin(progress * Mathf.PI) * 0.3f;
            beatIndicator.transform.localScale = originalScale * scale;
            elapsed += Time.deltaTime;
            yield return null;
        }

        beatIndicator.transform.localScale = originalScale;
    }

    IEnumerator ScaleDown(Transform target)
    {
        float duration = 0.1f;
        float elapsed = 0f;
        Vector3 startScale = target.localScale;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            target.localScale = Vector3.Lerp(startScale, Vector3.one, progress);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = Vector3.one;
    }

    void OnDrawGizmos()
    {
        // 공격 범위 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void OnGUI()
    {
        // 콤보 표시
        if (comboCount > 0)
        {
            GUI.Label(new Rect(10, 10, 200, 30), $"콤보: {comboCount}");
        }

        // 리듬 가이드
        float timeToBeat = Mathf.Abs((Time.time - gameStartTime) % beatInterval - beatInterval / 2);
        bool isInBeatWindow = timeToBeat <= beatTolerancePerfect;

        GUI.color = isInBeatWindow ? Color.green : Color.white;
        GUI.Label(new Rect(10, 50, 200, 30), isInBeatWindow ? "리듬 타이밍!" : "리듬 대기...");
        GUI.color = Color.white;
    }
}

