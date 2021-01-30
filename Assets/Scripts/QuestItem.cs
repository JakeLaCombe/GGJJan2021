using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour
{
    public string displayableName;
    public ItemType Item;
    public enum ItemType
    {
        CasioWatch,
        FeatherBoa,
        Keytar,
        MotoHelmet,
        ShutterShades,
        VHS,
        FluxCapacitor,
        Tuxedo,
        Jetpack,
        BrickPhone
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
