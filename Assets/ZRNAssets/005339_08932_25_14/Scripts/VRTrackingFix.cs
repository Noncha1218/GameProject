using UnityEngine;

public class VRTrackingFix : MonoBehaviour
{
    void Start()
    {
        OVRManager.instance.trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
    }
}
