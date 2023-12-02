/**
 * EasyOpenVROverlayForUnity by gpsnmeajp v0.24
 * 2019/07/15
 * 
 * v0.24 OpenGL環境で正常に動作しない問題修正
 *  showDevicesでエラーが発生することがあるためコメントアウト
 * 
 * v0.23 Side-by-Side 3D対応
 *
 * v0.22 デバイスアップデート
 *  デバイス情報を取得して一覧をログに出すように
 *  選択したデバイスの詳細情報を取得できるように
 *  トラッカーやベースステーションの位置にオーバーレイを出せるように
 *  Tagに対するWarnningsを抑制
 * 
 * v0.21 微修正
 * v0.2 大規模更新
 *  デバッグタグの方法を変更
 *  uGUIのクリックに対応
 *  コントローラーを選択できるように
 *  外部からのエラーチェック、表示状態管理関数を追加。
 *  各処理を関数化
 *  終了時に開放する処理を追加
 *  エラー時に開放する処理を追加
 *  マウススケールの処理を追加
 *  終了イベントのキャッチを追加
 * v0.1 公開 2018/08/25
 * 
 * 2DのテクスチャをVR空間にオーバーレイ表示します。
 * 現在動作中のアプリケーションに関係なくオーバーレイすることができます。
 * 
 * 入力機能は正常に動作していないようなので省いています。
 * ダッシュボードオーバーレイは省略しています。
 *  *
 * These codes are licensed under CC0.
 * http://creativecommons.org/publicdomain/zero/1.0/deed.ja
 */

/** uGUIのクリックについて
 * 
 * 使い方
 * 1. LaycastRootObjectには、操作したいCanvas(シーン直下に配置)を設定
 * 2. Buttonのクリックだけ対応(コントローラーの先端でOverlayを叩くとクリック)
 * 3. ButtonのRaycast TargetはONに。ButtonのTextにあるRaycast TargetはOFFに
 * 4. CanvasのRender Modeは"Screen Space - Camera"に。
 * 5. CanvasのRender Cameraは、RenderTextureを設定したCameraと同じものにすること
 * なお、LaycastRootObjectをnull(None)にするとGUI機能は無効化される
 */

using System.Text;
using UnityEngine;
using Valve.VR;


namespace EasyLazyLibrary
{
    public class EasyOpenVROverlayForUnity : MonoBehaviour
    {
        //エラーフラグ
        public bool error = true; //初期化失敗

        //イベントに関するログを表示するか
        public bool eventLog = false;

        [Header("RenderTexture")]
        //取得元のRenderTexture
        public RenderTexture renderTexture;

        [Header("Transform")]
        //Unity準拠の位置と回転
        public Vector3 position = new Vector3(0, -0.5f, 3);

        public Vector3 rotation = new Vector3(0, 0, 0);

        public Vector3 scale = new Vector3(1, 1, 1);

        //鏡像反転できるように
        public bool mirrorX = false;
        public bool mirrorY = false;

        [Header("Setting")]
        //オーバーレイの大きさ設定(幅のみ。高さはテクスチャの比から自動計算される)
        [Range(0, 100)]
        public float width = 5.0f;

        //オーバーレイの透明度を設定
        [Range(0, 1)] public float alpha = 0.2f;

        //表示するか否か
        public bool show = true;

        //サイドバイサイド3D
        public bool sideBySide = false;


        [Header("Name")]
        //ユーザーが確認するためのオーバーレイの名前
        public string overlayFriendlyName = "SampleOverlay";

        //グローバルキー(システムのオーバーレイ同士の識別名)。
        //ユニークでなければならない。乱数やUUIDなどを勧める
        public string overlayKeyName = "SampleOverlay";

        [Header("DeviceTracking")]
        //絶対空間か
        public bool deviceTracking = true;

        //追従対象デバイス。HMD=0
        //public uint DeviceIndex = OpenVR.k_unTrackedDeviceIndex_Hmd;
        public TrackingDeviceSelect deviceIndex = TrackingDeviceSelect.HMD;
        private int deviceIndexOld = (int)TrackingDeviceSelect.None;

