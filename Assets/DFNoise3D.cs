using UnityEngine;

[ExecuteInEditMode]
public class DFNoise3D : MonoBehaviour
{
    [Header("Noise")]
    [SerializeField] float _frequency = 3;

    [Header("Visualization")]
    [SerializeField] int _gridCount = 16;
    [SerializeField] float _lineScale = 0.1f;

    const float _epsilon = 0.0001f;
    static Vector3 _noiseOffset = new Vector3(77.7f, 33.3f, 11.1f);

    float GetNoise(Vector3 p)
    {
        return Perlin.Noise(p * _frequency);
    }

    Vector3 GetGradient(Vector3 p)
    {
        var n0 = GetNoise(p);
        var nx = GetNoise(p + Vector3.right * _epsilon);
        var ny = GetNoise(p + Vector3.up * _epsilon);
        var nz = GetNoise(p + Vector3.forward * _epsilon);
        return new Vector3(nx - n0, ny - n0, nz - n0) / _epsilon;
    }

    Vector3 GetDFNoise(Vector3 p)
    {
        var g1 = GetGradient(p);
        var g2 = GetGradient(p + _noiseOffset);
        return Vector3.Cross(g1, g2);
    }

    void OnDrawGizmos()
    {
        var lineScale = _lineScale / _gridCount;

        for (var ix = 0; ix < _gridCount; ix++)
        {
            for (var iy = 0; iy < _gridCount; iy++)
            {
                for (var iz = 0; iz < _gridCount; iz++)
                {
                    var p = new Vector3(
                        (ix + 0.5f) / _gridCount - 0.5f,
                        (iy + 0.5f) / _gridCount - 0.5f,
                        (iz + 0.5f) / _gridCount - 0.5f);

                    var n = GetDFNoise(p);

                    var c = p + Vector3.one * 0.5f;
                    Gizmos.color = new Color(c.x, c.y, c.z, 1);

                    Gizmos.DrawLine(p, p + n * lineScale);
                }
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 1000, 1000),
            "Please turn on 'Gizmos' on the Game view if you can't see anything.");
    }
}
