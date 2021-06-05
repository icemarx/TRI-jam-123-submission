using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSc : NPC {
    public bool held = false;

    public override void OnClicked() {
        held = true;
    }

    public override void OnDropped() {
        held = false;
    }
}
