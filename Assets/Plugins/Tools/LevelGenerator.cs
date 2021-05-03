using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Tools
{
    [System.Serializable]
    public struct ColorLevelTile
    {
        public Color color;
        public GameObject gameObject;
    }

    /// <summary>
    /// Generates Levels In base of an image and its pixels
    /// </summary>
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private Texture2D map;
        [SerializeField] private ColorLevelTile[] tiles;

        private GameObject p_tempParent;
        private List<GameObject> p_tempGroups;

        /// <summary>
        /// Iterates through the whole map and instantiates objects depending the ColorLevelTiles set
        /// </summary>
        public void GenerateLevel()
        {
            #region Organize_On_Groups

            (p_tempParent = new GameObject()).transform.name = "New Area";
            p_tempGroups = new List<GameObject>();
            foreach (ColorLevelTile tile in tiles)
            {
                var group = new GameObject();
                group.transform.name = tile.gameObject.name + " Group";
                group.transform.parent = p_tempParent.transform;
                p_tempGroups.Add(group);
            }

            #endregion

            for (var x = 0; x < map.width; x++)
            {
                for (var y = 0; y < map.width; y++) GenerateTile(x, y);
            }
        }

        /// <summary>
        /// Instantiates the corresponding gameObject on the position x, y transfered by parameters if the pixel is the corresponded color
        /// </summary>
        /// <param name="x">Position X of the pixel of the image</param>
        /// <param name="y">Position Y of the pixel of the image</param>
        public void GenerateTile(int x, int y)
        {
            Color pixelColor = map.GetPixel(x, y);
            if (pixelColor.a == 0) return;
            foreach (ColorLevelTile tile in tiles)
            {
                if (tile.color.Equals(pixelColor))
#if UNITY_EDITOR
                    UtilityMethods.InstantiatePrefab(tile.gameObject, new Vector2(x, y), tile.gameObject.transform.rotation)
                        .transform.parent = p_tempGroups
                        .Find(g => g.transform.name.Contains(tile.gameObject.transform.name)).transform;
#else
                    Instantiate(tile.gameObject, new Vector2(x, y), tile.gameObject.transform.rotation)
                        .transform.parent = p_tempGroups
                        .Find(g => g.transform.name.Contains(tile.gameObject.transform.name)).transform;
#endif
            }
        }
    }
}
