using UnityEngine;

public class Jugador : MonoBehaviour
{
    [Header("Configuración Haxball")]
    [SerializeField] private float velocidad = 8f;
    [SerializeField] private float fuerzaChut = 15f;
    [SerializeField] private float radioChut = 0.1f; // Distancia para alcanzar la pelota

    [Header("Límites del campo")]
    [SerializeField] private SpriteRenderer fieldSprite; // arrastra aquí el campo (campohaxball)
    [SerializeField] private float outsideMargin = 0.3f; // cuánto puede “salirse” (0.2–0.6)
    [SerializeField] private float playerRadius = 0.4f;  // aprox radio del jugador (según tu scale)

    
    private Rigidbody2D rb;
    private Vector2 movimiento;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Aseguramos que sea Top-Down (sin gravedad)
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // 1. INPUT CLÁSICO (Asegúrate de que en Edit > Project Settings > Player > Active Input Handling esté en "Both" o "Input Manager")
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        
        movimiento = new Vector2(x, y).normalized;

        // 2. DETECTAR CHUT
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IntentarChutar();
        }
    }

    private void FixedUpdate()
    {
        // 3. APLICAR MOVIMIENTO
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
        // Buscamos objetos con collider alrededor del jugador
        Collider2D[] objetosCercanos = Physics2D.OverlapCircleAll(transform.position, radioChut);

        foreach (Collider2D col in objetosCercanos)
        {
            // Ignorarnos a nosotros mismos
            if (col.gameObject == gameObject) continue;

            Rigidbody2D rbPelota = col.attachedRigidbody;

            // Si tiene Rigidbody (es una pelota u otro jugador)
            if (rbPelota != null)
            {
                // Calculamos dirección desde el jugador hacia la pelota
                Vector2 direccionChut = (rbPelota.transform.position - transform.position).normalized;
                
                // Aplicamos fuerza explosiva (Impulse)
            #if UNITY_6000_0_OR_NEWER
                rbPelota.linearVelocity = Vector2.zero; // Reseteamos un poco para control
                rbPelota.AddForce(direccionChut * fuerzaChut, ForceMode2D.Impulse);
            #else
                rbPelota.velocity = Vector2.zero;
                rbPelota.AddForce(direccionChut * fuerzaChut, ForceMode2D.Impulse);
            #endif
                
                Debug.Log($"¡Chutando {col.name}!");
            }
        }
    }

    // Para ver el radio de chut en el editor
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
    transform.position = p;
}

}
