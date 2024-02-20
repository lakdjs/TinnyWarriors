using System;
using UnityEngine;

namespace Ecs.Components
{
    [Serializable]
    public struct MoveStateComponent
    {
        public bool moveRequired;
        public Vector3 direction;
    }
}