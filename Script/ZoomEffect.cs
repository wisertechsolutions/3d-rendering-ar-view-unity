using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViitorCloud.ARModelViewer;

public class ZoomEffect : MonoBehaviour {
    public void DisbaleAnimation() {
        GameManager.instance.panelZoomInOut.SetActive(false);
    }
}
