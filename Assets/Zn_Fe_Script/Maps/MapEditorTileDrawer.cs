using System.Collections;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Zn_Fe.Maps
{
    internal sealed class MapEditorTileDrawer//<T> : TwoDimensionalArrayDrawer<T, SRPGTile> where T : IList
    {
        // protected override TableMatrixAttribute GetDefaultTableMatrixAttributeSettings()
        // {
        //     return new TableMatrixAttribute()
        //     {
        //         SquareCells = true,
        //         HideColumnIndices = true,
        //         HideRowIndices = true,
        //         ResizableColumns = false
        //     };
        // }
        //
        // protected override SRPGTile DrawElement(Rect rect, SRPGTile value)
        // {
        //     var  id = DragAndDropUtilities.GetDragAndDropId(rect);
        //     DragAndDropUtilities.DrawDropZone(rect, value.sprite ? value.sprite : null, null, id); // Draws the drop-zone using the items icon.
        //
        //     if (value.sprite != null)
        //     {
        //         // Item count
        //         var countRect = rect.Padding(2).AlignBottom(16);
        //         //value.avo = EditorGUI.IntField(countRect, Mathf.Max(1, 255));
        //         //GUI.Label(countRect, "/ " + 255, SirenixGUIStyles.RightAlignedGreyMiniLabel);
        //     }
        //
        //     value = DragAndDropUtilities.DropZone(rect, value);                                     // Drop zone for ItemSlot structs.
        //     value.sprite = DragAndDropUtilities.DropZone<Sprite>(rect, value.sprite);                     // Drop zone for Item types.
        //     value = DragAndDropUtilities.DragZone(rect, value, true, true);                         // Enables dragging of the ItemSlot
        //
        //     return value;
        // }
        //
        // protected override void DrawPropertyLayout(GUIContent label)
        // {
        //     base.DrawPropertyLayout(label);
        //    
        //     // Draws a drop-zone where we can destroy items.
        //     var rect = GUILayoutUtility.GetRect(0, 40).Padding(2);
        //     // var b = rect.Contains(Event.current.mousePosition);
        //     // Debug.Log(b);
        //     //Debug.Log(rect.center);
        //     
        //     GUI.Label(rect, "/ " + 255, SirenixGUIStyles.RightAlignedGreyMiniLabel);
        //     var id = DragAndDropUtilities.GetDragAndDropId(rect);
        //     //DragAndDropUtilities.DrawDropZone(rect, null as UnityEngine.Object, null, id);
        //     var a = DragAndDropUtilities.DropZone<SRPGTile>(rect, new SRPGTile(), false, id);
        //     if (a .sprite != null)
        //     {
        //         Debug.Log(a.sprite.name);
        //     }
        // }
    }
}