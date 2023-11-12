using Beatmap.Base.Customs;
using UnityEngine;

namespace ChroMapper_CameraMovement.Component
{
    public class CustomEventController
    {
        public void Output()
        {
            if (!Plugin.movement.customEventsObject)
                return;
            foreach (var loadedObject in CameraMovementController.customEventGridContainer.LoadedObjects)
            {
                var customEvent = loadedObject as BaseCustomEvent;
                var data = customEvent.Data;
                Debug.Log($"{customEvent.Type}:{customEvent.SongBpmTime}:{CameraMovementController.atsc.GetSecondsFromBeat(customEvent.SongBpmTime)}:{data}");
            }
        }
    }
}
