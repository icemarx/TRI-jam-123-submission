using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSc : NPC
{
    public int f_type = -1;
    public bool is_in_box = false;

    public bool is_running = false;
    protected float speed = 0;
    public RockSc hiding_place;

    private void Start() {
        GM = FindObjectOfType<GameManager>();
    }

    private void FixedUpdate() {
        if(is_running) {
            Vector3 diff_vec = hiding_place.transform.position - transform.position;
            transform.position += (diff_vec).normalized * Mathf.Min(diff_vec.magnitude, speed);
        }
    }

    public override void OnClicked() {
        is_running = false;

        GetComponent<Collider2D>().enabled = false;
    }

    public override void OnDropped() {
        GetComponent<Collider2D>().enabled = true;
        
        is_in_box = GM.IsInBox(transform);
        
        Run();

        if (GM.IsRecipeDone()) GM.Win();
    }

    public void Run() {
        if(!is_in_box) {
            is_running = true;
            hiding_place = GM.GetRandomRock().GetComponent<RockSc>();
            hiding_place.fruits_under_me.Add(gameObject);

        }
    }
}
