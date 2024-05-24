using System.Collections.Generic;
using UnityEngine;

namespace HexSystem
{
    public class HexGrid : MonoBehaviour
    {
        private Dictionary<Vector3Int, Hex> _hexTileDict = new Dictionary<Vector3Int, Hex>();
        private Dictionary<Vector3Int, List<Vector3Int>> _hexTileNeighboursDict = new Dictionary<Vector3Int, List<Vector3Int>>();
        private Dictionary<Vector3Int, List<Vector3Int>> _hexTileNeighboursDictInRangeOf2 = new Dictionary<Vector3Int, List<Vector3Int>>();

        private void Awake()
        {
            foreach (Hex hex in FindObjectsOfType<Hex>())//Поменяю!!!
            {
                _hexTileDict[hex.HexCoords] = hex;
            }
        }

        public Hex GetTileAt(Vector3Int hexCoordinates)
        {
            Hex result = null;
            _hexTileDict.TryGetValue(hexCoordinates, out result);
            return result;
        }

        public List<Vector3Int> GetNeighboursFor(Vector3Int hexCoordinates)
        {
            if (_hexTileDict.ContainsKey(hexCoordinates) == false)
                return new List<Vector3Int>();

            if (_hexTileNeighboursDict.ContainsKey(hexCoordinates))
            {
                return _hexTileNeighboursDict[hexCoordinates];
            }

            _hexTileNeighboursDict.Add(hexCoordinates, new List<Vector3Int>());

            foreach (Vector3Int direction in Direction.GetDirectionList(hexCoordinates.z))
            {
                if (_hexTileDict.ContainsKey(hexCoordinates + direction))
                {
                    _hexTileNeighboursDict[hexCoordinates].Add(hexCoordinates + direction);
                }
            }
            return _hexTileNeighboursDict[hexCoordinates];
        }
        public List<Vector3Int> GetNeighboursInRangeOf2(Vector3Int hexCoordinates)
        {
            if (_hexTileDict.ContainsKey(hexCoordinates) == false)
                return new List<Vector3Int>();

            if (_hexTileNeighboursDictInRangeOf2.ContainsKey(hexCoordinates))
            {
                return _hexTileNeighboursDictInRangeOf2[hexCoordinates];
            }

            _hexTileNeighboursDictInRangeOf2.Add(hexCoordinates, new List<Vector3Int>());

            foreach (Vector3Int direction in Direction.GetDirListInRangeOf2(hexCoordinates.z))
            {
                if (_hexTileDict.ContainsKey(hexCoordinates + direction))
                {
                    _hexTileNeighboursDictInRangeOf2[hexCoordinates].Add(hexCoordinates + direction);
                }
            }
            return _hexTileNeighboursDictInRangeOf2[hexCoordinates];
        }
        public Vector3Int GetClosestHex(Vector3 worldposition)
        {
            worldposition.y = 0;
            return HexCoordinates.ConvertPositionToOffset(worldposition);
        }
    }

    public static class Direction
    {
        public static List<Vector3Int> directionsOffsetOdd = new List<Vector3Int>
        {
            new Vector3Int(-1,0,1), //N1
            new Vector3Int(0,0,1), //N2
            new Vector3Int(1,0,0), //E
            new Vector3Int(0,0,-1), //S2
            new Vector3Int(-1,0,-1), //S1
            new Vector3Int(-1,0,0), //W
        };

        public static List<Vector3Int> directionsOffsetEven = new List<Vector3Int>
        {
            new Vector3Int(0,0,1), //N1
            new Vector3Int(1,0,1), //N2
            new Vector3Int(1,0,0), //E
            new Vector3Int(1,0,-1), //S2
            new Vector3Int(0,0,-1), //S1
            new Vector3Int(-1,0,0), //W
        };

        public static List<Vector3Int> directionsOffsetOddInRangeOf2 = new List<Vector3Int>
        {
            new Vector3Int(-2,0,-1),
            new Vector3Int(-1,0,-2),
            new Vector3Int(0,0,-2),
            new Vector3Int(1,0,-2),
            new Vector3Int(-1,0,-1),
            new Vector3Int(-2,0,0),
            new Vector3Int(1,0,1),
            new Vector3Int(1,0,2),
            new Vector3Int(0,0,1),
            new Vector3Int(-1,0,2),
            new Vector3Int(-2,0,1),
            new Vector3Int(-2,0,0)
        };
        public static List<Vector3Int> directionsOffsetEvenInRangeOf2 = new List<Vector3Int>
        {
            new Vector3Int(-1,0,-1),
            new Vector3Int(-1,0,-2),
            new Vector3Int(0,0,-2),
            new Vector3Int(1,0,-2),
            new Vector3Int(2,0,-1),
            new Vector3Int(2,0,0),
            new Vector3Int(2,0,1),
            new Vector3Int(1,0,2),
            new Vector3Int(0,0,2),
            new Vector3Int(-1,0,2),
            new Vector3Int(-1,0,1),
            new Vector3Int(-2,0,0)
        };

        public static bool IsOdd(int z) => z % 2 == 0 ? true : false;
        public static List<Vector3Int> GetDirectionList(int z)
            => z % 2 == 0 ? directionsOffsetEven : directionsOffsetOdd;

        public static List<Vector3Int> GetDirListInRangeOf2(int z)
            => z % 2 == 0 ? directionsOffsetEvenInRangeOf2 : directionsOffsetOddInRangeOf2;
    }
}