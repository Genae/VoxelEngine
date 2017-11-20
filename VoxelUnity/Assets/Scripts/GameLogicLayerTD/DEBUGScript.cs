using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUGScript : MonoBehaviour
{
    public int DEBUG_INDEX = 0;

    void OnGUI()
    {
        GUI.Label(new Rect(0, 50 * DEBUG_INDEX, 200,200), transform.name + " : " + transform.position.ToString());
    }
}
