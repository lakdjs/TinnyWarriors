using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialoges : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private GameObject canvasToOpen;
    private bool _isInRange = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            
            Debug.Log(PlayerPrefs.GetInt("Dialoge"));
           
        }
    }

    private void OnCollisionEnter (Collision other)
    {
        if ((playerMask & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log("Ye");
            _isInRange = true;
            canvasToOpen.SetActive(true);
        }

    }
    private void OnCollisionExit(Collision other)
    {
        if ((playerMask & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log("no");
            _isInRange = false;
            canvasToOpen.SetActive(false);
        }
    }
}
