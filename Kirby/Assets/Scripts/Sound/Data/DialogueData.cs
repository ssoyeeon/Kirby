using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Header("��ȭ ����")]
    public CharacterData speaker;       //���ϴ� ĳ����

    [TextArea(2, 4)]
    public string message;              //��ȭ ����

    [Header("Ư�� ȿ��")]
    public bool useSlowTyping;      //õõ�� Ÿ����
    public bool useEmphasis;        //���� ȿ��
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("��ȭ ����")]
    public string dialogueTitle;        //��ȭ ���� 

    [Header("��ȭ ����")]
    public DialogueLine[] dialogueLines;

    [Header("����")]
    public bool loopDialogue = true;        //��ȭ �ݺ� ����

    [Header("����")]
    [TextArea(2, 3)]
    public string description;      //��ȭ�� ����
}
