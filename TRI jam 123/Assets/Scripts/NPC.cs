using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    public GameManager GM;

    private void Start() {
        GM = FindObjectOfType<GameManager>();
    }

    public abstract void OnClicked();

    public abstract void OnDropped();
}
