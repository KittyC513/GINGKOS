using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class ShowDialogue : MonoBehaviour
{
    private static GameObject DemonAll;
    private static GameObject DemonP1;
    private static GameObject DemonP2;


    // Start is called before the first frame update
    void Start()
    {
        DemonAll = GameObject.Find("Line View Devil");
        DemonP1 = GameObject.Find("Line View Devil Player 1");
        DemonP2 = GameObject.Find("Line View Devil Player 2");

        DemonAll.SetActive(false);
        DemonP1.SetActive(false);
        DemonP2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [YarnCommand("DemonAllShow")]
    public static void DemonAllShow()
    {
        DemonAll.SetActive(true);
        DemonP1.SetActive(false);
        DemonP2.SetActive(false);
    }

    [YarnCommand("DemonP1Show")]
    public static void DemonP1Show()
    {
        DemonP1.SetActive(true);
        DemonP2.SetActive(false);
        DemonAll.SetActive(false);
    }

    [YarnCommand("DemonP2Show")]
    public static void DemonP2Show()
    {
        DemonP2.SetActive(true);
        DemonAll.SetActive(false);
        DemonP1.SetActive(false);
    }

    /*[YarnCommand("ShowDevilOnPlayerDeath")]
    public static void Show()
    {
        // if p1 died
        if() {
            // deactivate p1
            // activate p2
        }
        //else if p2 died
        //deactivate p2
        //activate p1
    }*/
}
