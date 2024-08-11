using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecter : MonoBehaviour
{
    public RectTransform Canvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Mouse = new Vector3((((Input.mousePosition.x / Camera.main.scaledPixelWidth)) * Canvas.position.x * 2), (((Input.mousePosition.y / Camera.main.scaledPixelHeight)) * Canvas.position.y * 2), -1);
        Vector2 MouseT = new Vector2((((Input.mousePosition.x / Camera.main.scaledPixelWidth)) * Canvas.position.x * 2), (((Input.mousePosition.y / Camera.main.scaledPixelHeight)) * Canvas.position.y * 2));
        //print(Physics2D.Raycast(Mouse, new Vector3(0, 0, -1)));
        if (Physics2D.Raycast(MouseT, new Vector2(0, 1)))
        {
            //print("true");
        }
        else
        {
           // print("false");
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 MouseT = new Vector2((((Input.mousePosition.x / Camera.main.scaledPixelWidth)) * Canvas.position.x * 2), (((Input.mousePosition.y / Camera.main.scaledPixelHeight)) * Canvas.position.y * 2));

        Gizmos.DrawRay(MouseT, new Vector2(0, 1));
    }
}
