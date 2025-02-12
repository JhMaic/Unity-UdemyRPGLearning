using System;
using System.Collections.Generic;
using CustomInspector;
using UnityEngine;

public class SwordSkill : Skill
{
    public enum SwordType
    {
        Default,
        Bounce,
        Pierce,
        Spin
    }

    [Header("Sword")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private float launchForce;
    public SwordType swordType;


    [Header("Skill Variant")]
    [SerializeField] [ShowIfIs(nameof(swordType), SwordType.Default)]
    private Sword_Controller.DefaultConfig defaultConfig;

    [SerializeField] [ShowIfIs(nameof(swordType), SwordType.Bounce)]
    private Sword_Controller.BounceConfig bounceConfig;

    [SerializeField] [ShowIfIs(nameof(swordType), SwordType.Pierce)]
    private Sword_Controller.PierceConfig pierceConfig;

    [SerializeField] [ShowIfIs(nameof(swordType), SwordType.Spin)]
    private Sword_Controller.SpinConfig spinConfig;


    [Header("Sword Return")]
    [SerializeField] private float returnSpeed = 20;
    [SerializeField] private float minCompleteDistance = 1.0f;
    public float catchImpact = 10;


    [Header("Aim Dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float timeSpaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;

    private List<GameObject> _dotPool;


    public override string SkillName => "Sword";

    public GameObject SwordInstance { get; private set; }
    private Vector2 InitVelocity => launchForce * CalcAimingDirection();

    private Sword_Controller.BaseConfig GetSwordConfig => swordType switch
    {
        SwordType.Default => defaultConfig,
        SwordType.Bounce => bounceConfig with { onCompleted = ReturnSwordToPlayer },
        SwordType.Pierce => pierceConfig,
        SwordType.Spin => spinConfig with { onCompleted = ReturnSwordToPlayer },
        _ => throw new Exception()
    };

    private void Awake()
    {
        var dotParent = new GameObject("Aim Dots");
        _dotPool = new List<GameObject>();
        for (var i = 0; i < numberOfDots; i++)
        {
            var dot = Instantiate(dotPrefab, dotParent.transform);
            dot.SetActive(false);
            _dotPool.Add(dot);
        }
    }

    public void DestroySwordInstance()
    {
        Destroy(SwordInstance);
        SwordInstance = null;
    }

    protected override bool SkillAvailability(out string reason)
    {
        if (SwordInstance is not null)
        {
            reason = "Already have sword";
            return false;
        }

        return base.SkillAvailability(out reason);
    }

    public void ReturnSwordToPlayer()
    {
        var swordController = SwordInstance.GetComponent<Sword_Controller>();

        swordController.ReturnToPlayer(returnSpeed, minCompleteDistance, () =>
        {
            player.CatchSword();
            DestroySwordInstance();
        });
    }


    protected override void UseSkill()
    {
        SwordInstance = Instantiate(swordPrefab);

        var swordController = SwordInstance.GetComponent<Sword_Controller>();
        swordController.Setup(GetSwordConfig with
        {
            launchStartPos = player.transform.position,
            launchVelocity = InitVelocity
        });

        swordController.Launch();
    }

    private Vector2 CalcAimingDirection()
    {
        var playerPos = (Vector2)player.transform.position;
        var mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);


        return (mousePos - playerPos).normalized;
    }

    public void DrawDotCurve()
    {
        for (var i = 0; i < _dotPool.Count; i++)
        {
            _dotPool[i].SetActive(true);
            _dotPool[i].transform.position = GetDotPosition(i * timeSpaceBetweenDots);
        }
    }

    public void DestroyDots()
    {
        _dotPool.ForEach(dot => dot.SetActive(false));
    }

    private Vector2 GetDotPosition(float time)
    {
        var pos = (Vector2)player.transform.position + InitVelocity * time +
                  Physics2D.gravity * (0.5f * GetSwordConfig.launchGravityScale * time * time);

        return pos;
    }
}