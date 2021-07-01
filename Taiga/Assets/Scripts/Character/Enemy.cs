using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : Character
{
    //Unique identifier of the enemy, used by Chapters to make sure that !resetable enemies doesnt reset.
    public string id;
    //If this is false, the enemy can only be killed once, and wont reset in the chapter upon Scene reset.
    public bool resetable;
    public bool isAlive;

    protected override void Start()
    {
        base.Start();
        if (target) { AstarMove(); }
    }

    //  Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    //Only enables the Enemy game object if it is resettable or hasnt been killed.
    public void TrySetActive()
    {
        if (resetable || isAlive)
        {
            gameObject.SetActive(true);
        }
    }

    public void SetInactive()
    {
        StopAstarMove();
        gameObject.SetActive(false);
    }
}
