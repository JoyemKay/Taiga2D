using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// + + DEPENDS ON GameController.Instance + + //

public class RoomChapter : MonoBehaviour
{
    //Chapter enemies, collected at setup.
    Enemy[] chapterEnemies;
    //Cached chapter enemies, that are killable only once. Saves the unique id of the enemy, and its alive state.
    EnemySessionState[] chapterEnemySessionState;
    //See above, but for objects that are only activatable once.
    WorldObject[] chapterObjects;
    WordlObjectSessionState[] chapterObjectsSessionState;
    Grid chapterGrid;
    public GameObject tilemap;

    //Change in inspector to make room objects that is only active during a certain chapter. Can be multiple chapters in one chapter.
    public Chapter[] chapter;

    //Initiates chapter and all objects in it, if there are saved states, ie. objects that are only interactable once, ore enemies killable once, check their state.
    //If the chapters parent room is not the active room, deactivate the object or enemy.
    public void Setup()
    {
        //chapterGrid = GetComponentInChildren<Grid>();
        //Get all enemies in chapter, and disable the ones that are not resetable and has been disabled earlier.
        chapterEnemies = GetComponentsInChildren<Enemy>();
        if (chapterEnemySessionState != null && chapterEnemies != null)
        {
            if (chapterEnemySessionState.Length > 0 && chapterEnemies.Length > 0)
            {
                for (int i = 0; i < chapterEnemies.Length; i++)
                {
                    for (int j = 0; j < chapterEnemySessionState.Length; j++)
                    {
                        if (chapterEnemies[i].id == chapterEnemySessionState[j].id)
                        {
                            if (!chapterEnemySessionState[j].enemyIsAlive)
                            {
                                chapterEnemies[i].isAlive = false;
                                chapterEnemies[i].SetInactive();
                            }
                        }
                    }
                }
            }
        }
        //As above, but for worldObjects that are not resetable, and has been interacted with.
        chapterObjects = GetComponentsInChildren<WorldObject>();
        if (chapterObjectsSessionState != null && chapterObjects != null)
        {
            if (chapterObjectsSessionState.Length > 0 && chapterObjects.Length > 0)
            {
                for (int i = 0; i < chapterObjects.Length; i++)
                {
                    for (int j = 0; j < chapterObjectsSessionState.Length; j++)
                    {
                        if (chapterObjects[i].id == chapterObjectsSessionState[j].id)
                        {
                            if (!chapterObjectsSessionState[j].notDisabled)
                            {
                                chapterObjects[i].hasBeenDisabled = true;
                                chapterObjects[i].gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }

        DisableChapter();
    }

    public void TryEnableChapter()
    {
        if (ChapterIsActiveOrDefault())
        {
            //if (chapterGrid) { chapterGrid.enabled = true; }
            tilemap.SetActive(true);
            for (int i = 0; i < chapterEnemies.Length; i++) { chapterEnemies[i].TrySetActive(); }
            for (int i = 0; i < chapterObjects.Length; i++) { chapterObjects[i].ResetPos(); }
        }
    }

    public void DisableChapter()
    {
        //if (chapterGrid) { chapterGrid.enabled = false; }
        tilemap.SetActive(false);
        for (int i = 0; i < chapterEnemies.Length; i++) { chapterEnemies[i].SetInactive(); }
        for (int i = 0; i < chapterObjects.Length; i++) { chapterObjects[i].gameObject.SetActive(false); }
    }

    //returns true if the chapter is baseChapter or the current chapter in GameController
    bool ChapterIsActiveOrDefault()
    {
        bool active = false;
        for (int i = 0; i < chapter.Length; i++)
        {
            if (chapter[i] == Chapter.baseChapter || chapter[i] == GameController.Instance.currentChapter)
            {
                active = true;
            }
        }
        return active;
        //return (chapter == Chapter.baseChapter || chapter == GameController.Instance.currentChapter);
    }

    public EnemySessionState[] GetEnemySessionStates()
    {
        List<EnemySessionState> enemyList = new List<EnemySessionState>();
        for (int i = 0; i < chapterEnemies.Length; i++)
        {
            if (!chapterEnemies[i].resetable)
            {
                EnemySessionState enemyState = new EnemySessionState();
                enemyState.id = chapterEnemies[i].id;
                enemyState.enemyIsAlive = chapterEnemies[i].isAlive;
                enemyList.Add(enemyState);
            }
        }
        return enemyList.ToArray();
    }

    public WordlObjectSessionState[] GetWordlObjectSessionStates()
    {
        List<WordlObjectSessionState> objectList = new List<WordlObjectSessionState>();
        for (int i = 0; i < chapterObjects.Length; i++)
        {
            if (!chapterObjects[i].resetable)
            {
                WordlObjectSessionState objectState = new WordlObjectSessionState();
                objectState.id = chapterObjects[i].id;
                objectState.notDisabled = chapterObjects[i].isActiveAndEnabled;
                objectList.Add(objectState);
            }
        }
        return objectList.ToArray();
    }

    public void SetupSessionState(EnemySessionState[] enemies) { chapterEnemySessionState = enemies; }
    public void SetupSessionState(WordlObjectSessionState[] wordlObjects) { chapterObjectsSessionState = wordlObjects; }
    public void SetupSessionState(EnemySessionState[] enemies, WordlObjectSessionState[] wordlObjects)
    {
        SetupSessionState(enemies);
        SetupSessionState(wordlObjects);
    }
}
