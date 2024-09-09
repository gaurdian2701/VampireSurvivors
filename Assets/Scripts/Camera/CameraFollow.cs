using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float camSmoothness;

    private Transform playerTransform;
    private Vector3 zOffset = new Vector3(0f, 0f, -1f);

    private void Awake()
    {
        playerTransform = playerController.transform;
    }

    private void Update()
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, playerTransform.position + zOffset, camSmoothness);
    }

}
