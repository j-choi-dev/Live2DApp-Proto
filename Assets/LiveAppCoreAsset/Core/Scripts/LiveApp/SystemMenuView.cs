using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace LiveAppUI.View
{
    public class SystemMenuView : MonoBehaviour
    {
        [SerializeField]
        private List<ButtonViewPair> _pairs = null;

        private void Awake()
        {
            for ( int i = 0 ; i < _pairs.Count ; ++i )
            {
                var viewPair = _pairs[ i ];
                viewPair.button.OnClick
                    .Subscribe( _ =>
                    {
                        for ( int i = 0 ; i < _pairs.Count ; ++i )
                        {
                            _pairs[ i ].view.SetActive( false );
                        }
                        viewPair.view.SetActive( true );
                    } )
                    .AddTo( this );
            }
        }
    }
}
