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

                level.geometryData = ConvertCsvToIntArray<GridTileType>(map.Layer[0].Data.Text.Split(','), map.Width * map.Height);

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

        T[] ConvertCsvToIntArray<T>(string[] csv, int arraySize) where T : Enum
        {
            int[] numbers = Array.ConvertAll(csv, int.Parse);
            T[] temp = new T[arraySize];

            for (int i = 0; i < numbers.Length; i++)
            {
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


