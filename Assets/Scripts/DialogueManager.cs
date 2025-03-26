using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    public Sprite characterPortrait;
    [TextArea(3, 5)] public string dialogueText;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/New Dialogue")]
public class DialogueData : ScriptableObject
{
    public List<DialogueLine> lines;
}

public class DialogueManager : MonoBehaviour
{
    public DialogueData dialogueData;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterName;
    public Image characterPortrait;
    public GameObject dialoguePanel;
    public GameObject endDialoguePanel; // 结束对话后显示的 UI 面板

    private int currentLineIndex = 0;
    private bool isDialogueActive = false;

    void Start()
    {
        StartDialogue();
    }

    void Update()
    {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            NextDialogueLine();
        }
    }

    void StartDialogue()
    {
        if (dialogueData.lines.Count > 0)
        {
            isDialogueActive = true;
            dialoguePanel.SetActive(true);
            endDialoguePanel.SetActive(false); // 确保开始对话时结束面板隐藏
            currentLineIndex = 0;
            ShowLine();
        }
    }

    void NextDialogueLine()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueData.lines.Count)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void ShowLine()
    {
        DialogueLine line = dialogueData.lines[currentLineIndex];
        characterName.text = line.characterName;
        characterPortrait.sprite = line.characterPortrait;
        dialogueText.text = line.dialogueText;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        endDialoguePanel.SetActive(true); // 显示结束对话的 UI 面板
    }
}
