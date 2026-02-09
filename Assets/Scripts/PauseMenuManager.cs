using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public Toggle goalSoundToggle;

    private bool isPaused;
    private const string PREF_GOAL_SOUND = "goal_sound_enabled";

    void Start()
{
    bool enabled = PlayerPrefs.GetInt(PREF_SOUND, 1) == 1;

    AudioListener.volume = enabled ? 1f : 0f;

    if (goalSoundToggle != null)
    {
        goalSoundToggle.isOn = enabled;
        goalSoundToggle.onValueChanged.AddListener(SetGoalSoundEnabled);
    }

    Pause(false);
}


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!isPaused);
        }
    }

    public void Resume()
    {
        Pause(false);
    }

    public void BackToMenu()
    {
        Pause(false);
        SceneManager.LoadScene("Menu"); // OJO: nombre exacto de tu escena
    }

   private const string PREF_SOUND = "sound_enabled";

    private void SetGoalSoundEnabled(bool enabled)
    {
        // Guardamos preferencia
        PlayerPrefs.SetInt(PREF_SOUND, enabled ? 1 : 0);
        PlayerPrefs.Save();

        // Activamos / desactivamos TODO el audio del juego
        AudioListener.volume = enabled ? 1f : 0f;
    }

    private void Pause(bool pause)
    {
        isPaused = pause;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(pause);
        Time.timeScale = pause ? 0f : 1f;
    }
}
