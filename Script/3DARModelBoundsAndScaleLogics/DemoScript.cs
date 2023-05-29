using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour {
    [SerializeField] private GameObject objParent; // parent will be automatically start with default values position (0,0,0) , scale (1,1,1) , Roatation (0,0,0)

    [SerializeField] private bool scallingEnable; // to enabling the auto scalling for the very first time

    [SerializeField] private int margin; // margin must be 20% as lower and highest 50%

    public BoundsRecalculations.SideSelect sideSelect;

    public void OnButtonClick() {
        BoundsRecalculations.instance.ChildAlignmentAndScalling(objParent, scallingEnable, margin, sideSelect);
    }
}