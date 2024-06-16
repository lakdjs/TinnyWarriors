using UnityEngine;
using UnityEngine.Events;

namespace HexSystem
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        public LayerMask selectionMask;

        //TODO поменять на явную подписку!!!!!!
        public UnityEvent<GameObject> OnUnitSelected;
        public UnityEvent<GameObject> TerrainSelected;
        public UnityEvent<GameObject> OnEnemySelected;

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        public void HandleClick(Vector3 mousePosition)
        {
            GameObject result;
            if (FindTarget(mousePosition, out result))
            {
                if (UnitSelected(result))
                {
                    OnUnitSelected?.Invoke(result);
                    Debug.Log("unit selected!");
                }
                if (EnemySelected(result))
                {
                    OnEnemySelected?.Invoke(result);
                    Debug.Log("enemy selected!");
                }
                else if(IsTerrainSelected(result))
                {
                    TerrainSelected?.Invoke(result);
                    Debug.Log("terrain selected!");
                }
            }
        }

        private bool UnitSelected(GameObject result)
        {
            return result.GetComponent<UnitMovement>() != null;
        }

        private bool EnemySelected(GameObject result)
        {
            return result.GetComponent<EnemyMovement>() != null;
        }
        private bool IsTerrainSelected(GameObject result)
        {
            return result.GetComponent<Hex>() != null;
        }

        private bool FindTarget(Vector3 mousePosition, out GameObject result)
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out hit, 100, selectionMask))
            {
                result = hit.collider.gameObject;
                return true;
            }
            result = null;
            return false;
        }
    }
}