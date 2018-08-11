using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TimeSystem : ComponentSystem {
    public struct TimeEntities {
        public TimeComponent timeComponent;
    }
    public float time;
    public float timeScale = 1f;
    protected override void OnUpdate() {
        time += Time.deltaTime;
    }

    protected override void OnStartRunning() {
        time = 0f;
    }
}
