using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimationRandomizer : MonoBehaviour {

    Animator animator;
    float elapsedTime = 0;
    float targetTime = 0;

    void Awake() {
        animator = GetComponent<Animator>();
    }


    void Update() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= targetTime) {
            animator.SetTrigger("glitch");
            elapsedTime = 0;
            targetTime = Random.Range(3, 5);
        }
    }
}
