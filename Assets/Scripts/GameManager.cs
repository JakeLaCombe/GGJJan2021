using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int NumberOfQuestItemsToReturnInOrderToWin = 2;
    private int _numberOfQuestItemsReturned = 0;
    public QuestItem _heldQuestItem;


    public void SetQuestItemPickedUp(QuestItem item)
    {
        if (item == null)
        {
            throw new System.ArgumentNullException(nameof(item));
        }

        this._heldQuestItem = item;
    }

    public bool TryReturnHeldQuestItem()
    {
        if (_heldQuestItem == null)
        {
            return false; // Not holding anything
        }

        this._heldQuestItem = null;  // It's been returned, so not holding it anymore
        this._numberOfQuestItemsReturned++;

        return true;
    }

    public int NumberOfQuestItemsReturned
    {
        get { return this._numberOfQuestItemsReturned; }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
