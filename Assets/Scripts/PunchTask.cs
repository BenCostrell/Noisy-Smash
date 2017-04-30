using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchTask : Task {

    private float timeElapsed;
    private float duration;
    private float activeDuration;
    private Player player;
    private bool punchActivated;

    public PunchTask(Player pl, float dur, float activeDur)
    {
        player = pl;
        duration = dur;
        activeDuration = activeDur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        player.Unactionable();
        player.ActivatePunch();
        punchActivated = true;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= activeDuration && punchActivated)
        {
            player.DeactivatePunch();
            punchActivated = false;
        }

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        player.Actionable();
    }
}
