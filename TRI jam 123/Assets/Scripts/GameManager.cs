using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Rock info
    private GameObject[] rocks;

    // Fruit info
    public int max_fruit_number = 3;
    public GameObject[] Fruit_GO_Types;

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

        SpawnMissingFruit();
    }

    // Update is called once per frame
    void Update() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);


        if (Input.GetMouseButtonDown(0)) {
            cursor_state = CURSOR_STATE_DRAGGING;

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            Debug.Log(hit.collider);

            if(hit.collider != null && (hit.collider.CompareTag("Fruit") || hit.collider.CompareTag("Rock"))) {
                Debug.Log("hit");

                held_go = hit.collider.gameObject;
                to_go_held = held_go.transform.position - mousePos;

                // TODO: call onclicked
            }
        }

        if(cursor_state == CURSOR_STATE_DRAGGING) {
            if(held_go != null) {
                held_go.transform.position = mousePos + to_go_held;
            }
        }

        if(cursor_state == CURSOR_STATE_DRAGGING && Input.GetMouseButtonUp(0)) {
            cursor_state = CURSOR_STATE_IDLE;

            if(held_go != null) {
                // TODO: call ondropped

                held_go = null;
            }
        }
    }

    public void SpawnMissingFruit() {
        int[] fruit = new int[Fruit_GO_Types.Length];

        // count existing fruit
        GameObject[] current_fruits = GameObject.FindGameObjectsWithTag("Fruit");
        for (int i = 0; i < current_fruits.Length; i++) {
            fruit[current_fruits[i].GetComponent<FruitSc>().f_type]++;
        }

        // add missing fruit
        for (int i = 0; i < fruit.Length; i++) {
            while(fruit[i] < max_fruit_number) {
                // spawn fruit behind rock
                GameObject r = GetRandomRock();
                GameObject spawned = Instantiate(Fruit_GO_Types[i], r.transform.position, Quaternion.identity);
                spawned.GetComponent<FruitSc>().f_type = i;

                fruit[i]++;
            }
        }

    }

    private GameObject GetRandomRock() {
        GameObject chosen = rocks[Random.Range(0, rocks.Length)];

        while(chosen.GetComponent<RockSc>().held)
            chosen = rocks[Random.Range(0, rocks.Length)];  // choose again

        return chosen;
    }
}
