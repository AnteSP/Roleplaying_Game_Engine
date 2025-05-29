using UnityEngine;
using UnityEngine.UI;

public class MouseHoverAnimControl : MonoBehaviour
{

    Animator anim;
    Image img;
    public bool Inside = false;
    Color OGColor;
    static int boxesOut = 0;
    bool started = false;

    public static void resetBoxesCount()
    {
        boxesOut = 0;
    }

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
            
        if(Stats.current != null)
        {
            if (Inside && Stats.current.PassTime)
            {
                Stats.StartStopTime(false, "ItemNotification");
                Stats.StartStopPlayerMovement(false, "ItemNotification");
            }
            else if (!Inside && !Stats.current.PassTime)
            {
                Stats.StartStopTime(true, "ItemNotification");
                Stats.StartStopPlayerMovement(true, "ItemNotification");
            }
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
        if(!started) boxesOut++;
        else anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0.54f);
        started = true;
    }

    public void TurnOff()
    {
        boxesOut -= 1;
        enabled = false;
        started = false;
        if (boxesOut == 0) Items.allShiftBoxesClear();
    }
}
