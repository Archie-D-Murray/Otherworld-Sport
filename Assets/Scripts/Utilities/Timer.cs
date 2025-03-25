using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace Utilities {
    public abstract class Timer {
        [SerializeField] protected float _initialTime;
        [SerializeField] protected float _time;

        public bool IsRunning { get; protected set; }
        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };
        protected Timer(float initialTime) {
            this._initialTime = initialTime;
            IsRunning = false;
        }

        public void Start() {
            _time = _initialTime;
            if (!IsRunning) {
                IsRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public void Stop() {
            if (IsRunning) {
                IsRunning = false;
                OnTimerStop.Invoke();
            }
        }

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;
        public abstract void Update(float deltaTime);
        public abstract float Progress();
    }

    [Serializable]
    public class CountDownTimer : Timer {
        public CountDownTimer(float initialTime) : base(initialTime) { }
        public override void Update(float deltaTime) {
            if (IsRunning && _time > 0f) {
                _time = Mathf.Max(_time - deltaTime, 0f);
            }

            if (IsRunning && _time == 0f) {
                Stop();
            }
        }

        public bool IsFinished => _time == 0f;
        public void Reset() => _time = _initialTime;
        public void Reset(float newTime, bool startTimer = true) {
            _initialTime = newTime;
            Reset();
            if (startTimer) {
                Start();
            }
        }

        public override float Progress() => 1f - _time / _initialTime;
    }

    [Serializable]
    public class StopwatchTimer : Timer {
        public StopwatchTimer() : base(0f) { }

        public override void Update(float deltaTime) {
            if (IsRunning) {
                _time += deltaTime;
            }
        }

        public void Reset() {
            _time = 0f;
        }

        public override float Progress() => _time / _initialTime;

        public float GetTime() => _time;
    }
}