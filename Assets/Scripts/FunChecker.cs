using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunChecker : MonoBehaviour
{
    [Range(1, 100)] public int FUNMin = 0, FUNMax = 100;
    
    private void Start()
    {
        if (FUNMax != FUNMin && !Progress.checkFUN(FUNMin, FUNMax)) Destroy(gameObject);
    }
}
