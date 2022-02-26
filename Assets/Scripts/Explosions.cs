using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosions : MonoBehaviour
{

    private void OnEnable()
    {
        float delay;
        foreach(Transform child in transform)
        {
            
            delay = Random.Range(0f, 2f);
            StartCoroutine(Launch(child, delay));

        }
    }

    private void OnDisable()
    {
        foreach (Transform child in transform)
        {

            child.gameObject.SetActive(false);

        }
    }

    private IEnumerator Launch(Transform child, float delay)
    {

        yield return new WaitForSeconds(delay);
        child.gameObject.SetActive(true);

    }
}
