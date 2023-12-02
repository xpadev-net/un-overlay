using System;
using EasyLazyLibrary;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject canvasObj;
    [SerializeField]
    private Camera eventCamera;

    private RectTransform canvasRectTransform;
    private Canvas canvas;
    
    private GameObject leftHoveredGameObject;
    private GameObject rightHoveredGameObject;
    
    private bool triggeredLeft;
    private bool triggeredRight;
    
    private readonly EasyOpenVRUtil util = new EasyOpenVRUtil(); // 姿勢取得ライブラリ


    private void Start()
    {
        canvasRectTransform = canvasObj.GetComponent<RectTransform>();
        canvas = canvasObj.GetComponent<Canvas>();
        leftHoveredGameObject = rightHoveredGameObject= gameObject;
    }


    private bool IsTriggered(bool isLeft)
    {
        if (isLeft)
        {
            if (util.IsControllerButtonPressed(util.GetLeftControllerIndex(), EVRButtonId.k_EButton_SteamVR_Trigger))
            {
                if (!triggeredLeft)
                {
                    triggeredLeft = true;
                    return true;
                }
            }
            else
            {
                triggeredLeft = false;
            }
        }
        else
        {
            if (util.IsControllerButtonPressed(util.GetRightControllerIndex(), EVRButtonId.k_EButton_SteamVR_Trigger))
            {
                if (!triggeredRight)
                {
                    triggeredRight = true;
                    return true;
                }
            }
            else
            {
                triggeredRight = false;
            }
        }

        return false;
    }
    public void UpdateCursor(bool isLeft,Vector2 cursorPos)
    {
        if (isLeft)
        {
            _UpdateCursor(IsTriggered(true),cursorPos,ref leftHoveredGameObject);
        }
        else
        {
            _UpdateCursor(IsTriggered(false),cursorPos,ref rightHoveredGameObject);
        }
    }

    private void _UpdateCursor(bool isClicked,Vector2 cursorPos,ref GameObject hoveredGameObject)
    {
        
        var position = transform.position;
        var sizeDelta = canvasRectTransform.sizeDelta;
        var screenPoint = cursorPos - new Vector2(sizeDelta.x / 2f, sizeDelta.y / 2f) + (Vector2)position;
        
        var result = lib.Raycast.Get(canvas,eventCamera, new Ray((Vector3)screenPoint, Vector3.forward*100));
        
        var pointer = new PointerEventData(EventSystem.current)
        {
            position = cursorPos
        };
        foreach (var res in result)
        {
            var parent = res.transform.parent.gameObject;
            if (!res.transform.IsChildOf(canvasObj.transform) ||
                !parent.GetComponent<UnityEngine.UI.Button>()) continue;
            if (hoveredGameObject.GetInstanceID() != parent.GetInstanceID())
            {
                    
                if (hoveredGameObject)
                {
                    ExecuteEvents.Execute(hoveredGameObject, pointer, ExecuteEvents.pointerExitHandler);
                }
                ExecuteEvents.Execute(parent, pointer, ExecuteEvents.pointerEnterHandler);
                hoveredGameObject = parent;
            }

            if (isClicked)
            {
                ExecuteEvents.Execute(parent, pointer, ExecuteEvents.pointerClickHandler);
            }

            return;
        }

        if (hoveredGameObject.GetInstanceID() == gameObject.GetInstanceID()) return;
        ExecuteEvents.Execute(hoveredGameObject, pointer, ExecuteEvents.pointerExitHandler);
        hoveredGameObject = gameObject;
    }

    public void HoverOut(bool isLeft)
    {
        if (isLeft)
        {
            _HoverOut(ref leftHoveredGameObject);
        }
        else
        {
            _HoverOut(ref rightHoveredGameObject);
        }
    }
    
    private void _HoverOut(ref GameObject hoveredGameObject)
    {
        
        var pointer = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(0,0)
        };
        if (hoveredGameObject.GetInstanceID() == gameObject.GetInstanceID()) return;
        ExecuteEvents.Execute(hoveredGameObject, pointer, ExecuteEvents.pointerExitHandler);
        hoveredGameObject = gameObject;
    }
}