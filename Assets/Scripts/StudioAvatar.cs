using AvatarSystem.Domain;
using Live2D.Cubism.Core;
using TMPro;
using UnityEngine;

namespace AvatarStstem
{
    public class StudioAvatar : MonoBehaviour
    {
        [SerializeField] private AvatarParameterPair _pair = null;
        [SerializeField] private CubismModelExtention _avatar = null;
        [SerializeField] private CubismMoc _moc;
        [SerializeField] private TMP_Text _log;
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


        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            _avatar.SetMoc( _moc );
            _avatar.SetParamteters( _pair.Parameters );
            InitCubismParameter();
            _isInitialized = true;
        }

        /// <summary>
        /// 아바타의 모션 정보를 멤버 변수에 대입
        /// </summary>
        /// <param name="model"></param>
        public void InitCubismParameter()
        {
            _faceAngleX = _avatar.Parameters[(int)AvatarPartsParameter.FaceAngle_X];
            _faceAngleY = _avatar.Parameters[( int )AvatarPartsParameter.FaceAngle_Y];
            _faceAngleZ = _avatar.Parameters[( int )AvatarPartsParameter.FaceAngle_Z];

            _leftEyeBlink = _avatar.Parameters[( int )AvatarPartsParameter.LeftEyeBlink];
            _rightEyeBlink = _avatar.Parameters[( int )AvatarPartsParameter.RightEyeBlink];

            _eyeBallX = _avatar.Parameters[( int )AvatarPartsParameter.EyeBallX];
            _eyeBallY = _avatar.Parameters[( int )AvatarPartsParameter.EyeBallY];
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
            _msg = $"{_faceAngleX.Value.ToString( "#.##" )}, {_faceAngleY.Value.ToString( "#.##" )}, {_faceAngleZ.Value.ToString( "#.##" )}";

            _leftEyeBlink.Value = _updateLeftEye;
            _rightEyeBlink.Value = _updateRightEye;
            
            _eyeBallX.Value = _updateEyeballX;
            _eyeBallY.Value = _updateEyeballY;
            _msg += $"\n{_eyeBallX.Value.ToString( "#.##" )}, {_eyeBallY.Value.ToString( "#.##" )}";
            _log.text = _msg;
            _mouthForm.Value = _updateMouthForm;
            _mouthOpen.Value = _updateMouthOpen;
            _msg = string.Empty;
        }
    }
}
