using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Cargar la escena del juego (1vs1 local)
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Salir del juego (opcional, para un botón de Salir)
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
