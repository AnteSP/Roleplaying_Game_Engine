using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewInv : MonoBehaviour
{
    [SerializeField] bool ChangeFrames;
    [SerializeField] Vector2 FrameCount;

    static public GridLayoutGroup Grid;
    static RectTransform Rect;

    public static int Frames;
    bool alreadyStarted = false;

    private void OnEnable()
    {
        if (alreadyStarted) return;
        alreadyStarted = true;
        Grid = GetComponent<GridLayoutGroup>();
        Rect = GetComponent<RectTransform>();

        Grid.cellSize = ChangeFrames ? Rect.sizeDelta / FrameCount : Grid.cellSize;
        Rect.sizeDelta = ChangeFrames ? Rect.sizeDelta : new Vector2(Mathf.FloorToInt(Rect.sizeDelta.x / Grid.cellSize.x) * Grid.cellSize.x, Mathf.FloorToInt(Rect.sizeDelta.y / Grid.cellSize.y) * Grid.cellSize.y);

        Frames = Mathf.RoundToInt(Rect.sizeDelta.x / Grid.cellSize.x * Rect.sizeDelta.y / Grid.cellSize.y);
        Items.Pictures = new Image[Frames];
        Items.ITEMQUANTITY = new int[Frames];
        Items.ITEMS = new int[Frames];
        Items.Quantities = new Text[Frames];
        Items.TxtBack = new Image[Frames];
        Items.Pictures[0] = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        Items.Quantities[0] = transform.GetChild(0).GetChild(2).GetComponent<Text>();
        Items.TxtBack[0] = transform.GetChild(0).GetChild(1).GetComponent<Image>();
        Items.TxtBackBase = Items.TxtBack[0].color;

        for (int i = 0; i < Frames - 1; i++)
        {
            Items.Pictures[i + 1] = Instantiate(transform.GetChild(0), transform).GetChild(0).GetComponent<Image>();
            Items.Pictures[i + 1].transform.parent.name = (i + 1) + "";

            Items.Pictures[i + 1].transform.parent.GetComponent<Frame>().ID = i + 1;

            Items.Quantities[i + 1] = Items.Pictures[i + 1].transform.parent.GetChild(2).GetComponent<Text>();
            Items.TxtBack[i + 1] = Items.Pictures[i + 1].transform.parent.GetChild(1).GetComponent<Image>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {


        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void MakeGrid(bool state)
    {

        Grid.enabled = state;

    }
}
