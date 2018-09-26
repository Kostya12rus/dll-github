// Decompiled with JetBrains decompiler
// Type: DetectorController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class DetectorController : MonoBehaviour
{
  public float detectionProgress;
  public float viewRange;
  public float fov;
  public GameObject[] detectors;

  public DetectorController()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.InvokeRepeating("RefreshDetectorsList", 10f, 10f);
  }

  public void RefreshDetectorsList()
  {
    this.detectors = GameObject.FindGameObjectsWithTag("Detector");
  }

  private void Update()
  {
    if (this.detectors.Length == 0)
      return;
    bool flag = false;
    foreach (GameObject detector in this.detectors)
    {
      if ((double) Vector3.Distance(detector.get_transform().get_position(), ((Component) this).get_transform().get_position()) > (double) this.viewRange)
      {
        Vector3 vector3 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), detector.get_transform().get_position());
        Vector3 normalized = ((Vector3) ref vector3).get_normalized();
        RaycastHit raycastHit;
        if ((double) Vector3.Dot(detector.get_transform().get_forward(), normalized) < (double) this.fov && Physics.Raycast(detector.get_transform().get_position(), normalized, ref raycastHit) && ((Component) ((RaycastHit) ref raycastHit).get_transform()).get_tag() == "Detector")
        {
          flag = true;
          break;
        }
      }
    }
    this.detectionProgress += Time.get_deltaTime() * (!flag ? -0.5f : 0.3f);
    this.detectionProgress = Mathf.Clamp01(this.detectionProgress);
  }
}
