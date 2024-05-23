using UnityEngine;

namespace UnitSystem
{
    public class Unit : MonoBehaviour, IUnit
    {
        public int Damage { get; }
        public int MaxHp { get; }
        public int CurrHp { get; }

        public void TakeDamage()
        {
            //
        }

        public void Attack()
        {
            //
        }

        public void Heal()
        {
            //
        }
    }
}