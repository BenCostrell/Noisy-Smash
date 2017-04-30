using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTask : Task {

    private float timeElapsed;
    private float duration;
    private Player player;

    public StunTask(Player pl, float dur)
    {
        player = pl;
        duration = dur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        player.Unactionable();
        player.GetComponent<SpriteRenderer>().color = Color.red;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        player.Actionable();
        player.GetComponent<SpriteRenderer>().color = Services.GameManager.playerColors[player.playerNum - 1];
    }
}
