using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Referencias")]
    public Jugador jugador1;
    public Jugador jugador2;
    public Transform pelota;
    public UIManager uiManager; // Referencia al nuevo script
    [Header("Configuración")]
    public float radioCirculoCentral = 2f; // Zona prohibida tras gol
    // Variables internas
    private Vector3 pelotaPosInicial;
    private Rigidbody2D pelotaRb;
    
    // Estados del juego
    public enum GameState { Playing, Scored, KickOff }
    public GameState CurrentState { get; private set; } = GameState.Playing;
    private int scoreP1 = 0;
    private int scoreP2 = 0;
    private int lastScorerId = 0; // Quién marcó el último gol
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (pelota != null)
        {
            pelotaPosInicial = pelota.position;
            pelotaRb = pelota.GetComponent<Rigidbody2D>();
        }
    }
    private void Start()
    {
        // Inicializar marcador a 0-0
        if (uiManager != null) uiManager.UpdateScore(scoreP1, scoreP2);
    }
    // Se llama cuando alguien marca gol
    public void Gol(int teamIdConceded)
    {
        if (CurrentState != GameState.Playing) return;
        CurrentState = GameState.Scored;
        // Si el equipo 1 recibe gol -> Punto para el 2
        if (teamIdConceded == 1)
        {
            scoreP2++;
            lastScorerId = 2;
            if (uiManager != null) uiManager.ShowGoalAnimation("JUGADOR 2");
        }
        // Si el equipo 2 recibe gol -> Punto para el 1
        else if (teamIdConceded == 2)
        {
            scoreP1++;
            lastScorerId = 1;
            if (uiManager != null) uiManager.ShowGoalAnimation("JUGADOR 1");
        }
        if (uiManager != null) uiManager.UpdateScore(scoreP1, scoreP2);
        // Esperar 2 segundos antes de resetear
        Invoke(nameof(ResetearPosiciones), 2f);
    }
    private void ResetearPosiciones()
    {
        if (jugador1 != null) jugador1.ResetPosition();
        if (jugador2 != null) jugador2.ResetPosition();
        if (pelota != null)
        {
            pelota.position = pelotaPosInicial;
            if (pelotaRb != null)
            {
                pelotaRb.linearVelocity = Vector2.zero; // O .velocity en Unity viejo
                pelotaRb.angularVelocity = 0f;
            }
        }
        
        // Poner estado de Saque (KickOff)
        CurrentState = GameState.KickOff;
    }
    // Helper para saber si un jugador tiene restricciones de movimiento (Saque de centro)
    public bool IsPlayerRestricted(int playerId)
    {
        return CurrentState == GameState.KickOff && playerId == lastScorerId;
    }

    // (Legacy) Verificar si un jugador tiene prohibido entrar al centro - MANTENIDA POR COMPATIBILIDAD PERO YA NO SE USA EN LA NUEVA LOGICA
    public bool IsRestricted(int playerId, Vector2 position)
    {
        if (CurrentState == GameState.KickOff)
        {
            if (playerId == lastScorerId)
            {
                 // ...
            }
        }
        return false;
    }
    // Avisar que la pelota se movió para terminar el saque
    // Avisar que la pelota se movió para terminar el saque
    public void OnBallMoved()
    {
        if (CurrentState == GameState.KickOff)
        {
            CurrentState = GameState.Playing;
        }
    }

    // Dibujar el círculo prohíbido en el Editor para verlo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, radioCirculoCentral);
    }
}