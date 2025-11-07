using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    [Header("Victory Text")]
    public TextMeshProUGUI victoryScoreText;
    public TextMeshProUGUI victoryCoinsText;

    [Header("Defeat Text")]
    public TextMeshProUGUI defeatScoreText;
    public TextMeshProUGUI defeatCoinsText;

    [Header("Buttons (optional)")]
    public Button victoryRestartButton;
    public Button defeatRestartButton;
    public Button quitButton;

    void Awake()
    {
        HideAll();
        // Wire buttons if assigned
        if (victoryRestartButton) victoryRestartButton.onClick.AddListener(RestartLevel);
        if (defeatRestartButton) defeatRestartButton.onClick.AddListener(RestartLevel);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
    }

    void HideAll()
    {
        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);
    }

    public void ShowVictory(int score, int coins)
    {
        StartCoroutine(ShowVictoryDelayed(score, coins, 3f)); // waits 3 seconds before showing UI
    }

    private IEnumerator ShowVictoryDelayed(int score, int coins, float delay)
    {
        yield return new WaitForSeconds(delay);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;

        if (victoryPanel) victoryPanel.SetActive(true);
        if (victoryScoreText) victoryScoreText.text = $"Score: {score}";
        if (victoryCoinsText) victoryCoinsText.text = $"Coins: {coins}";
    }

    public void ShowDefeat(int score, int coins)
    {
      StartCoroutine(ShowDefeatDelayed(score, coins, 4f)); // waits 2 seconds
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
    private IEnumerator ShowDefeatDelayed(int score, int coins, float delay)
    {
        yield return new WaitForSeconds(delay);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;

        if (defeatPanel) defeatPanel.SetActive(true);
        if (defeatScoreText) defeatScoreText.text = $"Score: {score}";
        if (defeatCoinsText) defeatCoinsText.text = $"Coins: {coins}";
    }

    public void OnQuitClicked()
    {
        if (quitButton) quitButton.onClick.AddListener(OnQuitClicked);

        Debug.Log("Quit button clicked");         // <-- Do you see this in Console? If not, wiring/raycast issue.
        QuitGame();
    }




    public void QuitGame()
    {
        Debug.Log("Quit button clicked — exiting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // Stop play mode inside Editor
#else
        Application.Quit();  // Quit the game in a built build
#endif
    }
}