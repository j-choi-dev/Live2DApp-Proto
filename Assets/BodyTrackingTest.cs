using AvatarStstem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BodyTrackingTest : MonoBehaviour
{
    [SerializeField] private ARHumanBodyManager _bodyManager;
    [SerializeField] private StudioAvatar _avatar;
    [SerializeField] private TMP_Text _logHeader;
    [SerializeField] private TMP_Text _logDetail;
    [SerializeField] private TMP_Text _logResult;

    private Quaternion _initialRotation = Quaternion.identity;
    private bool _initialized = false;

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
            ProcessBody( body );
        }
        foreach( var body in args.updated )  // ����� �����͵� üũ
        {
            ProcessBody( body );
        }
    }

    private void ProcessBody( ARHumanBody body )
    {
        var bodyDetectedLog = $"Body detected: {body.trackableId}";
        _logHeader.text = bodyDetectedLog;

        // ARHumanBody���� TryGetJoint�� �����Ƿ�, joints �迭���� �ش� ������ ���� �����´�.
        int hipsIndex = ( int )HumanBodyBones.Hips; // Unity�� HumanBodyBones Enum�� ���
        if( hipsIndex >= 0 && hipsIndex < body.joints.Length )
        {
            var hips = body.joints[hipsIndex];
            if( hips.tracked ) // �ش� ������ �����ǰ� �ִ��� Ȯ��
            {
                var posLog = $"Hips Position: {hips.anchorPose.position}\nHips Rotation: {hips.anchorPose.rotation.eulerAngles}";
                _logDetail.text = posLog;

                UpdateBodyDirection( hips );
            }
            else
            {
                _logDetail.text = "Hips not tracked.";
            }
        }
        else
        {
            _logDetail.text = "Hips index out of range.";
        }
    }


    private void UpdateBodyDirection( XRHumanBodyJoint hips )
    {
        Quaternion currentRotation = hips.anchorPose.rotation;

        // �ʱ� �ڼ� ���� (ù �����ӿ��� �ʱ�ȭ)
        if( !_initialized )
        {
            _initialRotation = currentRotation;
            _initialized = true;
        }

        // ��� ȸ�� ���
        Quaternion relativeRotation = Quaternion.Inverse( _initialRotation ) * currentRotation;
        Vector3 eulerAngles = relativeRotation.eulerAngles;

        // X, Y, Z ȸ������ Live2D�� �ݿ�
        float bodyAngleX = eulerAngles.x > 180 ? eulerAngles.x - 360 : eulerAngles.x;
        float bodyAngleY = eulerAngles.y > 180 ? eulerAngles.y - 360 : eulerAngles.y;
        float bodyAngleZ = eulerAngles.z > 180 ? eulerAngles.z - 360 : eulerAngles.z;

        _avatar.SetBodyAngleX( bodyAngleX );
        _avatar.SetBodyAngleY( bodyAngleY );
        _avatar.SetBodyAngleZ( bodyAngleZ );

        _logResult.text = $"BodyAngleX: {bodyAngleX}\nBodyAngleY: {bodyAngleY}\nBodyAngleZ: {bodyAngleZ}";
    }
}
