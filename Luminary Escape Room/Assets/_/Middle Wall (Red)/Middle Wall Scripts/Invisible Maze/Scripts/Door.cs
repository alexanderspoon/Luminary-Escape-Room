using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool open = false;
    [SerializeField] private GameObject invisibleMaze;
    [SerializeField] private GameObject spellScroll;

    void Update()
    {
        if (open == true)
        {
            spellScroll.SetActive(true);
            invisibleMaze.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            open = true;
        }
    }
}
