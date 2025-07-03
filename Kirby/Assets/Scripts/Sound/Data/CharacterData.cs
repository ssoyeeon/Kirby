using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Dialogue System/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("ĳ���� �⺻ ����")]
    public string characterName;
    public Color nameColor = Color.white;
    public Sprite characterPortrait;        //ĳ���� �ʻ�ȭ (���û���)

    [Header("���� ����")]
    public AudioClip aSound;            //��,��
    public AudioClip eSound;            //��,��
    public AudioClip iSound;            //��
    public AudioClip oSound;            //��,��
    public AudioClip uSound;            //��,��,��
    public AudioClip defaultSound;      //�⺻ �Ҹ�

    [Header("���� Ư��")]
    [Range(0.5f, 2f)]
    public float pitch = 1.0f;          //���� ������
    [Range(0.5f, 2f)]
    public float volume = 1.0f;         //���� ũ��

    [Header("Ÿ���� �ӵ�")]
    [Range(0.05f, 0.5f)]
    public float typingSpeed = 0.15f;          //ĳ���� �� ���ϴ� �ӵ�//ĳ���� ����

    [Header("ĳ���� ����")]
    [TextArea(2, 4)]
    public string description;          //ĳ���� �� ���ϴ� �ӵ�//ĳ���� ����
}