        [Header("Absolute space")]
        //(絶対空間の場合)ルームスケールか、着座状態か
        public bool seated = false;

        //着座カメラのリセット(リセット後自動でfalseに戻ります)
        public bool resetSeatedCamera = false;

        //追従対象リスト。コントロラーは変動するので特別処理
        public enum TrackingDeviceSelect
        {
            None = -99,
            RightController = -2,
            LeftController = -1,
            HMD = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
            Device1 = 1,
            Device2 = 2,
            Device3 = 3,
            Device4 = 4,
            Device5 = 5,
            Device6 = 6,
            Device7 = 7,
            Device8 = 8,
        }

        //--------------------------------------------------------------------------

        [Header("Device Info")]
        //現在接続されているデバイス一覧をログに出力(自動でfalseに戻ります)
        public bool putLogDevicesInfo = false;

        //(デバイスを選択した時点で)現在接続されているデバイス数
        public int connectedDevices = 0;

        //選択デバイス番号
        public int selectedDeviceIndex = 0;

        //選択デバイスのシリアル番号
        public string deviceSerialNumber = null;

        //選択デバイスのモデル名
        public string deviceRenderModelName = null;


        [Header("GUI Tap")]
        //レイキャスト対象識別用ルートCanvasオブジェクト
        public GameObject laycastRootObject = null;

        //タップ状態管理
        public bool tappedLeft = false;
        public bool tappedRight = false;

        //タップ距離
        private const float TapOnDistance = 0.04f;
        private const float TapOffDistance = 0.043f;

        //カーソル位置表示用変数
        public float leftHandU = -1f;
        public float leftHandV = -1f;
        public float leftHandDistance = -1f;
        public float rightHandU = -1f;
        public float rightHandV = -1f;
        public float rightHandDistance = -1f;

        //右手か左手か
        enum LeftOrRight
        {
            Left = 0,
            Right = 1
        }

        //--------------------------------------------------------------------------

        //オーバーレイのハンドル(整数)
        private ulong overlayHandle = InvalidHandle;

        //OpenVRシステムインスタンス
        private CVRSystem openvr = null;

        //Overlayインスタンス
        private CVROverlay overlay = null;

        //オーバーレイに渡すネイティブテクスチャ
        private Texture_t overlayTexture;

        //HMD視点位置変換行列
        private HmdMatrix34_t p;

        //無効なハンドル
        private const ulong InvalidHandle = 0;

        private bool initialized = false;
        
        [SerializeField]
        private CursorManager cursorManager;

        //--------------------------------------------------------------------------

        //外部から透明度設定切り替え
        public void SetAlpha(float a)
        {
            alpha = a;
        }

        //外部からdevice切り替え
        public void ChangeToHmd()
        {
            deviceIndex = TrackingDeviceSelect.HMD;
        }

        //外部からdevice切り替え
        public void ChangeToLeftController()
        {
            deviceIndex = TrackingDeviceSelect.LeftController;
        }

        //外部からdevice切り替え
        public void ChangeToRightController()
        {
            deviceIndex = TrackingDeviceSelect.RightController;
        }

        //--------------------------------------------------------------------------

        //Overlayが表示されているかどうか外部からcheck
        public bool IsVisible()
        {
            return overlay.IsOverlayVisible(overlayHandle) && !IsError();
        }

        //エラー状態かをチェック
        public bool IsError()
        {
            return error || overlayHandle == InvalidHandle || overlay == null || openvr == null;
        }

        //エラー処理(開放処理)
        private void ProcessError()
        {

#pragma warning disable 0219
            string currentMethod = "[" + this.GetType().Name + ":" +
                         System.Reflection.MethodBase.GetCurrentMethod(); //クラス名とメソッド名を自動取得
#pragma warning restore 0219
            Debug.Log(currentMethod + "Begin");

            //ハンドルを解放
            if (overlayHandle != InvalidHandle && overlay != null)
            {
                overlay.DestroyOverlay(overlayHandle);
            }

            overlayHandle = InvalidHandle;
            overlay = null;
            openvr = null;
            error = true;
        }

