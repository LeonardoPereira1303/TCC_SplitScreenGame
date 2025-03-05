using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplitScreenDynamic : MonoBehaviour
{

    [Header("Players Properties")]
    public Transform player1, player2;
    //public PlayerInput player1_Input, player2_Input;

    [Header("SplitScreen Properties")]
    public float splitDistance = 5f;
    public Color splitColor;
    public float splitWidth;

    [Header("Cameras Properties")]
    private GameObject cam1, cam2;
    private GameObject split, splitter;

    void Start()
    {
        /*cam1 está sendo referenciada na main camera, cam2 é gerada como um novo GameObject é pega o
        componente Camera*/
        cam1 = Camera.main.gameObject;
        Camera c1 = cam1.GetComponent<Camera>();
        cam2 = new GameObject("SplitScreen_Cam");
        Camera c2 = cam2.GetComponent<Camera>();

        /*c2 é dado como preferência para ser renderizado antes do c1*/
        c2.depth = c1.depth -1;

        //Inicializar a splitscreen e passando seus devidos parâmetros





        //Criar material para a criação da split screen
    }

    //LateUpdate é util para a CameraFollow
    void LateUpdate()
    {
        //Criar os calculos para a rotação da splitscreen
        //Rotacionar a splitscreen
        //Gera o valor do meio entre os 2 jogadores
        //Criar calculos para a splitscreen pegar o meio dependendo da distância dos jogadores

        //Ativar a splitscreen
        //Desativar a splitscreen

    }
}
