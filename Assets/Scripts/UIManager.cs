using UnityEngine;
using TMPro; // Necesario para TextMeshPro
using System.Collections;
public class UIManager : MonoBehaviour
{
    [Header("Marcador")]
    public TMP_Text scoreTextP1; // Texto puntos J1
    public TMP_Text scoreTextP2; // Texto puntos J2
    [Header("Animación Gol")]
    public TMP_Text goalText; // Texto central (¡GOL!)
    public float animationDuration = 2f;
    // Actualiza los textos del marcador
    public void UpdateScore(int score1, int score2)
    {
        if (scoreTextP1 != null) scoreTextP1.text = score1.ToString();
        if (scoreTextP2 != null) scoreTextP2.text = score2.ToString();
    }
    // Muestra la animación de gol
    public void ShowGoalAnimation(string scorerName)
    {
        if (goalText != null)
        {
            goalText.text = "¡GOL de " + scorerName + "!";
            goalText.gameObject.SetActive(true);
            StartCoroutine(AnimateGoalText());
        }
    }
    private IEnumerator AnimateGoalText()
    {
        float timer = 0f;
        while (timer < animationDuration)
        {
            // Efecto de latido (escala)
            float scale = Mathf.PingPong(timer * 2, 0.5f) + 1f;
            goalText.transform.localScale = Vector3.one * scale;
            timer += Time.deltaTime;
            yield return null;
        }
        goalText.gameObject.SetActive(false);
    }
}