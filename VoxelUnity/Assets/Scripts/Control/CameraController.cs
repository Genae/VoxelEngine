﻿using System;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class CameraController : MonoBehaviour
    {
        public Camera Eye;
        
        public const float MouseBoundary = 30f;

        //position limit
        public float LeftLimit = 0;
        public float RightLimit = 100;
        public float TopLimit = 100;
        public float BottomLimit = 0;
        public float CameraMaxHeight = 100;
        public float CameraMinHeight = 50;

        //rotation Limit
        public float MaxCameraTiltAngle = 50;
        public float MinCameraTiltAngle = 70;

        //speed
        public float ZoomStep = 30;
        public float CameraMoveSpeed = 50f;
        public float CameraRotationSpeed = 1;
        private const float LookSmothDamp = 0.2f;
        public float CameraTiltSpeed = 1;

        //internal values
        private float _desiredYRotation;
        private float _desiredXRotation;
        private float _currentYRotation;
        private float _currentXRotation;
        private float _yRotationV;
        private float _xRotationV;


        protected void Update()
        {
            RaycastHit hit;
            //geting the height
            var ray = new Ray(transform.position, Vector3.down);
            Physics.Raycast(ray, out hit, float.PositiveInfinity);
            if (hit.collider != null)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - hit.distance + CameraMinHeight + (CameraMaxHeight - CameraMinHeight)/2, transform.position.z);
            }

            //transform.rotation = Quaternion.Euler(0, CameraMove.CurrentYRotation, 0);
            TranslateCamera();
            RotateCamera();
            ZoomCamera();
        }

        public void TranslateCamera()
        {
            var moveSpeed = CameraMoveSpeed * Time.deltaTime;
            var desiredZ = 0f;
            var desiredX = 0f;

            if (AreCameraKeyoardButtonsPressed())
            {
                if (Input.GetKey(KeyCode.W))
                    desiredZ = moveSpeed;
                if (Input.GetKey(KeyCode.S))
                    desiredZ = moveSpeed * -1;
                if (Input.GetKey(KeyCode.A))
                    desiredX = moveSpeed * -1;
                if (Input.GetKey(KeyCode.D))
                    desiredX = moveSpeed;
            }
            else
            {
                if (Input.mousePosition.y > (Screen.height - MouseBoundary))
                    desiredZ = moveSpeed;
                if (Input.mousePosition.y < MouseBoundary)
                    desiredZ = moveSpeed * -1;
                if (Input.mousePosition.x < MouseBoundary)
                    desiredX = moveSpeed * -1;
                if (Input.mousePosition.x > (Screen.width - MouseBoundary))
                    desiredX = moveSpeed;
            }
            var dTrans = new Vector3(desiredX, 0, desiredZ);

            if (!IsDesiredPositionOverBoundaries(dTrans))
                transform.Translate(dTrans);
        }
        
        private void RotateCamera()
        {
            if (Input.GetKey(KeyCode.E))
            {
                _desiredYRotation += CameraRotationSpeed;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                _desiredYRotation -= CameraRotationSpeed;
            }
            if (Input.GetKey(KeyCode.X))
            {
                _desiredXRotation += CameraTiltSpeed;
            }
            if (Input.GetKey(KeyCode.Y))
            {
                _desiredXRotation -= CameraTiltSpeed;
            }

            _desiredXRotation = Mathf.Clamp(_desiredXRotation, MinCameraTiltAngle, MaxCameraTiltAngle);
            _currentXRotation = Mathf.SmoothDamp(_currentXRotation, _desiredXRotation, ref _xRotationV, LookSmothDamp);
            _currentYRotation = Mathf.SmoothDamp(_currentYRotation, _desiredYRotation, ref _yRotationV, LookSmothDamp);

            transform.rotation = Quaternion.Euler(0, _currentYRotation, 0);
            Eye.transform.localRotation = Quaternion.Euler(_currentXRotation, 0, 0);
        }

        private void ZoomCamera()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                var doZoom = Input.GetAxis("Mouse ScrollWheel") * ZoomStep;
                if (Eye.transform.position.y - doZoom > CameraMaxHeight  ||
                    Eye.transform.position.y - doZoom < CameraMinHeight) return;
                var zoomVector = new Vector3(0, 0, 1) * doZoom;//transform.forward.normalized*doZoom;
                gameObject.transform.Translate(zoomVector, Space.Self);
                Eye.transform.position = new Vector3(Eye.transform.position.x, Eye.transform.position.y - doZoom, Eye.transform.position.z);
            }
        }

        public bool IsDesiredPositionOverBoundaries(Vector3 desiredTranlation)
        {
            return (transform.position.x + desiredTranlation.x) < LeftLimit ||
                   (transform.position.x + desiredTranlation.x) > RightLimit ||
                   (transform.position.z + desiredTranlation.z) > TopLimit ||
                   (transform.position.z + desiredTranlation.z) < BottomLimit;
        }

        public static bool AreCameraKeyoardButtonsPressed()
        {
            return (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));
        }
    }
}