using UnityEngine;

namespace Utilities
{
    [ExecuteInEditMode]
    public class Turbulence : MonoBehaviour
    {
        public float TurbulenceStrength = 1;
        public Vector3 Frequency = new Vector3(1, 1, 1);
        public Vector3 OffsetSpeed = new Vector3(0.5f, 0.5f, 0.5f);
        public Vector3 Amplitude = new Vector3(5, 5, 5);
        public Vector3 GlobalForce;

        private float lastStopTime;
        private Vector3 currentOffset;
        private float deltaTime;
        private ParticleSystem.Particle[] particleArray;
        private ParticleSystem particleSys;

        private void Start()
        {
            particleSys = GetComponent<ParticleSystem>();

            if (particleArray == null || particleArray.Length < particleSys.main.maxParticles)
                particleArray = new ParticleSystem.Particle[particleSys.main.maxParticles];
        }


        private void Update()
        {
            int numParticlesAlive = particleSys.GetParticles(particleArray);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                deltaTime = Time.realtimeSinceStartup - lastStopTime;
                lastStopTime = Time.realtimeSinceStartup;
            }
            else
#endif
                deltaTime = Time.deltaTime;
            
            currentOffset += OffsetSpeed * deltaTime;

            for (var i = 0; i < numParticlesAlive; i++)
            {
                ParticleSystem.Particle particle = particleArray[i];

                Vector3 pos = particle.position;
                pos.x /= Frequency.x;
                pos.y /= Frequency.y;
                pos.z /= Frequency.z;

                var turbulenceVector = new Vector3
                {
                    x = ((Mathf.PerlinNoise(pos.z - currentOffset.z, pos.y - currentOffset.y) * 2 - 1) * Amplitude.x + GlobalForce.x) * deltaTime,
                    y = ((Mathf.PerlinNoise(pos.x - currentOffset.x, pos.z - currentOffset.z) * 2 - 1) * Amplitude.y + GlobalForce.y) * deltaTime,
                    z = ((Mathf.PerlinNoise(pos.y - currentOffset.y, pos.x - currentOffset.x) * 2 - 1) * Amplitude.z + GlobalForce.z) * deltaTime
                };

                turbulenceVector *= TurbulenceStrength;
                
                particleArray[i].position += turbulenceVector;
            }
            particleSys.SetParticles(particleArray, numParticlesAlive);
        }
    }
}
