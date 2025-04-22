using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : Resource
{
    [SerializeField] Transform Target;
    [SerializeField] int NewMax;
    
    public override void Use(float Amount)
    {
        Stats.current.Player.transform.position = new Vector3(Target.position.x, Target.position.y, Stats.current.Player.transform.position.z);
        Camera.main.transform.position = new Vector3(Target.position.x, Target.position.y, Camera.main.transform.position.z);
        Camera.main.GetComponent<CamZoom>().ChangeMax(NewMax);
    }
}
