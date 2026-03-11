//librerias para herramientas de hilos, tareas asincronas, corrutinas
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class Concurrencia : MonoBehaviour {
    //Crear apartados}
    [Header("Activa los metodos")]
    public bool useSincrono;
    public bool useThread;
    public bool useTask;
    public bool useCorutine;

    [Header("Esfera a mover")]
    public Transform sincronoSphere;
    public Transform threadSphere;
    public Transform taskSphere;
    public Transform corutineSphere;
    public Transform mainCube;

    //Acciones a ejecutar en el hilo secundario, en este caso se uso una cola
    private Queue<Action> mainThreadActions = new Queue<Action>();

    void Start() 
    {
        //ejecutar la bandera actuvas, solo las que esten en true
        if(useSincrono) MoveSincrono();
        if(useThread) MoveWithThread();
        if(useTask) MoveWithTask();
        if(useCorutine) StartCoroutine(MoveWithCorutine());
    }

    void Update() {
        //Siempre gira el cubo refenrencia
        mainCube.Rotate(Vector3.up, 50 * Time.deltaTime);

        //ejecuta las acciones en el hilo principal
        lock (mainThreadActions) 
        {
            //bloquear, si hay acciones que realizar, la saca de la cola y ejecuta la accion.
            while (mainThreadActions.Count > 0) 
            {
                mainThreadActions.Dequeue().Invoke();
            }
        }
    }

    //metodos para herramientas de concurrencia

    //sincrono
    void MoveSincrono() {
        for (int i = 0; i <= 100; i++) {
            sincronoSphere.position += Vector3.right * 0.05f;
        }
        Thread.Sleep(50);
    }

    //movimiento con hilo secundario 
    void MoveWithThread() 
    {
        //lamnda => significa "tal que", todo dentro de este hilo se ejecuta en un proceso secundario.
        new Thread(() => 
        {
            //que el hilo mueva la esfera
            for (int i = 0; i <= 100; i++) 
            {
                Thread.Sleep(50);
                lock (mainThreadActions) //hay que bloquear el hilo, o consumira muchos recursos sin saber si se cerro el hilo o no
                {
                    mainThreadActions.Enqueue(() => 
                    {
                        //movimiento que corresponde a desplazar a la esferea con el vector
                        threadSphere.position += Vector3.right * 0.05f;
                    });
                }
            }
        }).Start(); //inicialo
    }
    //si no se bloquea el hilo es como un virus


    //metodo con task asincrono
    async void MoveWithTask()
    {
        await Task.Run(() => 
        {
            Thread.Sleep(50);
            for (int i = 0; i <= 100; i++) 
            {
                //mov de seuridad
                lock (mainThreadActions) 
                {
                    mainThreadActions.Enqueue(() => 
                    {
                        taskSphere.position += Vector3.right * 0.05f;
                    });
                }
            }
        });
    }

 

    //corrutina
    IEnumerator MoveWithCorutine()
    {
        corutineSphere.position += Vector3.right * 0.05f;
        yield return new WaitForSeconds(0.05f);
    }

}
