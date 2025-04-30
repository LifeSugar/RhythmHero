using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace rhythmhero.audio
{
    /// <summary>
    /// 全局背景音乐管理器  
    /// - 节拍同步（按 BPM / 拍号）  
    /// - OnBeat / OnBar 事件回调  
    /// - 支持淡入淡出切歌
    /// </summary>
    public class BGMManager : MonoBehaviour
    {
        /* ─────────────── 单例 ─────────────── */
        public static BGMManager instance { get; private set; }

        public void StopBGM()
        {
            var bgmins = CurrentTrack.instance;
            if (currentTrackIndex != -1)
            {
                bgmins.stop(STOP_MODE.IMMEDIATE);
            }
        }

        /* ─────────────── 数据结构 ─────────────── */

        [System.Serializable]
        public class TrackInfo
        {
            [Header("FMOD Event")] public EventReference eventRef;

            [Header("Tempo")] [Range(30, 240)] public float bpm = 120f; // 曲目速度
            [Range(1, 12)] public int beatsPerBar = 4; // 拍号（每小节几拍）

            [HideInInspector] public EventInstance instance; // 运行时实例
            [HideInInspector] public int lastBeat = -1; // 节拍缓存
            [HideInInspector] public int lastBar = -1; // 小节缓存
        }

        [Header("Playlist")] public List<TrackInfo> tracks = new();

        /* ─────────────── 事件 ─────────────── */
        public event System.Action OnBeat; // 每一拍（quarter-note）
        public event System.Action OnBar; // 每一小节

        /* ─────────────── 私有字段 ─────────────── */
        private int currentTrackIndex = -1;

        private TrackInfo CurrentTrack =>
            (currentTrackIndex >= 0 && currentTrackIndex < tracks.Count)
                ? tracks[currentTrackIndex]
                : null;

        /* ─────────────── 生命周期 ─────────────── */

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("More than one BGMManager in scene.");
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (tracks.Count > 0)
                SwitchToTrack(0, 0f, 0.5f); // 开场播放第 0 首，淡入 0.5 秒
        }

        private void Update()
        {
            if (CurrentTrack != null)
                CheckBeats(CurrentTrack);
        }

        /* ─────────────── 节拍检测 ─────────────── */

        private void CheckBeats(TrackInfo track)
        {
            if (!track.instance.isValid()) return;

            track.instance.getTimelinePosition(out int posMs);

            // 1 拍时长（毫秒）
            float beatLenMs = 60000f / track.bpm;

            int beatIndex = Mathf.FloorToInt(posMs / beatLenMs);
            int barIndex = beatIndex / track.beatsPerBar;

            if (beatIndex != track.lastBeat)
            {
                track.lastBeat = beatIndex;
                OnBeat?.Invoke();
            }

            if (barIndex != track.lastBar)
            {
                track.lastBar = barIndex;
                OnBar?.Invoke();
            }
        }

        /* ─────────────── 公共 API：切歌 ─────────────── */

        /// <summary>
        /// 切换到 playlist 中指定索引的曲目。
        /// </summary>
        /// <param name="index">tracks 的索引</param>
        /// <param name="fadeOutTime">旧曲淡出时间（秒）</param>
        /// <param name="fadeInTime">新曲淡入时间（秒）</param>
        public void SwitchToTrack(int index, float fadeOutTime = 0.5f, float fadeInTime = 0.5f)
        {
            if (index < 0 || index >= tracks.Count)
            {
                Debug.LogWarning($"BGMManager: invalid track index {index}");
                return;
            }

            if (index == currentTrackIndex) return;

            StopAllCoroutines();
            StartCoroutine(SwitchCoroutine(index, fadeOutTime, fadeInTime));
        }
        

        private IEnumerator SwitchCoroutine(int newIndex, float fadeOut, float fadeIn)
        {
            TrackInfo oldTrack = CurrentTrack;
            TrackInfo newTrack = tracks[newIndex];

            /* 1. 准备新曲实例 */
            if (!newTrack.instance.isValid())
                newTrack.instance = RuntimeManager.CreateInstance(newTrack.eventRef);

            /* 2. 旧曲淡出 */
            if (oldTrack != null && oldTrack.instance.isValid())
            {
                oldTrack.instance.getVolume(out float startVol);
                for (float t = 0f; t < fadeOut; t += Time.unscaledDeltaTime)
                {
                    float v = Mathf.Lerp(startVol, 0f, t / fadeOut);
                    oldTrack.instance.setVolume(v);
                    yield return null;
                }

                oldTrack.instance.setVolume(0f);
                oldTrack.instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                oldTrack.instance.release();
            }

            /* 3. 切换索引 + 重置缓存 */
            currentTrackIndex = newIndex;
            newTrack.lastBeat = -1;
            newTrack.lastBar = -1;

            /* 4. 新曲淡入 */
            newTrack.instance.setVolume(0f);
            newTrack.instance.start();

            for (float t = 0f; t < fadeIn; t += Time.unscaledDeltaTime)
            {
                float v = Mathf.Lerp(0f, 1f, t / fadeIn);
                newTrack.instance.setVolume(v);
                yield return null;
            }

            newTrack.instance.setVolume(1f);
        }

        /* ─────────────── 调试用 Gizmo ─────────────── */
#if UNITY_EDITOR
        private void OnGUI()
        {
            if (CurrentTrack == null) return;
            GUILayout.Label(
                $"BGM: {currentTrackIndex}  |  BPM: {CurrentTrack.bpm}  |  Time: {GetTimelineMs() / 1000f:0.00}s");

            int GetTimelineMs()
            {
                CurrentTrack.instance.getTimelinePosition(out int ms);
                return ms;
            }
        }
#endif
    }
}