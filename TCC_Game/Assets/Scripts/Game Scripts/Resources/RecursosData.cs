using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ResourceData", menuName ="ResourceData", order =0)]
public class RecursosData : ScriptableObject
{
    public RecursosType type;
    public float processTime = 5.0f;
    public float cookTime = 4.0f;
}
