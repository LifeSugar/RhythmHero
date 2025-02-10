using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using rhythmhero;
using rhythmhero.audio;

namespace rhythmhero.UI
{
    public class ScoreDemo : MonoBehaviour
    {
        [Header("UI Image References")]
        public Image great;
        public Image perfect;
        public Image miss;
        public Text biasText;

        [Header("FMOD Events")]
        public EventReference greatSFX;
        public EventReference perfectSFX;
        public EventReference missSFX;
        
        [Header("Judgement Thresholds")]
        public float perfectThreshold = 0.02f;
        public float greatThreshold = 0.05f;

        [Header("Fade Settings")]
        // UI淡出速度
        public float fadeSpeed = 2f;

        private void Start()
        {
            // 确保开始时全部透明
            ClearScores();
        }

        private void Update()
        {
            ScoreInput();
            FadeScores();
        }

        /// <summary>
        /// 输入判定
        /// </summary>
        private void ScoreInput()
        {
            // 获取偏差值
            float bias = Mathf.Min(Mathf.Abs(BiasCalculator.instance.oneBeatBias - 0.5f), BiasCalculator.instance.oneBeatBias);

            if (Input.GetMouseButtonDown(0))
            {
                // 1. 清空旧 UI,显示误差
                ClearScores();
                biasText.text = bias.ToString();
                

                // 2. 根据当前的节拍偏差，决定显示哪一个判定并播放对应音效
                if (bias < perfectThreshold)
                {
                    AudioManager.instance.PlayOneShot(perfectSFX, Camera.main.transform.position);
                    SetImageAlpha(perfect, 1f);  // 设为不透明（显示出来）
                }
                else if (bias < greatThreshold)
                {
                    AudioManager.instance.PlayOneShot(greatSFX, Camera.main.transform.position);
                    SetImageAlpha(great, 1f);
                }
                else
                {
                    AudioManager.instance.PlayOneShot(missSFX, Camera.main.transform.position);
                    SetImageAlpha(miss, 1f);
                }
            }
        }

        /// <summary>
        /// 清空UI, 将三个判定的透明度全部设为0
        /// </summary>
        private void ClearScores()
        {
            SetImageAlpha(great, 0f);
            SetImageAlpha(perfect, 0f);
            SetImageAlpha(miss, 0f);
        }

        /// <summary>
        /// 对所有判定 UI 进行淡出
        /// </summary>
        private void FadeScores()
        {
            FadeImage(great, fadeSpeed);
            FadeImage(perfect, fadeSpeed);
            FadeImage(miss, fadeSpeed);
        }

        /// <summary>
        /// 将指定Image逐帧淡出
        /// </summary>
        private void FadeImage(Image image, float speed)
        {
            Color c = image.color;
            // 把 alpha 往 0 方向移动
            c.a = Mathf.MoveTowards(c.a, 0f, speed * Time.deltaTime);
            image.color = c;
        }

        /// <summary>
        /// 设置某个 Image 的透明度
        /// </summary>
        private void SetImageAlpha(Image image, float alpha)
        {
            Color c = image.color;
            c.a = alpha;
            image.color = c;
        }
    }
}
