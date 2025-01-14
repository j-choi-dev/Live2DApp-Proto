using Live2D.Cubism.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
#elif UNITY_2018_1_OR_NEWER
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;
#endif

namespace Live2D.Cubism.Core
{
    [ExecuteInEditMode, CubismDontMoveOnReimport]
    public class CubismModelExtention : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private CubismMoc _moc;

        /// <summary>
        /// Moc the instance was instantiated from.
        /// </summary>
        public CubismMoc Moc
        {
            get { return _moc; }
            private set { _moc = value; }
        }

        /// <summary>
        /// Handler for <see cref="CubismDynamicDrawableData"/>.
        /// </summary>
        /// <param name="sender">Model the dymanic data applies to.</param>
        /// <param name="data">New data.</param>
        public delegate void DynamicDrawableDataHandler( CubismModelExtention sender, CubismDynamicDrawableData[] data );

        /// <summary>
        /// Event triggered if new <see cref="CubismDynamicDrawableData"/> is available for instance.
        /// </summary>
        public event DynamicDrawableDataHandler OnDynamicDrawableData;

        /// <summary>
        /// Instantiates a <see cref="CubismMoc"/>.
        /// </summary>
        /// <param name="moc">Cubism moc to instantiate.</param>
        /// <returns>Instance.</returns>
        public static CubismModelExtention InstantiateFrom( CubismMoc moc )
        {
            // Return if argument is invalid.
            if( moc == null )
            {
                return null;
            }


            // Create model.
            var model = new GameObject( moc.name )
                .AddComponent<CubismModelExtention>();


            // Initialize it by resetting it.
            model.Reset( moc );


            return model;
        }

        public void SetMoc( CubismMoc moc ) => _moc = moc;

        /// <summary>
        /// TaskableModel for unmanaged backend.
        /// </summary>
        private CubismTaskableModel TaskableModel { get; set; }


        /// <summary>
        /// <see cref="Parameters"/> backing field.
        /// </summary>
        //[NonSerialized]
        public CubismParameter[] _parameters;

        /// <summary>
        /// Drawables of model.
        /// </summary>
        [SerializeField]
        public CubismParameter[] Parameters
        {
            get
            {
                if( _parameters == null )
                {
                    Revive();
                }


                return _parameters;
            }
        }

        public void SetParamteters( IEnumerable<CubismParameter> parameters )
            => _parameters = parameters.ToArray();

        /// <summary>
        /// <see cref="Parts"/> backing field.
        /// </summary>
        //[NonSerialized]
        public CubismPart[] _parts;

        /// <summary>
        /// Drawables of model.
        /// </summary>
        [SerializeField]
        public CubismPart[] Parts
        {
            get
            {
                if( _parts == null )
                {
                    Revive();
                }


                return _parts;
            }
            private set { _parts = value; }
        }

        /// <summary>
        /// <see cref="Drawables"/> backing field.
        /// </summary>
        [NonSerialized]
        public CubismDrawable[] _drawables;

        /// <summary>
        /// Drawables of model.
        /// </summary>
        public CubismDrawable[] Drawables
        {
            get
            {
                if( _drawables == null )
                {
                    Revive();
                }


                return _drawables;
            }
            private set { _drawables = value; }
        }

        /// <summary>
        /// <see cref="CanvasInformation"/> backing field.
        /// </summary>
        [NonSerialized]
        private CubismCanvasInformation _canvasInformation;

        /// <summary>
        /// Canvas information of model.
        /// </summary>
        public CubismCanvasInformation CanvasInformation
        {
            get
            {
                if( _canvasInformation == null )
                {
                    Revive();
                }


                return _canvasInformation;
            }
            private set { _canvasInformation = value; }
        }

        /// <summary>
        /// True if instance is revived.
        /// </summary>
        public bool IsRevived
        {
            get { return TaskableModel != null; }
        }

        /// <summary>
        /// True if instance can revive.
        /// </summary>
        private bool CanRevive
        {
            get { return Moc != null; }
        }

        /// <summary>
        /// Model update functions for player loop.
        /// </summary>
        [NonSerialized]
        private static Action _modelUpdateFunctions;

        private bool WasAttachedModelUpdateFunction { get; set; }


        /// <summary>
        /// True on the frame the instance was enabled.
        /// </summary>
        private bool WasJustEnabled { get; set; }

        /// <summary>
        /// Frame number last update was done.
        /// </summary>
        private int LastTick { get; set; }


        /// <summary>
        /// Revives instance.
        /// </summary>
        private void Revive()
        {
            // Return if already revive.
            if( IsRevived )
            {
                return;
            }


            // Return if revive isn't possible.
            if( !CanRevive )
            {
                return;
            }


            Reset( Moc );
        }

