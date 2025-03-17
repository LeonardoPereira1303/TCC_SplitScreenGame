using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IPickable
{
   GameObject GameObject { get; }
    public void PickUpItems();
    public void DropItems();
}
