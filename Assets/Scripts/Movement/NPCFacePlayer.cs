using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCFacePlayer : NPCMovement
{
    public bool FacePlayer = true;


    private void Update()
    {
        if (FacePlayer && Stats.current != null && Stats.current.Player != null) Face(Stats.current.Player.transform.position);
    }
}
