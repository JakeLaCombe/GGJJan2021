using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusionFog : MonoBehaviour
{
    private SpriteRenderer sprite;


    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        sprite.color = Color.Lerp(sprite.color, Random.ColorHSV(0, 1, 0.43f, 1, .7f, 1, 1, 1), .7f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.GetComponent<PlayerMovement>().ConfusionTime = 5;
        }
    }
}
