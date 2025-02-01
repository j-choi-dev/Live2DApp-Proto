using AvatarStstem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARSubsystems;

public class BodyTrackingTest : MonoBehaviour
{
    [SerializeField] private ARHumanBodyManager _bodyManager;
    [SerializeField] private StudioAvatar _avatar;
    [SerializeField] private TMP_Text _logHeader;
    [SerializeField] private TMP_Text _logDetail;
    [SerializeField] private TMP_Text _logResult;
    [SerializeField] private GameObject _bodyFlag1;
    [SerializeField] private GameObject _bodyFlag2;

    private Quaternion _initialRotation = Quaternion.identity;
    private bool _initialized = false;

    private void Awake()
    {
        _bodyFlag1.SetActive(false);
        _bodyFlag2.SetActive( false );

        // 현재 사용 가능한 XRSessionSubsystem 찾기
        List<XRSessionSubsystemDescriptor> descriptors = new List<XRSessionSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors( descriptors );

        foreach( var descriptor in descriptors )
        {
            Debug.Log( $"[XRSessionSubsystem] {descriptor.id}" );
        }

        // ARKit이 지원되는지 확인
        XRSessionSubsystem sessionSubsystem = null;
        foreach( var descriptor in descriptors )
        {
            if( descriptor.id == "ARKit-Session" )
            {
                sessionSubsystem = descriptor.Create();
                break;
            }
        }
        var bodyMessage1 = string.Empty;
        if( sessionSubsystem != null )
        {
            bodyMessage1 = "ARKit을 지원합니다.";
        }
        else
        {
            bodyMessage1 = "ARKit을 지원하지 않는 기기입니다.";
            _bodyFlag1.SetActive( true );
            return;
        }
        Debug.Log( bodyMessage1 );
        _logHeader.text = bodyMessage1;

        // 🔹 ARHumanBodyManager를 이용하여 Body Tracking 지원 여부 확인
        var bodyMessage2 = string.Empty;
        if( _bodyManager != null && _bodyManager.subsystem != null && _bodyManager.subsystem.running )
        {
            bodyMessage2 = "ARKit Body Tracking을 지원합니다!";
        }
        else
        {
            bodyMessage2 = "ARKit Body Tracking을 지원하지 않습니다.";
            _bodyFlag2.SetActive( true );
        }
        Debug.Log( bodyMessage2 );
        _logHeader.text += $"\n{bodyMessage2}";
    }

    private void Start()
    {
        CheckARKitBodyTracking();
    }
    private void CheckARKitBodyTracking()
    {
        if( _bodyManager.subsystem != null && _bodyManager.subsystem.running )
        {
            Debug.Log( "✅ ARKit Body Tracking이 실행 중입니다!" );
        }
        else
        {
            Debug.Log( "❌ ARKit Body Tracking이 실행되지 않았습니다." );
        }
    }

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
        foreach( var body in args.updated )  // 변경된 데이터도 체크
        {
            ProcessBody( body );
        }
    }

    private void ProcessBody( ARHumanBody body )
    {
        var bodyDetectedLog = $"Body detected: {body.trackableId}";
        _logHeader.text = bodyDetectedLog;

        // ARHumanBody에는 TryGetJoint가 없으므로, joints 배열에서 해당 관절을 직접 가져온다.
        int hipsIndex = ( int )HumanBodyBones.Hips; // Unity의 HumanBodyBones Enum을 사용
        if( hipsIndex >= 0 && hipsIndex < body.joints.Length )
        {
            var hips = body.joints[hipsIndex];
            if( hips.tracked ) // 해당 관절이 추적되고 있는지 확인
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

        // 초기 자세 설정 (첫 프레임에서 초기화)
        if( !_initialized )
        {
            _initialRotation = currentRotation;
            _initialized = true;
        }

        // 상대 회전 계산
        Quaternion relativeRotation = Quaternion.Inverse( _initialRotation ) * currentRotation;
        Vector3 eulerAngles = relativeRotation.eulerAngles;

        // X, Y, Z 회전값을 Live2D에 반영
        float bodyAngleX = eulerAngles.x > 180 ? eulerAngles.x - 360 : eulerAngles.x;
        float bodyAngleY = eulerAngles.y > 180 ? eulerAngles.y - 360 : eulerAngles.y;
        float bodyAngleZ = eulerAngles.z > 180 ? eulerAngles.z - 360 : eulerAngles.z;

        _avatar.SetBodyAngleX( bodyAngleX );
        _avatar.SetBodyAngleY( bodyAngleY );
        _avatar.SetBodyAngleZ( bodyAngleZ );

        _logResult.text = $"BodyAngleX: {bodyAngleX}\nBodyAngleY: {bodyAngleY}\nBodyAngleZ: {bodyAngleZ}";
    }
}
