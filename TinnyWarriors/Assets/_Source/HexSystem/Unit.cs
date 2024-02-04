using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexSystem
{
    [SelectionBase]
    public class Unit : MonoBehaviour
    {
        [SerializeField] private int movementPoints = 5;
        public int MovementPoints { get => movementPoints; }

        [SerializeField] private float movementDuration = 1, rotationDuration = 0.3f;

        private GlowHighlight _glowHighlight;
        private Queue<Vector3> _pathPositions = new Queue<Vector3>();

        public event Action<Unit> MovementFinished;

        private void Awake()
        {
            _glowHighlight = GetComponent<GlowHighlight>();
        }

        public void Deselect()
        {
            _glowHighlight.ToggleGlow(false);
        }

        public void Select()
        {
            _glowHighlight.ToggleGlow();
        }

        public void MoveThroughPath(List<Vector3> currentPath)
        {
            _pathPositions = new Queue<Vector3>(currentPath);
            Vector3 firstTarget = _pathPositions.Dequeue();
            StartCoroutine(RotationCoroutine(firstTarget, rotationDuration));
        }
        //TODO изменить движение и ротацию на безкорутинную!!!
        private IEnumerator RotationCoroutine(Vector3 endPosition, float rotationDuration)
        {
            Quaternion startRotation = transform.rotation;
            endPosition.y = transform.position.y;
            Vector3 direction = endPosition - transform.position;
            Quaternion endRotation = Quaternion.LookRotation(direction, Vector3.up);

            if (Mathf.Approximately(Mathf.Abs(Quaternion.Dot(startRotation, endRotation)), 1.0f) == false)
            {
                float timeElapsed = 0;
                while (timeElapsed < rotationDuration)
                {
                    timeElapsed += Time.deltaTime;
                    float lerpStep = timeElapsed / rotationDuration; // 0-1
                    transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpStep);
                    yield return null;
                }
                transform.rotation = endRotation;
            }
            StartCoroutine(MovementCoroutine(endPosition));
        }

        private IEnumerator MovementCoroutine(Vector3 endPosition)
        {
            Vector3 startPosition = transform.position;
            endPosition.y = startPosition.y;
            float timeElapsed = 0;

            while (timeElapsed < movementDuration)
            {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / movementDuration;
                transform.position = Vector3.Lerp(startPosition, endPosition, lerpStep);
                yield return null;
            }
            transform.position = endPosition;

            if (_pathPositions.Count > 0)
            {
                Debug.Log("Selecting the next position!");
                StartCoroutine(RotationCoroutine(_pathPositions.Dequeue(), rotationDuration));
            }
            else
            {
                Debug.Log("Movement finished!");
                MovementFinished?.Invoke(this);
            }
        }
    }
}
