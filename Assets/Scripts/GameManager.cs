using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private QuestItem _heldQuestItem;

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

        return true;
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
