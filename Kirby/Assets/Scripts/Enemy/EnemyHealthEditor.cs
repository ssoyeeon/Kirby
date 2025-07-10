#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyH))]
public class EnemyHealthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // �⺻ �ν����� �׸���
        DrawDefaultInspector();

        EnemyH enemyHealth = (EnemyH)target;

        // ���м�
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("�׽�Ʈ ��ư��", EditorStyles.boldLabel);

        // ���� ���� ���� ���� ��ư Ȱ��ȭ
        GUI.enabled = Application.isPlaying;

        EditorGUILayout.BeginHorizontal();

        // ������ ��ư
        if (GUILayout.Button($"������ �ֱ� ({enemyHealth.testDamageAmount})", GUILayout.Height(30)))
        {
            enemyHealth.TestDamageButton();
        }

        // ġ�� ��ư
        if (GUILayout.Button($"ġ���ϱ� ({enemyHealth.testDamageAmount})", GUILayout.Height(30)))
        {
            enemyHealth.TestHealButton();
        }

        EditorGUILayout.EndHorizontal();

        // ��� ���̱� ��ư (������)
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("��� ���̱�", GUILayout.Height(35)))
        {
            enemyHealth.TestKillButton();
        }
        GUI.backgroundColor = Color.white;

        // ������ ���� ���� �ƴ� �� �ȳ� �޽���
        if (!Application.isPlaying)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("������ ������ �� ��ư�� ����ϼ���!", MessageType.Info);
        }
        else
        {
            // �ǽð� ü�� ���� ǥ��
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("���� ����", EditorStyles.boldLabel);

            string healthInfo = $"ü��: {enemyHealth.currentHealth} / {enemyHealth.maxHealth}";
            float healthPercent = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;

            // ü�� �� ���� ����
            Color barColor = Color.green;
            if (healthPercent < 0.3f) barColor = Color.red;
            else if (healthPercent < 0.6f) barColor = Color.yellow;

            // ü�� �� �׸���
            Rect rect = GUILayoutUtility.GetRect(0, 20);
            EditorGUI.DrawRect(rect, Color.gray);
            rect.width *= healthPercent;
            EditorGUI.DrawRect(rect, barColor);

            EditorGUILayout.LabelField(healthInfo, EditorStyles.centeredGreyMiniLabel);
        }

        GUI.enabled = true;

        // �ڵ� ���ΰ�ħ (���� ���� ��)
        if (Application.isPlaying)
        {
            EditorUtility.SetDirty(target);
            Repaint();
        }
    }
}
#endif