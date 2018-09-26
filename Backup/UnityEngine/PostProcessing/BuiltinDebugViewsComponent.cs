// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.BuiltinDebugViewsComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
  public sealed class BuiltinDebugViewsComponent : PostProcessingComponentCommandBuffer<BuiltinDebugViewsModel>
  {
    private const string k_ShaderString = "Hidden/Post FX/Builtin Debug Views";
    private BuiltinDebugViewsComponent.ArrowArray m_Arrows;

    public override bool active
    {
      get
      {
        if (!this.model.IsModeActive(BuiltinDebugViewsModel.Mode.Depth) && !this.model.IsModeActive(BuiltinDebugViewsModel.Mode.Normals))
          return this.model.IsModeActive(BuiltinDebugViewsModel.Mode.MotionVectors);
        return true;
      }
    }

    public override DepthTextureMode GetCameraFlags()
    {
      BuiltinDebugViewsModel.Mode mode = this.model.settings.mode;
      DepthTextureMode depthTextureMode = (DepthTextureMode) 0;
      switch (mode)
      {
        case BuiltinDebugViewsModel.Mode.Depth:
          depthTextureMode = (DepthTextureMode) (depthTextureMode | 1);
          break;
        case BuiltinDebugViewsModel.Mode.Normals:
          depthTextureMode = (DepthTextureMode) (depthTextureMode | 2);
          break;
        case BuiltinDebugViewsModel.Mode.MotionVectors:
          depthTextureMode = (DepthTextureMode) (depthTextureMode | 5);
          break;
      }
      return depthTextureMode;
    }

    public override CameraEvent GetCameraEvent()
    {
      if (this.model.settings.mode == BuiltinDebugViewsModel.Mode.MotionVectors)
        return (CameraEvent) 18;
      return (CameraEvent) 12;
    }

    public override string GetName()
    {
      return "Builtin Debug Views";
    }

    public override void PopulateCommandBuffer(CommandBuffer cb)
    {
      BuiltinDebugViewsModel.Settings settings = this.model.settings;
      Material material = this.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
      material.set_shaderKeywords((string[]) null);
      if (this.context.isGBufferAvailable)
        material.EnableKeyword("SOURCE_GBUFFER");
      switch (settings.mode)
      {
        case BuiltinDebugViewsModel.Mode.Depth:
          this.DepthPass(cb);
          break;
        case BuiltinDebugViewsModel.Mode.Normals:
          this.DepthNormalsPass(cb);
          break;
        case BuiltinDebugViewsModel.Mode.MotionVectors:
          this.MotionVectorsPass(cb);
          break;
      }
      this.context.Interrupt();
    }

    private void DepthPass(CommandBuffer cb)
    {
      Material material = this.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
      BuiltinDebugViewsModel.DepthSettings depth = this.model.settings.depth;
      cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._DepthScale, 1f / depth.scale);
      cb.Blit((Texture) null, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), material, 0);
    }

    private void DepthNormalsPass(CommandBuffer cb)
    {
      Material material = this.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
      cb.Blit((Texture) null, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), material, 1);
    }

    private void MotionVectorsPass(CommandBuffer cb)
    {
      Material material = this.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
      BuiltinDebugViewsModel.MotionVectorsSettings motionVectors = this.model.settings.motionVectors;
      int num1 = BuiltinDebugViewsComponent.Uniforms._TempRT;
      cb.GetTemporaryRT(num1, this.context.width, this.context.height, 0, (FilterMode) 1);
      cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._Opacity, motionVectors.sourceOpacity);
      cb.SetGlobalTexture(BuiltinDebugViewsComponent.Uniforms._MainTex, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2));
      cb.Blit(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), RenderTargetIdentifier.op_Implicit(num1), material, 2);
      if ((double) motionVectors.motionImageOpacity > 0.0 && (double) motionVectors.motionImageAmplitude > 0.0)
      {
        int tempRt2 = BuiltinDebugViewsComponent.Uniforms._TempRT2;
        cb.GetTemporaryRT(tempRt2, this.context.width, this.context.height, 0, (FilterMode) 1);
        cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._Opacity, motionVectors.motionImageOpacity);
        cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._Amplitude, motionVectors.motionImageAmplitude);
        cb.SetGlobalTexture(BuiltinDebugViewsComponent.Uniforms._MainTex, RenderTargetIdentifier.op_Implicit(num1));
        cb.Blit(RenderTargetIdentifier.op_Implicit(num1), RenderTargetIdentifier.op_Implicit(tempRt2), material, 3);
        cb.ReleaseTemporaryRT(num1);
        num1 = tempRt2;
      }
      if ((double) motionVectors.motionVectorsOpacity > 0.0 && (double) motionVectors.motionVectorsAmplitude > 0.0)
      {
        this.PrepareArrows();
        float num2 = 1f / (float) motionVectors.motionVectorsResolution;
        float num3 = num2 * (float) this.context.height / (float) this.context.width;
        cb.SetGlobalVector(BuiltinDebugViewsComponent.Uniforms._Scale, Vector4.op_Implicit(new Vector2(num3, num2)));
        cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._Opacity, motionVectors.motionVectorsOpacity);
        cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._Amplitude, motionVectors.motionVectorsAmplitude);
        cb.DrawMesh(this.m_Arrows.mesh, Matrix4x4.get_identity(), material, 0, 4);
      }
      cb.SetGlobalTexture(BuiltinDebugViewsComponent.Uniforms._MainTex, RenderTargetIdentifier.op_Implicit(num1));
      cb.Blit(RenderTargetIdentifier.op_Implicit(num1), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2));
      cb.ReleaseTemporaryRT(num1);
    }

    private void PrepareArrows()
    {
      int vectorsResolution = this.model.settings.motionVectors.motionVectorsResolution;
      int columns = vectorsResolution * Screen.get_width() / Screen.get_height();
      if (this.m_Arrows == null)
        this.m_Arrows = new BuiltinDebugViewsComponent.ArrowArray();
      if (this.m_Arrows.columnCount == columns && this.m_Arrows.rowCount == vectorsResolution)
        return;
      this.m_Arrows.Release();
      this.m_Arrows.BuildMesh(columns, vectorsResolution);
    }

    public override void OnDisable()
    {
      if (this.m_Arrows != null)
        this.m_Arrows.Release();
      this.m_Arrows = (BuiltinDebugViewsComponent.ArrowArray) null;
    }

    private static class Uniforms
    {
      internal static readonly int _DepthScale = Shader.PropertyToID(nameof (_DepthScale));
      internal static readonly int _TempRT = Shader.PropertyToID(nameof (_TempRT));
      internal static readonly int _Opacity = Shader.PropertyToID(nameof (_Opacity));
      internal static readonly int _MainTex = Shader.PropertyToID(nameof (_MainTex));
      internal static readonly int _TempRT2 = Shader.PropertyToID(nameof (_TempRT2));
      internal static readonly int _Amplitude = Shader.PropertyToID(nameof (_Amplitude));
      internal static readonly int _Scale = Shader.PropertyToID(nameof (_Scale));
    }

    private enum Pass
    {
      Depth,
      Normals,
      MovecOpacity,
      MovecImaging,
      MovecArrows,
    }

    private class ArrowArray
    {
      public Mesh mesh { get; private set; }

      public int columnCount { get; private set; }

      public int rowCount { get; private set; }

      public void BuildMesh(int columns, int rows)
      {
        Vector3[] vector3Array = new Vector3[6]{ new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1f, 0.0f), new Vector3(0.0f, 1f, 0.0f), new Vector3(-1f, 1f, 0.0f), new Vector3(0.0f, 1f, 0.0f), new Vector3(1f, 1f, 0.0f) };
        int capacity = 6 * columns * rows;
        List<Vector3> vector3List = new List<Vector3>(capacity);
        List<Vector2> vector2List = new List<Vector2>(capacity);
        for (int index1 = 0; index1 < rows; ++index1)
        {
          for (int index2 = 0; index2 < columns; ++index2)
          {
            Vector2 vector2;
            ((Vector2) ref vector2).\u002Ector((0.5f + (float) index2) / (float) columns, (0.5f + (float) index1) / (float) rows);
            for (int index3 = 0; index3 < 6; ++index3)
            {
              vector3List.Add(vector3Array[index3]);
              vector2List.Add(vector2);
            }
          }
        }
        int[] numArray = new int[capacity];
        for (int index = 0; index < capacity; ++index)
          numArray[index] = index;
        Mesh mesh = new Mesh();
        ((Object) mesh).set_hideFlags((HideFlags) 52);
        this.mesh = mesh;
        this.mesh.SetVertices(vector3List);
        this.mesh.SetUVs(0, vector2List);
        this.mesh.SetIndices(numArray, (MeshTopology) 3, 0);
        this.mesh.UploadMeshData(true);
        this.columnCount = columns;
        this.rowCount = rows;
      }

      public void Release()
      {
        GraphicsUtils.Destroy((Object) this.mesh);
        this.mesh = (Mesh) null;
      }
    }
  }
}
