using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ChutRadioVisualizer : MonoBehaviour
{
    public float radius = 1f;
    public int segments = 64;
    public Color normalColor = new Color(1f, 1f, 1f, 0.4f);
    public Color activeColor = Color.white;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.widthMultiplier = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));

        DrawCircle();
        SetNormal();
    }

    public void DrawCircle()
    {
        line.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    public void SetActive()
    {
        line.startColor = activeColor;
        line.endColor = activeColor;
    }

    public void SetNormal()
    {
        line.startColor = normalColor;
        line.endColor = normalColor;
    }
        void OnValidate()
    {
        if (segments < 3) segments = 3;

        // Si aún no existe el LineRenderer, intenta cogerlo
        if (line == null) line = GetComponent<LineRenderer>();

        // Evita que reviente si todavía no está listo
        if (line == null) return;

        DrawCircle();
    }
    void Update()
    {
        DrawCircle();
    }


}
