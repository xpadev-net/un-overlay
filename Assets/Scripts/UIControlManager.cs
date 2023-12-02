using System;
using System.Collections.Generic;
using EasyLazyLibrary;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class UIControlManager : MonoBehaviour
{
    [SerializeField]
    private GameObject clockSystem;
    [SerializeField]
    private EasyOpenVROverlayForUnity easyOpenVROverlay;

    [SerializeField] 
    private Canvas canvas;

    private bool PresentOverlayPicker;

    private void Update()
    {
        PresentOverlayPicker = false;
    }

    // Update is called once per frame
    //Canvas上の要素を特定してクリックする
    public void OnAddDesktopClick()
    {
        Debug.Log("add desktop");
        if (PresentOverlayPicker) return;
        var picker = DesktopOverlayPickerManager.CreateInstance();
        PresentOverlayPicker = true;
        picker.deleteCallback = () =>
        {
            Debug.Log("delete");
            PresentOverlayPicker = false;
        };
    }

    public void OnResetClick()
    {
        Debug.Log("reset");
        WindowControl.instance.Reset();
        clockSystem.GetComponent<ArmOverlayManager>().RegisterWindow();
    }
    
    public void OnSettingClick()
    {
        Debug.Log("Setting!");
    }
}
