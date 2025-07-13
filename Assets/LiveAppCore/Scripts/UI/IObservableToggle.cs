using System;

namespace LiveApp.UI
{
    public interface IObservableToggle
    {
        IObservable<bool> OnToggleChanged { get; }
        string Name { get; }

        void SetName( string name );
    }
}
