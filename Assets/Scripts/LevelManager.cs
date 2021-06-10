using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //config params
    [SerializeField] GameObject winScreen;


    //cached refs
    int currentSceneIndex;

    private void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
    public void WinLevel()
    {
        winScreen.SetActive(true);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(1);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