        //オブジェクト破棄時
        private void OnDestroy()
        {

#pragma warning disable 0219
            string currentMethod = "[" + this.GetType().Name + ":" +
                         System.Reflection.MethodBase.GetCurrentMethod(); //クラス名とメソッド名を自動取得
#pragma warning restore 0219
            Debug.Log(currentMethod + "Begin");

            //ハンドル類の全開放
            ProcessError();
        }

        //アプリケーションの終了を検出した時
        private void OnApplicationQuit()
        {

#pragma warning disable 0219
            string currentMethod = "[" + this.GetType().Name + ":" +
                         System.Reflection.MethodBase.GetCurrentMethod(); //クラス名とメソッド名を自動取得
#pragma warning restore 0219
            Debug.Log(currentMethod + "Begin");

            //ハンドル類の全開放
            ProcessError();
        }

        //アプリケーションを終了させる
        private void ApplicationQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        //--------------------------------------------------------------------------

        //初期化処理
        private void Start()
        {
            initialized = false;
        }

        public void Init()
        {
            
#pragma warning disable 0219
            string currentMethod = "[" + this.GetType().Name + ":" +
                         System.Reflection.MethodBase.GetCurrentMethod(); //クラス名とメソッド名を自動取得
#pragma warning restore 0219
            Debug.Log(currentMethod + "Begin");
            initialized = true;

            error = false;

            //フレームレートを90fpsにする。(しないと無限に早くなることがある)
            Application.targetFrameRate = 90;
            Debug.Log(currentMethod + "Set Frame Rate 90");

            //OpenVRの初期化
            var openVRError = EVRInitError.None;
            openvr = OpenVR.Init(ref openVRError, EVRApplicationType.VRApplication_Overlay);
            if (openVRError != EVRInitError.None)
            {
                Debug.LogError(currentMethod + "OpenVRの初期化に失敗." + openVRError.ToString());
                ProcessError();
                return;
            }

            //オーバーレイ機能の初期化
            overlay = OpenVR.Overlay;
            var overlayError = overlay.CreateOverlay(overlayKeyName, overlayFriendlyName, ref overlayHandle);
            if (overlayError != EVROverlayError.None)
            {
                Debug.LogError(currentMethod + "Overlayの初期化に失敗. " + overlayError.ToString());
                ProcessError();
                return;
            }

            //オーバーレイに渡すテクスチャ種類の設定
            var OverlayTextureBounds = new VRTextureBounds_t();
            var isOpenGL = SystemInfo.graphicsDeviceVersion.Contains("OpenGL");
            if (isOpenGL)
            {
                //pGLuintTexture
                overlayTexture.eType = ETextureType.OpenGL;
                //上下反転しない
                OverlayTextureBounds.uMin = 0;
                OverlayTextureBounds.vMin = 0;
                OverlayTextureBounds.uMax = 1;
                OverlayTextureBounds.vMax = 1;
                overlay.SetOverlayTextureBounds(overlayHandle, ref OverlayTextureBounds);
            }
            else
            {
                //pTexture
                overlayTexture.eType = ETextureType.DirectX;
                //上下反転する
                OverlayTextureBounds.uMin = 0;
                OverlayTextureBounds.vMin = 1;
                OverlayTextureBounds.uMax = 1;
                OverlayTextureBounds.vMax = 0;
                overlay.SetOverlayTextureBounds(overlayHandle, ref OverlayTextureBounds);
            }

            //--------
            //showDevices();

            Debug.Log(currentMethod + "初期化完了しました");
        }
        
