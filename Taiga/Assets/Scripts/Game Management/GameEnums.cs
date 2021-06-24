using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//State used by character game objects
public enum State
{
    idle,
    moving,
    attacking,
    staggered,
    paused
}

//Used as an indicator for the progress of the game, Rooms has Chapter children that are enabled or disabled depending on the games current chapter.
public enum Chapter
{
    baseChapter,
    chapter01,
    chapter02,
    chapter03,
    chapter04
}

public enum TransitionDirection{
    horizontal,
    vertical
}

//Container to save the state of enemies with the bool 'resetable' false.
public struct EnemySessionState
{
    public string id;
    public bool enemyIsAlive;
}

//As above, but for worldObjects with the bool 'resetable' false.
public struct WordlObjectSessionState
{
    public string id;
    public bool notDisabled;
}

//Container for an item and a spawn chance, used by the ItemSpawner, set in inspector. change is relative to the spawner.
[System.Serializable]
public struct ItemSpawnStruct
{
    public Item item;
    public float chance;
}

public class GameEnums : MonoBehaviour
{
    //Contains the enums used in the game
}
