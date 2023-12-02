using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnchorHandSetting : MonoBehaviour
{
    public bool isLeft;
    public bool isActive;

    private Outline outline;
    // Start is called before the first frame update
    void Start()
    {
        isActive = Config.i.isLeftHand == isLeft;
        outline = gameObject.GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        isActive = Config.i.isLeftHand == isLeft;
        outline.effectColor = isActive ? Color.green : Color.black;
    }
    
    public void OnClick()
    {
        Config.i.SetLeftHand(isLeft);
    }
}
