using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kraken : MonoBehaviour
{
    public Transform FrontTarget;
    public Transform BackTarget;
    public float speed = 1f; // public speed variable

    private Transform closestTarget;

    private void Start()
    {
        InvokeRepeating("UpdateClosestTarget", 0f, 1f);
    }

    private void UpdateClosestTarget()
    {
        closestTarget = GetClosestTarget(transform);
    }

    private void Update()
    {
        if (closestTarget != null)
        {
            MoveTowardsTarget(closestTarget);
        }
    }

    public Transform GetClosestTarget(Transform krakenPosition)
    {
        float distanceToFront = Vector3.Distance(krakenPosition.position, FrontTarget.position);
        float distanceToBack = Vector3.Distance(krakenPosition.position, BackTarget.position);

        if (distanceToFront < distanceToBack)
        {
            return FrontTarget;
        }
        else
        {
            return BackTarget;
        }
    }

    private void MoveTowardsTarget(Transform targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
    }
}