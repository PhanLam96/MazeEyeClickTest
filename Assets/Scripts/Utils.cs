using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static IEnumerator IEDelayCall(Action callBack, float delay)
    {
        yield return new WaitForSeconds(delay);
        callBack?.Invoke();
    }
}
