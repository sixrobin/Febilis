using UnityEngine;
using RSLib.Extensions;

public class Test : MonoBehaviour
{
    public RSLib.Framework.Events.QuaternionEvent eve;

    public Color c;
    public string str;
    public string str2;

    // Start is called before the first frame update
    void Start() 
    {
        string hex = "#a5b487";
        c = hex.ToColorFromHex();
        str = c.ToHexRGB();
        str2 = c.ToHexRGBA();

        int a = -58;
        str = a.AddLeading0(5);
        Debug.Log(a);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
