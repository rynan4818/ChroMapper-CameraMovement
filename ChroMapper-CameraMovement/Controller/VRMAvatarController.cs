using UnityEngine;
using System.IO;
using VRM;
using VRMShaders;
using UniGLTF;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Util;
using System.Threading.Tasks;

namespace ChroMapper_CameraMovement.Controller
{
    public class VRMAvatarController
    {
        public static RuntimeGltfInstance avatar { set; get; }
        public static GameObject cm_MapEditorCamera { set; get; }
        public static VRMBlendShapeProxy m_proxy { set; get; }
        public static VRMLookAtHead lookAt { set; get; }
        public static Blinker m_blink { set; get; }
        public static Animation animation { set; get; }
        public static bool loadActive { set; get; } = false;

        public async Task LoadModelAsync()
        {
            if (!File.Exists(Options.Instance.avatarFileName))
            {
                Debug.LogError("Avatar File ERR!");
                return;
            }
            loadActive = true;
            if (avatar != null)
            {
                GameObject.Destroy(avatar.gameObject);
                avatar = null;
            }
            VrmUtility.MaterialGeneratorCallback materialCallback = (glTF_VRM_extensions vrm) => new VRMUrpMaterialDescriptorGenerator(vrm);
            try
            {
                avatar = await VrmUtility.LoadAsync(Options.Instance.avatarFileName, new RuntimeOnlyAwaitCaller(), materialCallback);
            }
            catch
            {
                Debug.LogWarning("VRM Avatar Load Error!");
                if (avatar != null)
                {
                    GameObject.Destroy(avatar.gameObject);
                    avatar = null;
                }
                loadActive = false;
                return;
            }
            avatar.EnableUpdateWhenOffscreen();
            avatar.ShowMeshes();
            UnityUtility.AllSetLayer(avatar.gameObject, CameraMovementController.avatarLayer);
            AvatarTransformSet();
            lookAt = avatar.GetComponent<VRMLookAtHead>();
            if (lookAt != null)
            {
                m_blink = avatar.gameObject.AddComponent<Blinker>();
                if (cm_MapEditorCamera == null)
                    cm_MapEditorCamera = GameObject.Find("MapEditor Camera");
                lookAt.Target = cm_MapEditorCamera.transform;
                lookAt.UpdateType = UpdateType.LateUpdate;
                m_proxy = avatar.GetComponent<VRMBlendShapeProxy>();
            }
            animation = avatar.GetComponent<Animation>();
            if (animation && animation.clip != null)
                animation.Play(animation.clip.name);
            loadActive = false;
        }

        public static void AvatarTransformSet()
        {
            if (avatar == null)
                return;
            var position = new Vector3(Options.Instance.originXoffset, Options.Instance.originYoffset + Options.Instance.avatarYoffset, Options.Instance.originZoffset);
            position *= Options.Instance.avatarCameraScale;
            avatar.Root.transform.position = new Vector3(position.x, position.y + Options.Instance.originMatchOffsetY, position.z + Options.Instance.originMatchOffsetZ);
            avatar.Root.transform.rotation = Quaternion.Euler(Vector3.zero);
            avatar.Root.transform.localScale = new Vector3(Options.Instance.avatarScale, Options.Instance.avatarScale, Options.Instance.avatarScale) * Options.Instance.avatarCameraScale;
            if (lookAt != null)
            {
                m_blink.enabled = Options.Instance.avatarBlinker;
                lookAt.enabled = Options.Instance.avatarLookAt;
            }
            if (Options.Instance.avatarAnimation && animation && animation.clip != null)
                animation.enabled = Options.Instance.avatarAnimation;
        }

        public static void AvatarEnable()
        {
            if (avatar != null)
                avatar.Root.SetActive(true);
        }

        public static void AvatarDisable()
        {
            if (avatar != null)
                avatar.Root.SetActive(false);
        }
    }
}
