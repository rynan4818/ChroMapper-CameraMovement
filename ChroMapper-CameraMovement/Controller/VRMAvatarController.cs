using UnityEngine;
using System.IO;
using VRM;
using VRMShaders;
using UniGLTF;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Util;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public static List<(Transform, Vector3, Quaternion, Vector3)> defaultTransform { set; get; }
        public static BlendShapeController blendShapeController { set; get; }
        public async Task LoadModelAsync()
        {
            if (loadActive)
                return;
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
            AvatarOptionLoad();
            SpringBoneEnable();
            loadActive = false;
        }

        public static void AvatarOptionLoad()
        {
            lookAt = avatar.GetComponent<VRMLookAtHead>();
            if (lookAt == null)
                return;
            if (cm_MapEditorCamera == null)
                cm_MapEditorCamera = GameObject.Find("MapEditor Camera");
            lookAt.Target = cm_MapEditorCamera.transform;
            lookAt.UpdateType = UpdateType.LateUpdate;
            m_blink = avatar.gameObject.AddComponent<Blinker>();
            animation = avatar.GetComponent<Animation>();
            if (animation != null && animation.clip != null)
                animation.Play(animation.clip.name);
            m_proxy = avatar.GetComponent<VRMBlendShapeProxy>();
            if (m_proxy == null)
                return;
            blendShapeController = new BlendShapeController
            {
                m_proxy = m_proxy,
                m_blink = m_blink
            };
            blendShapeController.Setup();
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
                lookAt.enabled = Options.Instance.avatarLookAt;
            if (m_blink != null)
                m_blink.enabled = Options.Instance.avatarBlinker;
            if (Options.Instance.avatarAnimation && animation && animation.clip != null)
                animation.enabled = Options.Instance.avatarAnimation;
            defaultTransform = new List<(Transform, Vector3, Quaternion, Vector3)>
            {
                (avatar.Root.transform, avatar.Root.transform.position, avatar.Root.transform.rotation, avatar.Root.transform.localScale)
            };
            foreach (var tarns in avatar.Root.GetComponentsInChildren<Transform>())
                defaultTransform.Add((tarns, tarns.position, tarns.rotation, tarns.localScale));
        }
        public static void SpringBoneEnable()
        {
            foreach (var springBone in avatar.Root.GetComponentsInChildren<VRMSpringBone>())
                springBone.enabled = Options.Instance.vrmSpringBone;
        }

        public static void SetDefaultTransform()
        {
            foreach (var trans in defaultTransform)
            {
                trans.Item1.position = trans.Item2;
                trans.Item1.rotation = trans.Item3;
                trans.Item1.localScale = trans.Item4;
            }
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
