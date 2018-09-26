// Decompiled with JetBrains decompiler
// Type: UBER_GlobalParams
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("UBER/Global Params")]
public class UBER_GlobalParams : MonoBehaviour
{
  public const float DEFROST_RATE = 0.3f;
  public const float RAIN_DAMP_ON_FREEZE_RATE = 0.2f;
  public const float FROZEN_FLOW_BUMP_STRENGTH = 0.1f;
  public const float FROST_RATE = 0.3f;
  public const float FROST_RATE_BUMP = 0.5f;
  public const float RAIN_TO_WATER_LEVEL_RATE = 2f;
  public const float RAIN_TO_WET_AMOUNT_RATE = 3f;
  public const float WATER_EVAPORATION_RATE = 0.001f;
  public const float WET_EVAPORATION_RATE = 0.0003f;
  public const float SNOW_FREEZE_RATE = 0.05f;
  public const float SNOW_INCREASE_RATE = 0.01f;
  public const float SNOW_MELT_RATE = 0.03f;
  public const float SNOW_MELT_RATE_BY_RAIN = 0.05f;
  public const float SNOW_DECREASE_RATE = 0.01f;
  [Range(0.0f, 1f)]
  [Tooltip("You can control global water level (multiplied by material value)")]
  [Header("Global Water & Rain")]
  public float WaterLevel;
  [Range(0.0f, 1f)]
  [Tooltip("You can control global wetness (multiplied by material value)")]
  public float WetnessAmount;
  [Tooltip("Time scale for flow animation")]
  public float flowTimeScale;
  [Range(0.0f, 1f)]
  [Tooltip("Multiplier of water flow ripple normalmap")]
  public float FlowBumpStrength;
  [Tooltip("You can control global rain intensity")]
  [Range(0.0f, 1f)]
  public float RainIntensity;
  [Range(0.0f, 1f)]
  [Tooltip("You can control global snow")]
  [Header("Global Snow")]
  public float SnowLevel;
  [Range(0.0f, 1f)]
  [Tooltip("You can control global frost")]
  public float Frost;
  [Range(0.0f, 4f)]
  [Tooltip("Global snow dissolve value")]
  public float SnowDissolve;
  [Range(0.001f, 0.2f)]
  [Tooltip("Global snow dissolve value")]
  public float SnowBumpMicro;
  [Tooltip("Global snow spec (RGB) & Gloss (A)")]
  public Color SnowSpecGloss;
  [Tooltip("Global snow glitter color/spec boost")]
  public Color SnowGlitterColor;
  [Range(0.0f, 4f)]
  [HideInInspector]
  [Header("Global Snow - cover state")]
  public float SnowDissolveCover;
  [HideInInspector]
  [Range(0.001f, 0.2f)]
  [Tooltip("Global snow dissolve value")]
  public float SnowBumpMicroCover;
  [Tooltip("Global snow spec (RGB) & Gloss (A)")]
  [HideInInspector]
  public Color SnowSpecGlossCover;
  [Tooltip("Global snow glitter color/spec boost")]
  [HideInInspector]
  public Color SnowGlitterColorCover;
  [Range(0.0f, 4f)]
  [HideInInspector]
  [Header("Global Snow - melt state")]
  public float SnowDissolveMelt;
  [Range(0.001f, 0.2f)]
  [Tooltip("Global snow dissolve value")]
  [HideInInspector]
  public float SnowBumpMicroMelt;
  [HideInInspector]
  [Tooltip("Global snow spec (RGB) & Gloss (A)")]
  public Color SnowSpecGlossMelt;
  [HideInInspector]
  [Tooltip("Global snow glitter color/spec boost")]
  public Color SnowGlitterColorMelt;
  [Header("Rainfall/snowfall controller")]
  public bool Simulate;
  [Range(0.0f, 1f)]
  public float fallIntensity;
  [Range(-50f, 50f)]
  [Tooltip("Temperature (influences melt/freeze/evaporation speed) - 0 means water freeze")]
  public float temperature;
  [Range(0.0f, 1f)]
  [Tooltip("Wind (1 means 4x faster evaporation and freeze rate)")]
  public float wind;
  [Range(0.0f, 1f)]
  [Tooltip("Speed of surface state change due to the weather dynamics")]
  public float weatherTimeScale;
  [Tooltip("We won't melt ice nor decrease water level while snow level is >5%")]
  public bool FreezeWetWhenSnowPresent;
  [Tooltip("Increase global Water level when snow appears")]
  public bool AddWetWhenSnowPresent;
  [Tooltip("Set to show and adjust below particle systems")]
  [Space(10f)]
  public bool UseParticleSystem;
  [Tooltip("GameObject with particle system attached controlling rain")]
  public GameObject rainGameObject;
  [Tooltip("GameObject with particle system attached controlling snow")]
  public GameObject snowGameObject;
  private Vector4 __Time;
  private float lTime;
  private bool paricleSystemActive;
  private ParticleSystem psRain;
  private ParticleSystem psSnow;

