using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDepth : MonoBehaviour
{
    BoxCollider2D P;
    [SerializeField] Transform T;

    private void Awake()
    {
        if(Stats.current == null || Stats.current.Player == null)
        {
            try
            {

                foreach (BoxCollider2D b in GameObject.FindGameObjectWithTag("Player").GetComponents<BoxCollider2D>())
                {
                    if (b.isTrigger) continue;
                    P = b;
                }
            }
            catch (System.Exception e2)
            {
                P = Camera.main.GetComponent<BoxCollider2D>();
                print("Camera backup being used " + gameObject.name);
            }
        }
        else
        {
            foreach (BoxCollider2D b in Stats.current.Player.GetComponents<BoxCollider2D>())
            {
                if (b.isTrigger) continue;
                P = b;
            }
        }

        if (T == null)
        {
            T = transform;
        }
    }

    private void OnWillRenderObject()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -(P.bounds.center.y - T.position.y) / 100);
    }
}
