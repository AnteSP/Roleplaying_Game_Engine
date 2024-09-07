using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class connectPoints : MonoBehaviour
{

    private LineRenderer lineRenderer;

    [SerializeField] Transform target,source;
    public bool behind = false;
    [SerializeField] Transform getZFrom;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    Vector3 s1, t1;

    // Update is called once per frame
    void Update()
    {
        s1 = source.position + (behind ? new Vector3(0, -0.05f, 0.1f) : Vector3.zero);
        s1 = new Vector3(s1.x, s1.y, getZFrom.position.z - 0.0001f);
        t1 = target.position + (behind ? new Vector3(0, 0, 0.1f) : Vector3.zero);
        t1 = new Vector3(t1.x, t1.y, getZFrom.position.z - 0.0001f );
        lineRenderer.SetPosition(0, s1);
        lineRenderer.SetPosition(1, t1);
    }
}
