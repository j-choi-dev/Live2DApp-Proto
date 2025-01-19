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

        // Live2D �Ķ���� �������
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
        /// �ƹ�Ÿ�� ��� ������ ��� ������ ����
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
                return;
            }
            _faceAngleX.Value = _updateFaceAngleX;
            _faceAngleY.Value = _updateFaceAngleY;
            _faceAngleZ.Value = _updateFaceAngleZ;

            _leftEyeBlink.Value = _updateLeftEye;
            _rightEyeBlink.Value = _updateRightEye;

            _eyeBallX.Value = _updateEyeballX;
            _eyeBallY.Value = _updateEyeballY;
        }
    }
}
