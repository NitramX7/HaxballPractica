using UnityEngine;
using UnityEngine.InputSystem;
public class Jugador : MonoBehaviour
{
    [Header("Configuración Haxball")]
    [SerializeField] private float velocidad = 8f;
    [SerializeField] private float fuerzaChut = 15f;
    [SerializeField] private float radioChut = 0.1f;
    [Header("Identificación")]
    public int playerId = 1; // 1 o 2
    [Header("Límites del campo")]
    [SerializeField] private SpriteRenderer fieldSprite;
    [SerializeField] private float outsideMargin = 0.3f;
    [SerializeField] private float playerRadius = 0.4f;
    private Rigidbody2D rb;
    private Vector2 movimiento;
    private Vector3 posicionInicial;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        posicionInicial = transform.position;
    }
    // Método para resetear la posición del jugador
    public void ResetPosition()
    {
        transform.position = posicionInicial;
        movimiento = Vector2.zero;
        if (rb != null)
        {
        #if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector2.zero;
        #else
            rb.velocity = Vector2.zero;
        #endif
        }
    }
    // Este método es llamado por Player Input cuando se mueve
    public void OnMovimientoJug(InputValue value)
    {
        movimiento = value.Get<Vector2>();
    }
    // Este método es llamado por Player Input cuando se chuta
    public void OnChutar(InputValue value)
    {
        if (value.isPressed)
        {
            IntentarChutar();
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
        ClampInsideField();
    }
    private void MovePlayer()
    {
    #if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = movimiento * velocidad;
    #else
        rb.velocity = movimiento * velocidad;
    #endif
    }
    private void IntentarChutar()
    {
        Collider2D[] objetosCercanos = Physics2D.OverlapCircleAll(transform.position, radioChut);
        foreach (Collider2D col in objetosCercanos)
        {
            if (col.gameObject == gameObject) continue;
            Rigidbody2D rbPelota = col.attachedRigidbody;
            if (rbPelota != null)
            {
                // Notificar GameManager: Bola se movió -> Fin del KickOff
                if (GameManager.Instance != null) GameManager.Instance.OnBallMoved();
                Vector2 direccionChut = (rbPelota.transform.position - transform.position).normalized;
                
            #if UNITY_6000_0_OR_NEWER
                rbPelota.linearVelocity = Vector2.zero;
                rbPelota.AddForce(direccionChut * fuerzaChut, ForceMode2D.Impulse);
            #else
                rbPelota.velocity = Vector2.zero;
                rbPelota.AddForce(direccionChut * fuerzaChut, ForceMode2D.Impulse);
            #endif
                
                Debug.Log($"¡Chutando {col.name}!");
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioChut);
    }
    private void ClampInsideField()
    {
        if (fieldSprite == null) return;
        Bounds b = fieldSprite.bounds;
        float minX = b.min.x + playerRadius - outsideMargin;
        float maxX = b.max.x - playerRadius + outsideMargin;
        float minY = b.min.y + playerRadius - outsideMargin;
        float maxY = b.max.y - playerRadius + outsideMargin;
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.y = Mathf.Clamp(p.y, minY, maxY);
        // --- Restricción de KickOff (Haxball style) ---
        if (GameManager.Instance != null && GameManager.Instance.IsPlayerRestricted(playerId))
        {
             // 1. RESTRICCIÓN DE CAMPO (NO PASAR DEL MEDIO)
             // Si soy P1 (Juego en la izquierda), no puedo pasar de X=0 hacia la derecha
             if (playerId == 1) 
             {
                 if (p.x > 0) p.x = 0;
             }
             // Si soy P2 (Juego en la derecha), no puedo pasar de X=0 hacia la izquierda
             else if (playerId == 2)
             {
                 if (p.x < 0) p.x = 0;
             }

             // 2. RESTRICCIÓN DE CÍRCULO CENTRAL
             // Además, no puedo entrar en el círculo
             float dist = Vector2.Distance(Vector2.zero, p);
             if (dist < GameManager.Instance.radioCirculoCentral)
             {
                 // Dirección desde el centro hacia el jugador
                 Vector2 dirFromCenter = ((Vector2)p).normalized;
                 if (dirFromCenter == Vector2.zero) dirFromCenter = (playerId == 1) ? Vector2.left : Vector2.right; 
                 
                 // Colocarlo justo en el borde del círculo
                 Vector2 newPos = dirFromCenter * GameManager.Instance.radioCirculoCentral;
                 p.x = newPos.x;
                 p.y = newPos.y;
             }
        }
        // ----------------------------------------------
        transform.position = p;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pelota") || collision.gameObject.name.Contains("Pelota"))
        {
             if (GameManager.Instance != null) GameManager.Instance.OnBallMoved();
        }
    }
}