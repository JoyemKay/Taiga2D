using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//  Ensures that objects placed in the world are alinged correctly in the z-depth
//  If the object has alternate sprites, randomizes apperance
public class WorldObject : MonoBehaviour
{
    public string id;
    public int floor;
    [Tooltip("If graphics are stored in a child of the game object, set parent as WOparent.")]
    public Transform /*wOparent,*/ gfxObject;
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

    private void Start()
    {
        SetFloor(floor);
    }

    public void Setup()
    {
        thisRenderer = GetComponent<SpriteRenderer>();
        if (gfxObject)
            thisRenderer = gfxObject.gameObject.GetComponent<SpriteRenderer>();

        SetDepth(transform.localPosition);
        startPos = transform.localPosition;

        AlternateSprite();
        selectedSprite = thisRenderer.sprite;

        Initiated = true;
    }

    public void ResetPos()
    {
        if (resetable || !hasBeenDisabled)
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
            if (!((Vector2)transform.localPosition == startPos))
            {
                transform.localPosition = startPos;
                SetDepth(transform.localPosition);
            }

            thisRenderer.sprite = selectedSprite;
        }
    }

    public void DisableWorldObject()
    {
        hasBeenDisabled = true;
        gameObject.SetActive(false);
    }

    //  If z-pos is not set in editor, update the z-pos to right depth
    //
    public void SetDepth(Vector3 objPos)
    {
        Vector3 pos;

        pos = new Vector3(transform.localPosition.x, transform.localPosition.y, (objPos.y + offset) / 100);

        if (Mathf.Abs(pos.z - objPos.z / 100) > Mathf.Epsilon)
        {
            if (gfxObject)
            {
                pos = new Vector3(gfxObject.localPosition.x, gfxObject.localPosition.y, pos.z);
                gfxObject.transform.localPosition = pos;
            }
            else
            {
                transform.localPosition = pos;
            }
        }
    }

    // Sets sorting order and collision layer
    //
    public void SetFloor(int setFloor)
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        if (sprites.Length > 0)
        {
            foreach (SpriteRenderer s in sprites)
            {
                s.sortingOrder = setFloor * 10 + 5;
            }
        }
        GameController.Instance.SetColliderLayer(this.gameObject, setFloor);
    }

    //  If alternate sprites exist in array, randomize sprite if roll is larger than (1- 1/(no of sprites))
    //  including the default sprite, (if there is one alternate sprite, the chance is 50 %)
    private void AlternateSprite()
    {
        if (alternateSprites.Length > 0)
        {

            float randomize = Random.Range(0.0f, 1.0f);

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
