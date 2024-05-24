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
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] protected UnitType CurrUnitType;
        [SerializeField] protected HexGrid hexGrid;
        [SerializeField] protected int Damage;
        [SerializeField] protected int MaxHp;
        [SerializeField] protected int CurrHp;
        [SerializeField] protected HexCoordinates hexCoordinates;
        private HexCoordinates _hexCoordinates;
        private void Start()
        {
            if (youWon != null)
            {
                youWon.enabled = false;
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
            Debug.Log($"Curr hp is{CurrHp}");
            if (CurrHp <= 0)
            {
                if (youWon != null)
                {
                    youWon.enabled = true;
                }
                hexCoordinates = this.gameObject.GetComponent<HexCoordinates>();
                Debug.Log(hexCoordinates);
                Vector3Int currCoords =
                    HexCoordinates.ConvertPositionToOffset(gameObject.transform.position);
                hexGrid.GetTileAt(new Vector3Int(currCoords.x, 0,
                    currCoords.z)).SetType(HexType.Default);
                Destroy(gameObject);
                Debug.Log("You died");
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