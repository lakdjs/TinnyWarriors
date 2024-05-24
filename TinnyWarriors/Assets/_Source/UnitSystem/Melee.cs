using HexSystem;
using UnityEngine;

namespace UnitSystem
{
    public class Melee : Unit
    {
        public override Vector3Int Attack(Hex hexToAttack)
        {
            Vector3Int hexCoords = hexCoordinates.GetHexCoords();
            foreach (Vector3Int direction in hexGrid.GetNeighboursFor(new Vector3Int(hexCoords.x,0,hexCoords.z) ))
            {
                if (hexGrid.GetTileAt(direction).IsEnemy() && hexGrid.GetTileAt(direction) == hexToAttack)
                {
                    Debug.Log($"Succesfull attack on{hexToAttack.HexCoords}");
                   return hexToAttack.HexCoords;
                }
            }

            return new Vector3Int(10000,10000,10000);
        }
    }
}
