using UnityEngine;
using System;

public class EnemyH : MonoBehaviour
{
    [Header("ü�� ����")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("���� ȿ��")]
    public GameObject deathEffect;        // ���� �� ����� ����Ʈ
    public float destroyDelay = 0.5f;     // ���� �� ������Ʈ ���������� ������

    [Header("�׽�Ʈ ����")]
    public int testDamageAmount = 25;     // �׽�Ʈ�� ������ ��

    // ���� �׾��� �� �����ʿ��� �˸��� ���� �̺�Ʈ
    public event Action OnEnemyDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // �������� �޴� �Լ�
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"{gameObject.name}�� {damage} �������� �޾ҽ��ϴ�. ���� ü��: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ��� ���̴� �Լ�
    public void Die()
    {
        if (currentHealth <= 0) return; // �̹� ���� ��� �ߺ� ���� ����

        currentHealth = 0;

        // ���� ����Ʈ ���
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

        // �����ʿ��� ���� �˸�
        OnEnemyDeath?.Invoke();

        Debug.Log($"{gameObject.name}�� �׾����ϴ�!");

        // ������Ʈ ����
        Destroy(gameObject, destroyDelay);
    }

    // ������ ��� ���̴� �Լ� (ü�� �������)
    public void ForceKill()
    {
        // ���� ����Ʈ ���
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

        // �����ʿ��� ���� �˸�
        OnEnemyDeath?.Invoke();

        currentHealth = 0;
        Debug.Log($"{gameObject.name}�� ������ �׾����ϴ�!");

        // ������Ʈ ����
        Destroy(gameObject, destroyDelay);
    }

    // ü�� ȸ�� �Լ�
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);

        Debug.Log($"{gameObject.name}�� {amount} ü���� ȸ���߽��ϴ�. ���� ü��: {currentHealth}");
    }

    // �׽�Ʈ�� - �ν����Ϳ��� ��Ŭ������ ���� ����
    [ContextMenu("������ �׽�Ʈ")]
    void TestDamage()
    {
        TakeDamage(testDamageAmount);
    }

    [ContextMenu("��� ���̱�")]
    void TestDie()
    {
        ForceKill(); // ���� ���̱� ���
    }

    // �ν����� ��ư�� �Լ���
    public void TestDamageButton()
    {
        if (Application.isPlaying)
        {
            TakeDamage(testDamageAmount);
        }
        else
        {
            Debug.LogWarning("������ ���� ���� ���� �������� �� �� �ֽ��ϴ�!");
        }
    }

    public void TestKillButton()
    {
        if (Application.isPlaying)
        {
            ForceKill(); // Die() ��� ForceKill() ���
        }
        else
        {
            Debug.LogWarning("������ ���� ���� ���� ���� ���� �� �ֽ��ϴ�!");
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
            Debug.LogWarning("������ ���� ���� ���� ġ���� �� �ֽ��ϴ�!");
        }
    }
}