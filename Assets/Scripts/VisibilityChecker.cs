using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{

    private Transform playerTransform;
    private MeshRenderer[] renderers; 
    public float visibilityRange = 10f;

    void Start()
    {
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        
        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        
        bool shouldBeVisible = distance <= visibilityRange;

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.enabled != shouldBeVisible)
            {
                renderer.enabled = shouldBeVisible;
            }
        }
    }
}
