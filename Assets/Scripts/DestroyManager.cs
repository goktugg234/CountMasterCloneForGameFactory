using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyManager : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(1f);
        gameObject.SetActive(false);
    }
}