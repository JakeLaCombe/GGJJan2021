using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const int NumberOfQuestItemsToReturnInOrderToWin = 2;
    private int _numberOfQuestItemsReturned = 0;
    public QuestItem _heldQuestItem;
    public Text timeLeftText;
    public Text CollectedText;

    public GameObject FailurePanel;
    private float TimeTillFailure = 30;
    private int timeToComplete = 30;
    public Image spriteRenderer;


    public void SetQuestItemPickedUp(QuestItem item)
    {
        this._heldQuestItem = item ?? throw new System.ArgumentNullException(nameof(item));
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = _heldQuestItem.GetComponent<SpriteRenderer>().sprite;
    }

    public bool TryReturnHeldQuestItem()
    {
        if (_heldQuestItem == null)
        {
            return false; // Not holding anything
        }

        _heldQuestItem = null;  // It's been returned, so not holding it anymore
        _numberOfQuestItemsReturned++;
        timeToComplete += 15;
        TimeTillFailure += timeToComplete;
        // Hack, side effect of "winning", but it's a Game Jam so who cares
        if (_numberOfQuestItemsReturned >= NumberOfQuestItemsToReturnInOrderToWin)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Winner");
        }

        spriteRenderer.enabled = false;

        return true;
    }

   

    public int NumberOfQuestItemsReturned
    {
        get { return this._numberOfQuestItemsReturned; }
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
        timeLeftText.text = ((int)TimeTillFailure).ToString();
    }
}
