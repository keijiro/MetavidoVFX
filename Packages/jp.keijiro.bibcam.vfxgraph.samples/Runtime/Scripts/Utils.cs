namespace Bibcam {

public static class CdsTween
{
    public static (float x, float v)
      Step((float x, float v) state, float target, float speed)
      => Step(state, target, speed, UnityEngine.Time.deltaTime);

    public static (float x, float v)
      Step((float x, float v) state, float target, float speed, float dt)
    {
        var n1 = state.v - (state.x - target) * (speed * speed * dt);
        var n2 = 1 + speed * dt;
        var nv = n1 / (n2 * n2);
        return (state.x + nv * dt, nv);
    }
}

} // namespace Bibcam
