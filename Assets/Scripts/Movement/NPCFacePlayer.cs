using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCFacePlayer : NPCMovement
{
    public bool FacePlayer = true;


    private void Update()
    {
        if (FacePlayer) Face(Stats.current.Player.transform.position);
    }
}