  public UBER_GlobalParams()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    this.AdvanceTime(Time.get_deltaTime());
  }

  private void Start()
  {
    this.SetupIt();
  }

  public void SetupIt()
  {
    Shader.SetGlobalFloat("_UBER_GlobalDry", 1f - this.WaterLevel);
    Shader.SetGlobalFloat("_UBER_GlobalDryConst", 1f - this.WetnessAmount);
    Shader.SetGlobalFloat("_UBER_GlobalRainDamp", 1f - this.RainIntensity);
    Shader.SetGlobalFloat("_UBER_RippleStrength", this.FlowBumpStrength);
    Shader.SetGlobalFloat("_UBER_GlobalSnowDamp", 1f - this.SnowLevel);
    Shader.SetGlobalFloat("_UBER_Frost", 1f - this.Frost);
    Shader.SetGlobalFloat("_UBER_GlobalSnowDissolve", this.SnowDissolve);
    Shader.SetGlobalFloat("_UBER_GlobalSnowBumpMicro", this.SnowBumpMicro);
    Shader.SetGlobalColor("_UBER_GlobalSnowSpecGloss", this.SnowSpecGloss);
    Shader.SetGlobalColor("_UBER_GlobalSnowGlitterColor", this.SnowGlitterColor);
  }

  public void AdvanceTime(float amount)
  {
    this.SimulateDynamicWeather(amount * this.weatherTimeScale);
    amount *= this.flowTimeScale;
    ref Vector4 local1 = ref this.__Time;
    local1.x = (__Null) (local1.x + (double) amount / 20.0);
    ref Vector4 local2 = ref this.__Time;
    local2.y = (__Null) (local2.y + (double) amount);
    ref Vector4 local3 = ref this.__Time;
    local3.z = (__Null) (local3.z + (double) amount * 2.0);
    ref Vector4 local4 = ref this.__Time;
    local4.w = (__Null) (local4.w + (double) amount * 3.0);
    Shader.SetGlobalVector("UBER_Time", this.__Time);
  }

  public void SimulateDynamicWeather(float dt)
  {
    if ((double) dt == 0.0 || !this.Simulate)
      return;
    float rainIntensity = this.RainIntensity;
    float temperature = this.temperature;
    float flowTimeScale = this.flowTimeScale;
    float flowBumpStrength = this.FlowBumpStrength;
    float waterLevel = this.WaterLevel;
    float wetnessAmount = this.WetnessAmount;
    float snowLevel = this.SnowLevel;
    float snowDissolve = this.SnowDissolve;
    float snowBumpMicro = this.SnowBumpMicro;
    Color snowSpecGloss = this.SnowSpecGloss;
    Color snowGlitterColor = this.SnowGlitterColor;
    float num1 = (float) ((double) this.wind * 4.0 + 1.0);
    float num2 = !this.FreezeWetWhenSnowPresent ? 1f : Mathf.Clamp01((float) ((0.0500000007450581 - (double) this.SnowLevel) / 0.0500000007450581));
    if ((double) this.temperature > 0.0)
    {
      float num3 = this.temperature + 10f;
      this.RainIntensity = this.fallIntensity * num2;
      this.flowTimeScale += (float) ((double) dt * (double) num3 * 0.300000011920929) * num2;
      if ((double) this.flowTimeScale > 1.0)
        this.flowTimeScale = 1f;
      this.FlowBumpStrength += (float) ((double) dt * (double) num3 * 0.300000011920929) * num2;
      if ((double) this.FlowBumpStrength > 1.0)
        this.FlowBumpStrength = 1f;
      this.WaterLevel += (float) ((double) this.RainIntensity * (double) dt * 2.0) * num2;
      if ((double) this.WaterLevel > 1.0)
        this.WaterLevel = 1f;
      this.WetnessAmount += (float) ((double) this.RainIntensity * (double) dt * 3.0) * num2;
      if ((double) this.WetnessAmount > 1.0)
        this.WetnessAmount = 1f;
      float delta = Mathf.Abs((float) ((double) dt * (double) num3 * 0.0299999993294477 + (double) dt * (double) this.RainIntensity * 0.0500000007450581));
      this.SnowDissolve = this.TargetValue(this.SnowDissolve, this.SnowDissolveMelt, delta * 2f);
      this.SnowBumpMicro = this.TargetValue(this.SnowBumpMicro, this.SnowBumpMicroMelt, delta * 0.1f);
      this.SnowSpecGloss.r = (__Null) (double) this.TargetValue((float) this.SnowSpecGloss.r, (float) this.SnowSpecGlossMelt.r, delta);
      this.SnowSpecGloss.g = (__Null) (double) this.TargetValue((float) this.SnowSpecGloss.g, (float) this.SnowSpecGlossMelt.g, delta);
      this.SnowSpecGloss.b = (__Null) (double) this.TargetValue((float) this.SnowSpecGloss.b, (float) this.SnowSpecGlossMelt.b, delta);
      this.SnowSpecGloss.a = (__Null) (double) this.TargetValue((float) this.SnowSpecGloss.a, (float) this.SnowSpecGlossMelt.a, delta);
      this.SnowGlitterColor.r = (__Null) (double) this.TargetValue((float) this.SnowGlitterColor.r, (float) this.SnowGlitterColorMelt.r, delta);
      this.SnowGlitterColor.g = (__Null) (double) this.TargetValue((float) this.SnowGlitterColor.g, (float) this.SnowGlitterColorMelt.g, delta);
      this.SnowGlitterColor.b = (__Null) (double) this.TargetValue((float) this.SnowGlitterColor.b, (float) this.SnowGlitterColorMelt.b, delta);
      this.SnowGlitterColor.a = (__Null) (double) this.TargetValue((float) this.SnowGlitterColor.a, (float) this.SnowGlitterColorMelt.a, delta);
      this.Frost -= (float) ((double) dt * (double) num3 * 0.300000011920929) * num2;
      if ((double) this.Frost < 0.0)
        this.Frost = 0.0f;
      this.SnowLevel -= (float) ((double) dt * (double) num3 * 0.00999999977648258);
      if ((double) this.SnowLevel < 0.0)
        this.SnowLevel = 0.0f;
    }
    else
    {
      float num3 = this.temperature - 10f;
      this.RainIntensity += (float) ((double) dt * (double) num3 * 0.200000002980232);
      if ((double) this.RainIntensity < 0.0)
        this.RainIntensity = 0.0f;
      this.flowTimeScale += (float) ((double) dt * (double) num3 * 0.300000011920929) * num1;
      if ((double) this.flowTimeScale < 0.0)
        this.flowTimeScale = 0.0f;
      if ((double) this.FlowBumpStrength > 0.100000001490116)
      {
        this.FlowBumpStrength += (float) ((double) dt * (double) num3 * 0.5) * this.flowTimeScale;
        if ((double) this.FlowBumpStrength < 0.100000001490116)
          this.FlowBumpStrength = 0.1f;
      }
      float delta = Mathf.Abs((float) ((double) dt * (double) num3 * 0.0500000007450581)) * this.fallIntensity;
      this.SnowDissolve = this.TargetValue(this.SnowDissolve, this.SnowDissolveCover, delta * 2f);
      this.SnowBumpMicro = this.TargetValue(this.SnowBumpMicro, this.SnowBumpMicroCover, delta * 0.1f);
      this.SnowSpecGloss.r = (__Null) (double) this.TargetValue((float) this.SnowSpecGloss.r, (float) this.SnowSpecGlossCover.r, delta);
      this.SnowSpecGloss.g = (__Null) (double) this.TargetValue((float) this.SnowSpecGloss.g, (float) this.SnowSpecGlossCover.g, delta);
      this.SnowSpecGloss.b = (__Null) (double) this.TargetValue((float) this.SnowSpecGloss.b, (float) this.SnowSpecGlossCover.b, delta);
      this.SnowSpecGloss.a = (__Null) (double) this.TargetValue((float) this.SnowSpecGloss.a, (float) this.SnowSpecGlossCover.a, delta);
      this.SnowGlitterColor.r = (__Null) (double) this.TargetValue((float) this.SnowGlitterColor.r, (float) this.SnowGlitterColorCover.r, delta);
      this.SnowGlitterColor.g = (__Null) (double) this.TargetValue((float) this.SnowGlitterColor.g, (float) this.SnowGlitterColorCover.g, delta);
      this.SnowGlitterColor.b = (__Null) (double) this.TargetValue((float) this.SnowGlitterColor.b, (float) this.SnowGlitterColorCover.b, delta);
      this.SnowGlitterColor.a = (__Null) (double) this.TargetValue((float) this.SnowGlitterColor.a, (float) this.SnowGlitterColorCover.a, delta);
      this.Frost -= (float) ((double) dt * (double) num3 * 0.300000011920929);
      if ((double) this.Frost > 1.0)
        this.Frost = 1f;
      this.SnowLevel -= this.fallIntensity * (float) ((double) dt * (double) num3 * 0.00999999977648258);
      if ((double) this.SnowLevel > 1.0)
        this.SnowLevel = 1f;
      if (this.AddWetWhenSnowPresent && (double) this.WaterLevel < (double) this.SnowLevel)
        this.WaterLevel = this.SnowLevel;
    }
    this.WaterLevel -= (float) ((double) num1 * ((double) this.temperature + 273.0) * (1.0 / 1000.0)) * this.flowTimeScale * dt * num2;
    if ((double) this.WaterLevel < 0.0)
      this.WaterLevel = 0.0f;
    this.WetnessAmount -= (float) ((double) num1 * ((double) this.temperature + 273.0) * 0.000300000014249235) * this.flowTimeScale * dt * num2;
    if ((double) this.WetnessAmount < 0.0)
      this.WetnessAmount = 0.0f;
    this.RefreshParticleSystem();
    bool flag = false;
    if (this.compareDelta(rainIntensity, this.RainIntensity))
      flag = true;
    else if (this.compareDelta(temperature, this.temperature))
      flag = true;
    else if (this.compareDelta(flowTimeScale, this.flowTimeScale))
      flag = true;
    else if (this.compareDelta(flowBumpStrength, this.FlowBumpStrength))
      flag = true;
    else if (this.compareDelta(waterLevel, this.WaterLevel))
      flag = true;
    else if (this.compareDelta(wetnessAmount, this.WetnessAmount))
      flag = true;
    else if (this.compareDelta(snowLevel, this.SnowLevel))
      flag = true;
    else if (this.compareDelta(snowDissolve, this.SnowDissolve))
      flag = true;
    else if (this.compareDelta(snowBumpMicro, this.SnowBumpMicro))
      flag = true;
    else if (this.compareDelta(snowSpecGloss, this.SnowSpecGloss))
      flag = true;
    else if (this.compareDelta(snowGlitterColor, this.SnowGlitterColor))
      flag = true;
    if (!flag)
      return;
    this.SetupIt();
  }

  private bool compareDelta(float propA, float propB)
  {
    return (double) Mathf.Abs(propA - propB) > 9.99999997475243E-07;
  }

  private bool compareDelta(Color propA, Color propB)
  {
    return (double) Mathf.Abs((float) (propA.r - propB.r)) > 9.99999997475243E-07 || (double) Mathf.Abs((float) (propA.g - propB.g)) > 9.99999997475243E-07 || ((double) Mathf.Abs((float) (propA.b - propB.b)) > 9.99999997475243E-07 || (double) Mathf.Abs((float) (propA.a - propB.a)) > 9.99999997475243E-07);
  }

  private float TargetValue(float val, float target_val, float delta)
  {
    if ((double) val < (double) target_val)
    {
      val += delta;
      if ((double) val > (double) target_val)
        val = target_val;
    }
    else if ((double) val > (double) target_val)
    {
      val -= delta;
      if ((double) val < (double) target_val)
        val = target_val;
    }
    return val;
  }

  public void RefreshParticleSystem()
  {
    if (this.paricleSystemActive != this.UseParticleSystem)
    {
      if (Object.op_Implicit((Object) this.rainGameObject))
        this.rainGameObject.SetActive(this.UseParticleSystem);
      if (Object.op_Implicit((Object) this.snowGameObject))
        this.snowGameObject.SetActive(this.UseParticleSystem);
      this.paricleSystemActive = this.UseParticleSystem;
    }
    if (!this.UseParticleSystem)
      return;
    if (Object.op_Inequality((Object) this.rainGameObject, (Object) null))
    {
      this.rainGameObject.get_transform().set_position(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 3f)));
      if (Object.op_Equality((Object) this.psRain, (Object) null))
        this.psRain = (ParticleSystem) this.rainGameObject.GetComponent<ParticleSystem>();
    }
    if (Object.op_Inequality((Object) this.snowGameObject, (Object) null))
    {
      this.snowGameObject.get_transform().set_position(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 3f)));
      if (Object.op_Equality((Object) this.psSnow, (Object) null))
        this.psSnow = (ParticleSystem) this.snowGameObject.GetComponent<ParticleSystem>();
    }
    if (Object.op_Inequality((Object) this.psRain, (Object) null))
    {
      ParticleSystem.EmissionModule emission = this.psRain.get_emission();
      ParticleSystem.MinMaxCurve minMaxCurve;
      ((ParticleSystem.MinMaxCurve) ref minMaxCurve).\u002Ector(this.fallIntensity * 3000f * Mathf.Clamp01(this.temperature + 1f));
      ((ParticleSystem.EmissionModule) ref emission).set_rateOverTime(minMaxCurve);
    }
    if (!Object.op_Inequality((Object) this.psSnow, (Object) null))
      return;
    ParticleSystem.EmissionModule emission1 = this.psSnow.get_emission();
    ParticleSystem.MinMaxCurve minMaxCurve1;
    ((ParticleSystem.MinMaxCurve) ref minMaxCurve1).\u002Ector(this.fallIntensity * 3000f * Mathf.Clamp01(1f - this.temperature));
    ((ParticleSystem.EmissionModule) ref emission1).set_rateOverTime(minMaxCurve1);
  }
}
