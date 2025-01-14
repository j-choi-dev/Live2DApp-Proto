using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using Unity.Collections;
using TMPro;
using AvatarStstem;
using System;

public class FaceTracking : MonoBehaviour
{
    [SerializeField] private ARFaceManager faceManager;
    [SerializeField] private TMP_Text _log;
    [SerializeField] private StudioAvatar _avatar;

    private ARKitFaceSubsystem _faceSubsystem;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        _log.text = Vector3.zero.ToString();
        Debug.Log( Vector3.zero );
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

        var x = NormalizeAngle( faceRotation.eulerAngles.x ) * 2f * -1f;
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
        var msg = string.Empty;
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
                    _avatar.SetEyeBlinkLeft( 1 - blendShapesARKit[i].coefficient );
                    break;
                case ARKitBlendShapeLocation.EyeLookInLeft:
                    _avatar.SetEyeLookHorizontal( -blendShapesARKit[i].coefficient );
                    msg += $"Look_L : {-blendShapesARKit[i].coefficient}";
                    break;
                case ARKitBlendShapeLocation.EyeLookInRight:
                    _avatar.SetEyeLookHorizontal( blendShapesARKit[i].coefficient );
                    msg += $"Look_R : {blendShapesARKit[i].coefficient}";
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
        _log.text = $"{msg}";
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

