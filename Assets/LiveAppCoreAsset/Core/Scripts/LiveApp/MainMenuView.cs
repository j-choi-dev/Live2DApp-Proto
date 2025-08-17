using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;
using LiveAppUI.Presenter;

namespace LiveAppUI.View
{
    public class MainMenuView : MonoBehaviour, IMainMenuView
    {
        [SerializeField]
        private List<ButtonViewPair> _pairs = null;

        [SerializeField]
        private ObservableToggleButton _recButton = null;

        [SerializeField]
        private ObservableButton _emergencyButton = null;


        private Subject<bool> _onRecordingChanged = new Subject<bool>();
        public IObservable<bool> OnRecordingChanged => _onRecordingChanged;
        public IObservable<Unit> OnClickEmergency => _emergencyButton.OnClick;

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
                .Subscribe( arg => _onRecordingChanged.OnNext(arg) )
                .AddTo( this );
        }
    }
}
