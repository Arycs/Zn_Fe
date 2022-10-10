using System.Linq;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace Zn_Fe.Maps
{
    public class MapEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("ZnTools/Editor/Map Editor")]
        private static void Open()
        {
            var window = GetWindow<MapEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;
            tree.AddAssetAtPath("MapEditor", "Assets/Zn_Fe_Script/Maps/New Zn Map Editor.asset").AddIcon(EditorIcons.Airplane);
            
            tree.EnumerateTree().Where(x => x.Value as Item).ForEach(AddDragHandles);
            
            return tree;
        }
        
        private void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }
    }
}