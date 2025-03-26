using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;

public class AnimalBandController : MonoBehaviour
{
    public EventReference fmodEvent;

    private EventInstance musicInstance;

    private List<string> allTracks = new List<string> { "MouseOn", "BirdOn", "MonkeyOn", "SnakeOn" };

    // 保存每个 slot（按钮位）当前选择的轨道名
    private Dictionary<int, string> selectedTracks = new Dictionary<int, string>();

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(fmodEvent);

        // 初始化全部轨道为静音
        foreach (var track in allTracks)
        {
            musicInstance.setParameterByName(track, 0f);
        }

        musicInstance.start();
        Debug.Log("🎧 FMOD 音乐启动成功");
    }

    public void SelectTrackFromSlot(int slotIndex, string trackName)
    {
        // 如果该 slot 原本就有轨道，先淡出
        if (selectedTracks.TryGetValue(slotIndex, out string oldTrack))
        {
            if (oldTrack != trackName)
            {
                StartCoroutine(FadeOutTrack(oldTrack, 1f));
            }
        }

        // 保存新选择
        selectedTracks[slotIndex] = trackName;

        // 淡入新轨道
        StartCoroutine(FadeInTrack(trackName, 1f));

        Debug.Log($"🎵 Slot {slotIndex} 选择了轨道: {trackName}");

        if (selectedTracks.Count == 4)
        {
            Debug.Log("🎉 合奏已准备就绪！");
            // TODO: 触发动画、奖励、特效等
        }
    }

    // 🌀 淡入逻辑：从 0 ➜ 1，触发 FMOD 曲线
    private IEnumerator FadeInTrack(string trackName, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float value = Mathf.Clamp01(time / duration);
            musicInstance.setParameterByName(trackName, value);
            yield return null;
        }
        musicInstance.setParameterByName(trackName, 1f); // 确保最终为1
    }

    // 🌀 淡出逻辑：从 1 ➜ 0
    private IEnumerator FadeOutTrack(string trackName, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float value = Mathf.Clamp01(time / duration);
            musicInstance.setParameterByName(trackName, 1f - value);
            yield return null;
        }
        musicInstance.setParameterByName(trackName, 0f); // 确保最终为0
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}
