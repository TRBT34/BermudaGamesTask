using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") // STUDY VE GAMES duvarlarýndan + veya - puan aldýktan sonra objeleri tamamen ortadan kaldýrmak için kullanýlýr.
        {
            Destroy(gameObject);
        }
    }
}
