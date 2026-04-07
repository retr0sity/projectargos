using UnityEngine;
using System.Collections;

namespace Game.Extensions
{
    public static class Executer
    {
        private static IEnumerator _coroutine;

        public static void RunAfter(this MonoBehaviour monoBehaviour, float duration, System.Action action)
        {
            _coroutine = CreateRoutine(duration, action);
            StartRoutine(monoBehaviour);
        }

        public static void RunAfter(this MonoBehaviour monoBehaviour, int FrameCount, System.Action action)
        {
            _coroutine = CreateRoutine(FrameCount, action);
            StartRoutine(monoBehaviour);
        }

        private static IEnumerator CreateRoutine(float duration, System.Action action)
        {
            yield return new WaitForSeconds(duration);

            if (_coroutine != null)
            {
                action();
            }
        }

        private static IEnumerator CreateRoutine(int FrameCount, System.Action action)
        {
            for (int i = 0; i < FrameCount; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            if (_coroutine != null)
            {
                action();
            }
        }

        private static void StartRoutine(MonoBehaviour monoBehaviour)
        {
            monoBehaviour.StartCoroutine(_coroutine);
        }
    }
}