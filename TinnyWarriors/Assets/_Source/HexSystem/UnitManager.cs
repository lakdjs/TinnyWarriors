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
    
    
        public bool PlayersTurn { get; private set; } = true;

        [SerializeField]
        private Unit selectedUnit;
        private Hex previouslySelectedHex;

        private void Awake()
        {
            buildingButton.onClick.AddListener(Build);
            removingButton.onClick.AddListener(Remove);
            PlayerSelected = false;
            _isBuilding = false;
            _isRemoving = false;    
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
                return;
            PlayerSelected = true;
            Unit unitReference = unit.GetComponent<Unit>();

            if (CheckIfTheSameUnitSelected(unitReference))
                return;

            PrepareUnitForMovement(unitReference);
        }

        private bool CheckIfTheSameUnitSelected(Unit unitReference)
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
        
            if (selectedUnit == null || PlayersTurn == false)
            {
                return;
            }
            Hex selectedHex = hexGO.GetComponent<Hex>();
        

            if (HandleHexOutOfRange(selectedHex.HexCoords) || HandleSelectedHexIsUnitHex(selectedHex.HexCoords))
                return;

            HandleTargetHexSelected(selectedHex);

        }

        private void PrepareUnitForMovement(Unit unitReference)
        {
            if (this.selectedUnit != null)
            {
                ClearOldSelection();
            }

            this.selectedUnit = unitReference;
            this.selectedUnit.Select();
            movementSystem.ShowRange(this.selectedUnit, this.hexGrid);
        }

        private void ClearOldSelection()
        {
            previouslySelectedHex = null;
            this.selectedUnit.Deselect();
            movementSystem.HideRange(this.hexGrid);
            this.selectedUnit = null;

        }

        private void HandleTargetHexSelected(Hex selectedHex)
        {
            if (previouslySelectedHex == null || previouslySelectedHex != selectedHex)
            {
                previouslySelectedHex = selectedHex;
                movementSystem.ShowPath(selectedHex.HexCoords, this.hexGrid);
            }
            else
            {
                movementSystem.MoveUnit(selectedUnit, this.hexGrid);
                PlayersTurn = false;
                selectedUnit.MovementFinished += ResetTurn;
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

        private void ResetTurn(Unit selectedUnit)
        {
            selectedUnit.MovementFinished -= ResetTurn;
            PlayersTurn = true;
        }
    }
}
