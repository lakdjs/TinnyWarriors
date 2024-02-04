using UnityEngine;

namespace HexSystem
{
    public class HexCoordinates : MonoBehaviour
    {
        private const float XOffset = 2;
        private const float YOffset = 1;
        private const float ZOffset = 1.73f;

        public Vector3Int GetHexCoords()
            => offsetCoordinates;

        [Header("Offset coordinates")]
        [SerializeField] private Vector3Int offsetCoordinates;

        private void Awake()
        {
            offsetCoordinates = ConvertPositionToOffset(transform.position);
        }

        public static Vector3Int ConvertPositionToOffset(Vector3 position)
        {
            int x = Mathf.CeilToInt(position.x / XOffset);
            int y = Mathf.RoundToInt(position.y / YOffset);
            int z = Mathf.RoundToInt(position.z / ZOffset);
            return new Vector3Int(x, y, z);
        }
    }
}