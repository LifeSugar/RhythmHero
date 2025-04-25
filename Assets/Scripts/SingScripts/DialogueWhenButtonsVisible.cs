using System;
using rhythmhero;
using UnityEngine;

public class DialogueWhenButtonsVisible : MonoBehaviour
{
    [Header("监控的四个按钮物体（不是组件！）")]
    public GameObject[] buttonObjects;

    [Header("监控的四个面板（全部隐藏时才触发）")]
    public GameObject[] panelObjectsToBeInvisible;

    [Header("只触发一次？")]
    public bool triggerOnce = true;

    [Header("对话内容")]
    public DialogueData dialogue;

    [Header("触发对话的对象（可选）")]
    public GameObject dialoguePanel;

    private bool hasTriggered = false;

    private void Start()
    {
        dialoguePanel = DialogueManager.instance.dialoguePanel;
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
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            DialogueManager.instance.currentData = dialogue;
            DialogueManager.instance.StartDialogue();
        }
    }
}

