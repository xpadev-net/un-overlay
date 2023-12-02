using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    private UnityEngine.UI.Image image;
    private UnityEngine.UI.Outline outline;
    [SerializeField] public Color defaultBgColor;
    [SerializeField] public Color hoverBgColor;
    [SerializeField] public Color defaultOutlineColor;
    [SerializeField] public Color hoverOutlineColor;
    private void Start()
    {
        image = gameObject.GetComponent<UnityEngine.UI.Image>();
        outline = gameObject.GetComponent<UnityEngine.UI.Outline>();
        OnMouseExit();
    }

    public void OnMouseEnter()
    {
        Debug.Log(gameObject.name+" OnMouseEnter");
        image.color = hoverBgColor;
        if (outline)
        {
            outline.effectColor = hoverOutlineColor;            
        }
    }

    public void OnMouseExit()
    {
        Debug.Log(gameObject.name+" OnMouseExit");
        image.color = defaultBgColor;
        if (outline)
        {
            outline.effectColor = defaultOutlineColor;
        }
    }
}
