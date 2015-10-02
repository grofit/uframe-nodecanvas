using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("Example Actions")]
[Name("ChangeColourWithHealth")]
[RequireComponent(typeof(Renderer))]
[AgentType(typeof(Renderer))]
public class ChangeColourWithHealthAction : ActionTask
{
    [RequiredField]
    public BBParameter<int> Health;

    [GetFromAgent]
    private Renderer _renderer;

    protected override void OnExecute()
    {
        base.OnExecute();

        var healthClamp = (float)Health.value / 100.0f;
        var healthColor = new Color(healthClamp, 0.0f, 0.0f);

        _renderer.material.color = healthColor;
        EndAction(true);
    }
}
