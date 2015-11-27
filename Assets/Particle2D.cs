using UnityEngine;

namespace DFNoise
{
    public class Particle2D : MonoBehaviour
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

        ParticleSystem.Particle[] _particles;

        float GetNoise(Vector2 p)
        {
            p *= _frequency;
            var t = Time.time * _animationSpeed;
            return Perlin.Noise(new Vector3(p.x, p.y, t));
        }

        Vector2 GetGradient(Vector2 p)
        {
            var n0 = GetNoise(p);
            var n1 = GetNoise(p + new Vector2(_epsilon, 0));
            var n2 = GetNoise(p + new Vector2(0, _epsilon));
            return new Vector3(n1 - n0, n2 - n0) / _epsilon;
        }

        Vector2 GetDFNoise(Vector2 p)
        {
            var g = GetGradient(p);
            return new Vector2(g.y, -g.x);
        }

        void MoveParticle(ref ParticleSystem.Particle p)
        {
            var n2 = GetDFNoise(p.position);
            var n3 = new Vector3(n2.x, n2.y, 0);
            p.velocity = 
                p.velocity * (1.0f - _damping) +
                n3 * Time.fixedDeltaTime * _advection;
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
