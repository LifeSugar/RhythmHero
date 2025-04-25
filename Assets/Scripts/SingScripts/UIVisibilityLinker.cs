using UnityEngine;

public class UIVisibilityLinker : MonoBehaviour
{
    [Tooltip("被观察的按钮（比如 A）")]
    public GameObject sourceButton;

    [Tooltip("要同步显示状态的图片（比如 B）")]
    public GameObject targetImage;

    private bool previousActiveState;

    void Start()
    {
        if (sourceButton != null && targetImage != null)
        {
            previousActiveState = sourceButton.activeSelf;
            targetImage.SetActive(previousActiveState);
        }
    }

    void Update()
    {
        if (sourceButton == null || targetImage == null) return;

        bool currentState = sourceButton.activeSelf;

        if (currentState != previousActiveState)
        {
            targetImage.SetActive(currentState);
            previousActiveState = currentState;
        }
    }
}

