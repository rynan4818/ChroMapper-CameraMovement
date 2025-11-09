using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.Component
{
    /// <summary>
    /// spline コマンドのビジュアル編集を管理
    /// </summary>
    public class SplineEditor : MonoBehaviour
    {
        // --- 入力アクション ---
        private InputAction splineDragAction;
        private InputAction mousePositionAction;

        // --- 内部ステート ---
        private List<GameObject> controlPointHandles = new List<GameObject>();
        private LineRenderer activeLineRenderer;
        private List<SplinePointData> pointDataCache = new List<SplinePointData>();
        private string originalTargetParams = "";
        private string originalEaseName = "";

        // --- マウスドラッグ用変数 ---
        private GameObject selectedHandle;
        private Camera mainCamera;
        private Plane dragPlane;
        private Vector3 offset;

        // --- マーカーの表示設定 ---
        private static readonly float handleScale = 0.15f;
        private static readonly Color handleColor = new Color(0.1f, 1.0f, 0.1f, 0.8f); // 緑
        private static readonly Color lineColor = Color.green;
        private static readonly float lineWidth = 0.05f;

        private class SplinePointData
        {
            public float rz;
            public float fov;
        }

        private void Awake()
        {
            splineDragAction = new InputAction("Spline Drag");
            splineDragAction.AddBinding("<Mouse>/leftButton");
            splineDragAction.started += OnDragStarted;
            splineDragAction.canceled += OnDragEnded;

            mousePositionAction = new InputAction("Spline Mouse Position");
            mousePositionAction.AddBinding("<Mouse>/position");
        }

        private void OnDestroy()
        {
            splineDragAction?.Disable();
            splineDragAction?.Dispose();
            mousePositionAction?.Disable();
            mousePositionAction?.Dispose();
            Cleanup();
        }

        public void StartEditing(string commandString)
        {
            Cleanup();

            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("SplineEditor: Main Camera が見つかりません。");
                return;
            }

            List<Vector3> parsedEditorPositions = ParseCommand(commandString);
            if (parsedEditorPositions == null) return;

            InstantiateHandles(parsedEditorPositions);
            UpdateLineRenderer();

            splineDragAction.Enable();
            mousePositionAction.Enable();
        }

        public string EndEditing()
        {
            string newCommand = RebuildCommandString();

            splineDragAction.Disable();
            mousePositionAction.Disable();

            Cleanup();
            return newCommand;
        }

        // --- 毎フレームの処理 (ドラッグと軌道更新) ---

        private void Update()
        {
            if (selectedHandle != null && mainCamera != null)
            {
                Vector2 mousePosition = mousePositionAction.ReadValue<Vector2>();
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);

                if (dragPlane.Raycast(ray, out float distance))
                {
                    selectedHandle.transform.position = ray.GetPoint(distance) + offset;
                }
            }
        }

        private void LateUpdate()
        {
            if (activeLineRenderer != null)
            {
                UpdateLineRenderer();
            }
        }

        // --- InputAction イベントハンドラ ---

        private void OnDragStarted(InputAction.CallbackContext context)
        {
            if (mainCamera == null) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector2 mousePosition = mousePositionAction.ReadValue<Vector2>();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (controlPointHandles.Contains(hit.collider.gameObject))
                {
                    selectedHandle = hit.collider.gameObject;
                    dragPlane = new Plane(mainCamera.transform.forward, selectedHandle.transform.position);
                    if (dragPlane.Raycast(ray, out float distance))
                    {
                        offset = selectedHandle.transform.position - ray.GetPoint(distance);
                    }
                }
            }
        }

        private void OnDragEnded(InputAction.CallbackContext context)
        {
            selectedHandle = null;
        }

        // --- コアロジック (解析、生成、再構築) ---

        /// <summary>
        /// q_... の座標の読み込み
        /// </summary>
        private List<Vector3> ParseCommand(string commandString)
        {
            string paramText;
            try
            {
                paramText = commandString.Split(new char[] { ',', '_' }, 2)[1];
            }
            catch
            {
                Debug.LogError($"SplineEditor: コマンド '{commandString}' の解析に失敗しました。spline,q_... の形式ではありません。");
                return null;
            }

            string[] paramsList = paramText.Split(',');

            List<Vector3> parsedEditorPositions = new List<Vector3>();
            pointDataCache.Clear();
            List<string> targetParamsList = new List<string>();
            originalEaseName = "";

            foreach (string param in paramsList)
            {
                string paramStripped = param.Trim();
                if (string.IsNullOrEmpty(paramStripped)) continue;

                if (paramStripped.StartsWith("q_"))
                {
                    string[] parts = paramStripped.Split('_');
                    if (parts.Length >= 8)
                    {
                        Vector3 finalPos = new Vector3(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)
                        );
                        parsedEditorPositions.Add(FinalToEditor(finalPos));
                        //rz と fov をキャッシュ
                        pointDataCache.Add(new SplinePointData
                        {
                            rz = float.Parse(parts[6], CultureInfo.InvariantCulture),
                            fov = float.Parse(parts[7], CultureInfo.InvariantCulture)
                        });
                    }
                }
                else if (char.IsLetter(paramStripped[0]))
                {
                    if (string.IsNullOrEmpty(originalEaseName))
                        originalEaseName = paramStripped;
                }
                else
                {
                    targetParamsList.Add(paramStripped);
                }
            }

            if (parsedEditorPositions.Count < 2)
            {
                Debug.LogError($"SplineEditor: 軌道には最低2点の q_... 制御点が必要です。");
                return null;
            }

            originalTargetParams = string.Join(",", targetParamsList);
            return parsedEditorPositions;
        }

        private void InstantiateHandles(List<Vector3> parsedEditorPositions)
        {
            controlPointHandles.Clear();

            for (int i = 0; i < parsedEditorPositions.Count; i++)
            {
                GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                handle.name = $"SplineHandle_{i}";
                handle.transform.position = parsedEditorPositions[i];
                handle.transform.localScale = Vector3.one * handleScale;

                Renderer handleRenderer = handle.GetComponent<Renderer>();
                if (handleRenderer != null)
                {
                    handleRenderer.material.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply");
                    handleRenderer.material.color = handleColor;
                    handleRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    handleRenderer.receiveShadows = false;
                }

                controlPointHandles.Add(handle);
            }

            GameObject lineObj = new GameObject("SplineVisualizer_Line");
            activeLineRenderer = lineObj.AddComponent<LineRenderer>();

            activeLineRenderer.startWidth = lineWidth;
            activeLineRenderer.endWidth = lineWidth;

            Material lineMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            activeLineRenderer.material = lineMaterial;
            activeLineRenderer.startColor = lineColor;
            activeLineRenderer.endColor = lineColor;
            activeLineRenderer.useWorldSpace = true;
        }

        private void UpdateLineRenderer()
        {
            if (controlPointHandles.Count < 2) return;

            List<Vector3> linePoints = new List<Vector3>();
            int numSegments = controlPointHandles.Count - 1;
            int samplesPerSegment = 20;

            for (int i = 0; i < numSegments; i++)
            {
                Vector3 p1 = controlPointHandles[i].transform.position;
                Vector3 p2 = controlPointHandles[i + 1].transform.position;
                Vector3 p0 = (i > 0) ? controlPointHandles[i - 1].transform.position : p1;
                Vector3 p3 = (i < numSegments - 1) ? controlPointHandles[i + 2].transform.position : p2;

                for (int j = 0; j <= samplesPerSegment; j++)
                {
                    float t = j / (float)samplesPerSegment;
                    Vector3 point = GetCatmullRomPosition(p0, p1, p2, p3, t);
                    linePoints.Add(point);
                }
            }

            activeLineRenderer.positionCount = linePoints.Count;
            activeLineRenderer.SetPositions(linePoints.ToArray());
        }

        /// <summary>
        /// マーカーの座標をq_...形式のコマンド文字列に再構築
        /// </summary>
        private string RebuildCommandString()
        {
            StringBuilder sb = new StringBuilder("spline");
            if (!string.IsNullOrEmpty(originalTargetParams))
            {
                sb.Append(",");
                sb.Append(originalTargetParams);
            }
            for (int i = 0; i < controlPointHandles.Count; i++)
            {
                sb.Append(",");
                Vector3 editorPos = controlPointHandles[i].transform.position;
                Vector3 finalPos = EditorToFinal(editorPos);
                SplinePointData data = pointDataCache[i];
                string q_str = string.Format(CultureInfo.InvariantCulture,
                    "q_{0:0.##}_{1:0.##}_{2:0.##}_0_0_{3:0.#}_{4:F0}",
                    finalPos.x, finalPos.y, finalPos.z, data.rz, data.fov
                );
                sb.Append(q_str);
            }
            if (!string.IsNullOrEmpty(originalEaseName))
            {
                sb.Append(",");
                sb.Append(originalEaseName);
            }
            return sb.ToString();
        }

        private void Cleanup()
        {
            foreach (GameObject handle in controlPointHandles)
            {
                if (handle != null) Destroy(handle);
            }
            controlPointHandles.Clear();
            if (activeLineRenderer != null)
            {
                if (activeLineRenderer.gameObject != null) Destroy(activeLineRenderer.gameObject);
            }
        }

        // --- スプライン計算 (Catmull-Rom) ---
        private static float CatmullRomInterpolate(float p0, float p1, float p2, float p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * ((2f * p1) +
                            (-p0 + p2) * t +
                            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                            (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
        }
        private static Vector3 GetCatmullRomPosition(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            return new Vector3(
                CatmullRomInterpolate(p0.x, p1.x, p2.x, p3.x, t),
                CatmullRomInterpolate(p0.y, p1.y, p2.y, p3.y, t),
                CatmullRomInterpolate(p0.z, p1.z, p2.z, p3.z, t)
            );
        }

        /// <summary>
        /// ChroMapper座標を q_...座標に変換
        /// </summary>
        private Vector3 EditorToFinal(Vector3 editorPos)
        {
            float scale = Options.Instance.avatarCameraScale;
            if (Mathf.Abs(scale) < 1e-6)
            {
                Debug.LogError("SplineEditor: avatarCameraScale がゼロのため、座標を正しく逆変換できません。");
                return editorPos;
            }
            Vector3 pos = editorPos;
            pos -= new Vector3(0, Options.Instance.originMatchOffsetY, Options.Instance.originMatchOffsetZ);
            pos /= scale;
            pos -= new Vector3(Options.Instance.originXoffset, Options.Instance.originYoffset, Options.Instance.originZoffset);
            return pos;
        }

        /// <summary>
        /// q_...座標をChroMpper座標に変換
        /// </summary>
        private Vector3 FinalToEditor(Vector3 finalPos)
        {
            Vector3 pos = finalPos;
            pos += new Vector3(Options.Instance.originXoffset, Options.Instance.originYoffset, Options.Instance.originZoffset);
            pos *= Options.Instance.avatarCameraScale;
            pos += new Vector3(0, Options.Instance.originMatchOffsetY, Options.Instance.originMatchOffsetZ);
            return pos;
        }
    }
}
