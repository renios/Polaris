namespace Sirenix.OdinInspector.Demos
{
    using Sirenix.OdinInspector.Editor;
    using System.Linq;
    using UnityEngine;
    using Sirenix.Utilities.Editor;
    using Sirenix.Serialization;
    using UnityEditor;

    public class OdinMenuEditorWindowExample : OdinMenuEditorWindow
    {
        [SerializeField, HideLabel]
        private SomeData someData = new SomeData(); // Take a look at SomeData to see how serialization works in Editor Windows.

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                { "Home",                           this,                           EditorIcons.House                       }, // draws the someDataField in this case.
                { "Odin Settings",                  null,                           EditorIcons.SettingsCog                 },
                { "Odin Settings/Color Palettes",   ColorPaletteManager.Instance,   EditorIcons.EyeDropper                  },
                { "Odin Settings/AOT Generation",   AOTGenerationConfig.Instance,   EditorIcons.SmartPhone                  },
                { "Player Settings",                Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault()       },
                { "Some Class",                     this.someData                                                           }
            };

            tree.AddAllAssetsAtPath("Odin Settings/More Odin Settings", "Plugins/Sirenix", typeof(ScriptableObject), true)
                .AddThumbnailIcons();

            tree.AddAssetAtPath("Odin Getting Started", "Plugins/Sirenix/Getting Started With Odin.asset");

            tree.MenuItems.Insert(2, new OdinMenuItem(tree, "Menu Style", tree.DefaultMenuStyle));

            tree.Add("Menu/Items/Are/Created/As/Needed", new GUIContent());
            tree.Add("Menu/Items/Are/Created", new GUIContent("And can be overridden"));

            // As you can see, Odin provides a few ways to quickly add editors / objects to your menu tree.
            // The API also gives you full control over the selection, etc..
            // Make sure to check out the API Documentation for OdinMenuEditorWindow, OdinMenuTree and OdinMenuItem for more information on what you can do!

            return tree;
        }
    }
}