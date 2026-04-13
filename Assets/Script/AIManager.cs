using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

// No es crear IA si no usar lo que ya tiene Unity
public class AIManager : MonoBehaviour
{
    [SerializeField]
    public GameObject player;
    [SerializeField]
    public Transform entrance;
    [SerializeField]
    public Transform exit;

    public float detectionRange = 2f; //distancia del enemigo al jugador, RAngo de deteccion
    public float exitRange = 2f;
    public float minDistanceFromEntrance = 15f;

    List<NavMeshAgent> agents =  new(); //se obvio todo usando new, se puede SI, es bueno NO xd; ideal new List<NavMeshAgent>()

    //condiciones iniciales
    NavMeshTriangulation triangulation;
    Vector3 entrancePos; //posicion de la entrada

    float detectionRangeSqr;
    float exitRangeSqr;
    float minDistanceSqr;

    bool gameWon = false;

    System.Random random = new();

    void Start()
    {
        entrancePos = transform.position;
        triangulation = NavMesh.CalculateTriangulation();

        detectionRangeSqr = detectionRange*detectionRange; //son lo mismo
        exitRangeSqr = Mathf.Pow(exitRange, 2);
        minDistanceSqr = Mathf.Pow(minDistanceFromEntrance, 2);

        FindAllEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = player.transform.position;

        //Deteccion de captura de jugador (Enemigo en posicion de jugador)
        bool playerCaugth = false;

        foreach(var agent in agents) 
        {
            if(!agent.enabled) continue;//asegurarse que solo sean los agents abilitados

            if((agent.transform.position - playerPos).sqrMagnitude < detectionRangeSqr) 
            {
                playerCaugth = true;
                break;
            }
        }

        //SI el jugador es atrapado
        if(playerCaugth) 
        {
            TeleportPlayerToEntrance();
            RelocateAllNPC();

            return;
        }

        //si el jugador llego a la salia
        if((playerPos - exit.position).sqrMagnitude < exitRangeSqr)
        {
            gameWon = true;
            Debug.Log("Felicidades! Ganaste");
        }

        //lo que se debe hacer constantemente PERSECUCION
        foreach(var agent in agents) 
        {
            if (agent.enabled && !agent.isStopped) 
            {
                agent.SetDestination(playerPos); //enemigos llendo a pos player 
            }
        }
    }

    //Metodo para llevar a player a la entrada
    void TeleportPlayerToEntrance() 
    {
        var cc = player.GetComponent<NavMeshAgent>(); //donde cc = CharacterController

        if (cc != null) 
        {
            cc.enabled = false;
        }
        player.transform.position = entrancePos;
        if (cc != null) 
        {
            cc.enabled = true;
        }
        Debug.Log("Teletransport: " + entrancePos);
    }

    //Metodo para posicionar los enemigos
    void RelocateAllNPC() 
    {
        if (triangulation.vertices.Length == 0) return;

        foreach(var agent in agents) 
        {
            agent.enabled = false; //Primro desabilitar para poderlos colocar en cualquier posicion y ya despues se vuelven a activar
            agent.transform.position = GetValidRandomPosition();
            agent.enabled = true;
        }
    }
    
    //Metodo para calcular Posiciones VALIDAS. (para que un enemigo no aparezca en la entrada)
    Vector3 GetValidRandomPosition() 
    {
        Vector3 pos;

        do 
        {
            //calculo random
            int i = random.Next(0, triangulation.indices.Length / 3) * 3; //enre 0 y las posibles vertices contiguos

            Vector3 v1 = triangulation.vertices[triangulation.indices[i]];
            Vector3 v2 = triangulation.vertices[triangulation.indices[i+1]];
            Vector3 v3 = triangulation.vertices[triangulation.indices[i+2]];

            //modulos para calculos 
            float r1 = (float)random.NextDouble(); 
            float r2 = (float)random.NextDouble(); 

            //determinar si la suma r1 + r2 > 1
            if( r1 + r2 > 1) 
            {
                r1 = 1f - r1;
                r2 = 1f - r2;
            }

            pos = v1 + r1 * (v2 - v1) + r2 * (v3 - v1);

        } while ((pos - entrancePos).sqrMagnitude < minDistanceSqr);
        return pos;
    }

    //metodo para ubicar los objetos enemigos
    void FindAllEnemies() 
    {
        agents.Clear();
        foreach(var agent in FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None)) 
        {
            if(agent.CompareTag("Enemy"))
            {
                agents.Add(agent);
            }
        }
    }
}