        private void Update()
        {

            if (!initialized) return;
#pragma warning disable 0219
            string currentMethod = "[" + this.GetType().Name + ":" +
                         System.Reflection.MethodBase.GetCurrentMethod(); //クラス名とメソッド名を自動取得
#pragma warning restore 0219

            //エラーが発生した場合や、ハンドルが無効な場合は実行しない
            if (IsError())
            {
                return;
            }

            if (show)
            {
                //オーバーレイを表示する
                overlay.ShowOverlay(overlayHandle);
            }
            else
            {
                //オーバーレイを非表示にする
                overlay.HideOverlay(overlayHandle);
            }

            //イベントを処理する(終了された時true)
            if (ProcessEvent())
            {
                Debug.Log(currentMethod + "VRシステムが終了されました");
                ApplicationQuit();
            }

            //オーバーレイが表示されている時
            if (overlay.IsOverlayVisible(overlayHandle))
            {
                //位置情報と各種設定の更新
                UpdatePosition();
                //表示情報の更新
                UpdateTexture();

                //Canvasが設定されている場合
                if (laycastRootObject != null)
                {
                    //GUIタッチ機能の処理
                    UpdateVRTouch();
                }
            }

            if (putLogDevicesInfo)
            {
                showDevices();
                putLogDevicesInfo = false;
            }
        }

        //位置情報を更新
        private void UpdatePosition()
        {

#pragma warning disable 0219
            string currentMethod = "[" + this.GetType().Name + ":" +
                         System.Reflection.MethodBase.GetCurrentMethod(); //クラス名とメソッド名を自動取得
#pragma warning restore 0219

            //RenderTextureが生成されているかチェック
            if (!renderTexture.IsCreated())
            {
                Debug.Log(currentMethod + "RenderTextureがまだ生成されていない");
                return;
            }

            //回転を生成
            Quaternion quaternion = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            //座標系を変更(右手系と左手系の入れ替え)
            Vector3 position = this.position;
            position.z = -this.position.z;
            //HMD視点位置変換行列に書き込む。
            Matrix4x4 m = Matrix4x4.TRS(position, quaternion, scale);

            //鏡像反転
            Vector3 Mirroring = new Vector3(mirrorX ? -1 : 1, mirrorY ? -1 : 1, 1);

            //4x4行列を3x4行列に変換する。
            p.m0 = Mirroring.x * m.m00;
            p.m1 = Mirroring.y * m.m01;
            p.m2 = Mirroring.z * m.m02;
            p.m3 = m.m03;
            p.m4 = Mirroring.x * m.m10;
            p.m5 = Mirroring.y * m.m11;
            p.m6 = Mirroring.z * m.m12;
            p.m7 = m.m13;
            p.m8 = Mirroring.x * m.m20;
            p.m9 = Mirroring.y * m.m21;
            p.m10 = Mirroring.z * m.m22;
            p.m11 = m.m23;

            //回転行列を元に相対位置で表示
            if (deviceTracking)
            {
                //deviceindexを処理(コントローラーなどはその時その時で変わるため)
                var idx = OpenVR.k_unTrackedDeviceIndex_Hmd;
                switch (deviceIndex)
                {
                    case TrackingDeviceSelect.LeftController:
                        idx = openvr.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
                        break;
                    case TrackingDeviceSelect.RightController:
                        idx = openvr.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
                        break;
                    default:
                        idx = (uint)deviceIndex;
                        break;
                }

                //device情報に変化があったらInspectorに反映
                if (deviceIndexOld != (int)idx)
                {
                    Debug.Log(currentMethod + "Device Updated");
                    UpdateDeviceInfo(idx);
                    deviceIndexOld = (int)idx;
                }

                //HMDからの相対的な位置にオーバーレイを表示する。
                overlay.SetOverlayTransformTrackedDeviceRelative(overlayHandle, idx, ref p);
            }
            else
            {
                //空間の絶対位置にオーバーレイを表示する
                if (!seated)
                {
                    overlay.SetOverlayTransformAbsolute(overlayHandle, ETrackingUniverseOrigin.TrackingUniverseStanding,
                        ref p);
                }
                else
                {
                    overlay.SetOverlayTransformAbsolute(overlayHandle, ETrackingUniverseOrigin.TrackingUniverseSeated,
                        ref p);
                }
            }

            if (resetSeatedCamera)
            {
                //OpenVR.System.ResetSeatedZeroPose();
                resetSeatedCamera = false;
            }

            //オーバーレイの大きさ設定(幅のみ。高さはテクスチャの比から自動計算される)
            overlay.SetOverlayWidthInMeters(overlayHandle, width);

            //オーバーレイの透明度を設定
            overlay.SetOverlayAlpha(overlayHandle, alpha);

            //マウスカーソルスケールを設定する(これにより表示領域のサイズも決定される)
            try
            {
                HmdVector2_t vecMouseScale = new HmdVector2_t
                {
                    v0 = renderTexture.width,
                    v1 = renderTexture.height
                };
                overlay.SetOverlayMouseScale(overlayHandle, ref vecMouseScale);
            }
            catch (UnassignedReferenceException e)
            {
                Debug.LogError(currentMethod + "RenderTextureがセットされていません " + e.ToString());
                ProcessError();
                return;
            }

        }

