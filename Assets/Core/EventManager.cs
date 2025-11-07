using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public enum GameEventType
    {
        EnemySpawned,
        EnemyDied,
        PlayerLevelUp,
        ExperienceGained,    // ← для получения опыта от врагов
        ExperienceChanged,   // ← для обновления HUD (ДОБАВЬТЕ ЭТО!)
        PlayerHealthChanged,
        GameStart,
        GameEnd
    }

    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        private Dictionary<GameEventType, Action<object>> eventDictionary;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                eventDictionary = new Dictionary<GameEventType, Action<object>>();
                DontDestroyOnLoad(gameObject);
                Debug.Log("✅ EventManager initialized successfully");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartListening(GameEventType eventType, Action<object> listener)
        {
            if (eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
            {
                thisEvent += listener;
                eventDictionary[eventType] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                eventDictionary.Add(eventType, thisEvent);
            }
        }

        public void StopListening(GameEventType eventType, Action<object> listener)
        {
            if (eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
            {
                thisEvent -= listener;
                eventDictionary[eventType] = thisEvent;
            }
        }

        public void TriggerEvent(GameEventType eventType, object eventParam = null)
        {
            Debug.Log($"🔈 Event triggered: {eventType} with param: {eventParam}");

            if (eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
            {
                thisEvent?.Invoke(eventParam);
            }
            else
            {
                Debug.LogWarning($"No listeners for event: {eventType}");
            }
        }
    }
}