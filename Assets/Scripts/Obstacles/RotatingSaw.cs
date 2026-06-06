using UnityEngine;

namespace PitaRunner.Obstacles
{
    public class RotatingSaw : ObstacleBase
    {
        [Header("Saw Settings")]
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private float oscillateSpeed = 1f;
        [SerializeField] private float oscillateAmount = 2f;
        [SerializeField] private bool oscillate = false;

        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
            if (oscillate)
            {
                float x = Mathf.Sin(Time.time * oscillateSpeed) * oscillateAmount;
                transform.position = startPos + new Vector3(x, 0, 0);
            }
        }
    }
}
