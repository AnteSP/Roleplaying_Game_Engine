using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObj : MonoBehaviour
{

    [SerializeField] RectTransform Leader;
    RectTransform Rect;

    Vector3 Offset;

    // Start is called before the first frame update
    void Start()
    {
        Rect = GetComponent<RectTransform>();
        Offset = Rect.position - Leader.position;
    }

    // Update is called once per frame
    void Update()
    {
        Rect.position = Leader.position + Offset;
    }
}
