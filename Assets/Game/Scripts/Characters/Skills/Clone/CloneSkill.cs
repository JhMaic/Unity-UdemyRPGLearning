using CustomInspector;
using Game.Scripts.Structs;
using UnityEngine;

public class CloneSkill : Skill
{
    [SerializeField] [ForceFill] private GameObject prefab;
    public float fadeTime;
    public float duration;
    public bool canAttack;

    [ShowIf(nameof(canAttack))]
    public Damage damage;


    public override string SkillName => "Clone";

    protected override void UseSkill()
    {
        // Observable.Repeat(Unit.Default, 10_000)
        //     .Chunk(50)
        //     .SubscribeAwait(async (_, ct) =>
        //     {
        //         _.ForEach(_ => Instantiate(prefab));
        //         await UniTask.Yield(PlayerLoopTiming.Update, ct);
        //     });

        var playerTransform = player.transform;

        var controller = Instantiate(prefab, playerTransform.position, playerTransform.rotation)
            .GetComponentInChildren<Clone_Controller>();

        controller.Setup(damage, fadeTime, duration, canAttack, player.ComboCounter % 3);
    }

    public void SpawnAndAttack(Vector2 position, Quaternion rotation, float fadeTime,
        float duration, Damage damage, int attackStyle)
    {
        var controller = Instantiate(prefab, position, rotation)
            .GetComponentInChildren<Clone_Controller>();


        controller.Setup(damage, fadeTime, duration, true, attackStyle);
    }
}