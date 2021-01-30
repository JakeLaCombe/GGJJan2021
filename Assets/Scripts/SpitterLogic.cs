using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterLogic : MonoBehaviour
{
    public Vector2 SpitDirection;
    public int Speed;
    public GameObject Projectile;
    private float TimeSinceLastSpit;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TimeSinceLastSpit += Time.deltaTime;
        if (TimeSinceLastSpit > Speed)
        {
            TimeSinceLastSpit = 0;
            GameObject proj = Instantiate(Projectile, this.transform.position, this.transform.rotation);
            proj.GetComponent<Rigidbody2D>().AddForce(SpitDirection * 500, ForceMode2D.Impulse);
        }

    }
}
