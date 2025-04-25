using System;
using rhythmhero;
using UnityEngine;

public class DialogueWhenButtonsVisible : MonoBehaviour
{
    [Header("监控的四个按钮物体（不是组件！）")]
    public GameObject[] buttonObjects;

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

        bool allVisible = true;
        foreach (GameObject btn in buttonObjects)
        {
            if (!btn.activeInHierarchy)
            {
                allVisible = false;
                break;
            }
        }

        if (allVisible)
        {
            TriggerDialogue();
            hasTriggered = true;
        }
    }

    void TriggerDialogue()
    {
        Debug.Log("四个按钮都可见，触发对话！");
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            DialogueManager.instance.currentData = dialogue;
            DialogueManager.instance.StartDialogue();
        }
        // 你也可以在这里播放语音、调动画、激活系统
    }
}
