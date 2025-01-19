using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using Unity.Collections;
using TMPro;
using AvatarStstem;
using Live2D.Cubism.Core;

public class FaceTracking : MonoBehaviour
{
    [SerializeField] private ARFaceManager faceManager;
    [SerializeField] private TMP_Text _logDetail;
    [SerializeField] private TMP_Text _logResult;
    [SerializeField] private TMP_Text _log3;
    [SerializeField] private StudioAvatar _avatar;

    private ARKitFaceSubsystem _faceSubsystem;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        _logDetail.text = Vector3.zero.ToString();
        _logResult.text = Vector3.zero.ToString();
    }

    private void OnEnable()
    {
        faceManager.facesChanged += OnFaceChanged;
    }

    private void OnDisable()
    {
        faceManager.facesChanged -= OnFaceChanged;
    }

    /// <summary>
    /// 얼굴 정보 변경 이벤트
    /// </summary>
    /// <param name="eventArgs">발생한 이벤트 값</param>
    private void OnFaceChanged( ARFacesChangedEventArgs eventArgs )
    {
        if( eventArgs.updated.Count != 0 )
        {
            var arFace = eventArgs.updated[0];
            if( arFace.trackingState == TrackingState.Tracking
                && ( ARSession.state > ARSessionState.Ready ) )
            {
                UpdateFaceTransform( arFace );
                UpdateEyeBlendShape( arFace );
                UpdateLive2DWithARKitData( arFace );
                UpdateMouthBlendShape( arFace );
            }
        }
    }

    /// <summary>
    /// 얼굴 방향dmf 변경
    /// </summary>
    /// <param name="arFace">ARFace 정보</param>
    private void UpdateFaceTransform( ARFace arFace )
    {
        // 얼굴의 위치 정보 취득
        var faceRotation = arFace.transform.rotation;

        var x = NormalizeAngle( faceRotation.eulerAngles.x ) * 2f;
        var y = NormalizeAngle( faceRotation.eulerAngles.y );
        var z = NormalizeAngle( faceRotation.eulerAngles.z ) * 2f;

        // 새로운 얼굴 회전값을 변수에 대입
        _avatar.SetFaceAngleX( y );
        _avatar.SetFaceAngleY( x );
        _avatar.SetFaceAngleZ( z );
    }

    /// <summary>
    /// 표정 정보를 변경
    /// </summary>
    /// <param name="arFace">ARFace 정보</param>
    private void UpdateEyeBlendShape( ARFace arFace )
    {
        _faceSubsystem = ( ARKitFaceSubsystem )faceManager.subsystem;
        using var blendShapesARKit = _faceSubsystem.GetBlendShapeCoefficients( arFace.trackableId, Allocator.Temp );

        for(var i=0; i<blendShapesARKit.Length; i++ )
        {
            switch( blendShapesARKit[i].blendShapeLocation )
            {
                case ARKitBlendShapeLocation.EyeBlinkLeft:
                    _avatar.SetEyeBlinkLeft( 1 - blendShapesARKit[i].coefficient );
                    break; ;
                case ARKitBlendShapeLocation.EyeBlinkRight:
                    _avatar.SetEyeBlinkRight( 1 - blendShapesARKit[i].coefficient );
                    break;

                //case ARKitBlendShapeLocation.EyeLookInLeft:
                //    leftEyeIn = blendShapesARKit[i].coefficient;
                //    break;
                //case ARKitBlendShapeLocation.EyeLookInRight:
                //    rightEyeIn = blendShapesARKit[i].coefficient;
                //    break;
                //case ARKitBlendShapeLocation.EyeLookOutLeft:
                //    leftEyeOut = blendShapesARKit[i].coefficient;
                //    break;
                //case ARKitBlendShapeLocation.EyeLookOutRight:
                //    rightEyeOut = blendShapesARKit[i].coefficient;
                //    break;
                case ARKitBlendShapeLocation.EyeLookUpLeft:
                case ARKitBlendShapeLocation.EyeLookUpRight:
                    _avatar.SetEyeLookVertical( -blendShapesARKit[i].coefficient );
                    break;
                case ARKitBlendShapeLocation.EyeLookDownLeft:
                case ARKitBlendShapeLocation.EyeLookDownRight:
                    _avatar.SetEyeLookVertical( blendShapesARKit[i].coefficient );
                    break;
            }
        }
    }

    private void UpdateLive2DWithARKitData( ARFace arFace )
    {
        var eyeLookInLeft = 0f;
        var eyeLookOutLeft = 0f;
        var eyeLookInRight = 0f;
        var eyeLookOutRight = 0f;

        if( arFace == null || _avatar == null ) return;

        // ARKit의 BlendShape 데이터 가져오기
        var blendShapes = _faceSubsystem = ( ARKitFaceSubsystem )faceManager.subsystem;
        using var blendShapesARKit = _faceSubsystem.GetBlendShapeCoefficients( arFace.trackableId, Allocator.Temp );

        for( var i = 0; i<blendShapesARKit.Length; i++ )
        {
            switch( blendShapesARKit[i].blendShapeLocation )
            {
                case ARKitBlendShapeLocation.EyeLookInLeft:
                    eyeLookInLeft = blendShapesARKit[i].coefficient;
                    break;
                case ARKitBlendShapeLocation.EyeLookInRight:
                    eyeLookInRight = blendShapesARKit[i].coefficient;
                    break;
                case ARKitBlendShapeLocation.EyeLookOutLeft:
                    eyeLookOutLeft = blendShapesARKit[i].coefficient;
                    break;
                case ARKitBlendShapeLocation.EyeLookOutRight:
                    eyeLookOutRight = blendShapesARKit[i].coefficient;
                    break;
            }
        }
        var eyeBallXValue = ( eyeLookOutLeft - eyeLookInLeft + eyeLookInRight - eyeLookOutRight ) / 2.0f;
        var xValue = Mathf.Clamp( eyeBallXValue, -1.0f, 1.0f );
        var lol = string.IsNullOrEmpty( eyeLookInLeft.ToString( "F3" ) ) ? "0.0" : eyeLookInLeft.ToString( "F3" );
        var lil = string.IsNullOrEmpty( eyeLookInLeft.ToString( "F3" ) ) ? "0.0" : eyeLookInLeft.ToString( "F3" );
        var lor = string.IsNullOrEmpty( eyeLookOutRight.ToString( "F3" ) ) ? "0.0" : eyeLookOutRight.ToString( "F3" );
        var lir = string.IsNullOrEmpty( eyeLookInRight.ToString( "F3" ) ) ? "0.0" : eyeLookInRight.ToString( "F3" );
        var xv = string.IsNullOrEmpty( xValue.ToString( "F3" ) ) ? "0.0" : xValue.ToString( "F3" );
        var result = string.IsNullOrEmpty( xValue.ToString( "F3" ) ) ? "0.0" : xValue.ToString( "F3" );
        _logDetail.text = $"O_L : {lol}, I_L : {lil}\nO_R : {lor}, I_R : {lir}";
        _logResult.text = $"xValue = {xv}, result = {result} ";
        _avatar.SetEyeLookHorizontal( xValue );
    }

    /// <summary>
    /// 표정 정보를 변경
    /// </summary>
    /// <param name="arFace">ARFace 정보</param>
    private void UpdateMouthBlendShape( ARFace arFace )
    {
        _faceSubsystem = ( ARKitFaceSubsystem )faceManager.subsystem;
        using var blendShapesARKit = _faceSubsystem.GetBlendShapeCoefficients( arFace.trackableId, Allocator.Temp );
        for( var i = 0; i<blendShapesARKit.Length; i++ )
        {
            switch( blendShapesARKit[i].blendShapeLocation )
            {
                case ARKitBlendShapeLocation.MouthFunnel:
                    _avatar.SetMouthForm( 1 - blendShapesARKit[i].coefficient * 2f );
                    break;
                case ARKitBlendShapeLocation.JawOpen:
                    _avatar.SetMouthOpen( blendShapesARKit[i].coefficient * 1.8f );
                    break;
            }
        }
    }

    /// <summary>
    /// 얼굴 각도를 정규화하는 함수
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private float NormalizeAngle( float angle )
    {
        if( angle > 180 )
        {
            return angle - 360;
        }
        return angle;
    }
}

