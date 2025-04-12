using rhythmhero;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;


public class FogRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class FogSettings
    {
        [FormerlySerializedAs("outlineMaterial")] [Tooltip("后处理材质，使用你写好的 Outline Shader（例如 Custom/TilemapOutlineWithMask）")]
        public Material fogMaterial = null;
        [Tooltip("Tilemap 摄像机渲染的 RenderTexture（确保格式支持透明）")]
        public RenderTexture tilemapRenderTexture = null;
        [Tooltip("Render Pass 事件，建议设置在 AfterRenderingPostProcessing")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        [Tooltip("玩家的位置，传入shader实时更新雾气的中心")]
        public Vector4 playerPos = new Vector4(0, 0, 0, 0);
        [Tooltip("传入雾气范围")]
        public float fogDensity = 30.0f;
    }
    
    public static FogRenderFeature instance;
    public FogSettings settings = new FogSettings();

    public void SetupPlayerPos(Vector3 playerPos)
    {
        this.settings.playerPos = new Vector4(playerPos.x, playerPos.y, playerPos.z, 0);
    }

    public void SetupFogIntensity(float fogDensity)
    {
        this.settings.fogDensity = fogDensity;
    }

    

    class FogRenderPass : ScriptableRenderPass
    {
        public Material FogMaterial = null;
        public RenderTexture tilemapRenderTexture = null;
        public Vector4 playerPos = new Vector4(0, 0, 0, 0);
        public float fogDensity = 30.0f;
        private RenderTargetHandle temporaryColorTexture;

        public FogRenderPass()
        {
            temporaryColorTexture.Init("_TemporaryColorTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (FogMaterial == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Fog Render Pass");
            
            
            
            FogMaterial.SetVector("_CenterPos", playerPos);
            FogMaterial.SetFloat("_FadeRange", fogDensity);

            // 在 Execute 中获取摄像机的颜色目标
            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

            cmd.Blit(tilemapRenderTexture, source, FogMaterial);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    FogRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new FogRenderPass();
        m_ScriptablePass.FogMaterial = settings.fogMaterial;
        m_ScriptablePass.tilemapRenderTexture = settings.tilemapRenderTexture;
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
        m_ScriptablePass.playerPos = settings.playerPos;
        m_ScriptablePass.fogDensity = settings.fogDensity;
        instance = this;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // if (renderingData.cameraData.camera.CompareTag("outliner"))
        //     return;
        
        // if (settings.outlineMaterial == null || settings.tilemapRenderTexture == null)
        //     return;
        
        if (settings.fogMaterial == null)
            return;
        
        m_ScriptablePass.playerPos = settings.playerPos;
        m_ScriptablePass.fogDensity = settings.fogDensity;
        // Debug.Log("Before update: settings.playerPos = " + settings.playerPos);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}