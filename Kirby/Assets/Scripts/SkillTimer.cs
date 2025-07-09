using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkillTimer
{
    public string skillName;
    public float cooldownTime;
    public Image cooldownImage;
    public Text cooldownText;

    [System.NonSerialized]
    public float currentCooldown;

    public bool IsReady => currentCooldown <= 0f;

    public void StartCooldown()
    {
        currentCooldown = cooldownTime;

        // ��Ÿ�� ���� �� UI�� 0(����)���� ����
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f;
        }
    }

    public void UpdateTimer(float deltaTime)
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= deltaTime;

            // UI ������Ʈ (��Ÿ���� �پ����� fillAmount ����)
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = 1f - (currentCooldown / cooldownTime);
            }

            if (cooldownText != null)
            {
                cooldownText.text = currentCooldown > 0f ? currentCooldown.ToString("F1") : "";
            }
        }
        else
        {
            // ��Ÿ���� ������ �� UI�� 1(������ ���̰�)
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = 1f;
            }
        }
    }
}
