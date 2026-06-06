using UnityEngine;
using PitaRunner.Core;

namespace PitaRunner.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Offset")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -10f);

        [Header("Smoothing")]
        [SerializeField] private float positionSmoothing = 6f;
        [SerializeField] private float rotationSmoothing = 4f;

        [Header("Look At")]
        [SerializeField] private float lookAheadDistance = 5f;

        [Header("Battle Camera")]
        [SerializeField] private Vector3 battleOffset = new Vector3(0f, 12f, -15f);
        [SerializeField] private float battleTransitionSpeed = 2f;

        private Vector3 currentOffset;
        private bool inBattle = false;

        private void Start()
        {
            if (target == null && PlayerController.Instance != null)
                target = PlayerController.Instance.transform;

            currentOffset = offset;
            GameManager.OnGameStateChanged += OnStateChanged;

            if (target != null)
                transform.position = target.position + currentOffset;
        }

        private void OnDestroy() => GameManager.OnGameStateChanged -= OnStateChanged;

        private void OnStateChanged(GameState state)
        {
            inBattle = (state == GameState.Battle);
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredOffset = inBattle ? battleOffset : offset;
            currentOffset = Vector3.Lerp(currentOffset, desiredOffset, battleTransitionSpeed * Time.deltaTime);

            Vector3 desiredPos = target.position + currentOffset;
            transform.position = Vector3.Lerp(transform.position, desiredPos, positionSmoothing * Time.deltaTime);

            Vector3 lookTarget = target.position + Vector3.forward * lookAheadDistance;
            Quaternion desiredRot = Quaternion.LookRotation(lookTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotationSmoothing * Time.deltaTime);
        }

        public void SetTarget(Transform newTarget) => target = newTarget;
    }
}
