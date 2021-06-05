using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Rock info
    private GameObject[] rocks;

    // Fruit info

    // Cursor info
    private readonly int CURSOR_STATE_IDLE = 0;
    private readonly int CURSOR_STATE_CLICKED = 1;
    private readonly int CURSOR_STATE_DRAGGING = 2;
    private int cursor_state = 0;
    private Vector3 to_go_held = Vector2.zero;
    private GameObject held_go = null;

    // Start is called before the first frame update
    void Start()
    {
        rocks = GameObject.FindGameObjectsWithTag("Rock");
        Debug.Log(rocks.Length);
    }

    // Update is called once per frame
    void Update() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);


        if (Input.GetMouseButtonDown(0)) {
            cursor_state = CURSOR_STATE_DRAGGING;

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if(hit.collider != null && (hit.collider.CompareTag("Fruit") || hit.collider.CompareTag("Rock"))) {
                Debug.Log("hit");

                held_go = hit.collider.gameObject;
                to_go_held = held_go.transform.position - mousePos;
            }
        }

        if(cursor_state == CURSOR_STATE_DRAGGING) {
            if(held_go != null) {
                held_go.transform.position = mousePos + to_go_held;
            }
        }

        if(cursor_state == CURSOR_STATE_DRAGGING && Input.GetMouseButtonUp(0)) {
            cursor_state = CURSOR_STATE_IDLE;

            held_go = null;
        }
    }
}
