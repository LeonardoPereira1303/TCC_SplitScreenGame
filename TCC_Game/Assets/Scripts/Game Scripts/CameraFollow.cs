using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Parâmetros")]

    [Tooltip("GameObject que terá a camera seguindo")] 
        public Transform target = null;
    [SerializeField, Range (0.01f, 1f), Tooltip("Velocidade da Camera")] 
        private float smoothSpeed = 0.125f;
    [SerializeField, Tooltip("Distânica da Camera")] 
        private Vector3 offset = new Vector3(0f, 2.25f, -1.5f);

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

    }

    public void CenterOnTarget()
    {
        transform.position = target.position + offset;
    }
}
