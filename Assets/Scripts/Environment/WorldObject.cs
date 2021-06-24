using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//  Ensures that objects placed in the world are alinged correctly in the z-depth
//  If the object has alternate sprites, randomizes apperance
public class WorldObject : MonoBehaviour
{
    public string id;
    [Tooltip("If graphics are stored in a child of the game object, set parent as WOparent.")]
    public Transform wOparent;
    public bool resetable = true, hasBeenDisabled = false;
    public float offset;
    public Sprite[] alternateSprites;

    Vector2 startPos;
    SpriteRenderer thisRenderer;
    Sprite selectedSprite;
    public bool Initiated { get; private set; }

    private void OnEnable()
    {
        if (!Initiated) { Setup(); }
    }

    public void Setup(){
        thisRenderer = GetComponent<SpriteRenderer>();
        AlternateSprite();
        selectedSprite = thisRenderer.sprite;
        if (wOparent)
        {
            SetDepth(wOparent.position);
            startPos = wOparent.position;
        }
        else
        {
            SetDepth(transform.localPosition);
            startPos = transform.localPosition;
        }
        Initiated = true;
    }

    public void ResetPos(){
        if (resetable || !hasBeenDisabled)
        {
            if (wOparent)
            {
                if(!wOparent.gameObject.activeInHierarchy){
                    wOparent.gameObject.SetActive(true);
                }
                if (!((Vector2)wOparent.position == startPos))
                {
                    wOparent.position = startPos;
                    SetDepth(wOparent.localPosition);
                }
            }
            else
            {
                if(!gameObject.activeInHierarchy){
                    gameObject.SetActive(true);
                }
                if (!((Vector2)transform.localPosition == startPos))
                {
                    transform.localPosition = startPos;
                    SetDepth(transform.localPosition);
                }
            }
            thisRenderer.sprite = selectedSprite;
        }
    }

    public void DisableGameObject(){
        if(wOparent){
            wOparent.gameObject.SetActive(false);
        }else{
            gameObject.SetActive(false);
        }
    }

    public void DisableWorldObject(){
        hasBeenDisabled = true;
        DisableGameObject();
    }


    //  If z-pos is not set in editor, update the z-pos to right depth
    //
    public void SetDepth(Vector3 objPos){
        Vector3 pos;
        if (!wOparent)
        {
            pos = new Vector3(objPos.x, objPos.y, (objPos.y + offset) / 100);
        }else{
            pos = new Vector3(transform.localPosition.x, transform.localPosition.y, (objPos.y + offset) / 100);
        }
        if (Mathf.Abs(pos.z - objPos.z / 100) > Mathf.Epsilon)
        {
            transform.localPosition = pos;
        }
    }

    //  If alternate sprites exist in array, randomize sprite if roll is larger than (1- 1/(no of sprites))
    //  including the default sprite, (if there is one alternate sprite, the chance is 50 %)
    private void AlternateSprite()
    {
        if (alternateSprites.Length > 0)
        {

            float randomize = Random.Range(0.0f, 1.0f);
            //  Debug.Log(name + " roll: " + randomize * 100f);
            if (randomize > 1.0f / (alternateSprites.Length + 1.0f))
            {
                for (int i = 0; i < alternateSprites.Length; i++)
                {
                    if (randomize <= (i + 2.0f) / (alternateSprites.Length + 1.0f))
                    {
                        thisRenderer.sprite = alternateSprites[i];
                        selectedSprite = alternateSprites[i];
                        return;
                    }
                }
            }
        }
    }

    //DEBUG//

    private void OnDrawGizmos()
    {
        if (thisRenderer != null)
        {
            Vector3 offsetVector = new Vector3(0, offset, 0);
            Vector3 extendsVector = new Vector3(thisRenderer.bounds.extents.x, 0, 0);
            Gizmos.color = Color.magenta;

            Gizmos.DrawLine(thisRenderer.bounds.center - extendsVector + offsetVector, thisRenderer.bounds.center + extendsVector + offsetVector);
        }
    }
}
