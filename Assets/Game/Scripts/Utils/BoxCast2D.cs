using CustomInspector;
using UnityEngine;

public class BoxCast2D : MonoBehaviour
{
    [SerializeField] [ForceFill] private Transform origin;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Color color;

    private Vector2 OriginPoint => origin.position;
    private Vector2 CurrentPoint => transform.position;


    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(CurrentPoint, boxSize);
        Gizmos.DrawLine(OriginPoint, CurrentPoint);
    }

    public bool IsHit()
    {
        var directedVector = CurrentPoint - OriginPoint;
        return Physics2D.BoxCast(OriginPoint, new Vector2(Mathf.Abs(boxSize.x), Mathf.Abs(boxSize.y)), 0,
            directedVector.normalized,
            directedVector.magnitude,
            targetLayer);
    }
}