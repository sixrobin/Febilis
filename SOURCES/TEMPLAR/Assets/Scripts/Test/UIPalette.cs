using UnityEngine;
using UnityEngine.UI;

public class UIPalette : MonoBehaviour
{
    Material mat;

    private void Start()
    {
        mat = GetComponent<Graphic>().materialForRendering;
    }

    private void Update()
    {
        Texture2D tex = FindObjectOfType<Templar.PaletteSelector>().GetCurrentRamp();
        mat.SetTexture("_RampTex", tex);
    }
}