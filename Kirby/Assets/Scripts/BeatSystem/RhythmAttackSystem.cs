using UnityEngine;
using System.Collections;

public class RhythmAttackSystem : MonoBehaviour
{
    [Header("���� ����")]
    public float bpm = 120f; // �д� ��Ʈ ��
    public float beatTolerance = 0.2f; // ���� ��� ����

    [Header("���� ����")]
    public float attackForce = 10f;
    public float attackRange = 2f;
    public LayerMask enemyLayer = 1;

    [Header("�ð� ȿ��")]
    public GameObject attackEffect;
    public GameObject beatIndicator;

    [Header("�����")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip beatSound;

    private float beatInterval;
    private float nextBeatTime;
    private float gameStartTime;
    private bool canAttack = true;

    // ���� �޺�
    private int comboCount = 0;
    private float comboTimer = 0f;
    private float comboWindow = 1f;

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
        // ��Ʈ Ÿ�̹� üũ
        if (Time.time >= nextBeatTime)
        {
            OnBeat();
            nextBeatTime += beatInterval;
        }

        // �Է� ó��
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }

        // �޺� Ÿ�̸� ������Ʈ
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
        // ��Ʈ ���� ���
        if (audioSource != null && beatSound != null)
        {
            audioSource.PlayOneShot(beatSound, 0.3f);
        }

        // ��Ʈ �ε������� ȿ��
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

        // ���뿡 �´��� üũ
        bool isOnBeat = timeToBeat <= beatTolerance;

        if (isOnBeat)
        {
            PerformAttack(true);
        }
        else
        {
            PerformAttack(false);
        }
    }

    void PerformAttack(bool isRhythmic)
    {
        StartCoroutine(AttackCooldown());

        float finalAttackForce = attackForce;

        if (isRhythmic)
        {
            // ���뿡 ������ ���� ����
            comboCount++;
            comboTimer = comboWindow;
            finalAttackForce *= (1f + comboCount * 0.5f); // �޺��� ���� ������ ����

            // ȭ�� ȿ��
            StartCoroutine(RhythmicAttackEffect());
        }
        else
        {
            // ���뿡 �� ������ ���� ����
            finalAttackForce *= 0.5f;
            comboCount = 0;
        }

        // ���� ����
        ExecuteAttack(finalAttackForce, isRhythmic);

        // ���� ���
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
        // ���� �� ���� ã��
        Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            // ������ ������ �ֱ�
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(force, isRhythmic);
            }

            // ���� �о��
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                Vector3 direction = (enemy.transform.position - transform.position).normalized;
                enemyRb.AddForce(direction * force, ForceMode.Impulse);
            }
        }

        // ���� ����Ʈ ����
        if (attackEffect != null)
        {
            GameObject effect = Instantiate(attackEffect, transform.position, Quaternion.identity);
            if (isRhythmic)
            {
                effect.transform.localScale *= 1.5f; // ���� ���ݽ� �� ū ����Ʈ
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
        // ȭ�� ���� ȿ��
        Camera.main.transform.position += Random.insideUnitSphere * 0.1f;
        yield return new WaitForSeconds(0.05f);
        Camera.main.transform.position -= Random.insideUnitSphere * 0.1f;

        // �ð� ������ ȿ��
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
        // ���� ���� ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void OnGUI()
    {
        // �޺� ǥ��
        if (comboCount > 0)
        {
            GUI.Label(new Rect(10, 10, 200, 30), $"�޺�: {comboCount}");
        }

        // ���� ���̵�
        float timeToBeat = Mathf.Abs((Time.time - gameStartTime) % beatInterval - beatInterval / 2);
        bool isInBeatWindow = timeToBeat <= beatTolerance;

        GUI.color = isInBeatWindow ? Color.green : Color.white;
        GUI.Label(new Rect(10, 50, 200, 30), isInBeatWindow ? "���� Ÿ�̹�!" : "���� ���...");
        GUI.color = Color.white;
    }
}

