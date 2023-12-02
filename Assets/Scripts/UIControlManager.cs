using System;
using System.Collections.Generic;
using EasyLazyLibrary;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class UIControlManager : MonoBehaviour
{
    [SerializeField]
    private ArmOverlayManager armOverlayManager;
    [SerializeField]
    private EasyOpenVROverlayForUnity easyOpenVROverlay;

    [SerializeField] 
    private Canvas canvas;

    private bool presentOverlayPicker;
    private bool presentSettingWindow;

    private void Start()
    {
        presentOverlayPicker = false;
        presentSettingWindow = false;
    }

    // Update is called once per frame
    //Canvas上の要素を特定してクリックする
    public void OnAddDesktopClick()
    {
        Debug.Log("[OnAddDesktopClick] add desktop "+presentOverlayPicker);
        if (presentOverlayPicker) return;
        var picker = DesktopOverlayPickerManager.CreateInstance();
        presentOverlayPicker = true;
        picker.deleteCallback = () =>
        {
            Debug.Log("[OnAddDesktopClick] delete");
            presentOverlayPicker = false;
        };
    }

    public void OnResetClick()
    {
        Debug.Log("reset");
        WindowControl.instance.Reset();
        armOverlayManager.RegisterWindow();
    }
    
    public void OnSettingClick()
    {
        Debug.Log("[OnSettingClick] add setting "+presentOverlayPicker);
        if (presentSettingWindow) return;
        var picker = SettingWindowManager.CreateInstance();
        presentSettingWindow = true;
        picker.deleteCallback = () =>
        {
            Debug.Log("[OnSettingClick] delete");
            presentSettingWindow = false;
        };
    }
}
