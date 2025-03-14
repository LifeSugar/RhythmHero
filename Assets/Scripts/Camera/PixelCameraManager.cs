using UnityEngine;

namespace PixelPerfectURP
{
    

/// <summary>
    /// Unity 编辑器中显示的提示信息（Tooltips），供字段 [Tooltip(...)] 标签使用。
    /// </summary>
    public static class Tooltips
    {
        public const string TT_FOLLOWED_TRANSFORM = "被跟随的 Transform，用于进行像素级别校正。对于相机控制器非常实用，允许对 Transform 进行完全控制。";
        public const string TT_GRID_MOVEMENT = "在 3D 世界中的静止对象保持静止，不会出现颜色或轮廓的抖动。相机沿着体素网格（voxel grid）移动。";
        public const string TT_SUB_PIXEL = "亚像素（Subpixel）调整可以抵消沿网格移动时的块状感。";
        public const string TT_FOLLOW_ROTATION = "相机是否跟随被跟随对象的旋转（rotation）以及位置（position）。";
        public const string TT_GAME_RESOLUTION = "游戏渲染纹理的分辨率。数值越低，看起来越像素化。";
        public const string TT_RESOLUTION_SYNCHRONIZATION_MODE = "如何计算 'GameResolution'。通过同步可确保在不同设备分辨率和美术风格下的像素化效果保持一致。";
        public const string TT_CONTROL_GAME_ZOOM = "是否由本脚本来控制游戏相机的正交尺寸(orthographic size)。如有其他脚本在控制缩放，请关闭此选项以避免冲突。";
        public const string TT_GAME_ZOOM = "在保持渲染分辨率看似不变的情况下放大或缩小，使场景看起来更精细。对应游戏相机的正交尺寸。";
        public const string TT_VIEW_ZOOM = "在像素大小保持恒定的情况下，对画面进行缩放。对应视图相机的正交尺寸。";
    }

    /// <summary>
    /// 管理像素相机系统的主要脚本。
    /// </summary>
    [ExecuteInEditMode]
    public class PixelCameraManager : MonoBehaviour
    {
        //------------------
        // 公开字段（带有提示）
        //------------------

        [Tooltip(Tooltips.TT_FOLLOWED_TRANSFORM)]
        public Transform FollowedTransform;

        [Header("Settings")]
        [Tooltip(Tooltips.TT_GRID_MOVEMENT)]
        public bool VoxelGridMovement = true;  // 是否启用体素网格移动
        [Tooltip(Tooltips.TT_SUB_PIXEL)]
        public bool SubpixelAdjustments = true; // 是否启用亚像素调整
        [Tooltip(Tooltips.TT_FOLLOW_ROTATION)]
        public bool FollowRotation = true;      // 是否跟随目标的旋转

        [Header("Resolution")]
        [Tooltip(Tooltips.TT_RESOLUTION_SYNCHRONIZATION_MODE)]
        public ResolutionSynchronizationMode resolutionSynchronizationMode = ResolutionSynchronizationMode.SetHeight;
        [Tooltip(Tooltips.TT_GAME_RESOLUTION)]
        public Vector2Int GameResolution = new Vector2Int(640, 360);  // 游戏渲染纹理的目标分辨率

        [Header("Zoom")]
        [Tooltip(Tooltips.TT_CONTROL_GAME_ZOOM)]
        public bool ControlGameZoom = true;     // 是否由本脚本控制游戏相机的缩放
        [Tooltip(Tooltips.TT_GAME_ZOOM)]
        public float GameCameraZoom = 5f;       // 游戏相机的正交尺寸
        [Tooltip(Tooltips.TT_VIEW_ZOOM)]
        [Range(-1f, 1f)]
        public float ViewCameraZoom = 1f;       // 视图相机的缩放倍率

        //------------------
        // 私有字段
        //------------------
        Camera gameCamera;           // 游戏相机引用
        CanvasViewCamera viewCamera; // 用于在 Canvas 上平滑显示的相机
        UpscaledCanvas upscaledCanvas; // 用于放大显示游戏渲染结果的画布
        float renderTextureAspect;   // 当前渲染纹理的宽高比，用于检测变化

        //------------------
        // 生命周期方法
        //------------------

        void OnEnable()
        {
            this.Initialize(); // 在脚本启用时进行初始化
        }

        void LateUpdate()
        {
            this.UpdateCameraSystem(); // 在每帧的后期更新相机系统
        }

        //------------------
        // 属性和工具方法
        //------------------

        /// <summary>
        /// 获取相机中每个像素在世界空间（World Space）中的大小（即 1 像素对应多少单位）。
        /// </summary>
        float PixelWorldSize
            => 2f * this.gameCamera.orthographicSize / this.gameCamera.pixelHeight;

        /// <summary>
        /// 获取目标纹理的分辨率。如果 targetTexture 为空，则返回 Vector2Int.left 进行标识。
        /// </summary>
        Vector2Int TargetTextureResolution
            => this.gameCamera.targetTexture == null
               ? Vector2Int.left
               : new Vector2Int(this.gameCamera.targetTexture.width, this.gameCamera.targetTexture.height);

