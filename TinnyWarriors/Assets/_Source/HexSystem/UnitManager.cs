using System.Collections.Generic;
using System.Linq;
using UnitSystem;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace HexSystem
{
    public class UnitManager : MonoBehaviour
    {
        [SerializeField]
        private HexGrid hexGrid;

        [SerializeField] private Button skipAttack;
        
        [SerializeField]
        private MovementSystem movementSystem;
        
            //TODO переписать полностью! постройку в другой класс!
        public bool PlayerSelected { get; private set; }
        private bool _isBattling;

        [SerializeField] private List<EnemyMovement> enemies;
        [SerializeField] private EnemyMovement king;
        public bool PlayersTurn { get; private set; } = true;
        public bool EnemiesTurn { get; private set; } = false;

        [SerializeField]
        private UnitMovement selectedUnit;

        private UnitMovement _unitToAttack;
        private Unit _unit;
        private Unit _enemy;
        private HexCoordinates _selectedUnitCoords;
        private HexCoordinates _enemyCoords;

        private BFSResult _bfs = new BFSResult();
        private GraphSearch _range = new GraphSearch();
        
        private UnitMovement _selectedEnemy;
        
        
        private Hex _previouslySelectedHex;
        private Hex _previousEnemyHex;

        private Hex _lastHex;

        private void Awake()
        {
            PlayerSelected = false;
            _isBattling = false;
            skipAttack.interactable = false;
        }

        public void HandleEnemySelected(GameObject enemy)
        {
            if (PlayersTurn == false)
            {
                if (_isBattling)
                {
                    EnemyMovement enemyReference = enemy.GetComponent<EnemyMovement>();
                   //new Vector3Int(_selectedUnitCoords.GetHexCoords().x,0,_selectedUnitCoords.GetHexCoords().z))
                   Vector3Int enemyCoords = enemyReference.gameObject.GetComponent<HexCoordinates>().GetHexCoords();
                   _enemy = enemy.GetComponent<Unit>();
                    Hex selectedHexToAttack =
                        hexGrid.GetTileAt(new Vector3Int(enemyCoords.x, 0, enemyCoords.z));
                    if (selectedHexToAttack.IsEnemy())
                    {
                        Battling(selectedHexToAttack);
                    }
                }
            }
        }

        public void DeleteEnemyFromList(EnemyMovement enemy)
        {
            enemies.Remove(enemy);
        }
        public void HandleUnitSelected(GameObject unit)
        {
            foreach (EnemyMovement enemy in enemies)
            {
                HexCoordinates coords = enemy.GetComponent<HexCoordinates>();
                coords.SetCoords();
            }
            if (PlayersTurn == false)
            {
                return;
            }
                
            PlayerSelected = true;
            UnitMovement unitReference = unit.GetComponent<UnitMovement>();
            //Unit unitInfo = unit.GetComponent<Unit>();

            if (CheckIfTheSameUnitSelected(unitReference))
            {
                return;
            }
                
            PrepareUnitForMovement(unitReference);
            Debug.Log(unitReference);
        }

        private bool CheckIfTheSameUnitSelected(UnitMovement unitReference)
        {
            if (this.selectedUnit == unitReference)
            {
                ClearOldSelection();
                return true;
            }
            return false;
        }

        public void HandleTerrainSelected(GameObject hexGO)
        {
        
            if (selectedUnit == null || PlayersTurn == false)
            {
                return;
            }
            Hex selectedHex = hexGO.GetComponent<Hex>();

            if (HandleHexOutOfRange(selectedHex.HexCoords) || HandleSelectedHexIsUnitHex(selectedHex.HexCoords))
                return;

            HandleTargetHexSelected(selectedHex);

        }

        private void PrepareUnitForMovement(UnitMovement unitReference)
        {
            if (this.selectedUnit != null)
            {
                ClearOldSelection();
            }
            this.selectedUnit = unitReference;
            _unit = unitReference.gameObject.GetComponent<Unit>();
            this._selectedUnitCoords = unitReference.gameObject.GetComponent<HexCoordinates>();
            this.selectedUnit.Select();
            movementSystem.ShowRange(this.selectedUnit, this.hexGrid);
        }

        private void ClearOldSelection()
        {
            _selectedUnitCoords = null;
            _previouslySelectedHex = null;
            this.selectedUnit.Deselect();
            movementSystem.HideRange(this.hexGrid);
            this.selectedUnit = null;
            //_unit = null;

        }

        private void HandleTargetHexSelected(Hex selectedHex)
        {
            if (_previouslySelectedHex == null || _previouslySelectedHex != selectedHex)
            {
                _previouslySelectedHex = selectedHex;
                movementSystem.ShowPath(selectedHex.HexCoords, this.hexGrid);
                _lastHex = selectedHex;
            }
            else
            {
                _selectedUnitCoords.SetCoords();
                hexGrid.GetTileAt(new Vector3Int(_selectedUnitCoords.GetHexCoords().x,0,_selectedUnitCoords.GetHexCoords().z)).SetType(HexType.Default);
                movementSystem.MoveUnit(selectedUnit, this.hexGrid);
                PlayersTurn = false;
                _previouslySelectedHex.SetType(HexType.Unit);
                selectedUnit.MovementFinished += ResetTurn;
                _unitToAttack = selectedUnit;
                ClearOldSelection();
            }
        }

        private bool HandleSelectedHexIsUnitHex(Vector3Int hexPosition)
        {
            if (hexPosition == hexGrid.GetClosestHex(selectedUnit.transform.position))
            {
                selectedUnit.Deselect();
                ClearOldSelection();
                return true;
            }
            return false;
        }

        private bool HandleHexOutOfRange(Vector3Int hexPosition)
        {
            if (movementSystem.IsHexInRange(hexPosition) == false)
            {
                Debug.Log("Hex Out of range!");
                return true;
            }
            return false;
        }

        private void CheckEnemyKing()
        {
            
            Vector3Int lastUnitCoords = new Vector3Int(_lastHex.HexCoords.x, 0, _lastHex.HexCoords.z);
            Vector3Int kingCoords = new Vector3Int(king.GetCoords().x, 0, king.GetCoords().z);
            hexGrid.GetTileAt(lastUnitCoords).SetType(HexType.Default);
            _bfs = GraphSearch.BFSGetRange(hexGrid, kingCoords, 30);
            
            foreach (Vector3Int direction in hexGrid.GetNeighboursInRangeOf2(kingCoords) )
            {
                if(_bfs.IsHexPositionInRange(direction) &&  lastUnitCoords.x == direction.x && lastUnitCoords.z == direction.z)
                {
                    MoveEnemyKing();
                    return;
                }
            }
            foreach (Vector3Int direction in hexGrid.GetNeighboursFor(kingCoords) )
            {
                if(_bfs.IsHexPositionInRange(direction)  && lastUnitCoords.x == direction.x && lastUnitCoords.z == direction.z)
                {
                    MoveEnemyKing();
                    return;
                }
            }
            Debug.Log("NOt in danger");
            hexGrid.GetTileAt(lastUnitCoords).SetType(HexType.Unit);
            
            //move melee or distant
            
            //enemies[0].MoveThroughPath();
        }

        private void MoveEnemyKing()
        {
            Debug.Log("King movement");
            _enemyCoords = king.GetComponent<HexCoordinates>();
            _enemyCoords.SetCoords();
            hexGrid.GetTileAt(new Vector3Int(_enemyCoords.GetHexCoords().x,0,_enemyCoords.GetHexCoords().z)).SetType(HexType.Default);
            Vector3Int lastUnitCoords = new Vector3Int(_lastHex.HexCoords.x, 0, _lastHex.HexCoords.z);
            Vector3Int kingCoords = new Vector3Int(king.GetCoords().x, 0, king.GetCoords().z);
            //_enemyCoords = king.GetComponent<HexCoordinates>();
            foreach (Vector3Int direction in hexGrid.GetNeighboursFor(kingCoords) )
            {
                if(direction != lastUnitCoords)
                {
                    //_previousEnemyHex.SetType(HexType.Default);
                    _enemyCoords.SetCoords();
                    hexGrid.GetTileAt(new Vector3Int(_enemyCoords.GetHexCoords().x,0,_enemyCoords.GetHexCoords().z)).SetType(HexType.Default);
                   
                    king.MoveThroughPath(new List<Vector3>{direction}.Select(pos => hexGrid.GetTileAt(new Vector3Int(direction.x,0,direction.z)).transform.position).ToList() );
                    hexGrid.GetTileAt(direction).SetType(HexType.Enemy);
                    
                   break;
                }
            }

            PlayersTurn = true;

        }

        private void OtherEnemyTurn()
        {
            
        }
         
        private void EnemyTurn()
        {
            skipAttack.onClick.RemoveAllListeners();
            skipAttack.interactable = false;
            Debug.Log("Enemies turn");
            CheckEnemyKing();
           // PlayersTurn = true;
        }
        private void ResetTurn(UnitMovement selectedUnit)
        {
            selectedUnit.MovementFinished -= ResetTurn;
            BattleTurn(selectedUnit);
            //PlayersTurn = true;
            //Debug.Log("Finished turn");
        }

        private void BattleTurn(UnitMovement selectedUnit)
        {
            skipAttack.interactable = true;
            skipAttack.onClick.AddListener(EnemyTurn);
            _isBattling = true;
            
           //Debug.Log("battle");
           
        }

        private void Battling(Hex hexToAttack)
        {
            //Debug.Log(_unit);
            if (_unit.GetUnitType() == UnitType.melee)
            {
                MeleeAttack(hexToAttack);
                return;
            }

            if (_unit.GetUnitType() == UnitType.distant)
            {
                DistantAttack(hexToAttack);
                return;
            }
            /*foreach (Vector3Int direction in hexGrid.GetNeighboursInRangeOf2(_lastHex.HexCoords) )
            {
                if (hexGrid.GetTileAt(direction).IsEnemy() && hexGrid.GetTileAt(direction) == hexToAttack)
                {
                    Debug.Log($"{_unitToAttack}Attacks ENEMY on: {hexToAttack.HexCoords}");
                   // Debug.Log(direction);
                   PlayersTurn = true;
                   _isBattling = false;
                }
            }*/
        }

        private void DistantAttack(Hex hexToAttack)
        {
            foreach (Vector3Int direction in hexGrid.GetNeighboursInRangeOf2(_lastHex.HexCoords) )
            {
                if (hexGrid.GetTileAt(direction).IsEnemy() && hexGrid.GetTileAt(direction) == hexToAttack)
                {
                    _enemy.TakeDamage(_unit.GetUnitDamage());
                    Debug.Log($"Удар!");
                    // Debug.Log(direction);
                    PlayersTurn = false;
                    _isBattling = false;
                    _unit = null;
                    EnemyTurn();
                }
            }
        }

        private void MeleeAttack(Hex hexToAttack)
        {
            foreach (Vector3Int direction in hexGrid.GetNeighboursFor(_lastHex.HexCoords) )
            {
                //Debug.Log(direction);
                if (hexGrid.GetTileAt(direction).IsEnemy() && hexGrid.GetTileAt(direction) == hexToAttack)
                {
                    _enemy.TakeDamage(_unit.GetUnitDamage());
                    Debug.Log($"Удар!");
                    // Debug.Log(direction);
                    PlayersTurn = false;
                    _isBattling = false;
                    _unit = null;
                    EnemyTurn();
                }
            }
        }
    }
}
