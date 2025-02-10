using UnityEngine;
using System.Collections;
using rhythmhero.audio;

public class LightColorCycleCoroutine : MonoBehaviour
{
    [Tooltip("要改变颜色的点光源")]
    public Light pointLight;

    [Tooltip("变色的时间间隔（秒）")]
    public float interval = 0.5f;

    // 依次循环的颜色数组
    public Color[] colors = { Color.red, Color.yellow, Color.blue, Color.green };
    private int colorIndex = 0;

    private void Start()
    {
        if (pointLight == null)
        {
            pointLight = GetComponent<Light>();
        }

        BGMManager.instance.OneBeat += ChangeIntensity;
        BGMManager.instance.FourBeat += ChangeColors;


    }

    void ChangeIntensity()
    {
        pointLight.intensity -= 0.25f;
    }

    void ChangeColors()
    {
        if (colorIndex >= colors.Length - 1)
        {
            colorIndex = 0;
        }
        else
        {
            colorIndex++;
        }
        pointLight.color = colors[colorIndex];
        pointLight.intensity = 1.25f;
    }

    void OnDestroy()
    {
        BGMManager.instance.OneBeat -= ChangeIntensity;
        BGMManager.instance.FourBeat -= ChangeColors;
    }
}