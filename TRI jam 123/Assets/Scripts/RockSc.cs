using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSc : NPC {
    public bool held = false;

    public List<GameObject> fruits_under_me = new List<GameObject>();

    private void Start() {
        GM = FindObjectOfType<GameManager>();
    }

    public override void OnClicked() {
        held = true;

        foreach(GameObject go in fruits_under_me) {
            go.GetComponent<FruitSc>().Run();
        }

        fruits_under_me.Clear();
    }

    public override void OnDropped() {
        held = false;
    }
}
