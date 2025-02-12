using Game.Scripts.Structs;
using PrimeTween;
using UnityEngine;

public class Clone_Controller : MonoBehaviour
{
    private Animator _anim;
    private Damage _damage;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
    }

    public void Setup(Damage damage, float fadeTime, float startDelay, bool canAttack, int attackStyle)
    {
        _damage = damage;
        _anim.SetInteger("ComboCounter", attackStyle);
        _anim.SetBool("CanAttack", canAttack);

        Tween.Color(_sr, Color.clear, fadeTime, startDelay: startDelay)
            .OnComplete(() => Destroy(transform.parent.gameObject));
    }

    public void OnHit(Collider2D other)
    {
        var enemy = other.GetComponent<Enemy>();
        enemy.HurtEvent.Invoke(_damage with { position = transform.position });
    }
}