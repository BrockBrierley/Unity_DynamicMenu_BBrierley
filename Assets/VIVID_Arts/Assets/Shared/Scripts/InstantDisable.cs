using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDisable : MonoBehaviour
{
    void Start()
    {
        this.gameObject.SetActive(false);
    }
}
