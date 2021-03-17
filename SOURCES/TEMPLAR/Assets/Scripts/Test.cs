using UnityEngine;
using RSLib.Extensions;

public class Test : MonoBehaviour
{
    public CircleCollider2D one, two;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(one.OverlapsWith(two));
    }
}
