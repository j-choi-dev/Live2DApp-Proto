using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace LiveApp.UI
{
    public class ObservableToggleTMPro : MonoBehaviour, IObservableToggle
    {
        [SerializeField] private Toggle _toggle = null;
        [SerializeField] private TMP_Text _text = null;

        private Subject<bool> _onToggleChanged = new Subject<bool>();
        public IObservable<bool> OnToggleChanged => _onToggleChanged;

        public string Name => _text.text;


        public void SetName( string name )
            => _text.text = name;
    }
}
