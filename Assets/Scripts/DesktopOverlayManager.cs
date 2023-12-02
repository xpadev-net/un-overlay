using EasyLazyLibrary;
using UnityEngine;
using uWindowCapture;

public class DesktopOverlayManager : MonoBehaviour {
    public int desktopIndex = -1;
    private GameObject desktopCapture;
    private Transform desktopCaptureTransform;
    private GameObject canvas;
    private RectTransform canvasRectTransform;
    private UwcWindowTexture windowTexture;
    private RenderTexture targetTexture;
    private GameObject overlayCameraObj;
    private Camera overlayCamera;
    private GameObject overlaySystem;
    private EasyOpenVROverlayForUnity easyOverlay;
    private string overlayId;

    private int windowWidth;
    private int windowHeight;
    private bool initialized;
    public static DesktopOverlayManager Init(int desktopIndex) {
        var prefab = Resources.Load<GameObject>("DesktopOverlayManager");
        var manager = Instantiate(prefab).GetComponent<DesktopOverlayManager>();
        manager.desktopIndex = desktopIndex;
        return manager;
    }

    private void Start() {
        overlayId = Const.overlayKeyPrefix + "." + Uuid.GetUuid();
        initialized = false;
        desktopCapture = transform.Find("DesktopCapture").gameObject;
        windowTexture = desktopCapture.GetComponent<UwcWindowTexture>();
        desktopCaptureTransform = desktopCapture.GetComponent<Transform>();
        canvas = transform.Find("Canvas").gameObject;
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        overlayCameraObj = transform.Find("OverlayCamera").gameObject;
        overlayCamera = overlayCameraObj.GetComponent<Camera>();
        overlaySystem = transform.Find("OverlaySystem").gameObject;
        easyOverlay = overlaySystem.GetComponent<EasyOpenVROverlayForUnity>();
        easyOverlay.OverlayKeyName = overlayId;
        easyOverlay.OverlayFriendlyName = overlayId;
        easyOverlay.Init();
    }

    // Update is called once per frame
    private void Update() {
        if (!initialized) {
            if (!windowTexture || windowTexture.window == null || desktopIndex < 0) {
                return;
            }
            windowWidth = windowTexture.window.width;
            windowHeight = windowTexture.window.height;
            if (windowHeight + windowWidth < 1) {
                return;
            }
            initialized = true;
            desktopCaptureTransform.localScale = new Vector3(windowWidth, windowHeight);
            targetTexture = new RenderTexture(windowWidth, windowHeight, 32);
            overlayCamera.targetTexture = targetTexture;
            easyOverlay.renderTexture = targetTexture;
            canvasRectTransform.sizeDelta = new Vector2(windowWidth, windowHeight);
            windowTexture.desktopIndex = desktopIndex;
            transform.position =
                new Vector3(WindowControl.instance.GetWindowLeft(windowWidth), 0, 0);
        }

        if (windowTexture.desktopIndex != desktopIndex) {
            windowTexture.desktopIndex = desktopIndex;
        }
    }
}
