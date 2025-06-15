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
        [SerializeField] private GameObject youWon;
        [SerializeField] private GameObject youLost;
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private TextMeshProUGUI gettingDamageText;
        [SerializeField] protected UnitType CurrUnitType;
        [SerializeField] protected HexGrid hexGrid;
        [SerializeField] protected int Damage;
        [SerializeField] protected int MaxHp;
        [SerializeField] protected int CurrHp;
        [SerializeField] protected HexCoordinates hexCoordinates;
        [SerializeField] private UnitManager unitManager;
 
        private HexCoordinates _hexCoordinates;

        private Education _education;
        private bool _isDiedFirstTime = false;
        private void Start()
        {
            if (youWon != null)
            {
                youWon.SetActive(false);
            }

            if (youLost != null)
            {
                youLost.SetActive(false); 
            }
            CurrHp = MaxHp;
            hexGrid = FindObjectOfType<HexGrid>();
        }
        public void Construct(Education education)
        {
            _education = education;
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
            gettingDamageText.text = dmg.ToString(); 
            _healthBar.UpdateBar((float)CurrHp/(float)MaxHp);
           
            if (CurrHp <= 0)
            {
                if(_education != null)
                {
                    if(_isDiedFirstTime == false)
                    {
                        _education.onEnemyDied.Invoke();
                        Debug.Log("Enemy died");
                        _isDiedFirstTime = true;
                    }
                }
                if (youWon != null && unitManager != null)
                {
                    Wining();
                }

                if (youLost != null && unitManager != null)
                {
                    Losing();
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
        public void Wining()
        {
            youWon.SetActive(true);
            unitManager.enabled = false;
        }
        public void Losing()
        {
            youLost.SetActive(true);
            unitManager.enabled = false;
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