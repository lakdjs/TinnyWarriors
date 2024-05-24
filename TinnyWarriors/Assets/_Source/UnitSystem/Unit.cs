using System;
using UnityEngine;

namespace UnitSystem
{
    public class Unit : MonoBehaviour, IUnit
    {
        public int Damage { get; private set; }
        public int MaxHp { get; private set; }
        public int CurrHp { get; private set; }

        private void Start()
        {
            CurrHp = MaxHp;
        }

        public void TakeDamage(int dmg)
        {
            CurrHp -= dmg;
            if (CurrHp <= 0)
            {
                Debug.Log("You died");
            }
        }

        public void Attack()
        {
            //
        }

        public void Heal(int hp)
        {
            if (CurrHp + hp <= MaxHp)
            {
                CurrHp += hp;
            }
            else
            {
                CurrHp = MaxHp;
            }
        }
    }
}