using UnityEngine;
using UnityEngine.Events;

public class ColliderEvents : MonoBehaviour
{
    public UnityEvent<Collider2D> TriggerEnter2DActions;
    public UnityEvent<Collider2D> TriggerExit2DActions;
    public UnityEvent<Collider2D> TriggerStay2DActions;
    public UnityEvent<Collision2D> CollisionEnter2DActions;
    public UnityEvent<Collision2D> CollisionExitActions;
    public UnityEvent<Collision2D> CollisionStay2DActions;

    private void OnCollisionEnter2D(Collision2D other)
    {
        CollisionEnter2DActions?.Invoke(other);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        CollisionExitActions?.Invoke(other);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        CollisionStay2DActions?.Invoke(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TriggerEnter2DActions?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TriggerExit2DActions?.Invoke(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TriggerStay2DActions?.Invoke(other);
    }
}