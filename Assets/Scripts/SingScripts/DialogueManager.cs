using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

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
    public DialogueData dialogueData1; //????????dialogueData??????? 

    public DialogueData currentData; //??????????dialogueData???currentData??????????????
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    
    
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterName;
    public Image characterPortrait;
    public GameObject dialoguePanel;
    public GameObject endDialoguePanel; 

    
    
    public static DialogueManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("More than one instance of DialogueManager");
        }
        
        instance = this;
    }

    void Start()
    {
        // StartDialogue(); 
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            NextDialogueLine();
        }
    }

    //???????Game Manager???????
    
    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            NextDialogueLine();
        }
    }

    void StartDialogue()
    {
        if (currentData.lines.Count > 0)
        {
            isDialogueActive = true;
            dialoguePanel.SetActive(true);
            endDialoguePanel.SetActive(false); 
            currentLineIndex = 0;
            ShowLine();
        }
    }

    void NextDialogueLine()
    {
        currentLineIndex++;
        if (currentLineIndex < currentData.lines.Count)
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
        DialogueLine line = currentData.lines[currentLineIndex];
        characterName.text = line.characterName;
        characterPortrait.sprite = line.characterPortrait;
        dialogueText.text = line.dialogueText;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        endDialoguePanel.SetActive(true); // ?????????????????UI? ???DialogueData????????bool??????currentData????????????
    }
}
