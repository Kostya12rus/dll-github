// Decompiled with JetBrains decompiler
// Type: DetectorController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class DetectorController : MonoBehaviour
{
  public float viewRange = 30f;
  public float fov = -0.75f;
  public float detectionProgress;
  public GameObject[] detectors;

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
      if ((double) Vector3.Distance(detector.transform.position, this.transform.position) > (double) this.viewRange)
      {
        Vector3 normalized = (this.transform.position - detector.transform.position).normalized;
        RaycastHit hitInfo;
        if ((double) Vector3.Dot(detector.transform.forward, normalized) < (double) this.fov && Physics.Raycast(detector.transform.position, normalized, out hitInfo) && hitInfo.transform.tag == "Detector")
        {
          flag = true;
          break;
        }
      }
    }
    this.detectionProgress += Time.deltaTime * (!flag ? -0.5f : 0.3f);
    this.detectionProgress = Mathf.Clamp01(this.detectionProgress);
  }
}
