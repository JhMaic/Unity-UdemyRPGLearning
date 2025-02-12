using PrimeTween;
using UnityEngine;

public class Global : MonoBehaviour
{
    private void Start()
    {
        PrimeTweenConfig.validateCustomCurves = false;
    }
}