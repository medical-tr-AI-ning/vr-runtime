using UnityEngine;
using Valve.VR;

/// <summary>
/// https://docs.unity3d.com/Manual/ExecutionOrder.html
/// Determines in 2 consecutive Fixed Update Cycles if the Trigger Collider(s) on the attached GameObject are overlapping with a MeshCollider.
/// 
/// 1. Fixed Update Cycle -> Clear all known Collisions
/// 2. The OnTriggerXXX Events are handled -> Check if we are colliding with any MeshColliders
/// 3. Fixed Update Cycle -> Check if we were colliding
///                                 yes -> Fade to Color
///                                 no  -> Fade to Clear
///                       -> Clear all known Collisions
///                       -> goto 2.
/// </summary>

public class FadeCamera : MonoBehaviour
{
    [SerializeField]
    private float _fadeDuration = 0.2f;
    [SerializeField]
    private Color _fadeColor = Color.black;
    [SerializeField]
    private LayerMask _layersToIgnore;

    private bool _isColliding = false;
    private bool _wasCollidingLastCycle = false;

    public float FadeDuration
    {
        get { return _fadeDuration; }
        set { _fadeDuration = value; }
    } 

    public Color FadeColor
    {
        get { return _fadeColor; }
        set { _fadeColor = value; }
    }

    public LayerMask LayersToIgnore
    {
        get { return _layersToIgnore; }
        set { _layersToIgnore = value; }
    }

    // is run before OnTriggerXXX in the same cycle
    private void FixedUpdate()
    {       
        if (_isColliding != _wasCollidingLastCycle)
        {
            if (_isColliding)
            {
                FadeFromClear();
            }
            else
            {
                FadeToClear();
            }
        }
        _wasCollidingLastCycle = _isColliding;
        _isColliding = false;
    }

    // is run after FixedUpdate in the same cycle
    private void OnTriggerStay(Collider other)
    {
        if (other is MeshCollider && _layersToIgnore != (_layersToIgnore | (1 << other.gameObject.layer)))
        {
            _isColliding = true;
        }
    } 
    
    public virtual void FadeFromClear()
    {
        // fade to set color
        SteamVR_Fade.View(_fadeColor, _fadeDuration);
    }
    public virtual void FadeToClear()
    {
        // fade to clear view
        SteamVR_Fade.View(Color.clear, _fadeDuration);
    }
}