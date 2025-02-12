using UnityEditor;
using UnityEngine;

public class OverlapBox2D : MonoBehaviour
{
    public Vector2 boxSize;
    public LayerMask layerMask;
    public Color color;
    public string text;

    private Vector2 Origin => transform.position;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(Origin, boxSize);
        Handles.Label(Origin + new Vector2(-boxSize.x, boxSize.y + 1) / 2, text);
    }
#endif

    public bool IsOverlapped()
    {
        if (boxSize.Equals(Vector2.zero))
            return false;

        return Physics2D.OverlapBox(Origin, boxSize, 0, layerMask);
    }
}