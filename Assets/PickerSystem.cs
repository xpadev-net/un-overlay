using System.Collections;
using System.Collections.Generic;
using EasyLazyLibrary;
using UnityEngine;

public class PickerSystem : MonoBehaviour
{
    [SerializeField]
    public GameObject leftCursorText; // 左手カーソル表示用Text
    [SerializeField]
    public GameObject rightCursorText; // 右手カーソル表示用Text
    private RectTransform leftCursorTextRectTransform;
    private RectTransform rightCursorTextRectTransform;
    [SerializeField]
    public EasyOpenVROverlayForUnity easyOpenVROverlay;
    [SerializeField]
    public GameObject canvasObj;
    private Canvas canvas;
    private RectTransform canvasRectTransform;
    // Start is called before the first frame update
    void Start()
    {
        leftCursorTextRectTransform = leftCursorText.GetComponent<RectTransform>();
        rightCursorTextRectTransform = rightCursorText.GetComponent<RectTransform>();
        canvas = canvasObj.GetComponent<Canvas>();
        canvasRectTransform = canvasObj.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        var sizeDelta = canvasRectTransform.sizeDelta;
        leftCursorText.SetActive(easyOpenVROverlay.leftHandU > -1f);
        rightCursorText.SetActive(easyOpenVROverlay.rightHandU > -1f);
        leftCursorTextRectTransform.anchoredPosition =
            new Vector2(easyOpenVROverlay.leftHandU - sizeDelta.x / 2f,
                easyOpenVROverlay.leftHandV - sizeDelta.y / 2f);
        rightCursorTextRectTransform.anchoredPosition =
            new Vector2(easyOpenVROverlay.rightHandU - sizeDelta.x / 2f,
                easyOpenVROverlay.rightHandV - sizeDelta.y / 2f);
        
    }
}
