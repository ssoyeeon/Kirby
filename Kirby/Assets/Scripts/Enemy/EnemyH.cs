using UnityEngine;
using System;

public class EnemyH : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("죽음 효과")]
    public GameObject deathEffect;        // 죽을 때 재생할 이펙트
    public float destroyDelay = 0.5f;     // 죽은 후 오브젝트 삭제까지의 딜레이

    [Header("테스트 설정")]
    public int testDamageAmount = 25;     // 테스트용 데미지 양

    // 적이 죽었을 때 스포너에게 알리기 위한 이벤트
    public event Action OnEnemyDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // 데미지를 받는 함수
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"{gameObject.name}이 {damage} 데미지를 받았습니다. 남은 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 즉시 죽이는 함수
    public void Die()
    {
        if (currentHealth <= 0) return; // 이미 죽은 경우 중복 실행 방지

        currentHealth = 0;

        // 죽음 이펙트 재생
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

        // 스포너에게 죽음 알림
        OnEnemyDeath?.Invoke();

        Debug.Log($"{gameObject.name}이 죽었습니다!");

        // 오브젝트 삭제
        Destroy(gameObject, destroyDelay);
    }

    // 강제로 즉시 죽이는 함수 (체력 상관없이)
    public void ForceKill()
    {
        // 죽음 이펙트 재생
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

        // 스포너에게 죽음 알림
        OnEnemyDeath?.Invoke();

        currentHealth = 0;
        Debug.Log($"{gameObject.name}이 강제로 죽었습니다!");

        // 오브젝트 삭제
        Destroy(gameObject, destroyDelay);
    }

    // 체력 회복 함수
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);

        Debug.Log($"{gameObject.name}이 {amount} 체력을 회복했습니다. 현재 체력: {currentHealth}");
    }

    // 테스트용 - 인스펙터에서 우클릭으로 실행 가능
    [ContextMenu("데미지 테스트")]
    void TestDamage()
    {
        TakeDamage(testDamageAmount);
    }

    [ContextMenu("즉시 죽이기")]
    void TestDie()
    {
        ForceKill(); // 강제 죽이기 사용
    }

    // 인스펙터 버튼용 함수들
    public void TestDamageButton()
    {
        if (Application.isPlaying)
        {
            TakeDamage(testDamageAmount);
        }
        else
        {
            Debug.LogWarning("게임이 실행 중일 때만 데미지를 줄 수 있습니다!");
        }
    }

    public void TestKillButton()
    {
        if (Application.isPlaying)
        {
            ForceKill(); // Die() 대신 ForceKill() 사용
        }
        else
        {
            Debug.LogWarning("게임이 실행 중일 때만 적을 죽일 수 있습니다!");
        }
    }

    public void TestHealButton()
    {
        if (Application.isPlaying)
        {
            Heal(testDamageAmount);
        }
        else
        {
            Debug.LogWarning("게임이 실행 중일 때만 치료할 수 있습니다!");
        }
    }
}