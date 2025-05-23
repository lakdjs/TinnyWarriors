using Unity.VisualScripting;
using UnityEngine;

namespace CameraSystem
{
    public class CameraRotation : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset;
        [SerializeField] float sensitivity = 3; // чувствительность мышки
        [SerializeField] float limit = 80; // ограничение вращения по Y
        [SerializeField] float zoom = 0.25f; // чувствительность при увеличении, колесиком мышки
        [SerializeField] float zoomMax = 10; // макс. увеличение
        [SerializeField] float zoomMin = 3; // мин. увеличение
        [SerializeField] float X, Y;

        private Education _education;
        private bool _isPkmPressed = false;
        public void DeleteEducation()
        {
            _education = null;
        }
        void Start () 
        {
            limit = Mathf.Abs(limit);
            if(limit > 90) limit = 90;
            offset = new Vector3(offset.x, offset.y, -Mathf.Abs(zoomMax)/2);
            transform.position = target.position + offset;
        }
        public void Construct(Education education)
        {
            _education = education;
        }

        void Update ()
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {

                if(_education != null)
                {
                    if(_isPkmPressed == false)
                    {
                        
                            _education.onPKMtapped.Invoke();
                            Debug.Log("Pkm pressed");
                            _isPkmPressed = true;
                        
                        
                    }
                }

                if (Input.GetAxis("Mouse ScrollWheel") > 0) offset.z += zoom;
                else if (Input.GetAxis("Mouse ScrollWheel") < 0) offset.z -= zoom;
                offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));
                X = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
                Y += Input.GetAxis("Mouse Y") * sensitivity;
                Y = Mathf.Clamp (Y, -limit, 0);
                transform.localEulerAngles = new Vector3(-Y, X, 0);
                transform.position = transform.localRotation * offset + target.position;
            }         
        }
    }
}
