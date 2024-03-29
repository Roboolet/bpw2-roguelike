using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeaTween {
    public class LeaTweener
        : MonoBehaviour
    {
        // static stuff
        static LeaTweener instance;

        public static LeaTweener GetTweenerInstance()
        {
            if (instance == null)
            {
                instance = CreateTweener();
                return instance;
            }
            else return instance;
        }

        static LeaTweener CreateTweener()
        {
            GameObject go = new GameObject("_LeaTween");
            LeaTweener tw = go.AddComponent<LeaTweener>();
            return tw;
        }

        public static LeaTFloat TweenFloat(float duration, float start, float end, LeaTEasing easing = LeaTEasing.None)
        {
            LeaTFloat tween = new LeaTFloat(duration, start, end, easing);
            GetTweenerInstance().AddTween(tween);

            return tween;
        }
        
        // stuff for the actual MonoBehaviour instance
        List<LeaTFloat> tweens = new List<LeaTFloat>();

        public void AddTween(LeaTFloat tween)
        {
            tweens.Add(tween);
            tween.OnFinish += (f) => tweens.Remove(tween);
        }

        private void Update()
        {
            for(int i = 0; i < tweens.Count; i++)
            {
                tweens[i].Update(Time.deltaTime);
            }
        }
    }

    public class LeaTFloat
    {      
        public readonly float duration;
        public readonly float start, end;
        public readonly LeaTEasing easing;
        float timeLeft;

        public delegate void EventFloat(float value);
        public event EventFloat OnFinish;
        public event EventFloat OnUpdate;

        public LeaTFloat(float duration, float start, float end, LeaTEasing easing)
        {
            this.duration = duration;
            timeLeft = duration;

            this.easing = easing;
            this.start = start;
            this.end = end;
        }

        internal void Update(float deltaTime)
        {
            timeLeft -= deltaTime;
            // check if there are any listeners first before calculating value
            if (OnUpdate != null)
            {
                OnUpdate.Invoke(GetValue());
            }

            if (timeLeft <= 0) Stop();
        }


        public void Stop()
        {
            if (OnFinish != null)
            {
                OnFinish.Invoke(GetValue());
            }
        }

        public float NormalizedTimeLeft()
        {
            return Mathf.Clamp(timeLeft / duration, 0, 1);
        }

        public float GetValue()
        {
            // do easings
            float t = NormalizedTimeLeft();            

            switch (easing)
            {
                default:
                    return Mathf.Lerp(end, start, t);

                case LeaTEasing.OutBounce:
                    // magic numbers from easings.net
                    const float n1 = 7.5625f;
                    const float d1 = 2.75f;
                    float mult;

                    if (t < 1 / d1)
                    {
                        mult = n1 * t * t;
                    }
                    else if (t < 2 / d1)
                    {
                        mult = n1 * ((t - 1.5f) / d1) * t + 0.75f;
                    }
                    else if (t < 2.5 / d1)
                    {
                        mult = n1 * (t -= 2.25f / d1) * t + 0.9375f;
                    }
                    else
                    {
                        mult = n1 * (t -= 2.625f / d1) * t + 0.984375f;
                    }

                    return start + mult * (start - end);
            }
        }
    }

    public enum LeaTEasing
    {
        None, OutBounce
    }
}
