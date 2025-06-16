using EnemySystem;
using System;
using System.Collections;
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
        public Action winingAction;
        public Action losingAction;

        [SerializeField]
        private HexGrid hexGrid;

        [SerializeField] private Button skipAttack;
        
        [SerializeField]
        private MovementSystem movementSystem;

        [SerializeField] private Material origMaterial;
        [SerializeField] private Material enemyMaterial;

        private int indexOfUnit;
        private int indexOfEnemy;
            
        public bool PlayerSelected { get; private set; }
        private bool _isBattling;

        [SerializeField] private List<EnemyMovement> enemies;
        [SerializeField] private List<UnitMovement> units;
        [SerializeField] private EnemyMovement king;
        [SerializeField] private UnitMovement unitKing;
        [SerializeField] private Unit enemyKingUnit;
        [SerializeField] private Unit kingUnit;
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

        private List<Vector3Int> enemiesToGlow = new List<Vector3Int>();

        private void Awake()
        {

            SetUp();
            
        }
        private void SetUp()
        {
            PlayerSelected = false;
            _isBattling = false;
            indexOfUnit = 0;
            indexOfEnemy = 0;
            StartCoroutine(LoadingScene());
            SelectUnit();
            //Debug.Log(enemies.Count);
        }
        IEnumerator LoadingScene()
        {
            yield return new WaitForSeconds(1.5f);
        }
        public void Construct(Education education)
        {
            _education = education;
        }
        public void HandleEnemySelected(GameObject enemy)
        {
            if (PlayersTurn == true)
            {
                
                    EnemyMovement enemyReference = enemy.GetComponent<EnemyMovement>();
                   
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
        private void SelectUnit()
        {
            foreach (UnitMovement unit in units)
            {
                HexCoordinates coordss = unit.GetComponent<HexCoordinates>();
                coordss.SetCoords();
            }
            foreach (EnemyMovement enemy in enemies)
            {
                HexCoordinates coordss = enemy.GetComponent<HexCoordinates>();
                coordss.SetCoords();
            }

            PlayersTurn = true;
            selectedUnit = units[indexOfUnit];
            HexCoordinates coords = selectedUnit.GetComponent<HexCoordinates>();
            coords.SetCoords();
            PrepareUnitForMovement(selectedUnit);
            indexOfUnit++;
        }
        private void SelectEnemy()
        {
            StartCoroutine(EnemyTurnCoroutine(1));
            //StartCoroutine(EnemyTurnCoroutine());
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
            
            PlayersTurn = false;
            MoveOtherEnemy(indexOfEnemy);
            
        }
        private void PrepareUnitForMovement(UnitMovement unitReference)
        {
           
            selectedUnit = unitReference;
            _unit = unitReference.gameObject.GetComponent<Unit>();
            _selectedUnitCoords = unitReference.gameObject.GetComponent<HexCoordinates>();
            selectedUnit.Select();
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
            if(selectedUnit!= null)
            this.selectedUnit.Deselect();
            movementSystem.HideRange(this.hexGrid);
            this.selectedUnit = null;
           

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
                
                _previouslySelectedHex.SetType(HexType.Unit);
                
                BattleTurn(selectedHex);
                
                //ClearOldSelection();
                _unitToAttack = selectedUnit;
                if (_education != null)
                {
                    if (_isPlayerMovedToEnemyFirstTime == false)
                    {
                        _education.onMovedToEnemy.Invoke();
                       
                        _isPlayerMovedToEnemyFirstTime = true;
                    }
                }
               
            }
        }
        private void GlowEnemies(List<Vector3Int> glowers)
        {
            foreach(Vector3Int enemyToGlow in glowers)
            {
                hexGrid.GetTileAt(enemyToGlow).GetComponentInChildren<Renderer>().material = enemyMaterial;
            }
        }
        private void UnGlowEnemies(List<Vector3Int> unGlowers)
        {
            foreach (Vector3Int enemyToGlow in unGlowers)
            {
                hexGrid.GetTileAt(enemyToGlow).GetComponentInChildren<Renderer>().material = origMaterial;
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
            
        }

        private void MoveEnemyKing()
        {
            _enemyCoords = king.GetComponent<HexCoordinates>();
            _enemyCoords.SetCoords();
            hexGrid.GetTileAt(new Vector3Int(_enemyCoords.GetHexCoords().x,0,_enemyCoords.GetHexCoords().z)).SetType(HexType.Default);
            Vector3Int lastUnitCoords = new Vector3Int(_lastHex.HexCoords.x, 0, _lastHex.HexCoords.z);
            Vector3Int kingCoords = new Vector3Int(king.GetCoords().x, 0, king.GetCoords().z);
           
            foreach (Vector3Int direction in hexGrid.GetNeighboursFor(kingCoords))
            {
                if(direction != lastUnitCoords)
                {
                    
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
           
            int enemyQuantity = enemies.Count;
            if(enemyQuantity <= 0)
            {
                winingAction.Invoke();
                
                return;
            }
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
            Debug.Log(indexOfEnemy + " " + "1");
            _enemyCoords = enemies[enemyId].GetComponent<HexCoordinates>();
            _enemyCoords.SetCoords();
            
            Vector3Int enemyCoords = new Vector3Int(enemies[enemyId].GetCoords().x, 0, enemies[enemyId].GetCoords().z);
            List<Vector3Int> currentPath = new List<Vector3Int>();
            List<Vector3Int> pathToKing = new List<Vector3Int>();

            Unit weakestUnit = null;
            int indexOfWeakestUnit = 0;
            int currIndex = 0;
            int lowestHp = 100;
            foreach(UnitMovement unit in units)
            {
                Unit unitToCheck = unit.GetComponent<Unit>();
                if(unitToCheck.GetUnitHP() <= lowestHp)
                {
                    weakestUnit = unitToCheck;
                    lowestHp = unitToCheck.GetUnitHP();
                    indexOfWeakestUnit = currIndex;
                }
                currIndex++;
            }
            UnitMovement weakestUnitToMove = weakestUnit.GetComponent<UnitMovement>();
            Vector3Int unitKingPos = weakestUnitToMove._hexCoordinates.GetHexCoords();
           
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
                
                EnemyAttackTurn(enemyId,enemyCoords);
                return;
            }
            
            else if(currentPath.Count == 2)
            {
                for (int i = 0; i < 1; i++)
                {
                    pathToKing.Add(currentPath[i]);
                }
            }
            Debug.Log(indexOfEnemy + " " + "2");
            hexGrid.GetTileAt(new Vector3Int(_enemyCoords.GetHexCoords().x, 0, _enemyCoords.GetHexCoords().z)).SetType(HexType.Default);
            
            enemies[enemyId].MoveThroughPath(pathToKing.Select(pos =>
                hexGrid.GetTileAt(new Vector3Int(pathToKing[pathToKing.Count - 1].x, 0,
                    pathToKing[pathToKing.Count - 1].z)).transform.position).ToList());
            hexGrid.GetTileAt(pathToKing[pathToKing.Count - 1]).SetType(HexType.Enemy);
            
            EnemyAttackTurn(enemyId,pathToKing[pathToKing.Count - 1]);
            Debug.Log(indexOfEnemy + " " + "3");
        }

        private void EnemyAttackTurn(int enemyID, Vector3Int lastEnemyPos)
        {
            Vector3Int enemyCoords = new Vector3Int(enemies[enemyID].GetCoords().x, 0, enemies[enemyID].GetCoords().z);
            _bfs = GraphSearch.BFSGetRange(hexGrid, lastEnemyPos, 30);
           
            foreach (UnitMovement unit in units)
            { 
                Vector3Int lastUnitCoords = new Vector3Int(unit._hexCoordinates.GetHexCoords().x, 0,
                    unit._hexCoordinates.GetHexCoords().z);
               
            }
            if (enemies[enemyID].Type == UnitType.melee)
            {
                foreach (Vector3Int direction in hexGrid.GetNeighboursFor(lastEnemyPos) )
                {
                   
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
                                    
                                }
                            }

                            
                        }
                    }
                    
                }
               

                
            }
            Debug.Log(indexOfEnemy + " " + "4");
            if (enemies[enemyID].Type == UnitType.distant)
            {
                foreach (Vector3Int direction in hexGrid.GetNeighboursInRangeOf2(lastEnemyPos))
                {
                    //Debug.Log(direction);
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
                                    
                                }
                            }
                            
                        }
                    }
                    
                }
                
            }
            indexOfEnemy++;
            Debug.Log(indexOfEnemy + " " + "5");
            if (indexOfEnemy >= enemies.Count)
            {
                indexOfEnemy = 0;
                SelectUnit();
                return;
            }
            indexOfUnit = 0;
            
            Debug.Log("Enemy select 532" + " " + enemies[enemyID]);
            Debug.Log(indexOfEnemy + " " + "1");
            
            SelectEnemy();
        }

        private void EnemyAttack(int enemyID, Vector3Int lastEnemyPos)
        {
            //if (enemies[enemyID].)
        }
        

        private void BattleTurn(Hex selectedUnit)
        {
            //Debug.Log("battleTurn");
            if (_unit.GetUnitType() == UnitType.melee)
            {
                int kolvo = 0;
                
                foreach (Vector3Int direction in hexGrid.GetNeighboursFor(_lastHex.HexCoords))
                {
                    if (hexGrid.GetTileAt(direction).IsEnemy())
                    {
                        enemiesToGlow.Add(direction);
                        kolvo++;
                    }
                }
                if(kolvo == 0)
                {
                    if(indexOfUnit >= units.Count)
                    {
                        indexOfUnit = 0;
                        ClearOldSelection();
                        Debug.Log("Enemy select 635");
                        SelectEnemy();
                        return;
                    }
                    else
                    {
                        indexOfEnemy = 0;
                        ClearOldSelection();
                        SelectUnit();
                        return;
                    }
                   
                }
                else
                {
                    ClearOldSelection();
                    GlowEnemies(enemiesToGlow);
                    //Battling(selectedUnit);
                }
            }
           
            if (_unit.GetUnitType() == UnitType.distant)
            {
                int kolvo = 0;
                
                foreach (Vector3Int direction in hexGrid.GetNeighboursInRangeOf2(_lastHex.HexCoords))
                {
                    if (hexGrid.GetTileAt(direction).IsEnemy())
                    {
                        enemiesToGlow.Add(direction);
                        kolvo++;
                    }
                    
                }
                foreach (Vector3Int direction in hexGrid.GetNeighboursFor(_lastHex.HexCoords))
                {
                    if (hexGrid.GetTileAt(direction).IsEnemy())
                    {
                        enemiesToGlow.Add(direction);
                        kolvo++;
                    }

                }
                if (kolvo == 0)
                {
                    if (indexOfUnit >= units.Count)
                    {
                        indexOfUnit = 0;
                        ClearOldSelection();
                        Debug.Log("Enemy select 690");
                        SelectEnemy();
                        return;
                    }
                    else
                    {
                        indexOfEnemy = 0;
                        ClearOldSelection();
                        SelectUnit();
                        return;
                    }
                   
                }
                else
                {
                    ClearOldSelection();
                    GlowEnemies(enemiesToGlow);
                    //Battling(selectedUnit);
                }
            }
            
            _isBattling = true;
            
          
           
        }

        private void Battling(Hex hexToAttack)
        {
           
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
           
        }

        private void DistantAttack(Hex hexToAttack)
        {
            List<Vector3Int> neighbours = new List<Vector3Int>();
            
            foreach (Vector3Int direction in hexGrid.GetNeighboursInRangeOf2(_lastHex.HexCoords))
            {
                neighbours.Add(direction);
               
            }
            foreach (Vector3Int direction in hexGrid.GetNeighboursFor(_lastHex.HexCoords))
            {
                neighbours.Add(direction);
                
            }
            foreach (Vector3Int direction in neighbours)
            {
                Debug.Log(direction);
                if (hexGrid.GetTileAt(direction).IsEnemy() && hexGrid.GetTileAt(direction) == hexToAttack)
                {
                    _enemy.TakeDamage(_unit.GetUnitDamage());
                    Debug.Log($"Удар!");
                    
                    _unit = null;
                    UnGlowEnemies(enemiesToGlow);
                    enemiesToGlow.Clear();
                    
                    if (indexOfUnit >= units.Count)
                    {
                        //ClearOldSelection();
                        indexOfUnit = 0;
                        Debug.Log("Enemy select 801");
                        SelectEnemy();
                        return;
                    }
                    else
                    {
                        //ClearOldSelection();
                        indexOfEnemy = 0;
                        SelectUnit();
                        return;
                    }
                }
            }
        }

        private void MeleeAttack(Hex hexToAttack)
        {
            foreach (Vector3Int direction in hexGrid.GetNeighboursFor(_lastHex.HexCoords) )
            {
               
                if (hexGrid.GetTileAt(direction).IsEnemy() && hexGrid.GetTileAt(direction) == hexToAttack)
                {
                    _enemy.TakeDamage(_unit.GetUnitDamage());
                    Debug.Log($"Удар!");
                    
                    _unit = null;
                    UnGlowEnemies(enemiesToGlow);
                    enemiesToGlow.Clear();
                   
                    if(indexOfUnit >= units.Count)
                    {
                        //ClearOldSelection();
                        indexOfUnit = 0;
                        Debug.Log("Enemy select 834");
                        SelectEnemy();
                        return;
                    }
                    else
                    {
                        //ClearOldSelection();
                        indexOfEnemy = 0;
                        SelectUnit();
                        return;
                    }
                }
            }
        }
        private IEnumerator EnemyTurnCoroutine(float time)
        {
           
            yield return new WaitForSeconds(time);

            Debug.Log(indexOfEnemy);

        }
    }
}
