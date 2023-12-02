using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    private UnityEngine.UI.Image image;
    private void Start()
    {
        image = gameObject.GetComponent<UnityEngine.UI.Image>();
        OnMouseExit();
    }

    public void OnMouseEnter()
    {
        Debug.Log(gameObject.name+" OnMouseEnter");
        image.color = new Color(255, 255, 255, 0.2f);
    }

    public void OnMouseExit()
    {
        Debug.Log(gameObject.name+" OnMouseExit");
        image.color = new Color(255, 255, 255, 0f);
    }
}
