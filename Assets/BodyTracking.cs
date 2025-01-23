using AvatarStstem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BodyTracking : MonoBehaviour
{
    [SerializeField] private ARHumanBodyManager _bodyManager;
    [SerializeField] private StudioAvatar _avatar;
    [SerializeField] private TMP_Text _logHeader;
    [SerializeField] private TMP_Text _logDetail;
    [SerializeField] private TMP_Text _logResult;

    private void OnEnable()
    {
        _bodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    private void OnDisable()
    {
        _bodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }

    private void OnHumanBodiesChanged( ARHumanBodiesChangedEventArgs args )
    {
        foreach( var body in args.added )
        {
            var bodyDetectedLog = $"New body detected : { body.trackableId}";
            _logHeader.text = bodyDetectedLog;
            var spine = body.joints[( int )HumanBodyBones.Spine];

            // 회전 데이터 출력
            var posLog = $"Spine Position: {spine.anchorPose.position}\nSpine Rotation: { spine.anchorPose.rotation}";
            _logDetail.text = posLog;
            UpdateBodyDirection( spine );
        }
    }
    private void UpdateBodyDirection( XRHumanBodyJoint spine )
    {
        // Live2D 연동: BodyAngleX, Y, Z
        var bodyAngleX = spine.anchorPose.rotation.eulerAngles.x;
        var bodyAngleY = spine.anchorPose.rotation.eulerAngles.y;
        var bodyAngleZ = spine.anchorPose.rotation.eulerAngles.z;
        _avatar.SetBodyAngleX( bodyAngleX );
        _avatar.SetBodyAngleY( bodyAngleY );
        _avatar.SetBodyAngleZ( bodyAngleZ );
    }
}
