using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrawberrySc : FruitSc {
    public float my_speed = 1;

    private void Start() {
        GM = FindObjectOfType<GameManager>();
        speed = my_speed;
    }
}
