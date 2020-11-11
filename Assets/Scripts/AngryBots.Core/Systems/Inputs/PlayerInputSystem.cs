using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AngryBots2.Core.Inputs.Systems {

    public class PlayerInputSystem : SystemBase {

        const string Horizontal = "Horizontal";
        const string Vertical = "Vertical";

        protected override void OnUpdate() {
            Entities.ForEach((ref InputContainer c0) => {
                float x = Input.GetAxis(Horizontal);
                float y = Input.GetAxis(Vertical);

                c0.Axis = new float2(x, y);
            }).WithoutBurst().Run();
        }
    }
}