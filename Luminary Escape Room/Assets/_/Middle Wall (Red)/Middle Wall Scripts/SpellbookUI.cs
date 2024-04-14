using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellbookUI : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject spellbookCanvas;
    [SerializeField] private GameObject openBook;

    private void Awake()
    {
        spellbookCanvas.SetActive(false);
        openBook.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            //add object to inventory if it has the ItemPickable script component and it will be destroyed
            if (Physics.Raycast(ray, out hitInfo))
            {
                if(hitInfo.collider.GetComponent<SpellbookUI>()  != null)
                {
                    spellbookCanvas.SetActive(true);
                    openBook.SetActive(true);
                }
            }
        }
    }

    public void DismissButton()
    {
        spellbookCanvas.SetActive(false);
        openBook.SetActive(false);
    }
}