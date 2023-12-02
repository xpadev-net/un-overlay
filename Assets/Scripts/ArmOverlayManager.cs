using System;
using EasyLazyLibrary;
using UnityEngine;

public class ArmOverlayManager : MonoBehaviour {
    [SerializeField]private GameObject canvas;
    private RectTransform canvasRectTransform;
    private RenderTexture targetTexture;
    [SerializeField]private GameObject overlayCameraObj;
    private Camera overlayCamera;
    [SerializeField]private GameObject overlaySystem;
    private EasyOpenVROverlayForUnity easyOverlay;
    private CameraManager cameraManager;
    private string overlayId;

    private int windowWidth;
    private int windowHeight;
    private bool initialized;
    public static ArmOverlayManager Init() {
        var prefab = Resources.Load<GameObject>("ArmOverlayManager");
        var manager = Instantiate(prefab).GetComponent<ArmOverlayManager>();
        return manager;
    }

    private void Start() {
        overlayId = Const.overlayKeyPrefix + "." + Uuid.GetUuid();
        initialized = false;
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        overlayCamera = overlayCameraObj.GetComponent<Camera>();
        easyOverlay = overlaySystem.GetComponent<EasyOpenVROverlayForUnity>();
        cameraManager = overlaySystem.GetComponent<CameraManager>();
        easyOverlay.overlayKeyName = overlayId;
        easyOverlay.overlayFriendlyName = overlayId;
        var sizeDelta = canvasRectTransform.sizeDelta;
        windowWidth = (int)sizeDelta.x;
        windowHeight = (int)sizeDelta.y;
        RegisterWindow();
        easyOverlay.Init();
    }

    // Update is called once per frame
    private void Update() {
        if (initialized)
            return;
        initialized = true;
        targetTexture = new RenderTexture(windowWidth, windowHeight, 32);
        overlayCamera.targetTexture = targetTexture;
        easyOverlay.renderTexture = targetTexture;
        canvasRectTransform.sizeDelta = new Vector2(windowWidth, windowHeight);
        cameraManager.UpdateCameraPov();
    }
    
    public void RegisterWindow() {
        transform.position = new Vector3(WindowControl.instance.RegisterWindow(null,windowWidth), 0, 0);
    }
}
