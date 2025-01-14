using AvatarSystem.Domain;
using Live2D.Cubism.Core;
using UnityEngine;

namespace AvatarStstem
{
    public class StudioAvatar : MonoBehaviour
    {
        [SerializeField] private AvatarParameterPair _pair = null;
        [SerializeField] private CubismModelExtention _avatar = null;
        [SerializeField] private CubismMoc _moc;

        // Live2D 파라미터 멤버변수
        private CubismParameter _faceAngleX;
        private CubismParameter _faceAngleY;
        private CubismParameter _faceAngleZ;
        private CubismParameter _bodyAngleX;
        private CubismParameter _bodyAngleY;
        private CubismParameter _leftEyeBlink;
        private CubismParameter _rightEyeBlink;
        private CubismParameter _eyeBallX;
        private CubismParameter _eyeBallY;
        private CubismParameter _mouthForm;
        private CubismParameter _mouthOpen;

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


        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            _avatar.SetMoc( _moc );
            _avatar.SetParamteters( _pair.Parameters );
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

        public void SetFaceAngleX( float value ) => _faceAngleX.Value = value;
        public void SetFaceAngleY( float value ) => _faceAngleY.Value = value;
        public void SetFaceAngleZ( float value ) => _faceAngleZ.Value = value;

        public void SetEyeBlinkLeft( float value ) => _updateLeftEye = value;
        public void SetEyeBlinkRight( float value ) => _updateRightEye = value;
        public void SetEyeLookHorizontal( float value ) => _updateEyeballX = value;
        public void SetEyeLookVertical( float value ) => _updateEyeballY = value;

        public void SetMouthForm( float value ) => _updateMouthForm = value;
        public void SetMouthOpen( float value ) => _updateMouthOpen = value;
    }
}
