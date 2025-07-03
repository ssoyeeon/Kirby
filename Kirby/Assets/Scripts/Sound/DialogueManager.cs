using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI ���")]
    public TMP_Text characterNameText;
    public TMP_Text dialogueText;
    public Image characterPortrait;     //ĳ���� �ʻ�ȭ

    [Header("���� ����")]
    public AudioSource audioSource;

    [Header("������ ���� (����")]
    public AudioClip dotSound;
    public AudioClip questionSound;
    public AudioClip exclamationSound;
    public AudioClip defaultPuncSound;

    [Header("��ȭ ������")]
    public DialogueData currentDialogue;

    [Header("���� ����")]
    public float defaultTypingSpeed = 0.15f;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private CharacterData currentCharacter; 

    void Start()
    {
        if (currentDialogue != null)
        {
            StartDialogue();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Ÿ���� ���̸� ��� �Ϸ�
                StopAllCoroutines();
                CompleteCurrentLine();
            }
            else
            {
                // ���� ��ȭ��
                NextDialogue();
            }
        }
    }

    public void SetDialogue(DialogueData newDialogue)
    {
        currentDialogue = newDialogue;
        currentLineIndex = 0;
        StartDialogue();
    }

    void StartDialogue()
    {
        if (currentDialogue == null || currentDialogue.dialogueLines.Length == 0)
        {
            Debug.LogWarning("��ȭ �����Ͱ� �����ϴ�!");
            return;
        }

        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        if (currentLineIndex >= currentDialogue.dialogueLines.Length)
        {
            if (currentDialogue.loopDialogue)
            {
                currentLineIndex = 0;  // ó������ �ٽ�
            }
            else
            {
                Debug.Log("��ȭ ����");
                return;
            }
        }

        DialogueLine currentLine = currentDialogue.dialogueLines[currentLineIndex];
        currentCharacter = currentLine.speaker;

        if (currentCharacter != null)
        {
            SetupCharacterUI();
            float speed = currentLine.useSlowTyping ? 0.3f :
                         (currentCharacter.typingSpeed > 0 ? currentCharacter.typingSpeed : defaultTypingSpeed);

            StartCoroutine(TypeText(currentLine.message, speed));
        }
        else
        {
            Debug.LogWarning($"��ȭ ���� {currentLineIndex}�� ĳ���Ͱ� �������� �ʾҽ��ϴ�!");
        }
    }

    void SetupCharacterUI()
    {
        // ĳ���� �̸��� ���� ����
        characterNameText.text = currentCharacter.characterName;
        characterNameText.color = currentCharacter.nameColor;

        // ĳ���� �ʻ�ȭ ���� (�ִ� ���)
        if (characterPortrait != null && currentCharacter.characterPortrait != null)
        {
            characterPortrait.sprite = currentCharacter.characterPortrait;
            characterPortrait.gameObject.SetActive(true);
        }
        else if (characterPortrait != null)
        {
            characterPortrait.gameObject.SetActive(false);
        }

        // ���� ����
        audioSource.pitch = currentCharacter.pitch;
        audioSource.volume = currentCharacter.volume;

        Debug.Log($"{currentCharacter.characterName}��(��) ���մϴ�!");
    }

    void NextDialogue()
    {
        currentLineIndex++;
        DisplayCurrentLine();
    }

    void CompleteCurrentLine()
    {
        isTyping = false;
        DialogueLine currentLine = currentDialogue.dialogueLines[currentLineIndex];
        dialogueText.text = currentLine.message;
    }

    IEnumerator TypeText(string message, float speed)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in message)
        {
            dialogueText.text += c;
            PlayCharacterSound(c);
            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
    }

    void PlayCharacterSound(char character)
    {
        if (currentCharacter == null) return;

        audioSource.Stop();

        if (char.IsLetter(character) || char.IsDigit(character))
        {
            AudioClip soundToPlay = GetVowelSound(character);

            if (soundToPlay != null)
            {
                audioSource.clip = soundToPlay;
                audioSource.Play();
                Debug.Log($"'{character}' �Ҹ� ��� (����/����)");
            }
        }
        else if (char.IsPunctuation(character))
        {
            AudioClip punctuationClip = GetPunctuationSound(character);

            if (punctuationClip != null)
            {
                audioSource.clip = punctuationClip;
                audioSource.Play();
                Debug.Log($"'{character}' ������ �Ҹ� ���: {punctuationClip.name}");
            }
        }
    }

    AudioClip GetVowelSound(char c)
    {
        // ���� ó��
        if (char.IsDigit(c))
        {
            return GetNumberSound(c);
        }

        // �ѱ� ó��
        if (c >= '��' && c <= 'R')
        {
            return GetKoreanVowelSound(c);
        }

        // ���� ó��
        if (char.IsLetter(c))
        {
            return GetEnglishVowelSound(c);
        }

        return currentCharacter.defaultSound;
    }

    AudioClip GetPunctuationSound(char punctuation)
    {
        switch (punctuation)
        {
            case '.':   // ��ħǥ
            case ',':   // ��ǥ
            case ';':   // �����ݷ�
            case ':':   // �ݷ�
                return dotSound;

            case '?':   // ����ǥ
                return questionSound;

            case '!':   // ����ǥ
                return exclamationSound;

            case '~':   // ����ǥ
            case '-':   // ������
            case '"':   // ����ǥ
            case '\'':  // ��������ǥ
            case '(':   // ��ȣ
            case ')':
            case '[':
            case ']':
            case '{':
            case '}':
            default:
                return defaultPuncSound;
        }
    }

    AudioClip GetNumberSound(char number)
    {
        // ���ڸ� �ѱ��� ������ �°� �������� ����
        switch (number)
        {
            case '0': return currentCharacter.oSound;   // ��(yeong) �� �� �� e
            case '1': return currentCharacter.iSound;   // ��(il) �� �� �� i  
            case '2': return currentCharacter.iSound;   // ��(i) �� �� �� i
            case '3': return currentCharacter.aSound;   // ��(sam) �� �� �� a
            case '4': return currentCharacter.aSound;   // ��(sa) �� �� �� a
            case '5': return currentCharacter.oSound;   // ��(o) �� �� �� o
            case '6': return currentCharacter.uSound;   // ��(yuk) �� �� �� u
            case '7': return currentCharacter.iSound;   // ĥ(chil) �� �� �� i
            case '8': return currentCharacter.aSound;   // ��(pal) �� �� �� a
            case '9': return currentCharacter.uSound;   // ��(gu) �� �� �� u
            default: return currentCharacter.defaultSound;
        }
    }

    AudioClip GetKoreanVowelSound(char korean)
    {
        int code = korean - '��';
        int vowelIndex = (code % 588) / 28;

        switch (vowelIndex)
        {
            case 0:
            case 1:
            case 2:
            case 3:  // ��, ��, ��, ��
                return currentCharacter.aSound;

            case 4:
            case 5:
            case 6:
            case 7:  // ��, ��, ��, ��
                return currentCharacter.eSound;

            case 8:
            case 12:  // ��, ��
                return currentCharacter.oSound;

            case 13:
            case 17:
            case 18:  // ��, ��, ��
                return currentCharacter.uSound;

            case 20:  // ��
                return currentCharacter.iSound;

            default:
                return currentCharacter.defaultSound;
        }
    }

    AudioClip GetEnglishVowelSound(char english)
    {
        char lower = char.ToLower(english);

        switch (lower)
        {
            case 'a': return currentCharacter.aSound;
            case 'e': return currentCharacter.eSound;
            case 'i': return currentCharacter.iSound;
            case 'o': return currentCharacter.oSound;
            case 'u': return currentCharacter.uSound;
            default: return currentCharacter.uSound; // ����
        }
    }

    // �����Ϳ��� ���� �׽�Ʈ�� �� �ִ� �Լ�
    [ContextMenu("���� ��ȭ")]
    void TestNextDialogue()
    {
        if (!isTyping)
        {
            NextDialogue();
        }
    }
}
