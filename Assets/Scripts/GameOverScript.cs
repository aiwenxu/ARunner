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
        GUI.Label(new Rect(Screen.width / 2 - 40, 50, 160, 60), "Game Over");

        GUI.Label(new Rect(Screen.width / 2 - 40, 300, 160, 60), "score:"  );

        if (GUI.Button(new Rect(Screen.width / 2 - 30, 350, 60, 30), "Retry?"))
        {
            SceneManager.LoadScene(0);

        }
    }
}
