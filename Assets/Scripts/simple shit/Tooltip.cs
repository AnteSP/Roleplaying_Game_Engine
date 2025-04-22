using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerExitHandler,IPointerEnterHandler
{
    [SerializeField] public string tooltip;

    void IPointerExitHandler.OnPointerExit(PointerEventData p)
    {

        NameIndic.Indicate("");
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData p)
    {

        NameIndic.Indicate(tooltip);
    }

}
