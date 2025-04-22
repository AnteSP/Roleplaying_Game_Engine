using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get the current mouse position in screen coordinates.
        Vector3 mousePosition = Input.mousePosition;

        // Convert the mouse position to a point in the world space.
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector2(mousePosition.x, mousePosition.y));

        // Move the object towards the mouse position.
        transform.position = new Vector3(worldPosition.x, worldPosition.y , transform.position.z);
    }
}
