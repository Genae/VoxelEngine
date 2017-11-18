using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenLogic : MonoBehaviour
{
    private int buttonW = 300;
    private int buttonH = 200;

    //Logic lol
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width/2 - buttonW/2, Screen.height/2 - buttonH/2, buttonW, buttonH), "start campaign"))
        {
            SceneManager.LoadScene(1); // 1 is TDAR_Scene, 0 is StartScreen
        }
    }
}
