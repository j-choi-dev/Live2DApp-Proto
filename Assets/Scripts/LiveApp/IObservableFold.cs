using System;

namespace LiveApp.UI
{
    public interface IObservableFold
    {
        IObservable<bool> OnShowChanged { get; }
        bool IsShow { get; }
        string Name { get; }

        void SetName( string name );
    }
}
