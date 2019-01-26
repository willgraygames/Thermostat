using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenOrClose : MonoBehaviour
{
    public GameObject openPanel = null;

    private bool _isInsideTrigger = false;

    public Animator _animator;

    public string OpenText = "Press 'E' to open ";

    public string CloseText = "Press 'E' to close";

    private bool _isOpen = false;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _isInsideTrigger = true;
            openPanel.SetActive(true);
            UpdatePanelText();
        }
    }

    void OnTriggerExit(Collider other)
    {
     if(other.tag == "Player")
        {
            _isInsideTrigger = false;
            openPanel.SetActive(false);
        }   
    }
    private bool IsOpenPanelActive
    {
        get
        {
            return openPanel.activeInHierarchy;
        }
    }

    private void UpdatePanelText()
    {
        Text panelText = openPanel.transform.Find("Text").GetComponent<Text>();
        if(panelText != null)
        {
            panelText.text = _isOpen ? CloseText : OpenText; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOpenPanelActive && _isInsideTrigger)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _isOpen = !_isOpen;

                Invoke("UpdatePanelText", 1.0f);

                _animator.SetBool("open", _isOpen);
            }
        }
    }
}
