using System.Collections;
using System.Collections.Generic;
using rhythmhero;
using UnityEngine;

public class quitpanel : MonoBehaviour
{
    // Start is called before the first frame update
    public void QuitGame()
    {
        Application.Quit();
    }

    public GameObject canvas;
    public void NoQuit()
    {
        canvas.gameObject.SetActive(false);
        GameManager.instance.gameState = GameState.ThirdPerson;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (PlayerState.instance != null)
        {
            PlayerState.instance.StopRunning();
            PlayerState.instance.inputDirection = Vector2.zero;
        }
    }
}
