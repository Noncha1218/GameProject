using UnityEngine;

public class BodyFollowHead : MonoBehaviour
{
    [SerializeField] private Transform centerEye;
    [SerializeField] private Transform body;

    private Vector3 initialBodyLocalPosition;

    void Start()
    {
        initialBodyLocalPosition = body.localPosition;
    }

    void LateUpdate()
    {
        Vector3 headOffset = centerEye.localPosition;

        // “ª‚Ì¶‰EE‘OŒã‚ÌˆÚ“®‚¾‚¯‘Ì‚É”½‰f
        body.localPosition = initialBodyLocalPosition
            + new Vector3(headOffset.x, 0f, headOffset.z);
    }
}