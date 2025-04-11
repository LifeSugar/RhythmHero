using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenFogRenderPass : ScriptableRenderPass
{
   private class CustomPassData
   {
      
   }
   
   
   public void Init()
   {
      profilingSampler = new ProfilingSampler("SS Fog Render Pass");
      renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
   }

   public void Setup()
   {
      
   }

   public void Dispose()
   {
      
   }

   public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
   {
      
   }
}
