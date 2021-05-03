using UnityEngine;
using UnityEngine.Events;

namespace Plugins.Tools
{
    [System.Serializable]
    public struct TimerEvents
    {
        public UnityEvent onTimerStart;
        public UnityEvent onTimerStop;
        public UnityEvent onTimerEnd;
    }

    public enum TimerType { Progressive, Regressive }

    public readonly struct TimerOptions
    {
        public readonly TimerType timerType;
        public readonly float time;
        public readonly bool resetOnEnd;

        public TimerOptions(float time, TimerType timerType, bool resetOnEnd)
        {
            this.time = time;
            this.resetOnEnd = resetOnEnd;
            this.timerType = timerType;
        }
    }

    public class Timer : MonoBehaviour
    {
        [Header("Settings")]
        public TimerType type;

        public float timerTime;
        protected float m_Timer;

        public bool resetOnEnd;

        [Space]
        public TimerEvents events;
        public bool IsTimerStarted { get; private set; }

        protected virtual void Update()
        {
            if (!IsTimerStarted) return;
            if (type == TimerType.Progressive) UpdateTimerProgressive();
            else UpdateTimerRegressive();
        }

        private void UpdateTimerProgressive()
        {
            if (m_Timer < timerTime) m_Timer += Time.deltaTime;
            else CallEvents();
        }

        private void UpdateTimerRegressive()
        {
            if (m_Timer > 0) m_Timer -= Time.deltaTime;
            else CallEvents();
        }

        private void CallEvents()
        {
            events.onTimerEnd?.Invoke();
            if (resetOnEnd) ResetTimer();
            else StopTimer();
        }

        public void ResetTimer()
        {
            if (type == TimerType.Progressive) m_Timer = 0;
            else m_Timer = timerTime;
        }

        public void StartTimer()
        {
            ResetTimer();
            IsTimerStarted = true;
            events.onTimerStart?.Invoke();
        }

        public void StopTimer()
        {
            IsTimerStarted = false;
            events.onTimerStop?.Invoke();
        }

        public void SetTimer(TimerOptions options)
        {
            type = options.timerType;
            timerTime = options.time;
            resetOnEnd = options.resetOnEnd;
        }

        public void AddListeners(UnityAction onTimerStart, UnityAction onTimerEnd = null, UnityAction onTimerStop = null)
        {
            if (onTimerStart != null) events.onTimerStart.AddListener(onTimerStart);
            if (onTimerEnd != null) events.onTimerEnd.AddListener(onTimerEnd);
            if (onTimerStop != null) events.onTimerStop.AddListener(onTimerStop);
        }

        public void RemoveListeners(UnityAction onTimerStart, UnityAction onTimerEnd = null, UnityAction onTimerStop = null)
        {
            if (onTimerStart != null) events.onTimerStart.RemoveListener(onTimerStart);
            if (onTimerEnd != null) events.onTimerEnd.RemoveListener(onTimerEnd);
            if (onTimerStop != null) events.onTimerStop.RemoveListener(onTimerStop);
        }
    }

    public static class TimerExtensions
    {
        public static Timer CreateTimerInstance(this GameObject caller)
        {
            var timerGameObject = new GameObject { name = $"Timer_{caller.name}" };
            timerGameObject.transform.SetParent(caller.transform);
            return timerGameObject.AddComponent<Timer>();
        }
    }
}