        //表示情報を更新
        private void UpdateTexture()
        {

#pragma warning disable 0219
            string currentMethod = "[" + this.GetType().Name + ":" +
                                   System.Reflection.MethodBase.GetCurrentMethod(); //クラス名とメソッド名を自動取得
#pragma warning restore 0219

            overlay.SetOverlayFlag(overlayHandle, VROverlayFlags.SideBySide_Parallel, sideBySide);

            //RenderTextureが生成されているかチェック
            if (!renderTexture.IsCreated())
            {
                Debug.Log(currentMethod + "RenderTextureがまだ生成されていない");
                return;
            }

            //RenderTextureからネイティブテクスチャのハンドルを取得
            try
            {
                overlayTexture.handle = renderTexture.GetNativeTexturePtr();
            }
            catch (UnassignedReferenceException e)
            {
                Debug.LogError(currentMethod + "RenderTextureがセットされていません " + e.ToString());
                ProcessError();
                return;
            }

            //オーバーレイにテクスチャを設定
            var overlayError = EVROverlayError.None;
            overlayError = overlay.SetOverlayTexture(overlayHandle, ref overlayTexture);
            if (overlayError != EVROverlayError.None)
            {
                Debug.LogError(currentMethod + "Overlayにテクスチャをセットできませんでした. " + overlayError.ToString());
                //致命的なエラーとしない
                return;
            }
        }

        //終了イベントをキャッチした時に戻す
        private bool ProcessEvent()
        {

#pragma warning disable 0219
            string currentMethod = "[" + this.GetType().Name + ":" +
                                   System.Reflection.MethodBase.GetCurrentMethod(); //クラス名とメソッド名を自動取得
#pragma warning restore 0219

            //イベント構造体のサイズを取得
            uint uncbVREvent = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VREvent_t));

            //イベント情報格納構造体
            VREvent_t vrEvent = new VREvent_t();
            //イベントを取り出す
            while (overlay.PollNextOverlayEvent(overlayHandle, ref vrEvent, uncbVREvent))
            {
                //イベントのログを表示
                if (eventLog)
                {
                    Debug.Log(currentMethod + "Event:" + ((EVREventType)vrEvent.eventType).ToString());
                }

                //イベント情報で分岐
                switch ((EVREventType)vrEvent.eventType)
                {
                    case EVREventType.VREvent_Quit:
                        Debug.Log(currentMethod + "Quit");
                        return true;
                }
            }

