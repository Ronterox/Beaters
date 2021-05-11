using UnityEngine;

namespace Plugins.Tools
{
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
        public float startTime;
        public delegate void TimerEvent();

        public TimerEvent onTimerStart, onTimerStop, onTimerEnd;

        public bool IsTimerStarted { get; private set; }
        public float CurrentTime => m_Timer;

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
            onTimerEnd?.Invoke();
            if (resetOnEnd) ResetTimer();
            else StopTimer();
        }

        public void PauseTimer() => IsTimerStarted = false;

        public void UnpauseTimer() => IsTimerStarted = true;

        public void ResetTimer() => m_Timer = type == TimerType.Progressive ? startTime : timerTime;

        public void StartTimer()
        {
            ResetTimer();
            IsTimerStarted = true;
            onTimerStart?.Invoke();
        }

        public void StopTimer()
        {
            IsTimerStarted = false;
            onTimerStop?.Invoke();
        }

        public void SetTimer(TimerOptions options)
        {
            type = options.timerType;
            timerTime = options.time;
            resetOnEnd = options.resetOnEnd;
        }

        public static Timer CreateTimerInstance(GameObject caller)
        {
            var timerGameObject = new GameObject { name = $"Timer_{caller.name}" };
            timerGameObject.transform.SetParent(caller.transform);
            return timerGameObject.AddComponent<Timer>();
        }
    }
}
