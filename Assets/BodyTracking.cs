using AvatarStstem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Linq;

public class BodyTracking : MonoBehaviour
{
    [SerializeField] private ARHumanBodyManager _bodyManager;
    [SerializeField] private StudioAvatar _avatar;
    [SerializeField] private TMP_Text _logHeader;
    [SerializeField] private TMP_Text _logDetail;
    [SerializeField] private TMP_Text _logResult;
    [SerializeField] private GameObject _flagARKit;
    [SerializeField] private GameObject _bodyFlag2;

    private void Awake()
    {
        _flagARKit.SetActive( false );
        _bodyFlag2.SetActive( false );

        var descriptors = new List<XRSessionSubsystemDescriptor>();
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
            bodyMessage1 = "ARKit Supported.";
        }
        else
        {
            bodyMessage1 = "ARKit Not Supported";
            _flagARKit.SetActive( true );
            return;
        }
        Debug.Log( bodyMessage1 );
        _logHeader.text = bodyMessage1;

        var bodyMessage2 = string.Empty;
        if( _bodyManager != null && _bodyManager.subsystem != null && _bodyManager.subsystem.running )
        {
            bodyMessage2 = "ARKit Body Tracking supported";
        }
        else
        {
            bodyMessage2 = "ARKit Body Tracking Not supported";
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
            Debug.Log( "ARKit Body Tracking Running" );
        }
        else
        {
            Debug.Log( "ARKit Body Tracking Not Running" );
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
        var bodyList = args.added;
        bodyList.AddRange( args.updated );
        bodyList.Distinct();
        foreach( var body in bodyList )
        {
            ProcessBody( body );
        }
    }

    private void ProcessBody( ARHumanBody body )
    {
        var bodyDetectedLog = $"Body : {body.trackableId}";
        _logHeader.text = bodyDetectedLog;

        int hipsIndex = ( int )HumanBodyBones.Hips;
        if( hipsIndex >= 0 && hipsIndex < body.joints.Length )
        {
            var hips = body.joints[hipsIndex];
            if( hips.tracked )
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
