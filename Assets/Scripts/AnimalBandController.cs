using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AnimalBandController : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string fmodEventPath = "event:/AnimalBand";

    private FMOD.Studio.EventInstance musicInstance;

    private List<string> allTracks = new List<string> { "MouseOn", "BirdOn", "MonkeyOn", "SnakeOn" };
    private string currentTrack = "";

    void Start()
    {
        musicInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEventPath);
        musicInstance.start();
        Debug.Log("🎵 FMOD 音乐启动成功");
    }

    public void SwitchToTrack(string trackName)
    {
        foreach (var track in allTracks)
        {
            musicInstance.setParameterByName(track, track == trackName ? 1f : 0f);
        }

        Debug.Log($"✅ 当前播放轨道: {trackName}");
        currentTrack = trackName;
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
    private Dictionary<int, string> selectedTracks = new Dictionary<int, string>();

    public void SelectTrackFromSlot(int slotIndex, string trackName)
    {
        selectedTracks[slotIndex] = trackName;

        // 先关闭所有轨道
        foreach (var track in allTracks)
        {
            musicInstance.setParameterByName(track, 0f);
        }

        // 然后播放当前所有已选择的轨道
        foreach (var selected in selectedTracks.Values)
        {
            musicInstance.setParameterByName(selected, 1f);
        }

        Debug.Log($"🎵 Slot {slotIndex} 选择了: {trackName}");

        if (selectedTracks.Count == 4)
        {
            Debug.Log("🎉 合奏已准备就绪！");
            // 可以在这里触发奖励、动画等
        }
    }

}

