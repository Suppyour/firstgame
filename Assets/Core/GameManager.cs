using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float gameDuration = 1800f; // 30 ЛХМСР

        public static GameManager Instance { get; private set; }
        public System.Action OnGameEnd;

        private float gameTimer;
        private bool isGameRunning;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            if (!isGameRunning) return;

            gameTimer += Time.deltaTime;
            if (gameTimer >= gameDuration)
            {
                EndGame(true);
            }
        }

        private void StartGame()
        {
            isGameRunning = true;
            gameTimer = 0f;
            EventManager.Instance?.TriggerEvent(GameEventType.GameStart);
        }

        private void EndGame(bool playerWon)
        {
            isGameRunning = false;
            EventManager.Instance?.TriggerEvent(GameEventType.GameEnd, playerWon);
            OnGameEnd?.Invoke();
        }

        // днаюбэре щрнр лернд
        public void PlayerDied()
        {
            EndGame(false);
        }
    }
}