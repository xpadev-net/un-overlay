/*
 * PositionManagerScript.cs
 *
 * ScreenMove And Cursor Sample for
 *  EasyOpenVRUtil
 *  https://github.com/gpsnmeajp/EasyOpenVRUtil
 *  EasyOpenVROverlayForUnity
 *  https://sabowl.sakura.ne.jp/gpsnmeajp/unity/EasyOpenVROverlayForUnity/
 *
 * gpsnmeajp 2019/01/04 v0.02
 * v0.02: ビルドすると位置が原点になる問題に対処
 * v0.01: 公開
 *
 * These codes are licensed under CC0.
 * http://creativecommons.org/publicdomain/zero/1.0/deed.ja
 */

using UnityEngine;
using EasyLazyLibrary;

public class ClockWindowManager : MonoBehaviour {
    [SerializeField]
    private EasyOpenVROverlayForUnity easyOpenVROverlay; // オーバーレイ表示用ライブラリ
    [SerializeField]
    private GameObject leftCursorText; // 左手カーソル表示用Text

    private RectTransform leftCursorTextRectTransform;
    [SerializeField]
    private GameObject rightCursorText; // 右手カーソル表示用Text

    private RectTransform rightCursorTextRectTransform;
    [SerializeField]
    private RectTransform canvasRectTransform; // 全体サイズ計算用Canvas位置情報

    private readonly EasyOpenVRUtil util = new EasyOpenVRUtil(); // 姿勢取得ライブラリ

    private bool positionInitialize = true; // 位置を初期化するフラグ(完了するとfalseになる)

    private Vector3 screenOffsetTransform;
    private Quaternion screenOffsetRotation;
    [SerializeField]
    public bool isLeftHand = true;

    private void Start() {
        // 姿勢取得ライブラリを初期化
        util.Init();
        leftCursorTextRectTransform = leftCursorText.GetComponent<RectTransform>();
        rightCursorTextRectTransform = rightCursorText.GetComponent<RectTransform>();
        isLeftHand = false;
    }

    private void Update() {
        // 姿勢取得ライブラリが初期化されていないとき初期化する
        //(OpenVRの初期化はeasyOpenVROverlayの方で行われるはずなので待機)
        if (!util.IsReady()) {
            util.Init();
            return;
        }

        // HMDの位置情報が使えるようになった & 初期位置が初期化されていないとき
        if (util.GetHMDTransform() != null && positionInitialize) {
            // とりあえずUnityスタート時のHMD位置に設定
            //(サンプル用。より適切なタイミングで呼び直してください。
            //  OpenVRが初期化されていない状態では原点になってしまいます)
            InitPosition();
            // 初期位置初期化処理を停止
            positionInitialize = false;
        }

        UpdateCursorPos();
        HandleInput();
    }

    private void UpdateCursorPos() {

        // カーソル位置を更新
        // オーバーレイライブラリが返す座標系をCanvasの座標系に変換している。
        // オーバーレイライブラリの座標サイズ(RenderTexture依存)と
        // Canvasの幅・高さが一致する必要がある。
        var sizeDelta = canvasRectTransform.sizeDelta;

        leftCursorText.SetActive(easyOpenVROverlay.leftHandU > -1f && !isLeftHand);
        rightCursorText.SetActive(easyOpenVROverlay.rightHandU > -1f && isLeftHand);
        leftCursorTextRectTransform.anchoredPosition =
            new Vector2(easyOpenVROverlay.leftHandU - sizeDelta.x / 2f,
                        easyOpenVROverlay.leftHandV - sizeDelta.y / 2f);
        rightCursorTextRectTransform.anchoredPosition =
            new Vector2(easyOpenVROverlay.rightHandU - sizeDelta.x / 2f,
                        easyOpenVROverlay.rightHandV - sizeDelta.y / 2f);
    }

    // コントローラによる画面移動モードにはいる
    private void HandleInput() {
        if (easyOpenVROverlay.leftHandU > -1f && !isLeftHand) {
            // if (!isScreenMoving&&util.IsControllerButtonPressed(util.GetLeftControllerIndex(),
            // EVRButtonId.k_EButton_Grip))
            // {
            // }
            return;
        }
        if (easyOpenVROverlay.rightHandU > -1f && isLeftHand) {
            // if (!isScreenMoving&&util.IsControllerButtonPressed(util.GetRightControllerIndex(),
            // EVRButtonId.k_EButton_Grip))
            // {
            // }
        }
    }

    // HMDの位置を基準に操作しやすい位置に画面を出す
    private void InitPosition() {
        // HMDの姿勢情報を取得する
        var pos = util.GetHMDTransform();

        // HMDの姿勢情報が無効な場合は
        if (pos == null) {
            return; // 更新しない
        }

        easyOpenVROverlay.deviceTracking = true;
        easyOpenVROverlay.deviceIndex =
            isLeftHand ? EasyOpenVROverlayForUnity.TrackingDeviceSelect.LeftController
                       : EasyOpenVROverlayForUnity.TrackingDeviceSelect.RightController;
        easyOpenVROverlay.position = isLeftHand ?new Vector3(-0.05f, 0, -0.1f):new Vector3(0.05f, 0, -0.1f);
        easyOpenVROverlay.rotation = isLeftHand ?new Vector3(190, 90,  0):new Vector3(190, 270,  0);
    }
}