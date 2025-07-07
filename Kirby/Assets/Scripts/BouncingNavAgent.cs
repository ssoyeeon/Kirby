using UnityEngine;
using UnityEngine.AI;

public class BouncingNavAgent : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceHeight = 1.5f;
    public float bounceSpeed = 6f;

    private NavMeshAgent agent;
    private float bounceTimer = 0f;
    private float originalY;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalY = transform.position.y;
    }

    void Update()
    {
        if (agent.velocity.magnitude > 0.1f) // �̵� ���� ��
        {
            // ���� Ƣ�� ȿ��
            bounceTimer += Time.deltaTime * bounceSpeed;
            float bounceY = Mathf.Sin(bounceTimer) * bounceHeight;

            Vector3 pos = transform.position;
            pos.y = originalY + bounceY;
            transform.position = pos;
        }
        else
        {
            // ������ �� ���� ���̷�
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, originalY, Time.deltaTime * 5f);
            transform.position = pos;
            bounceTimer = 0f;
        }
    }
}