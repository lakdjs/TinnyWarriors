using System;
using UnityEngine;

namespace HexSystem
{
    [SelectionBase]
    public class Hex : MonoBehaviour
    {
        [SerializeField] private GlowHighlight highlight;
        private HexCoordinates _hexCoordinates;
        private GameObject _building;

        [SerializeField] private HexType hexType;

        public Vector3Int HexCoords => _hexCoordinates.GetHexCoords();

        public int GetCost()
            => hexType switch
            {
                HexType.Difficult => 20,
                HexType.Default => 10,
                HexType.Road => 5,
                _ => throw new Exception($"Hex of type {hexType} not supported")
            };

        public void SetBuilding(GameObject building)
        {
            if (_building != null)
            {
                return;
            }
            _building = building;
            SetType(HexType.Obstacle);
        }

        public void ResetBuilding()
        {
            if (_building == null)
            {
                return;
            }
            Destroy(_building);
            _building = null;
            SetType(HexType.Default);
        }
        public void SetType(HexType newHexType)
        {
            hexType = newHexType;
        }
        public bool IsObstacle()
        {
            return this.hexType == HexType.Obstacle;
        }

        private void Awake()
        {
            _hexCoordinates = GetComponent<HexCoordinates>();
            highlight = GetComponent<GlowHighlight>();
        }
        public void EnableHighlight()
        {
            highlight.ToggleGlow(true);
        }

        public void DisableHighlight()
        {
            highlight.ToggleGlow(false);
        }

        internal void ResetHighlight()
        {
            highlight.ResetGlowHighlight();
        }

        internal void HighlightPath()
        {
            highlight.HighlightValidPath();
        }
    }

    public enum HexType
    {
        None,
        Default,
        Difficult,
        Road,
        Water,
        Obstacle
    }
}