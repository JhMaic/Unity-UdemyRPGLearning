using System;
using System.Collections.Generic;
using Game.Scripts.Structs;
using PrimeTween;
using R3;
using UnityEngine;

public partial class BlackholeSkill
{
    [Serializable]
    private record CloneAttackVariantConfig
    {
        public bool useAutoDuration;
        public int attackTimes;
        public float attackIntervalSecond;
        public float cloneFadeTime;
        public float cloneDuration;
    }

    private class CloneAttackType : SkillVariant
    {
        private readonly CloneAttackVariantConfig _config;
        private readonly List<Enemy> _enemies = new();

        public CloneAttackType(BlackholeSkill ctx) : base(ctx)
        {
            _config = ctx.cloneAttackVariantConfig;
        }

        public override void UseSkill()
        {
            base.UseSkill();

            ctx.player.Freeze();

            var playerSR = ctx.player.GetComponentInChildren<SpriteRenderer>();
            Tween.Color(playerSR, Color.clear, 1f);


            var duration = _config.useAutoDuration
                ? _config.attackIntervalSecond * _config.attackTimes + _config.cloneDuration + _config.cloneFadeTime
                : ctx.duration;

            ctx._controller.Setup(ctx.maxRadius, ctx.growSpeed, duration, 2, LayerMask.NameToLayer("PlayerHitBox"),
                () =>
                {
                    var count = _enemies.Count;
                    if (count.Equals(0))
                        return;

                    var idx = 0;
                    Observable.Interval(TimeSpan.FromSeconds(_config.attackIntervalSecond))
                        .Take(_config.attackTimes)
                        .Select(_ => _enemies[idx % count])
                        .Subscribe(enemy =>
                        {
                            idx++;
                            var clonePos = enemy.transform.TransformPoint(Vector2.left * 1);
                            SkillManager.Instance.Clone.SpawnAndAttack(
                                clonePos, enemy.transform.rotation,
                                _config.cloneFadeTime, _config.cloneDuration,
                                new Damage
                                {
                                    value = ctx.player.statAgent.CalcAttackDamage()
                                }, idx % 3);
                        });
                }, () =>
                {
                    isOpening = false;
                    Tween.Color(playerSR, Color.white, 0.3f);
                    ctx.player.UnFreeze();
                });
        }

        public override void OnEnemyEntered(Collider2D other)
        {
            base.OnEnemyEntered(other);
            _enemies.Add(other.GetComponent<Enemy>());
        }
    }
}