        /// <summary>
        /// 将给定的世界坐标转换为体素网格（voxel grid）上的位置（即四舍五入到相机像素大小的倍数）。
        /// </summary>
        /// <param name="worldPosition">世界坐标系位置</param>
        /// <returns>对齐到像素网格后的世界坐标</returns>
        public Vector3 PositionToGrid(Vector3 worldPosition)
        {
            // 1. 将世界坐标转换到相机的本地坐标（考虑相机方向）
            var localPosition = this.transform.InverseTransformDirection(worldPosition);
            // 2. 换算为像素单位
            var localPositionInPixels = localPosition / this.PixelWorldSize;
            // 3. 将像素坐标四舍五入至整数
            var integerMovement = (Vector3)Vector3Int.RoundToInt(localPositionInPixels);
            // 4. 再转换回实际的世界单位
            var movement = integerMovement * this.PixelWorldSize;
            // 5. 根据相机的方向重新组装位置
            return (movement.x * this.transform.right)
                 + (movement.y * this.transform.up)
                 + (movement.z * this.transform.forward);
        }

        /// <summary>
        /// 设置游戏相机的缩放（orthographicSize），并保证其不为 0。
        /// </summary>
        /// <param name="zoom">新的缩放值</param>
        /// <returns>修正后的缩放值，防止出现 0 的情况</returns>
        float SetGameZoom(float zoom)
        {
            // 如果近似为 0，则用一个极小值替代，避免相机出现异常
            var checkedZoom = Mathf.Approximately(zoom, 0f) ? 0.01f : zoom;
            this.gameCamera.orthographicSize = checkedZoom;
            return checkedZoom;
        }

        /// <summary>
        /// 同步裁剪平面，让视图相机的裁剪平面与游戏相机保持一致。
        /// </summary>
        void SynchronizeClipPlanes()
        {
            // 注意将视图相机的本地 z 偏移纳入考虑
            this.viewCamera.SetClipPlanes(0f, this.gameCamera.farClipPlane - this.viewCamera.transform.localPosition.z);
        }

        //------------------
        // 初始化方法
        //------------------

        /// <summary>
        /// 初始化像素相机系统，查找并缓存相关的相机和画布对象。
        /// </summary>
        private void Initialize()
        {
            // 1. 获取并检测游戏相机
            if (this.gameCamera == null)
            {
                if (!this.TryGetComponent(out this.gameCamera))
                {
                    Debug.LogError("未找到 Camera 组件，请在该对象上添加游戏相机组件！");
                }
            }

            // 2. 获取并检测画布相机（CanvasViewCamera）
            if (this.viewCamera == null)
            {
                this.viewCamera = FindAnyObjectByType(typeof(CanvasViewCamera)) as CanvasViewCamera;
                if (this.viewCamera == null)
                {
                    Debug.LogError("viewCamera 为 null，请在编辑器中指定或创建一个 CanvasViewCamera！");
                }
            }

            // 3. 获取并检测放大画布（UpscaledCanvas）
            if (this.upscaledCanvas == null)
            {
                this.upscaledCanvas = FindAnyObjectByType(typeof(UpscaledCanvas)) as UpscaledCanvas;
                if (this.upscaledCanvas == null)
                {
                    Debug.LogError("upscaledCanvas 为 null，请在编辑器中指定或创建一个 UpscaledCanvas！");
                }
            }

            // 4. 检测是否有父物体
            if (this.transform.parent == null)
            {
                Debug.LogError("当前对象没有父物体！请确认 Prefab 或层级结构是否正确设置。");
            }

            // 5. 警告父物体是否有多于 2 个子物体
            if (this.transform.parent.childCount > 2)
            {
                Debug.LogWarning("Pixel Camera Manager 的父物体应只包含 2 个子物体：本脚本所在对象和被跟随的 Transform。");
            }

            // 6. 检测被跟随的 Transform 是否存在
            if (this.FollowedTransform == null)
            {
                Debug.LogError("Followed Transform 未设置。请在同级或层级结构中创建一个空物体并在编辑器中指定。");
            }

            // 7. 初始化时立即同步裁剪平面
            this.SynchronizeClipPlanes();
        }

        //------------------
        // RenderTexture 相关方法
        //------------------

        /// <summary>
        /// 设置放大画布和游戏相机使用的渲染纹理，并记录纹理宽高比。
        /// </summary>
        /// <param name="aspect">纹理的宽高比</param>
        /// <param name="newRenderTexture">新的渲染纹理</param>
        void SetRenderTexture(float aspect, RenderTexture newRenderTexture)
        {
            // 将新纹理赋给画布材质和游戏相机
            this.upscaledCanvas.SetCanvasRenderTexture(newRenderTexture);
            this.gameCamera.targetTexture = newRenderTexture;
            // 记录当前纹理的宽高比
            this.renderTextureAspect = aspect;
        }

        //------------------
        // 核心更新方法
        //------------------

