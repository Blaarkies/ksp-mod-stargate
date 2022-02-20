using System;
using System.Collections;
using UnityEngine;

namespace Stargate.Utilities
{
    public class Waiter : MonoBehaviour
    {
        public void DoAction(Action action, float delay)
        {
            StartCoroutine(DelayAction(action, delay));
        }

        private static IEnumerator DelayAction(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action();
        }
    }
}
