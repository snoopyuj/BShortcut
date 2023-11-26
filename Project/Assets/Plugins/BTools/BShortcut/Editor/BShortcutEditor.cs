/*
 * @author	Wayne Su
 * @date	2018/02/09
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BTools.BShortcut
{
    /// <summary>
    /// BB 自製資源快捷鍵
    /// </summary>
    public class BShortcutEditor : EditorWindow
    {
        /// <summary>
        /// 此 Editor Window 之 Reference
        /// </summary>
        private static EditorWindow window = null;

        /// <summary>
        /// 設定檔名稱
        /// </summary>
        private readonly string BShortcutDataFileName = "BShortcutData.asset";

        /// <summary>
        /// 添加物件的欄位顏色
        /// </summary>
        private readonly Color AddFieldColor = new Color(0f, 0.25f, 0f, 0.2f);

        /// <summary>
        /// 清單欄位顏色
        /// </summary>
        private readonly Color ListFieldColor = new Color(0f, 0f, 0f, 0.2f);

        private BShortcutData shortcutData = null;
        private Object newObject = null;
        private Vector2 scrollPos = Vector2.zero;

        /// <summary>
        /// Unity Editor 可呼叫此編輯器之 Method
        /// </summary>
        [MenuItem("Window/bTools/bShortcut", false, 1)]
        private static void ShowWindow()
        {
            window = GetWindow(typeof(BShortcutEditor));
            window.titleContent.text = "bShortcut";
        }

        private void OnEnable()
        {
            CacheShortcutData();
            RemoveEmptyElementsInData();
        }

        private void CacheShortcutData()
        {
            string configFilePath = GetEditorScriptFilePath() + BShortcutDataFileName;

            shortcutData = (BShortcutData)(AssetDatabase.LoadAssetAtPath(configFilePath, typeof(BShortcutData)));

            if (shortcutData == null)
            {
                AssetDatabase.CreateAsset(CreateInstance<BShortcutData>(), configFilePath);
                AssetDatabase.SaveAssets();

                shortcutData = (BShortcutData)(AssetDatabase.LoadAssetAtPath(configFilePath, typeof(BShortcutData)));
            }
        }

        private string GetEditorScriptFilePath()
        {
            MonoScript ms = MonoScript.FromScriptableObject(this);
            string m_ScriptFilePath = AssetDatabase.GetAssetPath(ms);

            return m_ScriptFilePath.Split(new[] { ms.name + ".cs" }, System.StringSplitOptions.None)[0];
        }

        private void RemoveEmptyElementsInData()
        {
            shortcutData.dataInfoList = shortcutData.dataInfoList.Where(x => x.obj != null).Distinct().ToList();
            EditorUtility.SetDirty(shortcutData);
        }

        private void OnGUI()
        {
            List<BShortcutData.BShortcutDataInfo> dataInfoList = shortcutData.dataInfoList;

            #region Add Shortcut

            Rect addObjFieldRect = new Rect(1f, 0f, (position.width - 2f), 20f);
            DrawBackgroundBox(addObjFieldRect, AddFieldColor);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = 115f;
                newObject = EditorGUILayout.ObjectField("Drag anything here:", newObject, typeof(Object), true, GUILayout.Width(230f));

                if (newObject != null && !dataInfoList.Exists(x => x.obj == newObject))
                {
                    var firstEmptyIdx = dataInfoList.FindIndex(x => x.obj == null);
                    var newDataInfo = new BShortcutData.BShortcutDataInfo(newObject);

                    if (firstEmptyIdx < 0)
                    {
                        dataInfoList.Add(newDataInfo);
                    }
                    else
                    {
                        dataInfoList[firstEmptyIdx] = newDataInfo;
                    }

                    EditorUtility.SetDirty(shortcutData);
                }

                newObject = null;
            }
            EditorGUILayout.EndHorizontal();

            #endregion Add Shortcut

            #region Show list

            Rect listFieldRect = new Rect(1f, 21f, (position.width - 2f), position.height);
            DrawBackgroundBox(listFieldRect, ListFieldColor);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("[No.]", GUILayout.Width(40f));
                EditorGUILayout.LabelField("[Name]", GUILayout.Width(100f));
                EditorGUILayout.LabelField("", GUILayout.Width(5f));
                EditorGUILayout.LabelField("[Item]");
            }
            EditorGUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - 40f));
            {
                int showIdx = 0;
                BShortcutData.BShortcutDataInfo curInfo;
                StringBuilder labelSb = new StringBuilder();
                int deleteIdx = -1;

                for (var i = 0; i < dataInfoList.Count; ++i)
                {
                    curInfo = dataInfoList[i];

                    if (curInfo.obj == null)
                    {
                        continue;
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        labelSb.Length = 0;
                        labelSb.Append("[");
                        labelSb.Append(showIdx);
                        labelSb.Append("]");

                        EditorGUILayout.LabelField(labelSb.ToString(), GUILayout.Width(40f));

                        EditorGUI.BeginChangeCheck();
                        {
                            curInfo.name = EditorGUILayout.TextField(curInfo.name, GUILayout.Width(100f));

                            EditorGUILayout.LabelField("", GUILayout.Width(5f));

                            curInfo.obj = EditorGUILayout.ObjectField(curInfo.obj, typeof(Object), true);
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(shortcutData);
                        }
                        else if (GUILayout.Button("X", GUILayout.Width(20f)))
                        {
                            deleteIdx = i;
                        }

                        ++showIdx;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (deleteIdx >= 0)
                {
                    dataInfoList.RemoveAt(deleteIdx);
                }
            }
            EditorGUILayout.EndScrollView();

            #endregion Show list
        }

        private void DrawBackgroundBox(Rect _rect, Color _color)
        {
            EditorGUI.HelpBox(_rect, null, MessageType.None);
            EditorGUI.DrawRect(_rect, _color);
        }
    }
}