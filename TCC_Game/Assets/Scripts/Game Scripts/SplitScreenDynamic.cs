using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenDynamic : MonoBehaviour
{
    /*Vari�veis Necess�rias:
     * Transform player1, player2 (Se necess�rio, adaptar para o Input System)
     * float splitDistance (Dist�ncia necess�ria para ativar a Split Screen)
     * Color splitColor (Definir a cor da split screen)
     * float splitWidht (Define a espessura da split screen)
     * GameObject camera1, camera2 (Refer�nciar e inicializar as cam�ras)
     * GameObject split, splitter (Inicializar a segunda tela)
     */

    void Start()
    {
        //Refer�ncia camera1 e incializa camera2
        //Define profundidade com depth (ajuda com a renderiza��o)
        //Inicializar a splitscreen e passando seus devidos par�metros

        //Criar material para a cria��o da split screen
    }

    //LateUpdate � util para a CameraFollow
    void LateUpdate()
    {
        //Criar os calculos para a rota��o da splitscreen
        //Rotacionar a splitscreen
        //Gera o valor do meio entre os 2 jogadores
        //Criar calculos para a splitscreen pegar o meio dependendo da dist�ncia dos jogadores

        //Ativar a splitscreen
        //Desativar a splitscreen

    }
}
