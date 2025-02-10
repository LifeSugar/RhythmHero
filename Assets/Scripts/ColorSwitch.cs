using System.Collections;
using System.Collections.Generic;
using rhythmhero.audio;
using UnityEngine;

namespace rhythmhero
{
    public class ColorSwitch : MonoBehaviour
    {
        private Material material;
        public Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow };
        private int colorIndex = 0;

        void Start()
        {
            material = GetComponent<Renderer>().material;
            BGMManager.instance.OneBeat += ColorChange;
        }

        void ColorChange()
        {
            if (colorIndex >= colors.Length - 1)
            {
                colorIndex = 0;
            }
            else
            {
                colorIndex++;
            }
            material.SetColor("_DiffuseColor", colors[colorIndex]);
        }
    }

}