        /// <summary>
        /// Initializes instance for first use.
        /// </summary>
        /// <param name="moc">Moc to instantiate from.</param>
        private void Reset( CubismMoc moc )
        {
            Moc = moc;
            name = moc.name;
            TaskableModel = new CubismTaskableModel( moc );

            if( TaskableModel == null || TaskableModel.UnmanagedModel == null )
            {
                return;
            }


            //Parameters = GetComponentsInChildren<CubismParameter>();
            //if( Parameters.Length < 1 && ( transform.Find( "Parameters" ) == null ) )
            //{
            //    // Create and initialize proxies.
            //    var parameters = CubismParameter.CreateParameters( TaskableModel.UnmanagedModel );
            //    parameters.transform.SetParent( transform );
            //    Parameters = parameters.GetComponentsInChildren<CubismParameter>();
            //}
            //else
            //{
            //    Parameters.Revive( TaskableModel.UnmanagedModel );
            //}


            Parts = GetComponentsInChildren<CubismPart>();
            if( Parts.Length < 1 && ( transform.Find( "Parts" ) == null ) )
            {
                // Create and initialize proxies.
                var parts = CubismPart.CreateParts( TaskableModel.UnmanagedModel );
                parts.transform.SetParent( transform );
                Parts = parts.GetComponentsInChildren<CubismPart>();
            }
            else
            {
                Parts.Revive( TaskableModel.UnmanagedModel );
            }


            Drawables = GetComponentsInChildren<CubismDrawable>();
            if( Drawables.Length < 1 && ( transform.Find( "Drawables" ) == null ) )
            {
                // Create and initialize proxies.
                var drawables = CubismDrawable.CreateDrawables( TaskableModel.UnmanagedModel );
                drawables.transform.SetParent( transform );
                Drawables = drawables.GetComponentsInChildren<CubismDrawable>();
            }
            else
            {
                Drawables.Revive( TaskableModel.UnmanagedModel );
            }


            CanvasInformation = new CubismCanvasInformation( TaskableModel.UnmanagedModel );

            //RefreshParameterStore();
        }

        /// <summary>
        /// Forces update.
        /// </summary>
        public void ForceUpdateNow()
        {
            WasJustEnabled = true;
            LastTick = -1;


            Revive();

#if UNITY_2018_1_OR_NEWER
            OnModelUpdate();
#else
            OnRenderObject();
#endif
        }

        ///// <summary>
        ///// パラメータストアを最新の情報に更新する。
        ///// </summary>
        //public void RefreshParameterStore()
        //{
        //    // CubismParameterStore を取得する。
        //    _parameterStore = GetComponent<CubismParameterStore>();


        //    // Return early if empty.
        //    if( _parameterStore == null )
        //    {
        //        return;
        //    }


        //    // 最新の情報に更新する。
        //    _parameterStore.Refresh();
        //}

        /// <summary>
        /// Called by Unity. Triggers <see langword="this"/> to update.
        /// </summary>
        private void Update()
        {
#if UNITY_2018_1_OR_NEWER
            if( !WasAttachedModelUpdateFunction )
            {
                _modelUpdateFunctions += OnModelUpdate;


                WasAttachedModelUpdateFunction = true;
            }
#endif


            // Return on first frame enabled.
            if( WasJustEnabled )
            {
                return;
            }


            // Return unless revived.
            if( !IsRevived )
            {
                return;
            }


            // Return if backend is ticking.
            if( !TaskableModel.DidExecute )
            {
                return;
            }


            // Sync parameters back.
            TaskableModel.TryReadParameters( Parameters );

            //// restore last frame parameters value and parts opacity.
            //if( _parameterStore != null )
            //{
            //    _parameterStore.RestoreParameters();
            //}

            // Trigger event.
            if( OnDynamicDrawableData == null )
            {
                return;
            }


            OnDynamicDrawableData( this, TaskableModel.DynamicDrawableData );
        }


        /// <summary>
        /// Called by Unity. Blockingly updates <see langword="this"/> on first frame enabled; otherwise tries async update.
        /// </summary>
        private void OnRenderObject()
        {
#if !UNITY_2018_1_OR_NEWER
            OnModelUpdate();
#endif
        }

        /// <summary>
        /// Update model states.
        /// </summary>
        private void OnModelUpdate()
        {
            // Return unless revived.
            if( !IsRevived )
            {
                return;
            }


            // Return if already ticked this frame.
            if( LastTick == Time.frameCount && Application.isPlaying )
            {
                return;
            }


            LastTick = Time.frameCount;


            // Try to sync parameters and parts (without caring whether task is executing or not).
            TaskableModel.TryWriteParametersAndParts( Parameters, Parts );


            // Return if task is executing.
            if( TaskableModel.IsExecuting )
            {
                return;
            }


            // Force blocking update on first frame enabled.
            if( WasJustEnabled )
            {
                // Force sync update.
                TaskableModel.UpdateNow();


                // Unset condition.
                WasJustEnabled = false;


                // Fetch results by calling own 'Update()'.
                Update();


                return;
            }


            // Enqueue update task.
            TaskableModel.Update();
        }

        /// <summary>
        /// Called by Unity. Revives instance.
        /// </summary>
        private void OnEnable()
        {
            WasJustEnabled = true;


            Revive();
        }

        private void OnDisable()
        {
#if UNITY_2018_1_OR_NEWER
            if( WasAttachedModelUpdateFunction )
            {
                _modelUpdateFunctions -= OnModelUpdate;


                WasAttachedModelUpdateFunction = false;
            }
#endif
        }

        /// <summary>
        /// Called by Unity. Releases unmanaged memory.
        /// </summary>
        private void OnDestroy()
        {
            if( !IsRevived )
            {
                return;
            }


            TaskableModel.ReleaseUnmanaged();


            TaskableModel = null;
        }

        /// <summary>
        /// Called by Unity. Triggers <see cref="OnEnable"/>.
        /// </summary>
        private void OnValidate()
        {
            OnEnable();
        }
    }
}
