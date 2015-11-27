using UnityEngine;

[ExecuteInEditMode]
public class DFNoise2D : MonoBehaviour
{
    public enum LineMode { Gradient, DFNoise }

    [Header("Noise Parameters")]
    [SerializeField] float _frequency = 3;
    [SerializeField] float _animationSpeed = 0.2f;

    [Header("Vector Analysis")]
    [SerializeField] float _delta = 0.0001f;

    [Header("Visualization")]
    [SerializeField] LineMode _lineMode;
    [SerializeField] int _columns = 16;
    [SerializeField] int _rows = 16;
    [SerializeField] float _lineScale = 0.1f;
    [SerializeField] Color _lineColor = Color.red;

    float GetNoise(Vector2 p)
    {
        p *= _frequency;
        var t = Time.time * _animationSpeed;
        return Perlin.Noise(new Vector3(p.x, p.y, t));
    }

    Vector2 GetGradient(Vector2 p)
    {
        var n0 = GetNoise(p);
        var n1 = GetNoise(p + new Vector2(_delta, 0));
        var n2 = GetNoise(p + new Vector2(0, _delta));
        return new Vector3(n1 - n0, n2 - n0) / _delta;
    }

    Vector2 GetDFNoise(Vector2 p)
    {
        var g = GetGradient(p);
        return new Vector2(g.y, -g.x);
    }

    void OnDrawGizmos()
    {
        var cubeScale = new Vector3(1.0f / _columns, 1.0f / _rows, 1);
        var lineScale = _lineScale / Mathf.Max(_columns, _rows);

        for (var ix = 0; ix < _columns; ix++)
        {
            for (var iy = 0; iy < _rows; iy++)
            {
                var px = (ix + 0.5f) / _columns - 0.5f;
                var py = (iy + 0.5f) / _rows - 0.5f;
                var p = new Vector2(px, py);

                var n = GetNoise(p);
                var cn = (n + 1) / 2;

                var dp = _lineMode == LineMode.Gradient ?
                    GetGradient(p) : GetDFNoise(p);

                Gizmos.color = new Color(cn, cn, cn, 1);
                Gizmos.DrawCube(new Vector3(px, py, 1), cubeScale);

                Gizmos.color = _lineColor;
                Gizmos.DrawLine(p, p + dp * lineScale);
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 1000, 1000),
            "Please turn on 'Gizmos' on the Game view if you can't see anything.");
    }
}
