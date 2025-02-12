using CustomInspector;
using UnityEngine;

public class RayCast2D : MonoBehaviour
{
    [SerializeField] [ForceFill] private Transform origin;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Color color;

    private Vector2 OriginPoint => origin.position;
    private Vector2 CurrentPoint => transform.position;


    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawLine(OriginPoint, CurrentPoint);
    }

    public bool IsHit()
    {
        var directedVector = CurrentPoint - OriginPoint;
        return Physics2D.Raycast(OriginPoint, directedVector.normalized, directedVector.magnitude, targetLayer);
    }
}