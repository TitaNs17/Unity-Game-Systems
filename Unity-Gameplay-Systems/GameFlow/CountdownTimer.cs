using System;

namespace UnityGameSystems.GameFlow
{
    public sealed class CountdownTimer
    {
        public float Duration { get; private set; }
        public float Remaining { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsFinished => !IsRunning && Remaining <= 0f;
        public float Normalized => Duration <= 0f ? 0f : Remaining / Duration;

        public event Action<float> TickChanged;
        public event Action Finished;

        public CountdownTimer(float duration)
        {
            Reset(duration);
        }

        public void Reset(float duration)
        {
            Duration = Math.Max(0f, duration);
            Remaining = Duration;
            IsRunning = false;
            TickChanged?.Invoke(Remaining);
        }

        public void Start()
        {
            if (Remaining <= 0f)
                Remaining = Duration;

            IsRunning = Remaining > 0f;
            TickChanged?.Invoke(Remaining);
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Tick(float deltaTime)
        {
            if (!IsRunning || deltaTime <= 0f) return;

            Remaining = Math.Max(0f, Remaining - deltaTime);
            TickChanged?.Invoke(Remaining);

            if (Remaining > 0f) return;

            IsRunning = false;
            Finished?.Invoke();
        }
    }
}
