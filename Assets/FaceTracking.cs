using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using Live2D.Cubism.Core;
using UnityEngine.XR.ARKit;
using Unity.Collections;
using TMPro;

public class FaceTracking : MonoBehaviour
{
    [SerializeField] private ARFaceManager faceManager;
    [SerializeField] private TMP_Text _log;
    [SerializeField] private GameObject avatarPrefab;
    private CubismModel live2DModel;
    private ARKitFaceSubsystem faceSubsystem;

    private CubismParameter faceAngleX;
    private CubismParameter faceAngleY;
    private CubismParameter faceAngleZ;
    private CubismParameter bodyAngleX;
    private CubismParameter bodyAngleY;
    private CubismParameter leftEye;
    private CubismParameter rightEye;
    private CubismParameter mouthForm;
    private CubismParameter mouthOpen;
    private CubismParameter eyeBallX;
    private CubismParameter eyeBallY;

    private float updateFaceAngleX;
    private float updateFaceAngleY;
    private float updateFaceAngleZ;
    private float updateLeftEye;
    private float updateRightEye;
    private float updateMouthForm;
    private float updateMouthOpen;

    private void Awake()
    {
        _log.text = Vector3.zero.ToString();
        Debug.Log( Vector3.zero );
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        live2DModel = avatarPrefab.GetComponent<CubismModel>();

        // 表示したアバターとARKitを同期させる
        SetCubismParameter( live2DModel );
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
        // 表情
        leftEye.Value = updateLeftEye;
        rightEye.Value = updateRightEye;
        mouthForm.Value = updateMouthForm;
        mouthOpen.Value = updateMouthOpen;

        // // 顔の向き
        faceAngleX.Value = updateFaceAngleX;
        faceAngleY.Value = updateFaceAngleY;
        faceAngleZ.Value = updateFaceAngleZ;
    }

    private void OnFaceChanged( ARFacesChangedEventArgs eventArgs )
    {
        if( eventArgs.updated.Count != 0 )
        {
            var arFace = eventArgs.updated[0];
            if( arFace.trackingState == TrackingState.Tracking
                && ( ARSession.state > ARSessionState.Ready ) )
            {
                Debug.Log( arFace.transform.position );
                _log.text = arFace.transform.position.ToString();
                _log.text = arFace.transform.position.ToString();
            }
        }
    }

    // 変数にアバターのモーションを定義する
    private void SetCubismParameter( CubismModel model )
    {
        faceAngleX = model.Parameters[0];
        faceAngleY = model.Parameters[1];
        faceAngleZ = model.Parameters[2];
        bodyAngleX = model.Parameters[22];
        bodyAngleY = model.Parameters[23];
        leftEye = model.Parameters[3];
        rightEye = model.Parameters[5];
        mouthForm = model.Parameters[17];
        mouthOpen = model.Parameters[18];
    }

    //顔の向きを更新する
    private void UpdateFaceTransform( ARFace arFace )
    {
        // 顔の位置データを取得
        Quaternion faceRotation = arFace.transform.rotation;

        float x = NormalizeAngle( faceRotation.eulerAngles.x )* 2f;
        float y = NormalizeAngle( faceRotation.eulerAngles.y );
        float z = NormalizeAngle( faceRotation.eulerAngles.z )* 2f;

        // 新しい顔の情報を変数にいれる
        updateFaceAngleX = y;
        updateFaceAngleY = x;
        updateFaceAngleZ = z;
    }

    // 表情を更新する
    private void UpdateBlendShape( ARFace arFace )
    {
        faceSubsystem = ( ARKitFaceSubsystem )faceManager.subsystem;
        using var blendShapesARKit = faceSubsystem.GetBlendShapeCoefficients( arFace.trackableId, Allocator.Temp );
        foreach( var featureCoefficient in blendShapesARKit )
        {
            if( featureCoefficient.blendShapeLocation == ARKitBlendShapeLocation.EyeBlinkLeft )
            {
                updateLeftEye = 1 - featureCoefficient.coefficient;
            }
            if( featureCoefficient.blendShapeLocation == ARKitBlendShapeLocation.EyeBlinkRight )
            {
                updateRightEye = 1 - featureCoefficient.coefficient;
            }
            if( featureCoefficient.blendShapeLocation == ARKitBlendShapeLocation.MouthFunnel )
            {
                updateMouthForm = 1 - featureCoefficient.coefficient * 2;
            }
            if( featureCoefficient.blendShapeLocation == ARKitBlendShapeLocation.JawOpen )
            {
                updateMouthOpen = ( float )( featureCoefficient.coefficient * 1.8 );
            }
        }
    }

    // 顔の角度を正規化する
    private float NormalizeAngle( float angle )
    {
        if( angle > 180 )
        {
            return angle - 360;
        }
        return angle;
    }
}

