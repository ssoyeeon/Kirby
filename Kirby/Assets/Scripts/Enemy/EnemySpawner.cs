using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("������ ����")]
    public GameObject enemyPrefab;        // ������ �� ������
    public float respawnDelay = 3f;       // ������ ������ (��)
    public Transform spawnPoint;          // ���� ��ġ (��������� �ڽ��� ��ġ)

    [Header("����� ����")]
    public bool showGizmo = true;         // ����� ǥ�� ����
    public Color gizmoColor = Color.red;  // ����� ����
    public float gizmoSize = 1f;          // ����� ũ��

    [Header("���� ǥ��")]
    public bool isEnemyAlive = false;     // ���� ����ִ��� Ȯ�ο�

    private GameObject currentEnemy;      // ���� ������ ��
    private float respawnTimer = 0f;      // ������ Ÿ�̸�
    private bool waitingToRespawn = false; // ������ ��� ����

    void Start()
    {
        // ���� ��ġ�� �������� �ʾ����� �ڽ��� ��ġ ���
        if (spawnPoint == null)
            spawnPoint = transform;

        // ������ �� ù ��° �� ����
        SpawnEnemy();
    }

    void Update()
    {
        // ���� �׾��� ������ ��� ���̸� Ÿ�̸� ����
        if (waitingToRespawn)
        {
            respawnTimer -= Time.deltaTime;

            if (respawnTimer <= 0f)
            {
                SpawnEnemy();
                waitingToRespawn = false;
            }
        }

        // ���� ���� �����ϴ��� Ȯ��
        CheckEnemyStatus();
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // ���ο� �� ����
        currentEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        isEnemyAlive = true;

        // ���� �׾��� �� �˸��ޱ� ���� EnemyHealth ������Ʈ Ȯ��
        EnemyH enemyHealth = currentEnemy.GetComponent<EnemyH>();
        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDeath += OnEnemyDied;
        }

        Debug.Log($"���� �����Ǿ����ϴ�: {currentEnemy.name}");
    }

    void CheckEnemyStatus()
    {
        // ���� ���� �������� ������ (�����Ǿ�����)
        if (currentEnemy == null && isEnemyAlive)
        {
            OnEnemyDied();
        }
    }

    void OnEnemyDied()
    {
        isEnemyAlive = false;
        waitingToRespawn = true;
        respawnTimer = respawnDelay;

        Debug.Log($"{respawnDelay}�� �Ŀ� ���� �������˴ϴ�.");
    }

    // �������� ���� ��� �����ϴ� �Լ� (�׽�Ʈ��)
    [ContextMenu("��� ����")]
    public void ForceSpawn()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
        }

        waitingToRespawn = false;
        SpawnEnemy();
    }

    // �� �信�� ����� �׸���
    void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;

        // ������ ��ġ�� ��ü �׸���
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(position, gizmoSize);

        // �����ʰ� Ȱ��ȭ�Ǿ� ������ ä���� ��ü�� �׸���
        if (Application.isPlaying && isEnemyAlive)
        {
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
            Gizmos.DrawSphere(position, gizmoSize * 0.8f);
        }

        // ��� ���� ���� �ٸ� �������� ǥ��
        if (Application.isPlaying && waitingToRespawn)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, gizmoSize * 1.2f);
        }
    }

    // ���õǾ��� �� ����� �׸��� (�� ���� ���� ǥ��)
    void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;

        // ������ ���� ǥ��
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(position, Vector3.one * gizmoSize * 2f);

#if UNITY_EDITOR
        // ������ ������ 3D �ؽ�Ʈ�� ǥ��
        UnityEditor.Handles.color = Color.white;

        string spawnerInfo = "PAWNER";
        string monsterInfo = enemyPrefab != null ? $"{enemyPrefab.name}" : "No Prefab";
        string statusInfo = "";

        if (Application.isPlaying)
        {
            if (isEnemyAlive)
                statusInfo = "Enemy Alive";
            else if (waitingToRespawn)
                statusInfo = $"Respawn in {respawnTimer:F1}s";
            else
                statusInfo = "Ready to Spawn";
        }
        else
        {
            statusInfo = $"Respawn Delay: {respawnDelay}s";
        }

        // �ؽ�Ʈ ��ġ ���
        Vector3 textPos = position + Vector3.up * (gizmoSize + 1f);

        // GUI ��Ÿ�� ����
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.alignment = TextAnchor.MiddleCenter;

        // �ؽ�Ʈ ǥ��
        UnityEditor.Handles.Label(textPos, spawnerInfo, style);
        UnityEditor.Handles.Label(textPos + Vector3.down * 0.5f, monsterInfo, style);
        UnityEditor.Handles.Label(textPos + Vector3.down * 1f, statusInfo, style);
#endif
    }
}