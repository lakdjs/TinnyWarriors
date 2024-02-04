using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HexSystem
{
    public class PlayerInput : MonoBehaviour
    {
        //TODO Поменяю на явную подписку
        public UnityEvent<Vector3> pointerClick;

        void Update()
        {
            DetectMouseClick();
        }

        private void DetectMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Input.mousePosition;
                pointerClick?.Invoke(mousePos);
            }
        }
    }
}