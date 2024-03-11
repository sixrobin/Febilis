using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9)
        {
            transform.parent.gameObject.SetActive(false);
            FindObjectOfType<Templar.Camera.CameraController>().GetShake(Templar.Camera.CameraShake.ID_BIG).AddTrauma(1f, 1f);
        }
    }
}
