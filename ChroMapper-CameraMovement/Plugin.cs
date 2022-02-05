using UnityEngine;

namespace ChroMapper_CameraMovement
{
    [Plugin("Camera Movement")]
    public class Plugin
    {
        [Init]
        private void Init()
        {
            Debug.Log("Plugin has loaded!");
        }
        
        [Exit]
        private void Exit()
        {
            Debug.Log("Application has closed!");
        }
    }
}
