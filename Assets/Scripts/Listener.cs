using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Listener : MonoBehaviour
{

    [SerializeField] PayToSpawn KidsInBasement;

    private void Start()
    {
        //KidsInBasement.Use += TestF();
    }


    public void TestF()
    {

        print("SPECIAL");
    }

}
