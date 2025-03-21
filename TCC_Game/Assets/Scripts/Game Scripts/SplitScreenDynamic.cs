using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class SplitScreenDynamic : MonoBehaviour
{
    [Header("Players Properties")]
    public Transform player1, player2;
    //public PlayerInput player1_Input, player2_Input;

    [Header("SplitScreen Properties")]
    public float splitDistance = 20f;
    public Color splitterColor;
    public float splitterWidth;

    [Header("Cameras Properties")]
    private GameObject cam1, cam2;
    private GameObject split, splitter;

    void Start()
    {
        /*cam1 est� sendo referenciada na main camera, cam2 � gerada como um novo GameObject � pega o
        componente Camera*/
        cam1 = Camera.main.gameObject;
        Camera c1 = cam1.GetComponent<Camera>();
        cam2 = new GameObject("SplitScreen_Cam");
        Camera c2 = cam2.AddComponent<Camera>();

        /*c2 � dado como prefer�ncia para ser renderizado antes do c1*/
        c2.depth = c1.depth -1;
        c2.cullingMask = ~(1 << LayerMask.NameToLayer("TransparentFX"));

        /*splitter � criado para ser a splitscreen visual. LocalPosition define o eixo que ser� gerado;
         LocalScale define o tamanho do splitscreen visual; LocalEulerAngels define a angula��o da
        splitscreen*/
        splitter = GameObject.CreatePrimitive(PrimitiveType.Quad);
        splitter.transform.parent = gameObject.transform;
        splitter.transform.localPosition = Vector3.forward;
        splitter.transform.localScale = new Vector3(5, splitterWidth / 10, 1);
        splitter.transform.localEulerAngles = Vector3.zero;
        splitter.SetActive(false);

        //Cria um quad para renderizar a segunda camera
        split = GameObject.CreatePrimitive(PrimitiveType.Quad);
        split.transform.parent = splitter.transform;
        split.transform.localPosition = new Vector3 (0, -(1 / (splitterWidth / 10)), 0.0001f);
        split.transform.localScale = new Vector3(1, 2 / (splitterWidth / 10), 1);
        split.transform.localEulerAngles = Vector3.zero;

        /*Criamos um material temporário onde o splitter recebe esse material para a renderização
         e coloração da splitscreen visual*/
        Material materialTemp = new Material(Shader.Find ("Unlit/Color"));
        materialTemp.color = splitterColor;
        splitter.GetComponent<Renderer>().material = materialTemp;
        splitter.GetComponent<Renderer>().sortingOrder = 2;
        splitter.layer = LayerMask.NameToLayer("TransparentFX");
        Material materialTemp2 = new Material(Shader.Find("Mask/SplitScreen"));
        split.GetComponent<Renderer>().material = materialTemp2;
        split.layer = LayerMask.NameToLayer("TransparentFX");
    }

    void LateUpdate()
    {
        //Criando os variavéis e calculos que permitirá a rotação da splitscreen
        float distanceZ = player1.position.z - player2.transform.position.z;
        float distance = Vector3.Distance(player1.position, player2.transform.position);

        /*Se a posição x dos jogadores forem menores ou iguais, permite a rotação da splitscreen
         em um sentido. Caso contrário, ela irá rodar no sentido oposto*/
        float angle;
        if(player1.transform.position.x <= player2.transform.position.x)
            angle = Mathf.Rad2Deg * Mathf.Acos(distanceZ /distance);
        else
            angle = Mathf.Rad2Deg * Mathf.Asin(distanceZ /distance) - 90;

        //Rotaciona a splitscreen de acordo com sua angulação
        splitter.transform.localEulerAngles = new Vector3(0, 0, angle);

        //Gera o valor do meio entre os 2 jogadores
        Vector3 midPoint = new Vector3((player1.position.x + player2.position.x) / 2, 
            (player1.position.y + player2.position.y) / 2, 
            (player1.position.z + player2.position.z) / 2);

        //Criar calculos para a splitscreen pegar o meio dependendo da dist�ncia dos jogadores
        if(distance > splitDistance)
        {
            Vector3 offset = midPoint - player1.position;
            offset.x = Mathf.Clamp(offset.x, -splitDistance / 2, splitDistance / 2);
            offset.y = Mathf.Clamp(offset.y, -splitDistance / 2, splitDistance / 2);
            offset.z = Mathf.Clamp(offset.z, -splitDistance / 2, splitDistance / 2);
            midPoint = player1.position + offset;

            Vector3 offset2 = midPoint - player2.position;
            offset2.x = Mathf.Clamp(offset.x, -splitDistance / 2, splitDistance / 2);
            offset2.y = Mathf.Clamp(offset.y, -splitDistance / 2, splitDistance / 2);
            offset2.z = Mathf.Clamp(offset.z, -splitDistance / 2, splitDistance / 2);
            Vector3 midPoint2 = player2.position - offset;


            //Ativar a splitscreen
            if (!splitter.activeSelf)
            {
                splitter.SetActive(true);
                cam2.SetActive(true);

                cam2.transform.position = cam1.transform.position;
                cam2.transform.rotation = cam1.transform.rotation;
            }
            else
            {
                cam2.transform.position = Vector3.Lerp(cam2.transform.position,midPoint2 + 
                    new Vector3(0,50,-50),Time.deltaTime*5);
                Quaternion newRot2 = Quaternion.LookRotation(midPoint2 - cam2.transform.position);
                cam2.transform.rotation = Quaternion.Lerp(cam2.transform.rotation, newRot2, Time.deltaTime*5);
            }
        }
        else
        {
            //Desativar a splitscreen
            if (splitter.activeSelf)
            {
                splitter.SetActive(false);
                cam2.SetActive(false);
            }
        }

        cam1.transform.position = Vector3.Lerp(cam1.transform.position,midPoint + 
            new Vector3(0, 50, -50), Time.deltaTime*5);
        Quaternion newRot = Quaternion.LookRotation(midPoint - cam1.transform.position);
        cam1.transform.rotation = Quaternion.Lerp(cam1.transform.rotation, newRot, Time.deltaTime*5);
        

    }
}