            return false;
        }


        //----------おまけ(deviceの詳細情報)-------------

        //全てのdeviceの情報をログに出力する
        private void showDevices()
        {
            //すべてのdeviceの接続状態を取得
            var allDevicePose = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            openvr.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0f, allDevicePose);

            //接続されているdeviceの数をカウントする
            uint connectedDeviceNum = 0;
            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                if (allDevicePose[i].bDeviceIsConnected)
                {
                    connectedDeviceNum++;
                }
            }

            //deviceの詳細情報を1つづつ読み出す
            uint connectedDeviceCount = 0;
            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                //接続中だったら、読み取り完了数を1増やす
                if (GetPropertyAndPutLog(i, allDevicePose))
                {
                    connectedDeviceCount++;
                }

                //接続されている数だけ読み取り終わったら終了する
                if (connectedDeviceCount >= connectedDeviceNum)
                {
                    break;
                }
            }
        }

        //deviceの情報をログに出力する(1項目)
        private bool GetPropertyAndPutLog(uint index, TrackedDevicePose_t[] allDevicePose)
        {
#pragma warning disable 0219
            var currentMethod = "[" + this.GetType().Name + ":" +
                                System.Reflection.MethodBase.GetCurrentMethod(); //クラス名とメソッド名を自動取得
#pragma warning restore 0219

            //接続されているかをチェック
            if (allDevicePose[index].bDeviceIsConnected)
            {
                //接続されているdevice

                //デバイスシリアル番号(Trackerの識別によく使う)と、deviceモデル名(device種類)を取得
                var serial = GetProperty(index, ETrackedDeviceProperty.Prop_SerialNumber_String);
                var modelNumber = GetProperty(index, ETrackedDeviceProperty.Prop_RenderModelName_String);
                if (serial != null && modelNumber != null)
                {
                    //ログに表示
                    Debug.Log(currentMethod + "Device " + index + ":" + serial + " : " + modelNumber);
                }
                else
                {
                    //何らかの理由で取得失敗した
                    Debug.Log(currentMethod + "Device " + index + ": Error");
                }

                return true;
            }
            else
            {
                //接続されていないdevice
                Debug.Log(currentMethod + "Device " + index + ": Not connected");
                return false;
            }
        }

        //指定されたdeviceの情報をInspectorに反映する
        private void UpdateDeviceInfo(uint idx)
        {
            //すべてのdeviceの接続状態を取得
            var allDevicePose = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            openvr.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0f, allDevicePose);

            //接続されているdeviceの数をカウントする
            connectedDevices = 0;
            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                if (allDevicePose[i].bDeviceIsConnected)
                {
                    connectedDevices++;
                }
            }

            //deviceの情報をInspectorに反映する
            selectedDeviceIndex = (int)idx;
            deviceSerialNumber = GetProperty(idx, ETrackedDeviceProperty.Prop_SerialNumber_String);
            deviceRenderModelName = GetProperty(idx, ETrackedDeviceProperty.Prop_RenderModelName_String);
        }

        //device情報を取得する
        private string GetProperty(uint idx, ETrackedDeviceProperty prop)
        {
            var propertyError = new ETrackedPropertyError();
            var size = openvr.GetStringTrackedDeviceProperty(idx, prop, null, 0, ref propertyError);
            if (propertyError != ETrackedPropertyError.TrackedProp_BufferTooSmall) return null;

            var s = new StringBuilder
            {
                Length = (int)size
            };
            openvr.GetStringTrackedDeviceProperty(idx, prop, s, size, ref propertyError);
            if (propertyError != ETrackedPropertyError.TrackedProp_Success) return null;

            return s.ToString();
        }


        //----------おまけ(コントローラーでOverlayを叩いてuGUIをクリックできるやつ)-------------

        //uGUIクリックを実現する
        private void UpdateVRTouch()
        {
            var allDevicePose = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            openvr.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0f, allDevicePose);

            ProcessController(allDevicePose,true,ref leftHandU,ref leftHandV,ref leftHandDistance);
            ProcessController(allDevicePose,false,ref rightHandU,ref rightHandV,ref rightHandDistance);
        }

        private void ProcessController(TrackedDevicePose_t[] allDevicePose, bool isLeft,ref float u, ref float v, ref float distance)
        {
            var results = new VROverlayIntersectionResults_t();

            var index =
                openvr.GetTrackedDeviceIndexForControllerRole(isLeft
                    ?  ETrackedControllerRole.LeftHand
                    :ETrackedControllerRole.RightHand);
            if (CheckRay(index, allDevicePose, ref results))
            {
                //線上にオーバーレイがある場合は続けて処理
                if (isLeft)
                {
                    CollisionHaptic(results, LeftOrRight.Left, ref tappedLeft);
                }
                else
                {
                    CollisionHaptic(results, LeftOrRight.Right, ref tappedRight);
                }

                //カーソル表示用に更新
                u = results.vUVs.v0 * renderTexture.width;
                v = renderTexture.height - results.vUVs.v1 * renderTexture.height;
                distance = results.fDistance;
                cursorManager.UpdateCursor(isLeft,new Vector2(u,v));
            }
            else
            {
                if (distance > -1f)
                {
                    cursorManager.HoverOut(isLeft);
                }
                u = -1f;
                v = -1f;
                distance = -1f;
            }
        }

        //指定されたdeviceが有効かチェックした上で、オーバーレイと交点を持つかチェック
        private bool CheckRay(uint idx, TrackedDevicePose_t[] allDevicePose, ref VROverlayIntersectionResults_t results)
        {
            //device indexが有効
            if (idx == OpenVR.k_unTrackedDeviceIndexInvalid) return false;
            //接続されていて姿勢情報が有効
            if (!allDevicePose[idx].bDeviceIsConnected || !allDevicePose[idx].bPoseIsValid) return false;
            //姿勢情報などを変換してもらう
            var post = allDevicePose[idx];
            var rigidTransform =
                new SteamVR_Utils.RigidTransform(post.mDeviceToAbsoluteTracking);

            //コントローラー用に45度前方に傾けた方向ベクトルを計算
            var rotation = (rigidTransform.rot * Quaternion.AngleAxis(45, Vector3.right)) * Vector3.forward;

            return ComputeOverlayIntersection(rigidTransform.pos, rotation, ref results);
        }

        //オーバーレイと交点を持つかチェック
        private bool ComputeOverlayIntersection(Vector3 pos, Vector3 rotation,
            ref VROverlayIntersectionResults_t results)
        {

            //レイ照射情報
            VROverlayIntersectionParams_t param = new VROverlayIntersectionParams_t();
            //レイ発射元位置
            param.vSource = new HmdVector3_t
            {
                v0 = pos.x,
                v1 = pos.y,
                v2 = -pos.z //右手系 to 左手系
            };
            //レイ発射単位方向ベクトル
            param.vDirection = new HmdVector3_t
            {
                v0 = rotation.x,
                v1 = rotation.y,
                v2 = -rotation.z //右手系 to 左手系
            };
            //ルーム空間座標系で照射
            param.eOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding;

            //Overlayと交差していればtrue、していなければfalseで、詳細情報がresultsに入る
            return overlay.ComputeOverlayIntersection(overlayHandle, ref param, ref results);
        }

        //タップされているかどうかを調べる
        private void CollisionHaptic(VROverlayIntersectionResults_t results, LeftOrRight lr, ref bool tapped)
        {
            switch (results.fDistance)
            {
                //コントローラとオーバーレイの距離が一定以下なら
                case < TapOnDistance when !tapped:
                    //タップされた
                    tapped = true;
                    Haptic(lr);
                    break;
                case > TapOffDistance when tapped:
                    //離れた
                    tapped = false;
                    Haptic(lr);
                    break;
            }
        }

        //振動フィードバックを行う
        private void Haptic(LeftOrRight lr)
        {
            var index = openvr.GetTrackedDeviceIndexForControllerRole(lr == LeftOrRight.Left
                ? ETrackedControllerRole.LeftHand
                : ETrackedControllerRole.RightHand);
            if (index == OpenVR.k_unTrackedDeviceIndexInvalid) return;
            openvr.TriggerHapticPulse(index, 0, 3000);
        }
    }
}