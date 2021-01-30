using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDeath : MonoBehaviour
{
    private float life = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;
        if (life < 0)
            Destroy(transform.root.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        life = .01f;



    }
}
