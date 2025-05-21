using UnityEngine;
using UnityEngine.UI;

public class MouseHoverAnimControl : MonoBehaviour
{

    Animator anim;
    Image img;
    public bool Inside = false;
    Color OGColor;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        img = GetComponent<Image>();
        OGColor = img.color;
    }

    private void Update()
    {
        bool Inside = IsMouseWithinImage();

        if (Inside)
        {
            anim.speed = 0;
            anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0.54f);
            img.color = Color.white;
        }
        else
        {
            img.color = OGColor;
            anim.speed = 1;
        }
            

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

        return img.rectTransform.rect.Contains(localMousePosition) && Movement.IsMouseOverGameWindow;
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
