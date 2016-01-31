using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace display
{
    [XmlRoot(ElementName = "DOMSymbolItem", Namespace = "http://ns.adobe.com/xfl/2008/"), Serializable]
    public class Symbol
    {


        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("itemID")]
        public string itemID;
        [XmlAttribute("lastModified")]
        public string lastModified;
        [XmlArray("timeline")]
        [XmlArrayItem("DOMTimeline")]
        public List<DOMTimelineData> Timeline = new List<DOMTimelineData>();
    }
    [Serializable]
    public class DOMTimelineData {
        [XmlAttribute("name")]
        public string Name;
        [XmlArray("layers")]
        [XmlArrayItem("DOMLayer")]
        public List<LayerData> layers = new List<LayerData>();
    }
    [Serializable]
    public class LayerData
    {
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("color")]
        public string color;
        [XmlAttribute("current")]
        public bool current;
        [XmlAttribute("isSelected")]
        public bool isSelected;
        [XmlArray("frames")]
        [XmlArrayItem("DOMFrame")]
        public List<FrameData> frames = new List<FrameData>();
    }
    [Serializable]
    public class FrameData
    {
        [XmlAttribute("index")]
        public int index;
        [XmlAttribute("duration")]
        public int duration;
        [XmlAttribute("tweenType")]
        public string tweenType;
        [XmlAttribute("keyMode")]
        public int keyMode;
        [XmlAttribute("acceleration")]
        public int acceleration;
        //[XmlArray("elements")]
        //[XmlArrayItem("DOMSymbolInstance"), XmlArrayItem("DOMBitmapInstance")]
        //public List<InstanceData> instances = new List<InstanceData>();
        [XmlArray("elements")]
        [XmlArrayItem("DOMSymbolInstance",Type =typeof(InstanceData))]
        [XmlArrayItem("DOMBitmapInstance", Type = typeof(InstanceBitmap))]
        public List<InstanceBaseData> instances = new List<InstanceBaseData>();
    }
    [Serializable]
    public class InstanceBaseData
    {
        [XmlAttribute("libraryItemName")]
        public string libraryItemName;
        [XmlAttribute("symbolType")]
        public string symbolType;
        [XmlAttribute("loop")]
        public string loop;
        [XmlAttribute("centerPoint3DX")]
        public float centerPoint3DX;
        [XmlAttribute("centerPoint3DY")]
        public float centerPoint3DY;
        [XmlElement("matrix")]
        public MatrixHolder matrix = new MatrixHolder();
        [XmlElement("transformationPoint")]
        public TransformationPointData transformationPoint = new TransformationPointData();
        public InstanceType type;
    }
    public enum InstanceType
    {
        DisplayObject,
        Bitmap
    }
    [Serializable]
    public class InstanceData:InstanceBaseData
    {
        InstanceData()
        {
            type = InstanceType.DisplayObject;
        }
        
    }
    [Serializable]
    public class InstanceBitmap : InstanceBaseData
    {
        InstanceBitmap()
        {
            type = InstanceType.Bitmap;
        }
    }
    [Serializable]
    public class MatrixHolder
    {
        [XmlElement("Matrix")]
        public MatrixData matrix=new MatrixData();
    }
    [Serializable]
    public class MatrixData {
        [XmlAttribute("a")]
        public float a=1;
        [XmlAttribute("b")]
        public float b=0;
        [XmlAttribute("c")]
        public float c=0;
        [XmlAttribute("d")]
        public float d=1;
        [XmlAttribute("tx")]
        public float tx=0;
        [XmlAttribute("ty")]
        public float ty=0;
        public MatrixData clone()
        {
            MatrixData cloneMatrix = new MatrixData();
            cloneMatrix.a = a;
            cloneMatrix.b = b;
            cloneMatrix.c = c;
            cloneMatrix.d = d;
            cloneMatrix.tx = tx;
            cloneMatrix.ty = ty;
            return cloneMatrix;
        }
    }
    [Serializable]
    public class TransformationPointData
    {
        [XmlElement("Point")]
        public PointData Point = new PointData();
    }
    [Serializable]
    public class PointData {
        [XmlAttribute("x")]
        public float x=0;
        [XmlAttribute("y")]
        public float y=0;
        public PointData() { }
        public PointData(float x, float y) {
            this.x = x;
            this.y = y;
        }
    }

}
