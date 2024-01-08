using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
    {
    public Transform carTransform;
    [Range(1, 10)]
    public float followSpeed = 2;
    [Range(1, 10)]
    public float lookSpeed = 5;
    Vector3 initialCameraOffset; // Offset of the camera from the car
    Vector3 cameraPositionOffset; // Adjusted position of the camera

    public float distanceBehind = 5f; // Distance behind the car
    public float heightAbove = 5f; // Height above the car

    void Start()
        {
        if (carTransform != null)
            {
            // Calculate the initial offset of the camera from the car
            initialCameraOffset = new Vector3(0, heightAbove, -distanceBehind);
            cameraPositionOffset = carTransform.TransformPoint(initialCameraOffset);
            }
        }

    void FixedUpdate()
        {
        if (carTransform != null)
            {
            // Update the camera's position
            cameraPositionOffset = carTransform.TransformPoint(initialCameraOffset);
            transform.position = Vector3.Lerp(transform.position, cameraPositionOffset, followSpeed * Time.deltaTime);

            // Look at the car
            Quaternion targetRotation = Quaternion.LookRotation(carTransform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
            }
        }
    }
