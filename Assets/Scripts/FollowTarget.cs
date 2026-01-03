using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector3 _currentVelocity;

    protected void Update()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            target.position,
            ref _currentVelocity,
            0.2f);
    }
}
