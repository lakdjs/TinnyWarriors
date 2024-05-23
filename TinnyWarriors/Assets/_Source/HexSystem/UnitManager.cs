using UnitSystem;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace HexSystem
{
    public class UnitManager : MonoBehaviour
    {
        [SerializeField]
        private HexGrid hexGrid;

        [SerializeField]
        private MovementSystem movementSystem;

        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private Button buildingButton;
        [SerializeField] private Button removingButton;
        
        
        
            //TODO переписать полностью! постройку в другой класс!
        public bool PlayerSelected { get; private set; }
        private bool _isBuilding;
        private bool _isRemoving;
        private bool _isBattling;
    
    
        public bool PlayersTurn { get; private set; } = true;
        public bool EnemiesTurn { get; private set; } = false;

        [SerializeField]
        private UnitMovement selectedUnit;

        private UnitMovement _unitToAttack;
        private HexCoordinates _selectedUnitCoords;
        
        private UnitMovement _selectedEnemy;
        
        
        private Hex _previouslySelectedHex;

        private Hex _lastHex;

        private void Awake()
        {
            buildingButton.onClick.AddListener(Build);
            removingButton.onClick.AddListener(Remove);
            PlayerSelected = false;
            _isBuilding = false;
            _isRemoving = false;
            _isBattling = false;
        }

        private void Build()
        {
            _isBuilding = true;
        }
        private void Remove()
        {
            _isRemoving = true;
        }

        public void HandleUnitSelected(GameObject unit)
        {
            if (PlayersTurn == false)
            {
                if (_isBattling)
                {
                    
                }
                return;
            }
                
            PlayerSelected = true;
            UnitMovement unitReference = unit.GetComponent<UnitMovement>();
            //Unit unitInfo = unit.GetComponent<Unit>();

            if (CheckIfTheSameUnitSelected(unitReference))
                return;

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
            if (_isBuilding)
            {
                Hex selectedHexToBuild = hexGO.GetComponent<Hex>();
                if (Direction.IsOdd(selectedHexToBuild.HexCoords.z))
                {
                    selectedHexToBuild.SetBuilding(Instantiate(buildingPrefab, new Vector3((selectedHexToBuild.HexCoords.x) * 2, 2,
                        selectedHexToBuild.HexCoords.z * 1.73f), Quaternion.identity));
                }
                else
                {
                    selectedHexToBuild.SetBuilding(Instantiate(buildingPrefab, new Vector3(((selectedHexToBuild.HexCoords.x-1) * 2)+1, 2,
                        selectedHexToBuild.HexCoords.z * 1.73f), Quaternion.identity));
                }
                _isBuilding = false;
                selectedHexToBuild.SetType(HexType.Obstacle);
                return;
            }

            if (_isRemoving)
            {
                Hex selectedHexToBuild = hexGO.GetComponent<Hex>();
                if (selectedHexToBuild.IsObstacle())
                {
                    selectedHexToBuild.ResetBuilding();
                }

                _isRemoving = false;
                return;
            }

            if (_isBattling)
            {
                Hex selectedHexToAttack = hexGO.GetComponent<Hex>();
                if (selectedHexToAttack.IsEnemy())
                {
                    Battling(selectedHexToAttack);
                    
                }
            }
        
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

        void EnemyTurn()
        {
            
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
            _isBattling = true;
            
           //Debug.Log("battle");
           
        }

        private void Battling(Hex hexToAttack)
        {
            foreach (Vector3Int direction in hexGrid.GetNeighboursFor(_lastHex.HexCoords) )
            {
                if (hexGrid.GetTileAt(direction).IsEnemy() && hexGrid.GetTileAt(direction) == hexToAttack)
                {
                    Debug.Log($"{_unitToAttack}Attacks ENEMY on: {hexToAttack.HexCoords}");
                   // Debug.Log(direction);
                   PlayersTurn = true;
                   _isBattling = false;
                }
            }
        }
    }
}
