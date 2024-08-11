using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDepth : MonoBehaviour
{
    Transform P;
    [SerializeField] Transform T;

    private void Awake()
    {
        if(Stats.current == null || Stats.current.Player == null)
        {
            try
            {
                P = GameObject.FindGameObjectWithTag("Player").transform;
            }
            catch (System.Exception e2)
            {
                P = Camera.main.transform;
                print("Camera backup being used " + gameObject.name);
            }
        }
        else
            P = Stats.current.Player.transform;

        if (T == null)
        {
            T = transform;
        }
    }

    private void OnWillRenderObject()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -(P.position.y - T.position.y) / 100);
    }
}
