using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor.AssetImporters;
using UnityEditor;
using System.Xml;
using System.Xml.Serialization;

namespace TmxImporter
{
    [ScriptedImporter(1, "tmx")]
    public class TmxImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            try
            {
                LevelData level = ScriptableObject.CreateInstance<LevelData>();

                // load room data from xml
                TmxMap map;
                XmlSerializer serializer = new XmlSerializer(typeof(TmxMap));
                using (FileStream stream = new FileStream(ctx.assetPath, FileMode.Open))
                {
                    map = serializer.Deserialize(stream) as TmxMap;
                }

                level.width = map.Width;
                level.height = map.Height;

                // in the tiled editor, the bottom-most layer is layer 0
                level.geometryData = ConvertLayerToEnumArray<GridTileGeometry>(map.Layer[0]);
                if (map.Layer.Count > 1) level.spawnsData = ConvertLayerToEnumArray<GridTileSpawns>(map.Layer[1]);
                if (map.Layer.Count > 2) level.backgroundData = ConvertLayerToEnumArray<GridTileBackground>(map.Layer[2]);

                // finalize
                EditorUtility.SetDirty(this);
                ctx.AddObjectToAsset(Path.GetFileNameWithoutExtension(ctx.assetPath), level);
                ctx.SetMainObject(level);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        T[] ConvertLayerToEnumArray<T>(TmxLayer layer) where T : Enum
        {
            // convert the string into a list of individual numbers
            string[] csv = layer.Data.Text.Split(',');

            // turn the numbers into ints
            int[] numbers = Array.ConvertAll(csv, int.Parse);
            T[] temp = new T[layer.Width * layer.Height];

            for (int i = 0; i < numbers.Length; i++)
            {
                // cast numbers to enum
                temp[i] = (T)(object)numbers[i];
            }

            return temp;
        }
    }

    #region XML converted to C# class

    [Serializable, XmlRoot(ElementName = "tileset")]
    public class TmxTileset
    {

        [XmlAttribute(AttributeName = "firstgid")]
        public int Firstgid { get; set; }

        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }
    }

    [Serializable, XmlRoot(ElementName = "data")]
    public class TmxData
    {

        [XmlAttribute(AttributeName = "encoding")]
        public string Encoding { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [Serializable, XmlRoot(ElementName = "layer")]
    public class TmxLayer
    {

        [XmlElement(ElementName = "data")]
        public TmxData Data { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [Serializable, XmlRoot(ElementName = "map")]
    public class TmxMap
    {

        [XmlElement(ElementName = "tileset")]
        public TmxTileset Tileset { get; set; }

        [XmlElement(ElementName = "layer")]
        public List<TmxLayer> Layer { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "tiledversion")]
        public string Tiledversion { get; set; }

        [XmlAttribute(AttributeName = "orientation")]
        public string Orientation { get; set; }

        [XmlAttribute(AttributeName = "renderorder")]
        public string Renderorder { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }

        [XmlAttribute(AttributeName = "tilewidth")]
        public int Tilewidth { get; set; }

        [XmlAttribute(AttributeName = "tileheight")]
        public int Tileheight { get; set; }

        [XmlAttribute(AttributeName = "infinite")]
        public int Infinite { get; set; }

        [XmlAttribute(AttributeName = "nextlayerid")]
        public int Nextlayerid { get; set; }

        [XmlAttribute(AttributeName = "nextobjectid")]
        public int Nextobjectid { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    #endregion
}
#endif

public enum GridTileGeometry : int
{
    // The lightsource and chest IDs are leftovers and should remain unused
    Empty, Wall, Ladder, StairRight, StairLeft, LightSource, GeneratorStartPoint, GeneratorEndPoint, Chest
}

public enum GridTileSpawns : int
{
    Empty, ChestSpawnPoint=24, GroundEnemySpawnPoint=25, FlyingEnemySpawnPoint=26
}

public enum GridTileBackground : int
{
    Empty, LightSource=40
}

public static class GridTileTypeHelper
{
    public static bool IsTileEmpty(GridTileGeometry type)
    {
        return (type == GridTileGeometry.Empty || type == GridTileGeometry.LightSource || type == GridTileGeometry.Chest);
    }

    public static bool IsTileClimbable(GridTileGeometry type)
    {
        return (type == GridTileGeometry.Ladder || type == GridTileGeometry.GeneratorEndPoint || type == GridTileGeometry.GeneratorStartPoint);
    }

    public static bool IsTileSolid(GridTileGeometry type)
    {
        return (type == GridTileGeometry.Wall || type == GridTileGeometry.StairLeft || type == GridTileGeometry.StairRight);
    }

}

