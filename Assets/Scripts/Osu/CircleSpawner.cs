using UnityEngine;

namespace Osu
{
    public class CircleSpawner : MonoBehaviour
    {
        public GameObject prefab;

        private const int screenLimitMinimumX = -5, screenLimitMaximumX = 5,
                          screenLimitMinimumY = -3, screenLimitMaximumY = 3;

        private void Start() => GenerateCircle();

        public Vector3 GeneratedPosition()
        {
            int x = Random.Range(screenLimitMinimumX, screenLimitMaximumX);
            int y = Random.Range(screenLimitMinimumY, screenLimitMaximumY);
            
            return new Vector3(x, y, 0);
        }

        public void GenerateCircle() => Instantiate(prefab, GeneratedPosition(), Quaternion.identity);
    }
}
