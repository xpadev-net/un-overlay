using System;
using EasyLazyLibrary;
using UnityEngine;

public class ArmOverlayManager : MonoBehaviour {
    private GameObject canvas;
    private RectTransform canvasRectTransform;
    private RenderTexture targetTexture;
    private GameObject overlayCameraObj;
    private Camera overlayCamera;
    private GameObject overlaySystem;
    private EasyOpenVROverlayForUnity easyOverlay;
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
        canvas = transform.Find("Canvas").gameObject;
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        overlayCameraObj = transform.Find("OverlayCamera").gameObject;
        overlayCamera = overlayCameraObj.GetComponent<Camera>();
        overlaySystem = transform.Find("ClockSystem").gameObject;
        easyOverlay = overlaySystem.GetComponent<EasyOpenVROverlayForUnity>();
        easyOverlay.overlayKeyName = overlayId;
        easyOverlay.overlayFriendlyName = overlayId;
        easyOverlay.Init();
        var sizeDelta = canvasRectTransform.sizeDelta;
        windowWidth = (int)sizeDelta.x;
        windowHeight = (int)sizeDelta.y;
        var fovRadian = Math.Atan2(windowHeight, Math.Abs(overlayCameraObj.transform.position.z));
        var fovDegrees = fovRadian * 180.0 / Math.PI;
        overlayCamera.fieldOfView = (float)fovDegrees;
        transform.position = new Vector3(WindowControl.instance.GetWindowLeft(windowWidth), 0, 0);
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
    }
}
