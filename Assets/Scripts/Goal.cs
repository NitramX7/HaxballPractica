using UnityEngine;
public class Goal : MonoBehaviour
{
    // 1 = Portería del Jugador 1 (Izquierda)
    // 2 = Portería del Jugador 2 (Derecha)
    public int teamIdConceded = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pelota") || collision.name.Contains("Pelota"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Gol(teamIdConceded);
            }
        }
    }
}