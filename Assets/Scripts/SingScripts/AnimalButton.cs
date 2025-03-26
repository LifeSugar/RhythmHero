using UnityEngine;
using UnityEngine.UI;

public class AnimalButton : MonoBehaviour
{
    public int slotIndex; // 代表这是第几个选项，比如 Selection1 就是 slot 1
    public string trackName;
    public AnimalBandController controller;

    public void SelectTrack()
    {
        controller.SelectTrackFromSlot(slotIndex, trackName);
    }
}


