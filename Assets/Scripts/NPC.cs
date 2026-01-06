using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [SerializeField] private List<Transform> taskLocations;
    [SerializeField] private float waitTimeAtTask = 5f;

    public bool isImpostor = false;
    public bool isKilled = false;
    [SerializeField] private float killRange = 5f;
    private float detectionRange = 10f; //à mettre à jour si la taille de la vision du joueur change
    [SerializeField] private float chaseSpeed = 15f;
    [SerializeField] private float normalSpeed = 10f;

    private float killCooldownTimer = 15f; 
    private float nextKillDelay = 30f;    
    private NPC targetVictim;

    private NavMeshAgent agent;
    private Transform playerTransform;
    private Light playerLight;
    private int currentTaskIndex;
    private bool isDeadHandled = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        normalSpeed = agent.speed;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerLight = player.GetComponentInChildren<Light>();
        }

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
        // pour checker la distribution des impostors
        if (isImpostor)
        {
            Transform cubeLunettes = transform.GetChild(1);
            Renderer cubeRenderer = cubeLunettes.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                cubeRenderer.material.color = Color.red;
            }
        }

        if (isKilled)
        {
            HandleDeath();
            return; 
        }

        if (killCooldownTimer > 0) killCooldownTimer -= Time.deltaTime;

        if (isImpostor)
        {
            HandleImpostorAI();
        }
        else
        {
            HandleCrewmateAI();
        }
    }

    void HandleImpostorAI()
    {
        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        
        if (distToPlayer <= detectionRange)
        {
            targetVictim = null; 
            agent.speed = normalSpeed;
            HandleCrewmateAI();
            return;
        }

       
        if (killCooldownTimer <= 0)
        {
            FindOrChaseVictim();
        }
        else
        {
            
            HandleCrewmateAI();
        }
    }

    void FindOrChaseVictim()
    {
       
        if (targetVictim == null || targetVictim.isKilled)
        {
            targetVictim = FindClosestVulnerableNPC();
        }

        if (targetVictim != null)
        {
           
            agent.speed = chaseSpeed;
            agent.SetDestination(targetVictim.transform.position);

            
            float distToVictim = Vector3.Distance(transform.position, targetVictim.transform.position);
            if (distToVictim <= killRange)
            {
                
                float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                if (distToPlayer > detectionRange)
                {
                    ExecuteKill(targetVictim);
                }
            }
        }
    }

    NPC FindClosestVulnerableNPC()
    {
        NPC[] allNPCs = FindObjectsOfType<NPC>();
        NPC closest = null;
        float minDist = Mathf.Infinity;

        foreach (NPC npc in allNPCs)
        {
            if (npc == this || npc.isImpostor || npc.isKilled) continue;

            float d = Vector3.Distance(transform.position, npc.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = npc;
            }
        }
        return closest;
    }

    void ExecuteKill(NPC victim)
    {
        victim.isKilled = true;
        killCooldownTimer = nextKillDelay; 
        targetVictim = null;
        agent.speed = normalSpeed;

        Debug.Log("<color=red>CRIME : </color>" + name + " a éliminé " + victim.name);

        
        GoToRandomTask();
    }

    void HandleCrewmateAI()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!IsInvoking("GoToRandomTask"))
                Invoke("GoToRandomTask", waitTimeAtTask);
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

    public void GoToRandomTask()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        // SÉCURITÉ : Si l'agent n'est pas actif, on ne fait rien
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }

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
