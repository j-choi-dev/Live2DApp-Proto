using AvatarStstem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BodyTrackingTest : MonoBehaviour
{
    [SerializeField] ARHumanBodyManager m_HumanBodyManager;
    [SerializeField] private StudioAvatar _avatar;
    [SerializeField] private TMP_Text _logHeader;
    [SerializeField] private TMP_Text _logDetail;
    [SerializeField] private TMP_Text _logResult;

    private Animator _animator; 
    
    private Dictionary<string, int> jointMapping = new Dictionary<string, int>
    {
        { "neck", 20 }, // ARKit���� �� ���� �ε���
        { "spineBase", 1 } // ô���� Base ���� �ε���
    };

    void OnEnable()
    {
        m_HumanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    void OnDisable()
    {
        if( m_HumanBodyManager != null )
        {
            m_HumanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
        }
    }

    void OnHumanBodiesChanged( ARHumanBodiesChangedEventArgs eventArgs )
    {
        if( _animator == null )
        {
            return;
        }

        foreach( var body in eventArgs.updated )
        {
            if( body != null && body.trackingState == TrackingState.Tracking )
            {
                ProcessBodyPose( body );
            }
            var bodyDetectedLog = $"New body Update : {body.trackableId}";
            _logHeader.text = bodyDetectedLog;
            var spine = body.joints[( int )HumanBodyBones.Spine];
        }
    }

    void ProcessBodyPose( ARHumanBody body )
    {
        // ���� ��ġ ��������
        Vector3? neckPosition = GetJointPosition( body, "neck" );
        Vector3? spineBasePosition = GetJointPosition( body, "spineBase" );

        if( neckPosition.HasValue && spineBasePosition.HasValue )
        {
            Vector3 neckPos = neckPosition.Value;
            Vector3 spinePos = spineBasePosition.Value;

            // ô�� ���� ���
            Vector3 spineDirection = neckPos - spinePos;

            // ���� ���
            float bodyAngleX = Vector3.Angle( Vector3.up, spineDirection ); // �յ� ����
            float bodyAngleY = Mathf.Atan2( spineDirection.x, spineDirection.z ) * Mathf.Rad2Deg; // �¿� ȸ��
            float bodyAngleZ = Mathf.Atan2( spineDirection.y, spineDirection.x ) * Mathf.Rad2Deg; // �¿� ����
            var posLog = $"{bodyAngleX}, {bodyAngleY}, {bodyAngleZ}"; 
            _logDetail.text = posLog;

            // Live2D �ƹ�Ÿ�� ����
        }
    }

    Vector3? GetJointPosition( ARHumanBody body, string jointName )
    {
        // ���ε� �ε��� ��������
        if( jointMapping.TryGetValue( jointName, out int jointIndex ) )
        {
            if( jointIndex >= 0 && jointIndex < body.joints.Length )
            {
                var joint = body.joints[jointIndex];
                if( joint.tracked )
                {
                    return joint.anchorPose.position;
                }
            }
        }
        return null;
    }
}
