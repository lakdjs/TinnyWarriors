using System;
using UnityEngine;

namespace BattleSystem
{
    public enum BattleState
    {
        Start,
        PlayerTurn,
        EnemyTurn,
        Won,
        Lost
    }

    public class Battle : MonoBehaviour
    {
        private BattleState _state;
        private void Start()
        {
            _state = BattleState.Start;
            SetUpBattle();
        }

        private void SetUpBattle()
        {
            _state = BattleState.PlayerTurn;
        }
        
        void PlayerTurn()
        {
            
        }

        void EnemyTurn()
        {
            
        }
    }
}
