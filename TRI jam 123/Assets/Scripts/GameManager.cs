using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // UI references
    [SerializeField]
    private TextMeshProUGUI strawberryText;
    [SerializeField]
    private TextMeshProUGUI blueberryText;
    [SerializeField]
    private TextMeshProUGUI grapeText;
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private TextMeshProUGUI gameOverText;

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
    public readonly float BOX_LIMIT = -2.5f;

    // Recipe info
    public int[] recipe;

    // Timer info
    private float timer = 0;
    private bool timer_running = false;

    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        rocks = GameObject.FindGameObjectsWithTag("Rock");
        Debug.Log(rocks.Length);

        SpawnMissingFruit();
        SetRecipe();
        SetTimer();
    }

    // Update is called once per frame
    void Update() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);


        if (Input.GetMouseButtonDown(0)) {
            cursor_state = CURSOR_STATE_DRAGGING;

            RaycastHit2D[] hit = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

            GameObject found = null;
            for(int i = 0; i < hit.Length; i++) {
                if(hit[i].collider.CompareTag("Fruit")) {
                    found = hit[i].collider.gameObject;
                } else if(hit[i].collider.CompareTag("Rock")) {
                    found = hit[i].collider.gameObject;
                    break;
                }
            }

            if (found != null) {
                held_go = found;
                to_go_held = held_go.transform.position - mousePos;

                held_go.GetComponent<NPC>().OnClicked();
            }
        }

        if(cursor_state == CURSOR_STATE_DRAGGING) {
            if(held_go != null) {
                held_go.transform.position = mousePos + to_go_held;

                if (held_go.CompareTag("Rock") && IsInBox(held_go.transform)) {
                    held_go.transform.position = new Vector3(held_go.transform.position.x, BOX_LIMIT, 0);
                }
            }
        }

        if(cursor_state == CURSOR_STATE_DRAGGING && Input.GetMouseButtonUp(0)) {
            cursor_state = CURSOR_STATE_IDLE;

            if(held_go != null) {
                held_go.GetComponent<NPC>().OnDropped();

                held_go = null;
            }
        }
    }

    private void FixedUpdate() {
        if(timer_running && timer > 0) {
            timer -= Time.deltaTime;
            timerText.SetText(String.Format("{0}s", Mathf.FloorToInt(timer)));
        } else if(timer_running) {
            // Time ran out
            timer = 0;
            timer_running = false;

            timerText.SetText(String.Format("{0}s", Mathf.FloorToInt(timer)));
            Lose();
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
                r.GetComponent<RockSc>().fruits_under_me.Add(spawned);

                fruit[i]++;
            }
        }

    }

    public GameObject GetRandomRock() {
        GameObject chosen = rocks[UnityEngine.Random.Range(0, rocks.Length)];

        while(chosen.GetComponent<RockSc>().held)
            chosen = rocks[UnityEngine.Random.Range(0, rocks.Length)];  // choose again

        return chosen;
    }

    public bool IsInBox(Transform t) {
        // TODO: fix
        return t.position.y <= BOX_LIMIT;
    }

    private void SetRecipe() {
        recipe = new int[Fruit_GO_Types.Length];

        for(int i = 0; i < recipe.Length; i++) {
            recipe[i] = UnityEngine.Random.Range(0, max_fruit_number+1);
        }

        bool all_zero = true;
        for (int i = 0; i < recipe.Length; i++) {
            if(recipe[i] != 0) {
                all_zero = false;
                break;
            }
        }

        if (all_zero) SetRecipe();
        else
        {
            strawberryText.SetText("" + recipe[0] + "x<sprite name=\"strawberry\">");
            blueberryText.SetText("" + recipe[1] + "x<sprite name=\"blueberry\">");
            grapeText.SetText("" + recipe[2] + "x<sprite name=\"grape\">");
        }
    }

    public bool IsRecipeDone() {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Fruit");
        int[] num_in_box = new int[Fruit_GO_Types.Length];

        // count the fruit in the box
        foreach(GameObject go in gos) {
            FruitSc fruit = go.GetComponent<FruitSc>();

            if (fruit.is_in_box)
                num_in_box[fruit.f_type]++;
        }

        // compare recipe and box contents
        for(int i = 0; i < Fruit_GO_Types.Length; i++) {
            if(num_in_box[i] < recipe[i]) return false;
        }

        return true;
    }

    public void Win() {
        //Debug.Log("A winer is you!");
        timer_running = false;
        gameOverText.SetText("You win!");
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void Lose() {
        //Debug.Log("Ha ha, you're a lose!");
        timer_running = false;
        gameOverText.SetText("You lose!");
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    private void SetTimer() {
        int sum_fruit = 0;
        for(int i = 0; i < recipe.Length; i++) {
            sum_fruit += recipe[i];
        }
        timer = 5 * sum_fruit + 5;

        timer_running = true;
    }
}
