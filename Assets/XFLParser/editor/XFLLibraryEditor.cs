using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using display;

namespace editor
{
    [CustomEditor(typeof(XFLLibrary))]
    class XFLLibraryEditor:Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            XFLLibrary library = (XFLLibrary)target;
            if (GUILayout.Button("Update Library"))
            {
                //clear current library
                library.symbolNames.Clear();
                library.symbolList.Clear();
                library.bitmapNames.Clear();
                library.bitmapList.Clear();
                //
                string objectPath = AssetDatabase.GetAssetPath(target);
                DirectoryInfo targetDirectory= Directory.GetParent(objectPath);
                string workingPath = Path.Combine(targetDirectory.FullName, "LIBRARY");

                workingPath = workingPath.Replace("\\","/");
                string relativePath = workingPath.Replace(Application.dataPath, "Assets");

                
                
                if (Directory.Exists(workingPath))
                {
                    DirectoryInfo workingDirectory = new DirectoryInfo(workingPath);
                    FileInfo[] files = workingDirectory.GetFiles();
                    foreach(FileInfo f in files)
                    {
                        
                        switch (f.Extension)
                        {
                            case ".xml":
                                //parse xml here
                                
                                XmlSerializer serializer = new XmlSerializer(typeof(Symbol));
                                FileStream stream = new FileStream(f.FullName, FileMode.Open);
                                Symbol symbol = (Symbol)serializer.Deserialize(stream);
                                stream.Close();
                                library.symbolNames.Add(symbol.name);
                                library.symbolList.Add(symbol);

                                Debug.Log(symbol.Timeline.Count);
                                break;
                            case ".png":
                            case ".jpg":
                            case ".jpeg":
                                //bitmap assets;
                                Bitmap bitmap = new Bitmap();
                                bitmap.name = f.Name.Remove(f.Name.Length-f.Extension.Length);
                                //Debug.Log("load path " + Path.Combine(relativePath, f.Name));
                                bitmap.texture= AssetDatabase.LoadAssetAtPath(Path.Combine(relativePath, f.Name), typeof(Texture)) as Texture;
                                library.bitmapNames.Add(bitmap.name);
                                library.bitmapList.Add(bitmap);
                                break;
                        }
                    }
                    library.RebuildData();
                }
                else
                {
                    Debug.LogError("Copy & paste the LIBRARY folder from xfl files here");
                }
                //library.RebuildData();
            }
        }
    }
}
