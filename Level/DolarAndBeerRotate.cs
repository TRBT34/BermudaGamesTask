using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DolarAndBeerRotate : MonoBehaviour
{
    float TurnSpeed = 5f;
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, 10f) * TurnSpeed * Time.deltaTime); // Level sahnesindeki dolarlarýn kendi ekrafýnda dönmesi için kullanýlan kod.
        
    }
}
