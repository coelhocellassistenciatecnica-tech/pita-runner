using UnityEngine;

namespace PitaRunner.Obstacles
{
    public class Hammer : ObstacleBase
    {
        [Header("Hammer Settings")]
        [SerializeField] private float swingSpeed = 90f;
        [SerializeField] private float minAngle = -60f;
        [SerializeField] private float maxAngle = 60f;
        [SerializeField] private Transform pivotPoint;

        private float currentAngle = 0f;
        private float direction = 1f;

        private void Update()
        {
            currentAngle += swingSpeed * direction * Time.deltaTime;
            if (currentAngle >= maxAngle) { currentAngle = maxAngle; direction = -1f; }
            if (currentAngle <= minAngle) { currentAngle = minAngle; direction = 1f; }

            Transform pivot = pivotPoint != null ? pivotPoint : transform.parent;
            if (pivot != null)
                pivot.localRotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }
}
