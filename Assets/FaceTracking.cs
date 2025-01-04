using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Live2D.Cubism.Core;
using UnityEngine.XR.ARKit;
using Unity.Collections;
using TMPro;

public class FaceTracking : MonoBehaviour
{
    [SerializeField] private ARFaceManager faceManager;
    [SerializeField] private TMP_Text _log;
    [SerializeField] private GameObject _avatarPrefab;

    private CubismModel _live2DModel;
    private ARKitFaceSubsystem _faceSubsystem;

    // Live2D 파라미터 멤버변수
    private CubismParameter _faceAngleX;
    private CubismParameter _faceAngleY;
    private CubismParameter _faceAngleZ;
    private CubismParameter _bodyAngleX;
    private CubismParameter _bodyAngleY;
    private CubismParameter _leftEye;
    private CubismParameter _rightEye;
    private CubismParameter _mouthForm;
    private CubismParameter _mouthOpen;

    // 얼굴 회전값 멤버변수
    private float _updateFaceAngleX;
    private float _updateFaceAngleY;
    private float _updateFaceAngleZ;

    // 눈동자 정보 멤버변수
    private float _updateLeftEye;
    private float _updateRightEye;

    // 입술 정보 멤버변수
    private float _updateMouthForm;
    private float _updateMouthOpen;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        _log.text = Vector3.zero.ToString();
        _live2DModel = _avatarPrefab.GetComponent<CubismModel>();
        InitCubismParameter( _live2DModel );
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

    private void LateUpdate()
    {
        // 표정 : 눈
        _leftEye.Value = _updateLeftEye;
        _rightEye.Value = _updateRightEye;

        // 표정 : 입
        _mouthForm.Value = _updateMouthForm;
        _mouthOpen.Value = _updateMouthOpen;

        // 얼굴 방향
        _faceAngleX.Value = _updateFaceAngleX;
        _faceAngleY.Value = _updateFaceAngleY;
        _faceAngleZ.Value = _updateFaceAngleZ;
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
                UpdateBlendShape( arFace );

                _log.text = arFace.transform.position.ToString();

                // TODO 삭제 대상 @Choi 25.01.05
                Debug.Log( arFace.transform.position );
            }
        }
    }

    /// <summary>
    /// 아바타의 모션 정보를 멤버 변수에 대입
    /// </summary>
    /// <param name="model"></param>
    private void InitCubismParameter( CubismModel model )
    {
        _faceAngleX = model.Parameters[0];
        _faceAngleY = model.Parameters[1];
        _faceAngleZ = model.Parameters[2];

        _bodyAngleX = model.Parameters[22];
        _bodyAngleY = model.Parameters[23];

        _leftEye = model.Parameters[3];
        _rightEye = model.Parameters[5];

        _mouthForm = model.Parameters[17];
        _mouthOpen = model.Parameters[18];
    }

    /// <summary>
    /// 얼굴 방향dmf 변경
    /// </summary>
    /// <param name="arFace">ARFace 정보</param>
    private void UpdateFaceTransform( ARFace arFace )
    {
        // 얼굴의 위치 정보 취득
        var faceRotation = arFace.transform.rotation;

        var x = NormalizeAngle( faceRotation.eulerAngles.x )* 2f;
        var y = NormalizeAngle( faceRotation.eulerAngles.y );
        var z = NormalizeAngle( faceRotation.eulerAngles.z )* 2f;

        // 새로운 얼굴 회전값을 변수에 대입
        _updateFaceAngleX = y;
        _updateFaceAngleY = x;
        _updateFaceAngleZ = z;
    }

    /// <summary>
    /// 표정 정보를 변경
    /// </summary>
    /// <param name="arFace">ARFace 정보</param>
    private void UpdateBlendShape( ARFace arFace )
    {
        _faceSubsystem = ( ARKitFaceSubsystem )faceManager.subsystem;
        using var blendShapesARKit = _faceSubsystem.GetBlendShapeCoefficients( arFace.trackableId, Allocator.Temp );
        for(var i=0; i<blendShapesARKit.Length; i++ )
        {
            switch( blendShapesARKit[i].blendShapeLocation )
            {
                case ARKitBlendShapeLocation.EyeBlinkLeft:
                    _updateLeftEye = 1 - blendShapesARKit[i].coefficient;
                    continue;
                case ARKitBlendShapeLocation.EyeBlinkRight:
                    _updateRightEye = 1 - blendShapesARKit[i].coefficient;
                    continue;
                case ARKitBlendShapeLocation.MouthFunnel:
                    _updateMouthForm = 1 - blendShapesARKit[i].coefficient * 2;
                    continue;
                case ARKitBlendShapeLocation.JawOpen:
                    _updateMouthOpen = ( float )( blendShapesARKit[i].coefficient * 1.8 );
                    continue;
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

