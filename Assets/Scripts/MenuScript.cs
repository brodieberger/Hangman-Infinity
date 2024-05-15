using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public static string gamemode = "Endless"; // Default difficulty
    public static string difficulty = "Hard"; //1 = easy, 2 = normal, 3 = hard

    void Start(){
        AudioManager.Instance.PlayMainMenuMusic();
    }

    public void EndlessMode(string difficultyButton)
    {
        difficulty = difficultyButton;
        gamemode = "Endless";
        SceneManager.LoadScene("GameScene");
    }
    public void ScoreAttackMode(string difficultyButton)
    {
        difficulty = difficultyButton;
        gamemode = "ScoreAttack";
        SceneManager.LoadScene("GameScene");
    }
    public void EndLeader()
    {
        SceneManager.LoadScene("DemoScene");
    }
    public void ScoreLeader()
    {
        SceneManager.LoadScene("scatboard");
    }

    public void QuittheGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }

}