        /// <summary>
        /// 每帧（LateUpdate）更新整个像素相机系统，包括分辨率检测、缩放和位置调整等。
        /// </summary>
        void UpdateCameraSystem()
        {
            // 1. 检测分辨率或宽高比是否发生变化
            var aspectRatioChanged = this.renderTextureAspect != this.viewCamera.Aspect;
            var pixelResolutionChanged = this.GameResolution != this.TargetTextureResolution;
            var resizeCanvas = false;

            // 如果宽高比变了，或像素分辨率变了，或当前无渲染纹理，则需要重新生成渲染纹理
            if (aspectRatioChanged || pixelResolutionChanged || this.gameCamera.targetTexture == null)
            {
                // 根据视图相机的实际宽高比来计算游戏分辨率（视同步模式而定）
                this.GameResolution = RenderTextureFunctions.TextureResultion(
                    this.viewCamera.Aspect,
                    this.GameResolution,
                    this.resolutionSynchronizationMode
                );

                // 若已有纹理，先释放旧的
                if (this.gameCamera.targetTexture != null)
                {
                    this.gameCamera.targetTexture.Release();
                }

                // 创建新的渲染纹理
                var newRenderTexture = RenderTextureFunctions.CreateRenderTexture(this.GameResolution);
                // 设置到相机和画布上
                this.SetRenderTexture(this.viewCamera.Aspect, newRenderTexture);

                resizeCanvas = true; // 需要重新调整画布
            }
            else if (Application.isEditor && this.upscaledCanvas.MaterialHasRenderTexture)
            {
                // 在编辑器环境下，如果已有材质绑定 RenderTexture，仍然做一次同步操作（防止拖拽更换等情况）
                this.SetRenderTexture(this.renderTextureAspect, this.gameCamera.targetTexture);
                resizeCanvas = true;
            }

            // 2. 处理游戏相机的缩放逻辑
            var orthographicSizeChanged = this.gameCamera.orthographicSize != this.GameCameraZoom;

            // 如果不由本脚本控制游戏相机缩放，就将实际缩放值回写到 GameCameraZoom 上
            if (!this.ControlGameZoom)
            {
                this.GameCameraZoom = this.gameCamera.orthographicSize;
                resizeCanvas = true;
            }

            // 如果由本脚本控制且正交尺寸发生了变化，则更新相机的正交尺寸
            if (this.ControlGameZoom && orthographicSizeChanged)
            {
                this.GameCameraZoom = this.SetGameZoom(this.GameCameraZoom);
                resizeCanvas = true;
            }

            // 3. 处理视图相机的缩放逻辑（ViewCameraZoom）
            // 如果正交尺寸或像素分辨率变更，或者视图相机缩放和记录值不同，则更新
            if (orthographicSizeChanged || pixelResolutionChanged || this.ViewCameraZoom != this.viewCamera.Zoom)
            {
                // 防止画布在分辨率极低时出现越界
                var canvasOnScreenLimit = 1 - (2f / this.GameResolution.y);
                if (this.GameResolution.y < 3)
                {
                    canvasOnScreenLimit = 1f;
                    Debug.LogWarning("GameResolution 太小，可能会导致一些意想不到的行为。");
                }

                // 确保 ViewCameraZoom 在 [-1,1] 范围内，且不为 0
                this.ViewCameraZoom = Mathf.Approximately(this.ViewCameraZoom, 0f)
                                      ? 0.01f
                                      : Mathf.Clamp(this.ViewCameraZoom, -1, 1f);

                // 设置视图相机的缩放
                this.viewCamera.SetZoom(this.ViewCameraZoom * canvasOnScreenLimit, this.GameCameraZoom);
            }

            // 4. 如果需要，重新调整放大画布大小
            if (resizeCanvas)
            {
                // 计算游戏渲染的宽高比
                var gameResolutionAspect = (float)this.GameResolution.x / this.GameResolution.y;
                // 根据相机正交大小（GameCameraZoom * 2）来调整画布大小
                this.upscaledCanvas.ResizeCanvas(gameResolutionAspect, this.GameCameraZoom * 2f);
            }

            // 5. 跟随目标变换（位置 + 旋转）
            if (this.FollowRotation)
            {
                this.transform.rotation = this.FollowedTransform.rotation;
            }

            // 如果启用体素网格移动，就将目标位置对齐到像素网格，否则直接跟随
            this.transform.position = this.VoxelGridMovement
                ? this.PositionToGrid(this.FollowedTransform.position)
                : this.FollowedTransform.position;

            // 6. 亚像素（Subpixel）修正：在放大画布上微调视图相机的位置，平滑移动，减少抖动
            if (this.SubpixelAdjustments)
            {
                // 先将跟随对象位置转换为 Viewport 坐标
                var targetViewPosition = this.gameCamera.WorldToViewportPoint(this.FollowedTransform.position);
                // 用视图相机的脚本做平滑移动校正
                this.viewCamera.AdjustSubPixelPosition(targetViewPosition, this.upscaledCanvas.transform.localScale);
            }
        }
    }
}
