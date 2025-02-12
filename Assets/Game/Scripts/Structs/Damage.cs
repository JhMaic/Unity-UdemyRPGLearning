using System;
using CustomInspector;
using UnityEngine;

namespace Game.Scripts.Structs
{
    [Serializable]
    public record Damage
    {
        public Vector2 force;
        public float value;
        [HideField] public Vector3 position;
    }
}