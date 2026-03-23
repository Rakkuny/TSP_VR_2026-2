using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.InputSystem.iOS;
using System.IO;

public class FlightThreadNOSinc : MonoBehaviour 
{
    public float speed = 50f;
    public float rotationSpeed = 80f;
    public Transform cameraTransform;
    public Vector2 movementInput;

    //Valor para Act 1 y 2
    //Control de Iteraciones, mientras mas iteraciones mas recursos consume
    public int turbulenceIterations = 1000;

    //lista de vectores de posicion calculados PARA TURBULENCIA
    private List<Vector3> turbulenceForces = new List<Vector3>();

    //Variables para manipular el hilo secundario
    private Thread turbulenceThread; //instancia del hilo secundario
    private bool isTurbulenceRunning = false; // Bander para saber si el hilo sigue el calculo 
    private bool stopTurbulenceThread = false; //bandera para saber si el hilo termino
    private float capturedTime; //variable para almacenar el tiempo transcurrio

    //banderas de control sobre lectura
    public bool read = false;

    //ruta para guardar el archivo
    string filepath;


    //metodo para mover la nave
    //debe ser un escuha del inputSysem
    public void OnMovement(InputValue value) 
    {
        //action tipe de tipo vector dos (de vector properties)
        movementInput = value.Get<Vector2>(); //get valor que se asocio a la variable value
    }

    void Start()
    {
        filepath = Application.dataPath + "/TurbulenceData.txt";
        //para saber donde esta la ruta
        Debug.Log("Ruta de archivo: " +  filepath);
    }

    void Update() 
    {
        //lo que se manda a llamar en cada frame
        if (cameraTransform == null) 
        {
            Debug.LogError("No hay camara asignada");
            return;
        }

        //---------------- ACTIVIDAD 1 ------------ Proceso en hilo secundario

        //tiempo trascurrido
        capturedTime = Time.deltaTime; //acumulativo aqui delta time

        //Proceso pesado en hilo secundario
        if (!isTurbulenceRunning) 
        {
            isTurbulenceRunning = true;
            stopTurbulenceThread = false;

            turbulenceThread = new Thread(() =>
            SimulateTurbulence(capturedTime));
            turbulenceThread.Start();
        }

        // Mover la nave linealmente
        Vector3 moveDirection = cameraTransform.forward * movementInput.y * speed * Time.deltaTime;
        //donde vemos * direccion del input * con cierta vel e incremento
        this.transform.position += moveDirection;

        //Mover la nave en Rotacion
        float yaw = movementInput.x * rotationSpeed * Time.deltaTime; //da una resta delta time
        this.transform.Rotate(0, yaw, 0);

        //--------------- ACTIVIDAD 3 - Parte 1 ------------------------ Meteodo para lectura y escritura de archivos
        //LECTURA de archivos
        TryReadFile();
    }


    //part ACT 1 (esto se va al hilo scundario)
    public void SimulateTurbulence(float time) {
        //limpiar lista para evitar duplicados
        turbulenceForces.Clear();

        //repeticiones
        for (int i = 0; i < turbulenceIterations; i++) //numero de itereciones declaradas en este caso 1000000
        {
            //primero debe verificar si el hilo ya se mando detener, si no, continua con calculo de vectores
            if (stopTurbulenceThread) {
                break; //sale de ciclo
            }

            // Calculo de los vectores
            Vector3 force = new Vector3(
                //Posicion inicial para el calculo, timepo que lleva ctivo y cambia a la otra linea
                Mathf.PerlinNoise(i * 0.001f, time) * 2 - 1,
                Mathf.PerlinNoise(i * 0.002f, time) * 2 - 1,
                Mathf.PerlinNoise(i * 0.003f, time) * 2 - 1
                );

            turbulenceForces.Add(force);
        }

        //Señal en consola de inicio de hilo
        Debug.Log("iniciando simulacion de turbuencia");

        

        //-------- ACTIVIDAD 3 --------------------

        //Escritura del archivo
        //objeto que diga que vas a escribir y donde
        using (StreamWriter writer = new StreamWriter(filepath, false)) 
        {
            foreach(var force in turbulenceForces) //forces coleccion generica, para convertir a texto a escribir
            {
                writer.WriteLine(force.ToString());
            }
            writer.Flush(); //impiar memoria cached el buffer, no conviene cerrar el archivo porque habria que volver a abrirlo
        }

        Debug.Log("Archivo escrito");

        //Simulacion completa; CAMBIO de lugar, pirque la simuacion esta completa tras terminar de escribir
        isTurbulenceRunning = false;
    }
    
    //--------------- ACTIVIDAD 3 - Parte 1 ------------------------ Meteodo para lectura y escritura de archivos
    //ESCRITURA de archivos
    void TryReadFile() 
    {
        try 
        {
            string content = File.ReadAllText(filepath);
            Debug.Log("Archivo leido: " + content);
        }
        catch (IOException ex) 
        {
            Debug.LogError("Error de acceso al archivo: " + ex.Message);
        }
    }


    //Cerrar hilo
    private void OnDestroy() {
        //indicar el cierre del hilo secudario
        stopTurbulenceThread = true;

        //verificar si el hilo existe y se esta ejecutado
        if (turbulenceThread != null && turbulenceThread.IsAlive) {
            //lo unimos al hilo principal y cerramos ejecucion
            turbulenceThread.Join();
        }
    }
}


// ACTIVIDAD 2 cambiar valor de iteraciones y comparar graficas de profiler
