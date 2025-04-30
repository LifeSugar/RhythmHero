using System;
using rhythmhero;
using UnityEngine;
using UnityEngine.UI;

public class DialogueWhenButtonsVisible : MonoBehaviour
{
    [Header("监控的四个按钮物体（不是组件！）")]
    public GameObject[] buttonObjects;

    [Header("监控的四个面板（全部隐藏时才触发）")]
    public GameObject[] panelObjectsToBeInvisible;

    [Header("只触发一次？")]
    public bool triggerOnce = true;

    [Header("对话内容")]
    public DialogueData dialogue1;
    public DialogueData dialogue2;
    public DialogueData dialogue3;

    // [Header("触发对话的对象（可选）")]
    // public GameObject dialoguePanel;

    private bool hasTriggered = false;

    private void Start()
    {
        // dialoguePanel = DialogueManager.instance.dialoguePanel;
        InitialQuestion();
    }

    void Update()
    {
        if (hasTriggered && triggerOnce) return;

        // 检查按钮是否全部可见
        bool allButtonsVisible = true;
        foreach (GameObject btn in buttonObjects)
        {
            if (!btn.activeInHierarchy)
            {
                allButtonsVisible = false;
                break;
            }
        }

        // 检查面板是否全部不可见
        bool allPanelsHidden = true;
        foreach (GameObject panel in panelObjectsToBeInvisible)
        {
            if (panel.activeInHierarchy)
            {
                allPanelsHidden = false;
                break;
            }
        }

        // 两个条件同时满足才触发
        if (allButtonsVisible && allPanelsHidden)
        {
            TriggerDialogue();
            hasTriggered = true;
        }
    }

    void TriggerDialogue()
    {
        Debug.Log("四个按钮都可见，且四个面板都隐藏，触发对话！");
        DialogueManager.instance.currentData = dialogue1;
        DialogueManager.instance.StartDialogue();
    }

    
    [Header("第一次问题")] public GameObject FirstQuestion;
    public Button correctButtonOne;
    
    [Header("第二次问题")]
    public GameObject SecondQuestion;
    public Button correctButtonTwo;
    
    void DialogueOneEnd(int currentIndex)
    {
        if (currentIndex == dialogue1.lines.Count)
        {
            FirstQuestion.SetActive(true);
        }
    }

    public void NextDialogueTwo()
    {
        FirstQuestion.SetActive(false);
        DialogueManager.instance.currentData = dialogue2;
        DialogueManager.instance.StartDialogue();
    }

    void DialogueTwoEnd(int currentIndex)
    {
        if (currentIndex == dialogue2.lines.Count)
        {
            SecondQuestion.SetActive(true);
        }
    }

    public void NextDialogueThree()
    {
        SecondQuestion.SetActive(false);
        DialogueManager.instance.currentData = dialogue3;
        DialogueManager.instance.StartDialogue();
    }

    void DialogueThreeEnd(int currentIndex)
    {
        if (currentIndex == dialogue3.lines.Count)
        {
            EndPanel.SetActive(true);
        }
    }

    public GameObject EndPanel;

    void InitialQuestion()
    {
        dialogue1.OnDialogueLineChanged += DialogueOneEnd;
        dialogue2.OnDialogueLineChanged += DialogueTwoEnd;
        dialogue3.OnDialogueLineChanged += DialogueThreeEnd;
        correctButtonOne.onClick.AddListener(() => NextDialogueTwo());
        correctButtonTwo.onClick.AddListener(() => NextDialogueThree());
    }
}

