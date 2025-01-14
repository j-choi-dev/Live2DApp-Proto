using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using AvatarSystem.Domain;
using AvatarStstem;

namespace StudioSystem.Core.Editor
{
    [CustomEditor( typeof( AvatarParameterPair ) )]
    public class AvatarParameterPairStorageEditor : UnityEditor.Editor
    {
        private SerializedProperty _pairProperty;

        private void OnEnable()
        {
            _pairProperty = serializedObject.FindProperty( "_pair" );
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if( GUILayout.Button( "Auto-Generate AvatarParameterPairs" ) )
            {
                GenerateParameterPairs();
            }
        }

        private void GenerateParameterPairs()
        {
            var targetScript = ( AvatarParameterPair )target;
            var enumValues = System.Enum.GetValues( typeof( AvatarPartsParameter ) );

            targetScript.ClearAndInitializePairList( enumValues.Length );

            for( int i = 0; i < enumValues.Length; i++ )
            {
                var enumValue = ( AvatarPartsParameter )enumValues.GetValue( i );
                targetScript.SetPairAtIndex( i, enumValue );
            }

            EditorUtility.SetDirty( targetScript );
        }
    }
}
