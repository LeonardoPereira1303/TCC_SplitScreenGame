using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Levels To Load")]
    public string newGameLevel;
    private string levelToLoad;

    [SerializeField] private GameObject noSavedGame = null;

    //The Button YES will load the scene "Gameplay"
    public void NewGameDialogueYes()
    {
        SceneManager.LoadScene(newGameLevel);
    }

    //The Button YES will load the scene "Gameplay" where was save
    public void LoadGameDialogueYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);

            //To load a saved scene, need one more line for it and the script for Save/Load Game
        }
        else
        {
            noSavedGame.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
