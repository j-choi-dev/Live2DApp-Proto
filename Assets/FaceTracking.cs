using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using Unity.Collections;
using TMPro;
using AvatarStstem;
using Live2D.Cubism.Core;
using System.Security.AccessControl;
using Live2D.Cubism.Framework.Json;

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
                UpdateEyeBallDirection( arFace );
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

    private void UpdateEyeBallDirection( ARFace arFace )
    {
        var eyeLookInLeft = 0f;
        var eyeLookOutLeft = 0f;
        var eyeLookInRight = 0f;
        var eyeLookOutRight = 0f;

        var eyeLookUpLeft = 0f;
        var eyeLookDownLeft = 0f;
        var eyeLookUpRight = 0f;
        var eyeLookDownRight = 0f;

        if( arFace == null || _avatar == null ) return;

        // ARKit의 BlendShape 데이터 가져오기
        var blendShapes = _faceSubsystem = ( ARKitFaceSubsystem )faceManager.subsystem;
        using var blendShapesARKit = _faceSubsystem.GetBlendShapeCoefficients( arFace.trackableId, Allocator.Temp );

        for( var i = 0; i<blendShapesARKit.Length; i++ )
        {
            switch( blendShapesARKit[i].blendShapeLocation )
            {
                case ARKitBlendShapeLocation.EyeLookDownLeft:
                    eyeLookDownLeft = blendShapesARKit[i].coefficient;
                    break;
                case ARKitBlendShapeLocation.EyeLookDownRight:
                    eyeLookDownRight = blendShapesARKit[i].coefficient;
                    break;
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
                case ARKitBlendShapeLocation.EyeLookUpLeft:
                    eyeLookUpLeft = blendShapesARKit[i].coefficient;
                    break;
                case ARKitBlendShapeLocation.EyeLookUpRight:
                    eyeLookUpRight = blendShapesARKit[i].coefficient;
                    break;
            }
        }
        var eyeBallXValue = ( eyeLookOutLeft - eyeLookInLeft + eyeLookInRight - eyeLookOutRight ) / 2.0f;
        var eyeBallYValue = ( eyeLookUpLeft - eyeLookDownLeft + eyeLookUpRight - eyeLookDownRight ) / 2.0f;

        var resultX = Mathf.Clamp( eyeBallXValue, -1f, 1f );
        var resultY = Mathf.Clamp( eyeBallXValue, -1f, 1f );

        _logDetail.text = $"{GetTempLogMessageForEyeBalls( eyeLookOutLeft, eyeLookInLeft, eyeLookOutRight, eyeLookInRight )}\n{GetTempLogMessageForEyeBallResult( eyeBallXValue, resultX )}";
        _logResult.text = $"{GetTempLogMessageForEyeBalls( eyeLookUpLeft, eyeLookDownLeft, eyeLookUpRight, eyeLookDownRight )}\n{GetTempLogMessageForEyeBallResult( eyeBallYValue, resultY )}";

        _avatar.SetEyeLookHorizontal( resultX );
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

    private string GetTempLogMessageForEyeBalls(float valL1 , float valL2, float valR1, float valR2 )
        => $"外視L : {GetFormatedStringByFloat( valL1 )}, 内視L : {GetFormatedStringByFloat( valL2 )}\n外視R : {GetFormatedStringByFloat( valR1 )}, 内視R : {GetFormatedStringByFloat( valR2 )}";
    private string GetTempLogMessageForEyeBallResult( float raw, float result )
        => $"raw = {GetFormatedStringByFloat( raw )}\nresult = {GetFormatedStringByFloat( result )} ";

    private string GetFormatedStringByFloat(float val)
        => string.IsNullOrEmpty( val.ToString( "F2" ) ) ? "0.0" : val.ToString( "F2" );
}

