using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") // STUDY VE GAMES duvarlar�ndan + veya - puan ald�ktan sonra objeleri tamamen ortadan kald�rmak i�in kullan�l�r.
        {
            Destroy(gameObject);
        }
    }
}
