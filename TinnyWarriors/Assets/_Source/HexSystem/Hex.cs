using System;
using UnityEngine;

namespace HexSystem
{
    [SelectionBase]
    public class Hex : MonoBehaviour
    {
        [SerializeField] private GlowHighlight highlight;
        public HexCoordinates _hexCoordinates { get; private set; }
        private GameObject _building;

        [SerializeField] private HexType hexType;

        public Vector3Int HexCoords => _hexCoordinates.GetHexCoords();

        public int GetCost()
            => hexType switch
            {
                HexType.Difficult => 20,
                HexType.Default => 10,
                HexType.Road => 5,
                HexType.Unit => 1000,
                HexType.Enemy => 1000,
                _ => throw new Exception($"Hex of type {hexType} not supported")
            };
        
        public void SetEnemy()
        {
            
        }
        public void SetType(HexType newHexType)
        {
            hexType = newHexType;
        }
        public bool IsObstacle()
        {
            return this.hexType == HexType.Obstacle;
        }

        public bool IsEnemy()
        {
            return this.hexType == HexType.Enemy;
        }

        public bool IsUnit()
        {
            return this.hexType == HexType.Enemy;
        }

        public void SetUp()
        {
            _hexCoordinates = GetComponent<HexCoordinates>();
            highlight = GetComponent<GlowHighlight>();
        }
        private void Awake()
        {
            
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
        Enemy,
        Water,
        Unit,
        Obstacle
    }
}