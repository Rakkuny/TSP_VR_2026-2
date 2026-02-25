using Unity.VisualScripting;
using UnityEngine;

public class VRWalk : MonoBehaviour
{
    //Atributos(metodos y constructor)/variables de clase
    public Transform vrCamera;
    public float angulo = 30.0f;
    public float speed = 3.0f;
    public bool move;
    //el bool es una condicion, si esta inclinado 30 grados avanza

    private CharacterController controller;
    // el privado solo se puede acceder en la clase a la que pertenece, el restringido los hijos tambien pueden acceder a el


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        controller = GetComponent<CharacterController>();
        //el objeto que esta dentro de la clase, tiene tambien esta propiedad, por lo que tambien puede acceder al controlador 
        //En este caso el unico objeto que tiene el controlador es el objeto player

    }

    // Update is called once per frame
    void Update() {
        //para saber si la camara se inclino
        if (vrCamera.eulerAngles.x >= angulo && vrCamera.eulerAngles.x < 60.0f) //angulo ,ayor a 30 pero menor a 60
        //Euler angles 
        //cuaternion, todo lo que tengamos tiene 4 represantaciones:
        //q = qw + qx*i + qy*j +qz*k, una posición(xyz) y un componente real(giro/rotacion) contiene angulos euler.
        {
            move = true;
        } 
        else 
        {
            move = false;
        } 
        if (move)
        {
            Vector3 direction = vrCamera.TransformDirection(Vector3.forward);
            controller.SimpleMove(direction * speed);
        }
    }
}
