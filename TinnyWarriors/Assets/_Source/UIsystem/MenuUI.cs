using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Popup canvasMenuUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            canvasMenuUI.gameObject.SetActive(true);
            canvasMenuUI.Show();
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            canvasMenuUI.Hide();
        }    
    }
}
