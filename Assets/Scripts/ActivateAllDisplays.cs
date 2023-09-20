using UnityEngine;
using System.Collections;

public class ActivateAllDisplays : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("ScreenActivation");

        if(objs.Length > 1)
        {
            Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        foreach(var disp in Display.displays)
        {
            disp.Activate(disp.systemWidth, disp.systemHeight, 60);
        }
    }

}
