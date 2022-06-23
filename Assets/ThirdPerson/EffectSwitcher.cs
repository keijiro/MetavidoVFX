using UnityEngine;

namespace Bibcam {

public sealed class EffectSwitcher : MonoBehaviour
{
    #region Scene object references

    [SerializeField] GameObject[] _effectRoots = null;

    #endregion

    #region Public method (switcher method)

    public void SwitchEffect(int index)
    {
        for (var i = 0; i < _effectRoots.Length; i++)
            _effectRoots[i].SetActive(index == i);
    }

    #endregion
}

} // namespace Bibcam
