using UnityEngine;

namespace DFNoise
{
    public class Particle3D : MonoBehaviour
    {
        [Header("Noise")]
        [SerializeField] float _frequency = 3;
        [SerializeField] float _animationSpeed = 0.2f;

        [Header("Dynamics")]
        [SerializeField] float _advection = 0.1f;
        [SerializeField] float _damping = 0.01f;

        [Header("Visualization")]
        [SerializeField] ParticleSystem _particleSystem;

        const float _epsilon = 0.0001f;
        static Vector3 _noiseOffset = new Vector3(77.7f, 33.3f, 11.1f);

        ParticleSystem.Particle[] _particles;

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

        void MoveParticle(ref ParticleSystem.Particle p)
        {
            var n = GetDFNoise(p.position + Vector3.up * _animationSpeed * Time.time);
            p.velocity = 
                p.velocity * (1.0f - _damping) +
                n * Time.fixedDeltaTime * _advection;
        }

        void FixedUpdate()
        {
            var maxParticles = _particleSystem.maxParticles;

            if (_particles == null || _particles.Length < maxParticles)
                _particles = new ParticleSystem.Particle[maxParticles];

            var particleCount = _particleSystem.GetParticles(_particles);

            for (var i = 0; i < particleCount; i++)
            {
                var p = _particles[i];
                MoveParticle(ref p);
                _particles[i] = p;
            }

            _particleSystem.SetParticles(_particles, particleCount);
        }
    }
}
