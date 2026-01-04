using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [SerializeField] private List<Transform> taskLocations;
    [SerializeField] private float waitTimeAtTask = 5f;

    public bool isKilled = false;

    private NavMeshAgent agent;
    private int currentTaskIndex;
    private bool isDeadHandled = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = true;

        if (taskLocations.Count > 0)
        {
            GoToRandomTask();
        }
        else
        {
            Debug.LogWarning("La liste des tâches est vide sur " + gameObject.name);
        }
    }

    void Update()
    {
        if (isKilled)
        {
            HandleDeath();
            return; 
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {

            if (!IsInvoking("GoToRandomTask"))
            {
               
                Invoke("GoToRandomTask", waitTimeAtTask);
            }
        }
    }

    void HandleDeath()
    {
        if (!isDeadHandled)
        {
          
            if (agent.enabled)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }


            CancelInvoke("GoToRandomTask");


            transform.rotation = Quaternion.Euler(0, 0, 90f);

 
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);

            isDeadHandled = true;
            Debug.Log(gameObject.name + " a été éliminé.");
        }
    }

    void GoToRandomTask()
    {
        if (taskLocations.Count == 0) return;


        int randomIndex = Random.Range(0, taskLocations.Count);


        if (taskLocations.Count > 1 && randomIndex == currentTaskIndex)
        {
            randomIndex = (randomIndex + 1) % taskLocations.Count;
        }

        currentTaskIndex = randomIndex;

        
        agent.SetDestination(taskLocations[currentTaskIndex].position);
    }
}
