using UnityEngine;

public class HeadLookOnly : MonoBehaviour
{
    public Transform centerEyeAnchor; // OVRCameraRig“à‚ÌCenterEyeAnchor
    private Vector3 fixedLocalPosition;

    void Start()
    {
        fixedLocalPosition = centerEyeAnchor.localPosition;
    }

    void LateUpdate()
    {
        // ˆÊ’u‚ÍŒÅ’èA‰ñ“]‚¾‚¯HMD‚Ì“®‚«‚ğ”½‰f
        centerEyeAnchor.localPosition = fixedLocalPosition;
    }
}