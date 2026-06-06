using UnityEngine;
using PitaRunner.Core;
using PitaRunner.Crowd;

namespace PitaRunner.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float horizontalSpeed = 10f;
        [SerializeField] private float horizontalLimit = 4f;
        [SerializeField] private float smoothing = 8f;

        [Header("Lane Settings")]
        [SerializeField] private float levelEndZ = 200f;

        private float targetX = 0f;
        private float lastTouchX = 0f;
        private bool isDragging = false;
        private bool isRunning = false;
        private Rigidbody rb;

        public float LevelProgress => Mathf.Clamp01(transform.position.z / levelEndZ);
        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            GameManager.OnGameStateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            isRunning = (state == GameState.Playing);
            if (state == GameState.Battle || state == GameState.Victory || state == GameState.Defeat)
            {
                isRunning = false;
            }
        }

        private void Update()
        {
            if (!isRunning) return;

            HandleInput();
            GameManager.Instance?.SetLevelProgress(LevelProgress);

            if (transform.position.z >= levelEndZ)
            {
                GameManager.Instance?.StartBattle();
            }
        }

        private void FixedUpdate()
        {
            if (!isRunning) return;

            float currentX = Mathf.Lerp(transform.position.x, targetX, smoothing * Time.fixedDeltaTime);
            Vector3 newPos = new Vector3(currentX, transform.position.y, transform.position.z + runSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }

        private void HandleInput()
        {
#if UNITY_EDITOR
            HandleMouseInput();
#else
            HandleTouchInput();
#endif
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        lastTouchX = touch.position.x;
                        isDragging = true;
                        break;
                    case TouchPhase.Moved:
                        if (isDragging)
                        {
                            float delta = (touch.position.x - lastTouchX) / Screen.width * horizontalSpeed;
                            targetX = Mathf.Clamp(targetX + delta, -horizontalLimit, horizontalLimit);
                            lastTouchX = touch.position.x;
                        }
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        isDragging = false;
                        break;
                }
            }
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastTouchX = Input.mousePosition.x;
                isDragging = true;
            }
            else if (Input.GetMouseButton(0) && isDragging)
            {
                float delta = (Input.mousePosition.x - lastTouchX) / Screen.width * horizontalSpeed;
                targetX = Mathf.Clamp(targetX + delta, -horizontalLimit, horizontalLimit);
                lastTouchX = Input.mousePosition.x;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }

        public float GetRunSpeed() => runSpeed;

        public void SetRunSpeed(float speed) => runSpeed = speed;

        public void ApplySpeedUpgrade(float multiplier) => runSpeed *= multiplier;
    }
}
