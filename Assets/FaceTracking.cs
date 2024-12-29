using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class FaceTracking : MonoBehaviour
{
    [SerializeField] private ARFaceManager faceManager;
    [SerializeField] private Text _log;

    private void Awake()
    {
        _log.text = Vector3.zero.ToString();
        Debug.Log( Vector3.zero );
    }

    private void OnEnable()
    {
        faceManager.facesChanged += OnFaceChanged;
    }

    private void OnDisable()
    {
        faceManager.facesChanged -= OnFaceChanged;
    }

    private void OnFaceChanged( ARFacesChangedEventArgs eventArgs )
    {
        if( eventArgs.updated.Count != 0 )
        {
            var arFace = eventArgs.updated[0];
            if( arFace.trackingState == TrackingState.Tracking
                && ( ARSession.state > ARSessionState.Ready ) )
            {
                Debug.Log( arFace.transform.position );
                _log.text = arFace.transform.position.ToString();
                _log.text = arFace.transform.position.ToString();
            }
        }
    }
}

