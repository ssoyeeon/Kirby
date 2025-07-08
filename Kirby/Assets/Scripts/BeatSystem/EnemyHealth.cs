using UnityEngine;
using System.Collections;


public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 1000f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, bool isRhythmic)
    {
        currentHealth -= damage;

        // ���뿡 ���� ȿ�� ����
        if (isRhythmic)
        {
            StartCoroutine(RhythmicDamageEffect()); // �ʷϻ�
        }
        else
        {
            StartCoroutine(NormalDamageEffect()); // ������
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator RhythmicDamageEffect()
    {
        // ���� �ʷϻ����� �����̴� ȿ�� (���� ��Ʈ)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Debug.Log("�ƾ�!");
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.green;
            yield return new WaitForSeconds(0.2f);
            renderer.material.color = originalColor;
        }
    }

    IEnumerator NormalDamageEffect()
    {
        // ���� ���������� �����̴� ȿ�� (���� �̽�)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            renderer.material.color = originalColor;
        }
    }

    void Die()
    {
        // ���� �� ȿ��
        GameObject.Destroy(gameObject);
    }
}