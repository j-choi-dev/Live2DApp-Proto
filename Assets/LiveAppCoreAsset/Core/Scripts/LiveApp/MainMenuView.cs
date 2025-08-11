using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace LiveAppUI.View
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField]
        private List<ButtonViewPair> _pairs = null;

        [SerializeField]
        private ObservableToggleButton _recButton = null;

        [SerializeField]
        private ObservableButton _emergencyButton = null;

        private void Awake()
        {
            for ( int i = 0 ; i < _pairs.Count ; ++i )
            {
                var viewPair = _pairs[ i ];
                viewPair.view.SetActive( false );
                viewPair.button.OnClick
                    .Subscribe( _ =>
                    {
                        for ( int i = 0 ; i < _pairs.Count ; ++i )
                        {
                            _pairs[i].view.SetActive( false );
                        }
                        viewPair.view.SetActive( true );
                    } )
                    .AddTo( this );
            }

            _recButton.OnActiveChange
                .Subscribe( arg => Debug.Log( _recButton.IsActive ? "Recording" : "Stop" ) )
                .AddTo( this );

            _emergencyButton.OnClick
                .Subscribe( arg => Debug.Log( "Emergency" ) )
                .AddTo( this );
        }
    }
}
