using System.Collections.Generic;
using System.Linq;
using UnitSystem;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Random = System.Random;

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
        [SerializeField] private List<UnitMovement> units;
        [SerializeField] private EnemyMovement king;
        [SerializeField] private UnitMovement unitKing;
        public bool PlayersTurn { get; private set; } = true;
        public bool EnemiesTurn { get; private set; } = false;

        [SerializeField]
        private UnitMovement selectedUnit;

        private BFSResult enemyMovementRange;
        
        
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

        private Education _education;
        private bool _isPlayerSelectedFirstTime = false;
        private bool _isTerraianSelectedFirstTime = false;
        private bool _isPlayerMovedToEnemyFirstTime = false;

        private void Awake()
        {
            PlayerSelected = false;
            _isBattling = false;
            skipAttack.interactable = false;
        }
        public void Construct(Education education)
        {
            _education = education;
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

        public void DeleteUnitFromList(UnitMovement unit)
        {
            units.Remove(unit);
        }
        public void HandleUnitSelected(GameObject unit)
        {
            foreach (EnemyMovement enemy in enemies)
            {
                HexCoordinates coords = enemy.GetComponent<HexCoordinates>();
                coords.SetCoords();
            }
            foreach (UnitMovement uni in units)
            {
                HexCoordinates coords = uni.GetComponent<HexCoordinates>();
                coords.SetCoords();
            }
            if (PlayersTurn == false)
            {
                return;
            }
            if (_education != null)
            {
                if (_isPlayerSelectedFirstTime == false)
                {
                    _education.onPlayerTargeted.Invoke();
                    Debug.Log("Player Selected first time");
                    _isPlayerSelectedFirstTime = true;
                }
            }
            PlayerSelected = true;
            UnitMovement unitReference = unit.GetComponent<UnitMovement>();
            //Unit unitInfo = unit.GetComponent<Unit>();

            if (CheckIfTheSameUnitSelected(unitReference))
            {
                return;
            }
                
            PrepareUnitForMovement(unitReference);
            
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
            if (_education != null)
            {
                if (_isTerraianSelectedFirstTime == false)
                {
                    _education.onPathTargeted.Invoke();
                    Debug.Log("Terrain Selected first time");
                    _isTerraianSelectedFirstTime = true;
                }
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
            foreach (UnitMovement unit in units)
            {
                HexCoordinates coords = unit.GetComponent<HexCoordinates>();
                coords.SetCoords();
            }
            foreach (EnemyMovement enemy in enemies)
            {
                HexCoordinates coords = enemy.GetComponent<HexCoordinates>();
                coords.SetCoords();
            }
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
                if (_education != null)
                {
                    if (_isPlayerMovedToEnemyFirstTime == false)
                    {
                        _education.onMovedToEnemy.Invoke();
                        Debug.Log("Moved to enemy first time");
                        _isPlayerMovedToEnemyFirstTime = true;
                    }
                }
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
            
            foreach (Vector3Int direction in hexGrid.GetNeighboursInRangeOf2(kingCoords))
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
            hexGrid.GetTileAt(lastUnitCoords).SetType(HexType.Unit);
            
            PrepareOtherEnemyTurn();
            //move melee or distant
            
            //enemies[0].MoveThroughPath();
        }

        private void MoveEnemyKing()
        {
            _enemyCoords = king.GetComponent<HexCoordinates>();
            _enemyCoords.SetCoords();
            hexGrid.GetTileAt(new Vector3Int(_enemyCoords.GetHexCoords().x,0,_enemyCoords.GetHexCoords().z)).SetType(HexType.Default);
            Vector3Int lastUnitCoords = new Vector3Int(_lastHex.HexCoords.x, 0, _lastHex.HexCoords.z);
            Vector3Int kingCoords = new Vector3Int(king.GetCoords().x, 0, king.GetCoords().z);
            //_enemyCoords = king.GetComponent<HexCoordinates>();
            foreach (Vector3Int direction in hexGrid.GetNeighboursFor(kingCoords))
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

        private void PrepareOtherEnemyTurn()
        {
            //Vector3Int lastUnitCoords = new Vector3Int(_lastHex.HexCoords.x, 0, _lastHex.HexCoords.z);
            //hexGrid.GetTileAt(lastUnitCoords).SetType(HexType.Default);
            int enemyQuantity = enemies.Count;
            int curEnemy = 0;
            Random random = new();
            while (true)
            {
                int enemy = random.Next(0, enemyQuantity);
                if (enemies[enemy].Type != UnitType.king)
                {
                    curEnemy = enemy;
                    break;
                }
            }
            MoveOtherEnemy(curEnemy);
            
        }

        private void MoveOtherEnemy(int enemyId)
        {
            _enemyCoords = enemies[enemyId].GetComponent<HexCoordinates>();
            _enemyCoords.SetCoords();
            
            Vector3Int enemyCoords = new Vector3Int(enemies[enemyId].GetCoords().x, 0, enemies[enemyId].GetCoords().z);
            List<Vector3Int> currentPath = new List<Vector3Int>();
            List<Vector3Int> pathToKing = new List<Vector3Int>();
            Vector3Int unitKingPos = unitKing._hexCoordinates.GetHexCoords();
           // movementSystem.ShowPath(new Vector3Int(unitKingPos.x, 0, unitKingPos.z),hexGrid);
            _bfs = GraphSearch.BFSGetRange(hexGrid, enemyCoords, 10000);
            currentPath = _bfs.GetPathTo(new Vector3Int(unitKingPos.x, 0, unitKingPos.z));
            if (currentPath.Count > 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    pathToKing.Add(currentPath[i]);
                }
            }
            else if(currentPath.Count == 1)
            {
                
                EnemyAttack(enemyId,enemyCoords );
                return;
            }
            
            else if(currentPath.Count == 2)
            {
                for (int i = 0; i < 1; i++)
                {
                    pathToKing.Add(currentPath[i]);
                }
            }
            hexGrid.GetTileAt(new Vector3Int(_enemyCoords.GetHexCoords().x, 0, _enemyCoords.GetHexCoords().z)).SetType(HexType.Default);
            //king.MoveThroughPath(new List<Vector3>{direction}.Select(pos => hexGrid.GetTileAt(new Vector3Int(direction.x,0,direction.z)).transform.position).ToList() );
            enemies[enemyId].MoveThroughPath(pathToKing.Select(pos =>
                hexGrid.GetTileAt(new Vector3Int(pathToKing[pathToKing.Count - 1].x, 0,
                    pathToKing[pathToKing.Count - 1].z)).transform.position).ToList());
            hexGrid.GetTileAt(pathToKing[pathToKing.Count - 1]).SetType(HexType.Enemy);
            
            EnemyAttack(enemyId,pathToKing[pathToKing.Count - 1]);
           
        }

        private void EnemyAttack(int enemyID, Vector3Int lastEnemyPos)
        {
            Vector3Int enemyCoords = new Vector3Int(enemies[enemyID].GetCoords().x, 0, enemies[enemyID].GetCoords().z);
            _bfs = GraphSearch.BFSGetRange(hexGrid, lastEnemyPos, 30);
           
            foreach (UnitMovement unit in units)
            { 
                Vector3Int lastUnitCoords = new Vector3Int(unit._hexCoordinates.GetHexCoords().x, 0,
                    unit._hexCoordinates.GetHexCoords().z);
               Debug.Log(lastUnitCoords);
            }
            if (enemies[enemyID].Type == UnitType.melee)
            {
                foreach (Vector3Int direction in hexGrid.GetNeighboursFor(lastEnemyPos) )
                {
                    Debug.Log(direction);
                    foreach (UnitMovement unit in units)
                    { 
                        Vector3Int lastUnitCoords = new Vector3Int(unit._hexCoordinates.GetHexCoords().x, 0,
                            unit._hexCoordinates.GetHexCoords().z);
                        if(lastUnitCoords.x == direction.x && lastUnitCoords.z == direction.z)
                        {
                            foreach (UnitMovement unitToAttack in units)
                            {
                                if (unitToAttack._hexCoordinates.GetHexCoords().x == lastUnitCoords.x &&
                                    unitToAttack._hexCoordinates.GetHexCoords().z == lastUnitCoords.z)
                                {
                                    unitToAttack.GetComponent<Unit>().TakeDamage(enemies[enemyID].GetComponent<Unit>().GetUnitDamage());
                                    PlayersTurn = true;
                                    return;
                                }
                            }

                            
                        }
                    }
                    
                } 
            }
            if (enemies[enemyID].Type == UnitType.distant)
            {
                foreach (Vector3Int direction in hexGrid.GetNeighboursInRangeOf2(lastEnemyPos))
                {
                    Debug.Log(direction);
                    foreach (UnitMovement unit in units)
                    { 
                        Vector3Int lastUnitCoords = new Vector3Int(unit._hexCoordinates.GetHexCoords().x, 0,
                            unit._hexCoordinates.GetHexCoords().z);
                        if( lastUnitCoords.x == direction.x && lastUnitCoords.z == direction.z)
                        {
                            foreach (UnitMovement unitToAttack in units)
                            {
                                if (unitToAttack._hexCoordinates.GetHexCoords().x == lastUnitCoords.x &&
                                    unitToAttack._hexCoordinates.GetHexCoords().z == lastUnitCoords.z)
                                {
                                    unitToAttack.GetComponent<Unit>().TakeDamage(enemies[enemyID].GetComponent<Unit>().GetUnitDamage());
                                    PlayersTurn = true;
                                    return;
                                }
                            }
                            
                        }
                    }
                }
                
            }
            PlayersTurn = true;
        }
        //
        private void EnemyTurn()
        {
            foreach (UnitMovement unit in units)
            {
                HexCoordinates coords = unit.GetComponent<HexCoordinates>();
                coords.SetCoords();
            }
            foreach (EnemyMovement enemy in enemies)
            {
                HexCoordinates coords = enemy.GetComponent<HexCoordinates>();
                coords.SetCoords();
            }
            skipAttack.onClick.RemoveAllListeners();
            skipAttack.interactable = false;
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
