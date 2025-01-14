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

        // Live2D �Ķ���� �������
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

        // �� ȸ���� �������
        private float _updateFaceAngleX;
        private float _updateFaceAngleY;
        private float _updateFaceAngleZ;

        // ������ ���� �������
        private float _updateLeftEye;
        private float _updateRightEye;

        // ������ X �� �������
        private float _updateEyeballX;
        // ������ Y �� �������
        private float _updateEyeballY;

        // �Լ� ���� �������
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
        /// �ƹ�Ÿ�� ��� ������ ��� ������ ����
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
