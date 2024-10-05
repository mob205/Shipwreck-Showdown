using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void Retry()
    {
        EndStateController.Instance.Restart();
    }

    public void Quit()
    {
        Debug.Log("Bye");
        Application.Quit();
    }

}
