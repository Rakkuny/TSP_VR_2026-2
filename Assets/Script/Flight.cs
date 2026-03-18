using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

//script para simulacion de vuelo
public class Flight : MonoBehaviour
{
    public float speed = 50f;
    public float rotationSpeed = 80f;
    public Transform cameraTransform;
    public Vector2 movementInput;

    //Valor para Act 1 y 2
    //Control de Iteraciones, mientras mas iteraciones mas recursos consume
    public int turbulenceIterations = 1000;

    //lista de vectores de posicioncalculados,(para turbulencia)
    private List<Vector3> turbulenceForces = new List<Vector3>();

    //metodo para mover la nave
    //debe ser un escuha del inputSysem
    public void OnMovement(InputValue value) 
    {
        //action tipe de tipo vector dos (de vector properties)
        movementInput = value.Get<Vector2>(); //get valor que se asocio a la variable value
    }

    void Start()
    {
        
    }

    void Update()
    {
        //lo que se manda a llamar en cada frame
        if(cameraTransform == null) 
        {
            Debug.LogError("No hay camara asignada");
            return;
        }

        //---------------- ACTIVIDAD 1 ------------ Proceso pesado que consume recursos (simulacion de turbulencia)

        SimulateTurbulence();

        // Mover la nave linealmente
        Vector3 moveDirection = cameraTransform.forward * movementInput.y * speed*Time.deltaTime;
        //donde vemos * ireccion del imput * con cierta vel e incremento
        this.transform.position += moveDirection;

        //Mover la nave en Rotacion
        float yaw = movementInput.x * rotationSpeed * Time.deltaTime;
        this.transform.Rotate(0, yaw, 0);
    }


    //part ACT 1 (esto se va al hilo scundario)
    public void SimulateTurbulence() 
    {
        //limpiar lista para evitar duplicados
        turbulenceForces.Clear(); 

        //repeticiones
        for(int i = 0; i < turbulenceIterations; i++) //numero de itereciones declaradas n este caso 1000000
        {
            //Ruido de Perling para turbulencias
            Vector3 force = new Vector3(
                                //Posicion inicial para el calculo, timepo que lleva ctivo y cambia a la otra linea
                Mathf.PerlinNoise(i * 0.001f, Time.time) * 2 - 1, 
                Mathf.PerlinNoise(i * 0.002f, Time.time) * 2 - 1,
                Mathf.PerlinNoise(i * 0.003f, Time.time) * 2 - 1
                ); 

            turbulenceForces.Add(force);
        }
    }
}
