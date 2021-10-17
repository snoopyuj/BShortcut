/*
 * @author	Wayne Su
 * @date	2018/02/09
 */

using System.Collections.Generic;
using UnityEngine;

namespace BTools.BShortcut
{
    /// <summary>
    /// 快速鍵資料格式
    /// </summary>
    public class BShortcutData : ScriptableObject
    {
        /// <summary>
        /// 儲存的資料格式
        /// </summary>
        [System.Serializable]
        public class BShortcutDataInfo
        {
            public string name = null;
            public Object obj = null;

            public BShortcutDataInfo(Object _obj)
            {
                Debug.Assert(_obj != null, "Object can't be null.");

                name = _obj.name;
                obj = _obj;
            }
        }

        /// <summary>
        /// 資料清單
        /// </summary>
        public List<BShortcutDataInfo> dataInfoList = new List<BShortcutDataInfo>();
    }
}