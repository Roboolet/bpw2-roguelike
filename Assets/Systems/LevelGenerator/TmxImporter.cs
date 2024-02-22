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
                // turn the comma-seperated csv into a byte array, and then into a
                byte[] numbers = Array.ConvertAll(map.Layer.Data.Text.Split(','), byte.Parse);

                level.data = new GridTileType[level.width*level.height];
                for(int i = 0; i < numbers.Length; i++)
                {
                    level.data[i] = (GridTileType)numbers[i];
                }                

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
    }
    #region Xml stuff

    [XmlRoot(ElementName = "data")]
    public class TmxData
    {
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "layer")]
    public class TmxLayer
    {
        [XmlElement(ElementName = "data")]
        public TmxData Data { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }
    }

    [XmlRoot(ElementName = "map")]
    public class TmxMap
    {
        [XmlElement(ElementName = "layer")]
        public TmxLayer Layer { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "tiledversion")]
        public string Tiledversion { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }
        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }
    }
    #endregion
}
#endif


