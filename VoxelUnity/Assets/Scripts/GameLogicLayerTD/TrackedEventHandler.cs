using UnityEngine;
using Vuforia;

public class TrackedEventHandler : MonoBehaviour, ITrackableEventHandler
{
    //Put this on rune image target to see when it is tracked or use variable Tracked of this class
    
    public Transform Painter3DTransform;
    private TrackableBehaviour mTrackableBehaviour;
    public bool Tracked = false;

    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }
    }

    private void OnTrackingFound()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = true;
        }
        Tracked = true;

        //TODO Insert Code for OnTrackingFound

    }

    private void OnTrackingLost()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = false;
        }
        Tracked = false;

        //TODO Insert Code for OnTrackingLost

    }
}
