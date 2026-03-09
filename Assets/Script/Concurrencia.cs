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
    public Transform ThreadSphere;
    public Transform taskSphere;
    public Transform corutineSphere;
    public Transform mainCube;

    //Acciones a ejecutar en el hilo secundario, en este caso se uso una cola
    private Queue<Action> mainThreadActions = new Queue<Action>();

    void Start() {

    }

    void Update() {
        //Siempre gira el cubo refenrencia
        mainCube.Rotate(Vector3.up, 50 * Time.deltaTime);

        //ejecuta las acciones en el hilo principal
        lock (mainThreadActions) {
            //bloquear, si hay acciones que realizar, la saca de la cola y ejecuta la accion.
            while (mainThreadActions.Count > 0) {
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

    //corrutina
    IEnumerator MoveWhitCorutine()
    {
        corutineSphere.position += Vector3.right * 0.05f;
        yield return new WaitForSeconds(0.05f);
    }

}
