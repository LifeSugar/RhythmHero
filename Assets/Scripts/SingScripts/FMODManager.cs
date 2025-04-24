using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODManager : MonoBehaviour
{
    [Header("FMOD Event")]
    [SerializeField] private EventReference animalConcertEvent;

    private EventInstance instance;

    // 当前每一轮的选择（默认值为0表示未选择）
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

        instance.start(); // 默认启动，但不播放任何轨道
    }

    // 外部按钮调用这个函数：传入 1~12 的编号，代表轮次与动物类型
    public void SetTrack(int trackCode)
    {
        int round = (trackCode - 1) / 3 + 1; // 1~3 -> Round 1, 4~6 -> Round 2, etc.

        // 动物类型映射：鸟→1，猴→3，蛇→2
        int[] animalMap = { 1, 3, 2 };
        int animalIndex = animalMap[(trackCode - 1) % 3];

        Debug.Log( "  设置 Round = {round}, Animal = {animalIndex}");

        // 关键：强制重播音频（确保与动画同步）
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); // 立刻停止当前播放
        instance.start(); // 重新播放事件

        //  重新设置参数，确保重新触发声音变化
        instance.setParameterByName("Animal_R" + round, 0);
        instance.setParameterByName("Animal_R" + round, animalIndex);

        // 更新选择记录
        roundSelections[round] = animalIndex;
    }

    // 外部可调用：清除所有轮次的选择
    public void ResetAll()
    {
        for (int i = 1; i <= 4; i++)
        {
            instance.setParameterByName("Animal_R" + i, 0);
            roundSelections[i] = 0;
        }
    }
}
