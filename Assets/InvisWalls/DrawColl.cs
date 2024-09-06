using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawColl : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab;
    LineRenderer lineRenderer;
    PolygonCollider2D polygonCollider2D;
    float time = 1f;

    void Start()
    {
        lineRenderer = Instantiate(linePrefab).GetComponent<LineRenderer>();
        lineRenderer.gameObject.SetActive(true);
        lineRenderer.transform.SetParent(transform);
        lineRenderer.transform.localPosition = Vector3.zero;
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        HiliteCollider();
    }

    void Update()
    {
        if(time > 0)
        {
            time -= Time.deltaTime;

            lineRenderer.material.color = new Color(1, 1, 1, time);
        }
        else
        {
            this.enabled = false;
        }
    }

    void HiliteCollider()
    {
        var points = polygonCollider2D.GetPath(0); // dumb assumption for demo -- only one path

        Vector3[] positions = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            positions[i] = transform.TransformPoint(points[i]);
        }
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(positions);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            time = 1f;
            this.enabled = true;
        }

    }

    private void OnDrawGizmos()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        var points = polygonCollider2D.GetPath(0).Select(a => a += (Vector2)polygonCollider2D.transform.position); // dumb assumption for demo -- only one path

        Vector2 lastP = points.Cast<Vector2>().Last();

        Gizmos.color = Color.magenta;
        foreach (Vector2 v in points)
        {
            Gizmos.DrawLine(v,lastP);
            lastP = v;
        }

        
    }
}
