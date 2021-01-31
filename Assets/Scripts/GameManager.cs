using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public QuestItem _heldQuestItem;
    public Text timeLeftText;
    public Text CollectedText;
    private int Collected;
    public GameObject FailurePanel;
    private float TimeTillFailure = 120;

    public Image spriteRenderer;

    public void SetQuestItemPickedUp(QuestItem item)
    {
        if (item == null)
        {
            throw new System.ArgumentNullException(nameof(item));
        }

        this._heldQuestItem = item;
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = _heldQuestItem.GetComponent<SpriteRenderer>().sprite;
    }

    public bool TryReturnHeldQuestItem()
    {
        if (_heldQuestItem == null)
        {
            return false; // Not holding anything
        }

        this._heldQuestItem = null;  // It's been returned, so not holding it anymore
        spriteRenderer.enabled = false;
        return true;
    }

    public void ResetClock()
    {
        TimeTillFailure = 120;
        Collected++;
        CollectedText.text = Collected.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        TimeTillFailure -= Time.deltaTime;
        if (TimeTillFailure < 0)
        {
            FailurePanel.SetActive(true);
            Time.timeScale = 0;
            TimeTillFailure = -1;
        }
        timeLeftText.text = TimeTillFailure.ToString();
    }
}
