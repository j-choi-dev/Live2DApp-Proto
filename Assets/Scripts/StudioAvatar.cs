using AvatarSystem.Domain;
using Live2D.Cubism.Core;
using System.Linq;
using TMPro;
using UnityEngine;

namespace AvatarStstem
{
    public class StudioAvatar : MonoBehaviour
    {
        [SerializeField] private AvatarParameterPair _pair = null;
        [SerializeField] private CubismModel _avatar = null;
        [SerializeField] private CubismMoc _moc;
        private bool _isInitialized = false;

        // Live2D 파라미터 멤버변수
        private CubismParameter _faceAngleX = null;
        private CubismParameter _faceAngleY = null;
        private CubismParameter _faceAngleZ = null;
        private CubismParameter _bodyAngleX = null;
        private CubismParameter _bodyAngleY = null;
        private CubismParameter _leftEyeBlink = null;
        private CubismParameter _rightEyeBlink = null;
        private CubismParameter _eyeBallX = null;
        private CubismParameter _eyeBallY = null;
        private CubismParameter _mouthForm = null;
        private CubismParameter _mouthOpen = null;

        // 얼굴 회전값 멤버변수
        private float _updateFaceAngleX;
        private float _updateFaceAngleY;
        private float _updateFaceAngleZ;

        // 눈동자 정보 멤버변수
        private float _updateLeftEye;
        private float _updateRightEye;

        // 눈동자 X 값 멤버변수
        private float _updateEyeballX;
        // 눈동자 Y 값 멤버변수
        private float _updateEyeballY;

        // 입술 정보 멤버변수
        private float _updateMouthForm;
        private float _updateMouthOpen;

        private string _msg = string.Empty;

        public string AvatarMsg = string.Empty;
        public string AvatarLog = string.Empty;
        public bool IsInitialized => _isInitialized;


        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            _avatar.SetCubismMoc( _moc );
            InitCubismParameter();
            _isInitialized = true;
        }

        /// <summary>
        /// 아바타의 모션 정보를 멤버 변수에 대입
        /// </summary>
        /// <param name="model"></param>
        public void InitCubismParameter()
        {
            _faceAngleX = _avatar.Parameters.First(arg => arg.Id == _pair.ParameterPairs[(int)AvatarPartsParameter.FaceAngle_X].parameter.Id);
            _faceAngleY = _avatar.Parameters.First( arg => arg.Id == _pair.ParameterPairs[( int )AvatarPartsParameter.FaceAngle_Y].parameter.Id );
            _faceAngleZ = _avatar.Parameters.First( arg => arg.Id == _pair.ParameterPairs[( int )AvatarPartsParameter.FaceAngle_Z].parameter.Id );

            _leftEyeBlink = _avatar.Parameters.First( arg => arg.Id ==_pair.ParameterPairs[( int )AvatarPartsParameter.LeftEyeBlink].parameter.Id);
            _rightEyeBlink = _avatar.Parameters.First( arg => arg.Id == _pair.ParameterPairs[( int )AvatarPartsParameter.RightEyeBlink].parameter.Id );

            _eyeBallX = _avatar.Parameters.First( arg => arg.Id == _pair.ParameterPairs[( int )AvatarPartsParameter.EyeBallX].parameter.Id );
            _eyeBallY = _avatar.Parameters.First( arg => arg.Id == _pair.ParameterPairs[( int )AvatarPartsParameter.EyeBallY].parameter.Id );
            AvatarMsg = $"{_faceAngleX.Id}, {_faceAngleY.Id}, {_faceAngleZ.Id}";
            Debug.Log( AvatarMsg );            
        }

        public void SetFaceAngleX( float value ) => _updateFaceAngleX = value;
        public void SetFaceAngleY( float value ) => _updateFaceAngleY = value;
        public void SetFaceAngleZ( float value ) => _updateFaceAngleZ = value;

        public void SetEyeBlinkLeft( float value ) => _updateLeftEye = value;
        public void SetEyeBlinkRight( float value ) => _updateRightEye = value;
        public void SetEyeLookHorizontal( float value ) => _updateEyeballX = value;
        public void SetEyeLookVertical( float value ) => _updateEyeballY = value;

        public void SetMouthForm( float value ) => _updateMouthForm = value;
        public void SetMouthOpen( float value ) => _updateMouthOpen = value;

        private void LateUpdate()
        {
            if (_isInitialized == false)
            {
                _msg = string.Empty;
                return;
            }
            _faceAngleX.Value = _updateFaceAngleX;
            _faceAngleY.Value = _updateFaceAngleY;
            _faceAngleZ.Value = _updateFaceAngleZ;
            AvatarLog = $"{_faceAngleX.Value.ToString( "#.##" )}, {_faceAngleY.Value.ToString( "#.##" )}, {_faceAngleZ.Value.ToString( "#.##" )}";

            _leftEyeBlink.Value = _updateLeftEye;
            _rightEyeBlink.Value = _updateRightEye;
            
            _eyeBallX.Value = _updateEyeballX;
            _eyeBallY.Value = _updateEyeballY;
            AvatarLog += $"\n{_eyeBallX.Value.ToString( "#.##" )}, {_eyeBallY.Value.ToString( "#.##" )}";
            //_mouthForm.Value = _updateMouthForm;
            //_mouthOpen.Value = _updateMouthOpen;
            AvatarLog = string.Empty;
        }
    }
}
