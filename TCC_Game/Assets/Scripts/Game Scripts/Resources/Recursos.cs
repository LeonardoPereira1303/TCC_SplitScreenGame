using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Recursos : Interactable, IPickable
{
    [SerializeField] private RecursosData data;
    private Rigidbody rb;
    private Collider cl;

    public RecursosStatus status { get; private set; }
    public RecursosType Type => data.type;

    [SerializeField] private RecursosStatus startStatus = RecursosStatus.Neutro;

    public float ProcessTime => data.processTime;
    public float CookTime => data.cookTime;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        cl = GetComponent<Collider>();
        Setup();
    }

    private void Setup()
    {
        rb.isKinematic = true;
        cl.enabled = false;

        status = RecursosStatus.Neutro;

        if (startStatus == RecursosStatus.Processed)
        {
            ChangeToProcess();
        }
    }

    public void PickUpItems()
    {
        rb.isKinematic = true;
        cl.enabled = false;
    }

    public void DropItems()
    {
        gameObject.transform.SetParent(null);
        rb.isKinematic = false;
        cl.enabled = true;
    }

    public void ChangeToProcess()
    {
        status = RecursosStatus.Processed;

        //Atualizar a sprite
    }

    public void ChangeToReady()
    {
        status = RecursosStatus.Ready;

        //Atualizar a sprite
    }

    public override bool TryToDropIntoSlot(IPickable pickableToDrop)
    {
        return false;
    }

    public override IPickable TryToPickUpFromSlot(IPickable playerHoldPickable)
    {
        rb.isKinematic = true;
        return this;
    }
}
