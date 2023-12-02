using System;
using System.Collections.Generic;
using EasyLazyLibrary;
using UnityEngine;
using UnityEngine.UI;
using uWindowCapture;

public class DesktopOverlayPickerManager : MonoBehaviour
{
    public GameObject canvasObj;
    public HorizontalLayoutGroup buttonGroup;
    public EasyOpenVROverlayForUnity easyOverlay;
    public GameObject pickerSystem;
    public Camera overlayCamera;
    private CameraAdjuster cameraAdjuster;
    private RenderTexture targetTexture;
    private string overlayId;
    private bool initialized;
    private List<DesktopButton> buttons;
    public delegate void OnDeleteCallback();
    public OnDeleteCallback deleteCallback;
    
    public static DesktopOverlayPickerManager CreateInstance() {
        var prefab = Resources.Load<GameObject>("DesktopOverlayPickerManager");
        var manager = Instantiate(prefab).GetComponent<DesktopOverlayPickerManager>();
        return manager;
    }

    public void Init()
    {
        if (initialized)return;
        overlayId = Const.overlayKeyPrefix + "." + Uuid.GetUuid();
        Debug.Log(" DesktopOverlayPickerManager Init Start with id:"+overlayId);
        easyOverlay.overlayKeyName = overlayId;
        easyOverlay.overlayFriendlyName = overlayId;
        easyOverlay.deviceIndex = EasyOpenVROverlayForUnity.TrackingDeviceSelect.RightController;
        cameraAdjuster = pickerSystem.GetComponent<CameraAdjuster>();
        cameraAdjuster.UpdateCameraPov();
        var sizeDelta = canvasObj.GetComponent<RectTransform>().sizeDelta;
        var windowWidth = (int)sizeDelta.x;
        var windowHeight = (int)sizeDelta.y;
        targetTexture = new RenderTexture(windowWidth, windowHeight, 32);
            transform.position =
                new Vector3(WindowControl.instance.RegisterWindow(null,windowWidth), 0, 0);
        overlayCamera.targetTexture = targetTexture;
        easyOverlay.renderTexture = targetTexture;
        easyOverlay.Init();
        initialized = true;
        Debug.Log(" DesktopOverlayPickerManager Init End with id:"+overlayId);
    }

    private void CreateButtons()
    {
        foreach (var button in buttons)
        {
            Destroy(button.gameObject);
        }
        buttons.Clear();
        var count = UwcManager.desktopCount;
        for (var i = 0; i < count; i++)
        {
            var button = DesktopButton.CreateInstance(i,canvasObj);
            button.transform.SetParent(buttonGroup.transform);
            button.windowCreateHandler = CloseWindow;
            button.Init();
            buttons.Add(button);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        initialized = false;
        buttons = new List<DesktopButton>();
    }

    private void Update()
    {
        if (!initialized)
        {
            Init();
            return;
        }
        if (buttons.Count != UwcManager.desktopCount)
        {
            if (UwcManager.desktopCount < 1)
            {
                return;
            }
            CreateButtons();
            return;
        }
    }

    private void CloseWindow()
    {
        Debug.Log("DesktopOverlayPickerManager CloseWindow");
        deleteCallback?.Invoke();
        Destroy(gameObject);
    }
}
