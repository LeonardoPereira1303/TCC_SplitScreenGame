using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenDynamic : MonoBehaviour
{
    /*Variáveis Necessárias:
     * Transform player1, player2 (Se necessário, adaptar para o Input System)
     * float splitDistance (Distância necessária para ativar a Split Screen)
     * Color splitColor (Definir a cor da split screen)
     * float splitWidht (Define a espessura da split screen)
     * GameObject camera1, camera2 (Referênciar e inicializar as camêras)
     * GameObject split, splitter (Inicializar a segunda tela)
     */

    void Start()
    {
        //Referência camera1 e incializa camera2
        //Define profundidade com depth (ajuda com a renderização)
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
