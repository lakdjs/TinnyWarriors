using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using CameraSystem;
using HexSystem;
using UnitSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    public class Bootstrapper: MonoBehaviour
    {
        [SerializeField] private List<UnitMovement>  units;
        [SerializeField] private List<EnemyMovement>  enemies;
        [SerializeField] private HexGrid hexGrid;
        [SerializeField] private CameraRotation cameraRotation;
        [SerializeField] private Education education;
        [SerializeField] private UnitManager unitManager;
        [SerializeField] private UnitSystem.Unit unit;

        private void Awake()
        {
            SetUp();
        }

        private void SetUp()
        {
            foreach (Hex hex in FindObjectsOfType<Hex>())//Поменяю!!!
            {
                hex.SetUp();
                hex._hexCoordinates.SetCoords();
                hexGrid._hexTileDict[hex.HexCoords] = hex;
            }
            foreach (UnitMovement unit in units)
            {
                unit.SetUp();
                unit._hexCoordinates.SetCoords();
                Vector3Int coords = new Vector3Int(unit._hexCoordinates.GetHexCoords().x,0,unit._hexCoordinates.GetHexCoords().z);
                hexGrid.GetTileAt(coords).SetType(HexType.Unit);
            }
            foreach (EnemyMovement enemy in enemies)
            {
                enemy.SetUp();
                enemy._hexCoordinates.SetCoords();
                Vector3Int coords = new Vector3Int(enemy._hexCoordinates.GetHexCoords().x,0,enemy._hexCoordinates.GetHexCoords().z);
                hexGrid.GetTileAt(coords).SetType(HexType.Enemy);
            }
            if(education != null)
            {
                cameraRotation.Construct(education);
                unitManager.Construct(education);
                unit.Construct(education);
            }
            
        }
    }
}