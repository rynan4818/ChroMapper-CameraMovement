using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.Component
{
    public class SplineEditor : MonoBehaviour
    {
        // --- データ構造 ---
        private class SplineBlock
        {
            public string CommandType; // "spline" or "bspline"
            public List<GameObject> Handles = new List<GameObject>();
            public List<SplinePointData> PointData = new List<SplinePointData>();
            public GameObject TargetStartHandle;
            public GameObject TargetEndHandle;
            public LineRenderer PathLineRenderer;
            public LineRenderer TargetLineRenderer; // 移動モード時のみ

            // パラメータ
            public bool IsMovingTargetMode = false;
            public bool HasCnct = false;
            public bool HasSync = false;
            public string EaseName = "";
            public List<string> OtherParams = new List<string>();
        }

        private class SplinePointData
        {
            public float rz;
            public float fov;
        }

        // --- 入力アクション ---
        private InputAction splineDragAction;
        private InputAction mousePositionAction;

        // --- 内部ステート ---
        private List<SplineBlock> splineBlocks = new List<SplineBlock>();

        // --- マウスドラッグ用 ---
        private GameObject selectedHandle;
        private Camera mainCamera;
        private Plane dragPlane;
        private Vector3 offset;

        // --- 見た目設定 ---
        private static readonly float handleScale = 0.15f;
        private static readonly float lineWidth = 0.05f;

        private static readonly Color cameraHandleColor = new Color(0.1f, 1.0f, 0.1f, 0.8f);
        private static readonly Color cameraLineColor = Color.green;
        private static readonly Color bsplineLineColor = new Color(0.0f, 0.8f, 1.0f, 1.0f);
        private static readonly Color targetHandleColor = new Color(1.0f, 0.0f, 1.0f, 0.8f);
        private static readonly Color targetLineColor = new Color(1.0f, 0.0f, 1.0f, 0.5f);

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

        public void StartEditing(string fullCommandString)
        {
            Cleanup();

            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("SplineEditor: Main Camera が見つかりません。");
                return;
            }

            string[] commands = fullCommandString.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (commands.Length == 0) return;

            foreach (var cmd in commands)
            {
                ParseAndCreateBlock(cmd);
            }

            UpdateAllLineRenderers();

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
            UpdateAllLineRenderers();
        }

        private void OnDragStarted(InputAction.CallbackContext context)
        {
            if (mainCamera == null) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 mousePosition = mousePositionAction.ReadValue<Vector2>();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObj = hit.collider.gameObject;

                foreach (var block in splineBlocks)
                {
                    if (block.Handles.Contains(hitObj) || hitObj == block.TargetStartHandle || hitObj == block.TargetEndHandle)
                    {
                        selectedHandle = hitObj;
                        dragPlane = new Plane(mainCamera.transform.forward, selectedHandle.transform.position);
                        if (dragPlane.Raycast(ray, out float distance))
                        {
                            offset = selectedHandle.transform.position - ray.GetPoint(distance);
                        }
                        return;
                    }
                }
            }
        }

        private void OnDragEnded(InputAction.CallbackContext context)
        {
            selectedHandle = null;
        }

        // --- コアロジック: 解析と生成 ---

        private void ParseAndCreateBlock(string commandString)
        {
            SplineBlock block = new SplineBlock();

            if (commandString.StartsWith("bspline"))
            {
                block.CommandType = "bspline";
            }
            else
            {
                block.CommandType = "spline";
            }

            string paramText;
            try
            {
                var split = commandString.Split(new char[] { ',', '_' }, 2);
                if (split.Length < 2) return;
                paramText = split[1];
            }
            catch
            {
                Debug.LogError($"SplineEditor: コマンド解析失敗: {commandString}");
                return;
            }

            string[] paramsList = paramText.Split(',');
            List<string> targetParamsList = new List<string>();

            foreach (string param in paramsList)
            {
                string p = param.Trim();
                if (string.IsNullOrEmpty(p)) continue;

                if (p.StartsWith("q_"))
                {
                    string[] parts = p.Split('_');
                    if (parts.Length >= 8)
                    {
                        Vector3 finalPos = new Vector3(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)
                        );

                        GameObject handle = CreateHandle($"Handle_{block.Handles.Count}", FinalToEditor(finalPos), cameraHandleColor);
                        block.Handles.Add(handle);

                        block.PointData.Add(new SplinePointData
                        {
                            rz = float.Parse(parts[6], CultureInfo.InvariantCulture),
                            fov = float.Parse(parts[7], CultureInfo.InvariantCulture)
                        });
                    }
                }
                else if (p.ToLower() == "cnct")
                {
                    block.HasCnct = true;
                }
                else if (p.ToLower() == "sync")
                {
                    block.HasSync = true;
                }
                else if (char.IsLetter(p[0]) && string.IsNullOrEmpty(block.EaseName) && p.ToLower() != "cnct" && p.ToLower() != "sync" && !p.StartsWith("r"))
                {
                    block.EaseName = p;
                }
                else
                {
                    if (IsNumeric(p))
                    {
                        targetParamsList.Add(p);
                    }
                    else
                    {
                        block.OtherParams.Add(p);
                    }
                }
            }

            ParseTargetParams(block, targetParamsList);

            GameObject lineObj = new GameObject($"SplineLine_{splineBlocks.Count}");
            Color lineColor = (block.CommandType == "bspline") ? bsplineLineColor : cameraLineColor;
            block.PathLineRenderer = CreateLineRenderer(lineObj, lineColor);

            if (block.IsMovingTargetMode)
            {
                GameObject tLineObj = new GameObject($"TargetLine_{splineBlocks.Count}");
                block.TargetLineRenderer = CreateLineRenderer(tLineObj, targetLineColor);
            }

            splineBlocks.Add(block);
        }

        private bool IsNumeric(string s)
        {
            return float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private void ParseTargetParams(SplineBlock block, List<string> paramsList)
        {
            Vector3? tStart = null;
            Vector3? tEnd = null;

            if (paramsList.Count >= 6)
            {
                Vector3 t1 = new Vector3(float.Parse(paramsList[0]), float.Parse(paramsList[1]), float.Parse(paramsList[2]));
                Vector3 t2 = new Vector3(float.Parse(paramsList[3]), float.Parse(paramsList[4]), float.Parse(paramsList[5]));
                tStart = FinalToEditor(t1);
                tEnd = FinalToEditor(t2);
                block.IsMovingTargetMode = true;
            }
            else if (paramsList.Count >= 3)
            {
                Vector3 t1 = new Vector3(float.Parse(paramsList[0]), float.Parse(paramsList[1]), float.Parse(paramsList[2]));
                tStart = FinalToEditor(t1);
                block.IsMovingTargetMode = false;
            }
            else
            {
                tStart = FinalToEditor(new Vector3(0, 1.6f, 0));
                block.IsMovingTargetMode = false;
            }

            if (tStart.HasValue)
            {
                block.TargetStartHandle = CreateHandle("TargetStart", tStart.Value, targetHandleColor);
            }
            if (tEnd.HasValue && block.IsMovingTargetMode)
            {
                block.TargetEndHandle = CreateHandle("TargetEnd", tEnd.Value, targetHandleColor);
            }
        }

        // --- 描画更新 ---

        private void UpdateAllLineRenderers()
        {
            for (int i = 0; i < splineBlocks.Count; i++)
            {
                SplineBlock prevBlock = (i > 0) ? splineBlocks[i - 1] : null;
                // ★修正: 次のブロックを取得して渡す
                SplineBlock nextBlock = (i < splineBlocks.Count - 1) ? splineBlocks[i + 1] : null;

                UpdateBlockLines(splineBlocks[i], prevBlock, nextBlock);
            }
        }

        private void UpdateBlockLines(SplineBlock block, SplineBlock prevBlock, SplineBlock nextBlock)
        {
            // 1. カメラ軌道
            if (block.PathLineRenderer != null)
            {
                // 描画用の座標リストを構築
                List<Vector3> drawPoints = new List<Vector3>();

                // cnctがある場合、前のブロックの最後の点を「始点」として追加
                if (block.HasCnct && prevBlock != null && prevBlock.Handles.Count > 0)
                {
                    drawPoints.Add(prevBlock.Handles[prevBlock.Handles.Count - 1].transform.position);
                }

                // 現在のブロックの点を追加
                foreach (var h in block.Handles)
                {
                    drawPoints.Add(h.transform.position);
                }

                // 点が2つ未満なら線は引けない
                if (drawPoints.Count < 2)
                {
                    block.PathLineRenderer.positionCount = 0;
                }
                else
                {
                    List<Vector3> linePoints = new List<Vector3>();
                    int numSegments = drawPoints.Count - 1;
                    int samplesPerSegment = 20;
                    bool isBSpline = (block.CommandType == "bspline");

                    for (int i = 0; i < numSegments; i++)
                    {
                        Vector3 p1 = drawPoints[i];
                        Vector3 p2 = drawPoints[i + 1];

                        // ガイド点 p0 の計算 (始点側)
                        Vector3 p0;
                        if (i > 0)
                        {
                            p0 = drawPoints[i - 1];
                        }
                        else
                        {
                            // 最初の区間の場合
                            if (block.HasCnct && prevBlock != null && prevBlock.Handles.Count >= 2)
                            {
                                // cnct時は、前のブロックの「後ろから2番目」をガイドにする
                                p0 = prevBlock.Handles[prevBlock.Handles.Count - 2].transform.position;
                            }
                            else
                            {
                                p0 = p1;
                            }
                        }

                        // ガイド点 p3 の計算 (終点側)
                        Vector3 p3;
                        if (i < numSegments - 1)
                        {
                            p3 = drawPoints[i + 2];
                        }
                        else
                        {
                            // ★修正: 最後の区間の処理
                            // 「次のブロック」が存在し、かつ「次のブロックがcnct (接続) モード」である場合、
                            // 現在の軌道は次のブロックの始点へ向かって滑らかに繋がるべきなので、
                            // 次のブロックの最初のハンドルを P3 (ガイド点) として参照する。
                            if (nextBlock != null && nextBlock.HasCnct && nextBlock.Handles.Count > 0)
                            {
                                p3 = nextBlock.Handles[0].transform.position;
                            }
                            else
                            {
                                // 接続がない場合は従来どおり p2 を代用 (あるいは p2 + (p2-p1) 等で外挿)
                                p3 = p2;
                            }
                        }

                        for (int j = 0; j <= samplesPerSegment; j++)
                        {
                            float t = j / (float)samplesPerSegment;
                            if (isBSpline)
                            {
                                linePoints.Add(GetBSplinePosition(p0, p1, p2, p3, t));
                            }
                            else
                            {
                                linePoints.Add(GetCatmullRomPosition(p0, p1, p2, p3, t));
                            }
                        }
                    }
                    block.PathLineRenderer.positionCount = linePoints.Count;
                    block.PathLineRenderer.SetPositions(linePoints.ToArray());
                }
            }

            // 2. 注視点軌道 (Linear)
            if (block.IsMovingTargetMode && block.TargetStartHandle != null && block.TargetEndHandle != null && block.TargetLineRenderer != null)
            {
                block.TargetLineRenderer.positionCount = 2;
                block.TargetLineRenderer.SetPosition(0, block.TargetStartHandle.transform.position);
                block.TargetLineRenderer.SetPosition(1, block.TargetEndHandle.transform.position);
            }
        }

        // --- コマンド再構築 ---

        private string RebuildCommandString()
        {
            StringBuilder fullSb = new StringBuilder();

            for (int b = 0; b < splineBlocks.Count; b++)
            {
                var block = splineBlocks[b];

                if (b > 0) fullSb.Append(" ");

                StringBuilder sb = new StringBuilder(block.CommandType);

                if (block.TargetStartHandle != null)
                {
                    sb.Append(",");
                    Vector3 finalStart = EditorToFinal(block.TargetStartHandle.transform.position);

                    if (block.IsMovingTargetMode && block.TargetEndHandle != null)
                    {
                        Vector3 finalEnd = EditorToFinal(block.TargetEndHandle.transform.position);
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0:0.##},{1:0.##},{2:0.##},{3:0.##},{4:0.##},{5:0.##}",
                            finalStart.x, finalStart.y, finalStart.z,
                            finalEnd.x, finalEnd.y, finalEnd.z);
                    }
                    else
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0:0.##},{1:0.##},{2:0.##}",
                            finalStart.x, finalStart.y, finalStart.z);
                    }
                }

                for (int i = 0; i < block.Handles.Count; i++)
                {
                    sb.Append(",");
                    Vector3 finalPos = EditorToFinal(block.Handles[i].transform.position);
                    SplinePointData data = block.PointData[i];

                    string q_str = string.Format(CultureInfo.InvariantCulture,
                        "q_{0:0.##}_{1:0.##}_{2:0.##}_0_0_{3:0.#}_{4:F0}",
                        finalPos.x, finalPos.y, finalPos.z, data.rz, data.fov
                    );
                    sb.Append(q_str);
                }

                if (!string.IsNullOrEmpty(block.EaseName))
                {
                    sb.Append(",");
                    sb.Append(block.EaseName);
                }

                if (block.HasCnct) sb.Append(",cnct");
                if (block.HasSync) sb.Append(",sync");

                foreach (var p in block.OtherParams)
                {
                    sb.Append(",");
                    sb.Append(p);
                }

                fullSb.Append(sb.ToString());
            }

            return fullSb.ToString();
        }

        private void Cleanup()
        {
            foreach (var block in splineBlocks)
            {
                foreach (var h in block.Handles) if (h) Destroy(h);
                if (block.TargetStartHandle) Destroy(block.TargetStartHandle);
                if (block.TargetEndHandle) Destroy(block.TargetEndHandle);
                if (block.PathLineRenderer) Destroy(block.PathLineRenderer.gameObject);
                if (block.TargetLineRenderer) Destroy(block.TargetLineRenderer.gameObject);
            }
            splineBlocks.Clear();
            selectedHandle = null;
        }

        // --- 共通生成メソッド ---
        private GameObject CreateHandle(string name, Vector3 pos, Color color)
        {
            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            handle.name = name;
            handle.transform.position = pos;
            handle.transform.localScale = Vector3.one * handleScale;

            Renderer r = handle.GetComponent<Renderer>();
            if (r != null)
            {
                r.material.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply");
                r.material.color = color;
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                r.receiveShadows = false;
            }
            return handle;
        }

        private LineRenderer CreateLineRenderer(GameObject obj, Color color)
        {
            LineRenderer lr = obj.AddComponent<LineRenderer>();
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lr.startColor = color;
            lr.endColor = color;
            lr.useWorldSpace = true;
            return lr;
        }

        // --- 座標変換 ---
        private Vector3 EditorToFinal(Vector3 editorPos)
        {
            float scale = Options.Instance.avatarCameraScale;
            if (Mathf.Abs(scale) < 1e-6) return editorPos;
            Vector3 pos = editorPos;
            pos -= new Vector3(0, Options.Instance.originMatchOffsetY, Options.Instance.originMatchOffsetZ);
            pos /= scale;
            pos -= new Vector3(Options.Instance.originXoffset, Options.Instance.originYoffset, Options.Instance.originZoffset);
            return pos;
        }

        private Vector3 FinalToEditor(Vector3 finalPos)
        {
            Vector3 pos = finalPos;
            pos += new Vector3(Options.Instance.originXoffset, Options.Instance.originYoffset, Options.Instance.originZoffset);
            pos *= Options.Instance.avatarCameraScale;
            pos += new Vector3(0, Options.Instance.originMatchOffsetY, Options.Instance.originMatchOffsetZ);
            return pos;
        }

        // --- 数学関数 ---
        private static float CatmullRomInterpolate(float p0, float p1, float p2, float p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return 0.5f * ((2f * p1) + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 + (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
        }
        private static Vector3 GetCatmullRomPosition(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            return new Vector3(
                CatmullRomInterpolate(p0.x, p1.x, p2.x, p3.x, t),
                CatmullRomInterpolate(p0.y, p1.y, p2.y, p3.y, t),
                CatmullRomInterpolate(p0.z, p1.z, p2.z, p3.z, t)
            );
        }

        private static float BSplineInterpolate(float p0, float p1, float p2, float p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return (p0 * Mathf.Pow(1 - t, 3) +
                    p1 * (3 * t3 - 6 * t2 + 4) +
                    p2 * (-3 * t3 + 3 * t2 + 3 * t + 1) +
                    p3 * t3) / 6.0f;
        }
        private static Vector3 GetBSplinePosition(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            return new Vector3(
                BSplineInterpolate(p0.x, p1.x, p2.x, p3.x, t),
                BSplineInterpolate(p0.y, p1.y, p2.y, p3.y, t),
                BSplineInterpolate(p0.z, p1.z, p2.z, p3.z, t)
            );
        }
    }
}
