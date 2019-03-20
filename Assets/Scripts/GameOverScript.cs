using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()

    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(Screen.width / 2 - 40, 350, 160, 60), "Game Over",style);

        //GUI.Label(new Rect(Screen.width / 2 - 40, 600, 160, 60), "score:"  );

        if (GUI.Button(new Rect(Screen.width / 2 - 30, 650, 160, 80), "Retry?",style))
        {
            SceneManager.LoadScene(0);

        }
    }
}
