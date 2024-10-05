using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected BaseState currentState;
    
    // Start is called before the first frame update
    void Start() {
        currentState = GetInitialState();
    }

    // Update is called once per frame
    void Update() {
        if (currentState != null)
            currentState.UpdateLogic();
    }

    private void LateUpdate() {
        if (currentState != null)
            currentState.UpdatePhysics();
        
    }
    
    public void ChangeState(BaseState newState) {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
    
    protected virtual BaseState GetInitialState() {
        return null;
    }

    private void OnGUI() {
        string content = currentState != null ? currentState.name : "none";
        GUI.Label(new Rect(0, 0, 100, 50), content);
    }
}
