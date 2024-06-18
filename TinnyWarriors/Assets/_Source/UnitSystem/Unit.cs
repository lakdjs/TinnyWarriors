using System;
using HexSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnitSystem
{
    public enum UnitType
    {
        melee,
        distant,
        king
    }
    public class Unit : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI youWon;
        [SerializeField] private TextMeshProUGUI youLost;
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] protected UnitType CurrUnitType;
        [SerializeField] protected HexGrid hexGrid;
        [SerializeField] protected int Damage;
        [SerializeField] protected int MaxHp;
        [SerializeField] protected int CurrHp;
        [SerializeField] protected HexCoordinates hexCoordinates;
        [SerializeField] private UnitManager unitManager;
        private HexCoordinates _hexCoordinates;
        private void Start()
        {
            if (youWon != null)
            {
                youWon.enabled = false;
            }

            if (youLost != null)
            {
                youLost.enabled = false;
            }
            CurrHp = MaxHp;
            hexGrid = FindObjectOfType<HexGrid>();
        }

        public int GetUnitDamage()
        {
            return Damage;
        }
        public UnitType GetUnitType()
        {
            return CurrUnitType;
        }
        public void TakeDamage(int dmg)
        {
            CurrHp -= dmg;
            _healthBar.UpdateBar((float)CurrHp/(float)MaxHp);
           
            if (CurrHp <= 0)
            {
                if (youWon != null && unitManager != null)
                {
                    youWon.enabled = true;
                    unitManager.enabled = false;    
                }

                if (youLost != null && unitManager != null)
                {
                    youLost.enabled = true;
                    unitManager.enabled = false;   
                }
                hexCoordinates = this.gameObject.GetComponent<HexCoordinates>();
                Debug.Log(hexCoordinates);
                Vector3Int currCoords =
                    HexCoordinates.ConvertPositionToOffset(gameObject.transform.position);
                hexGrid.GetTileAt(new Vector3Int(currCoords.x, 0,
                    currCoords.z)).SetType(HexType.Default);
                Destroy(gameObject);
                
            }
        }

        public virtual Vector3Int Attack(Hex hexToAttack)
        {
            return new Vector3Int();
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