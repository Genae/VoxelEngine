using UnityEngine;

namespace Assets.Scripts.Control
{
    public class CameraMoveScript : MonoBehaviour 
    {
        protected float LookSensitivity = 3;



        

        public CameraController Controller;
        
        protected void Update()
        {
            /*
            if (Input.GetKey(KeyCode.R) || Input.GetMouseButton(2))
            {
                YRotation += (Input.GetAxis("Mouse X") * LookSensitivity);
                //_xRotation += (Input.GetAxis("Mouse Y") * _lookSensitivity);
            }
            

            */
        }
    }
}

