// FMODManager.cs
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODManager : MonoBehaviour
{
    [Header("FMOD Event")]
    [SerializeField] private EventReference animalConcertEvent;

    private EventInstance instance;

    // 用于追踪当前每一轮的选择（默认值为0表示未选择）
    private Dictionary<int, int> roundSelections = new Dictionary<int, int>()
    {
        { 1, 0 },
        { 2, 0 },
        { 3, 0 },
        { 4, 0 },
    };

    void Start()
    {
        instance = RuntimeManager.CreateInstance(animalConcertEvent);

        // 初始化参数，确保不会自动播放
        for (int i = 1; i <= 4; i++)
        {
            instance.setParameterByName("Animal_R" + i, 0);
        }

        instance.start();
    }

    // 外部按钮调用这个函数，只需要传一个编号（如 6 表示第二轮的蛇）
    public void SetTrack(int trackCode)
    {
        int round = (trackCode - 1) / 3 + 1; // 1~3 -> Round 1, 4~6 -> Round 2, etc.

        // 修正动物映射关系（1:鸟, 2:蛇, 3:猴）
        int[] animalMap = { 1, 3, 2 }; // 鸟、猴、蛇 → 脚本到FMOD的映射
        int animalIndex = animalMap[(trackCode - 1) % 3];

        Debug.Log($"设置 Round={round}, Animal={animalIndex}");

        // 如果选择重复，仍然触发播放（通过先设为0再设目标）
        instance.setParameterByName("Animal_R" + round, 0);
        instance.setParameterByName("Animal_R" + round, animalIndex);

        // 更新记录
        roundSelections[round] = animalIndex;
    }

    // 如果你需要在代码中重置所有选择，可调用此方法
    public void ResetAll()
    {
        for (int i = 1; i <= 4; i++)
        {
            instance.setParameterByName("Animal_R" + i, 0);
            roundSelections[i] = 0;
        }
    }
}