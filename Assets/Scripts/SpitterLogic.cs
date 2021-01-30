using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterLogic : MonoBehaviour
{
    public Vector2 SpitDirection;
    public float Speed = 3;
    public GameObject Projectile;
    private float TimeSinceLastSpit;
    // Start is called before the first frame update
    void Start()
    {
        Speed += Random.value;
        TimeSinceLastSpit = Random.Range(0, Speed);
    }

    // Update is called once per frame
    void Update()
    {
        TimeSinceLastSpit += Time.deltaTime;
        if (TimeSinceLastSpit > Speed)
        {
            TimeSinceLastSpit = 0;
            GameObject proj = Instantiate(Projectile, this.transform.position, this.transform.rotation);
            proj.GetComponent<Rigidbody2D>().AddForce(SpitDirection * 644, ForceMode2D.Impulse);
        }

    }
}
