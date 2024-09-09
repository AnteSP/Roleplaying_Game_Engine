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

    Vector3 s, t;

    // Update is called once per frame
    void Update()
    {
        //s = source.position + (behind ? new Vector3(0, -0.05f, 0.1f) : Vector3.zero);
        s = new Vector3(source.position.x, source.position.y - (behind ? 0.05f : 0), (getZFrom.position.z - 0.0001f) +  (behind ? 0.1f : 0) );
        //t = target.position + (behind ? new Vector3(0, 0, 0.1f) : Vector3.zero);
        t = new Vector3(target.position.x, target.position.y, (getZFrom.position.z - 0.0001f) +  (behind ? 0.1f : 0) );
        lineRenderer.SetPosition(0, s);
        lineRenderer.SetPosition(1, t);
    }
}
