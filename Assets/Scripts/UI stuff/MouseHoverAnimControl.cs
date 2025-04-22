using UnityEngine;
using UnityEngine.UI;

public class MouseHoverAnimControl : MonoBehaviour
{

    Animator anim;
    Image img;
    public bool Inside = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        img = GetComponent<Image>();
    }

    private void Update()
    {
        bool Inside = IsMouseWithinImage();

        anim.speed = Inside ? 0 : 1;

        if (Inside && Stats.current.PassTime)
        {
            Stats.StartStopTime(false,"ItemNotification");
            Stats.StartStopPlayerMovement(false, "ItemNotification");
        }else if(!Inside && !Stats.current.PassTime){
            Stats.StartStopTime(true, "ItemNotification");
            Stats.StartStopPlayerMovement(true, "ItemNotification");
        }
    }

    bool IsMouseWithinImage()
    {
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(img.rectTransform, Input.mousePosition, null, out localMousePosition);

        return img.rectTransform.rect.Contains(localMousePosition);
    }

    public void TurnOn()
    {
        enabled = true;
    }

    public void TurnOff()
    {
        enabled = false;
    }
}
