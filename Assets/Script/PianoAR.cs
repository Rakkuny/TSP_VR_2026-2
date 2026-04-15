using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PianoAR : MonoBehaviour
{
    //Arreglo para los sonidos
    public AudioClip[] clips;
    public AudioSource audioSource;
    string btnName;


    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)) //indice 0 boton izuqierdo, 1 derecho, 2 scroll 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); ///crea un rayo a patir de donde se hizo el click

            //Deteccion de colision de rayo
            RaycastHit hit; //hit es objeto, objeto en escena con el que colisiono

            if (Physics.Raycast(ray, out hit)) //primer hit lo asigna arriba
            { 
                btnName = hit.collider.name; //nombre del posicion, ej: "se hizo click en la tecla do"

                switch (btnName) 
                {
                    case "Do":
                        audioSource.clip = clips[0];
                        audioSource.Play();
                        break;

                    case "Re":
                        audioSource.clip = clips[1];
                        audioSource.Play();
                        break;

                    case "Mi":
                        audioSource.clip = clips[2];
                        audioSource.Play();
                        break;

                    case "Fa":
                        audioSource.clip = clips[3];
                        audioSource.Play();
                        break;

                    case "Sol":
                        audioSource.clip = clips[4];
                        audioSource.Play();
                        break;

                    case "La":
                        audioSource.clip = clips[5];
                        audioSource.Play();
                        break;

                    case "Si":
                        audioSource.clip = clips[6];
                        audioSource.Play();
                        break;

                    default:
                        break;
                }
            }
        }


        else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) //indice 0 boton izuqierdo, 1 derecho, 2 scroll 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position); ///crea un rayo a patir de donde se hizo el click
            RaycastHit hit; //hit es objeto, objeto en escena con el que colisiono

            if (Physics.Raycast(ray, out hit)) //primer hit lo asigna arriba
            {
                btnName = hit.collider.name; //nombre del posicion, ej: "se hizo click en la tecla do"

                switch (btnName) {
                    case "Do":
                        audioSource.clip = clips[0];
                        audioSource.Play();
                        break;

                    case "Re":
                        audioSource.clip = clips[1];
                        audioSource.Play();
                        break;

                    case "Mi":
                        audioSource.clip = clips[2];
                        audioSource.Play();
                        break;

                    case "Fa":
                        audioSource.clip = clips[3];
                        audioSource.Play();
                        break;

                    case "Sol":
                        audioSource.clip = clips[4];
                        audioSource.Play();
                        break;

                    case "La":
                        audioSource.clip = clips[5];
                        audioSource.Play();
                        break;

                    case "Si":
                        audioSource.clip = clips[6];
                        audioSource.Play();
                        break;

                    default:
                        break;
                }
            }
        }
    }
}


