using System.Collections.Generic;
using EasyLazyLibrary;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class UIControlManager : MonoBehaviour
{
    [SerializeField]
    private EasyOpenVROverlayForUnity easyOpenVROverlay;

    [SerializeField] 
    private Canvas canvas;
    
    // Update is called once per frame
    //Canvas上の要素を特定してクリックする
    public void OnAddDesktopClick()
    {
        Debug.Log("Add Desktop!");
    }

    public void OnResetClick()
    {
        Debug.Log("Reset!");
    }
    
    public void OnSettingClick()
    {
        Debug.Log("Setting!");
    }
}
