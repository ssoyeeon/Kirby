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

        // 리듬에 따른 효과 구분
        if (isRhythmic)
        {
            StartCoroutine(RhythmicDamageEffect()); // 초록색
        }
        else
        {
            StartCoroutine(NormalDamageEffect()); // 빨간색
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator RhythmicDamageEffect()
    {
        // 적이 초록색으로 깜빡이는 효과 (정박 히트)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Debug.Log("아야!");
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.green;
            yield return new WaitForSeconds(0.2f);
            renderer.material.color = originalColor;
        }
    }

    IEnumerator NormalDamageEffect()
    {
        // 적이 빨간색으로 깜빡이는 효과 (정박 미스)
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
        // 죽을 때 효과
        GameObject.Destroy(gameObject);
    }
}