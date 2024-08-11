using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTING : MonoBehaviour
{
    float n;
    public Resource R;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnThingHappen()
    {
        print("THING HAPPENED" + gameObject.name);
    }

    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Main Menu");
    }
}
