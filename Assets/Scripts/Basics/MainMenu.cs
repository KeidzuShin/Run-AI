using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject CreditPanel;

    private void Start()
    {
        if (CreditPanel != null) CloseCredit();
    }

    //Load Main Scene 0
    public void StartMain()
    {
        SceneManager.LoadScene(0);
    }
    
    //Load Level Select 1
    public void StartLevelSelect()
    {
        SceneManager.LoadScene(1);
    }

    //Load Run From Player 2
    public void StartLevel2()
    {
        SceneManager.LoadScene(2);
    }    

    //Load Attack Player 3
    public void StartLevel3()
    {
        SceneManager.LoadScene(3);
    }    
    
    //Load Full Run 4
    public void StartLevel4()
    {
        SceneManager.LoadScene(4);
    }

    //Quit Game
    public void QuitGame()
    {
        Application.Quit();
    }

    //Open Browser to a link
    public void OpenUrl(string link)
    {
        Application.OpenURL(link);
    }

    public void OpenCredit()
    {
        CreditPanel.SetActive(true);
    }

    public void CloseCredit()
    {
        CreditPanel.SetActive(false);
    }
}
