using UnityEngine;
using System.Collections.Generic;
using display;
using System;

[CreateAssetMenu, Serializable]
public class XFLLibrary : ScriptableObject {
    public List<string> symbolNames = new List<string>();
    public List<Symbol> symbolList = new List<Symbol>();
    public List<string> bitmapNames = new List<string>();
    public List<Bitmap> bitmapList = new List<Bitmap>();
    public void RebuildData()
    {
       //prepare stuffs here
    }
    public Symbol getSymbol(string symbolName)
    {
        int index = symbolNames.IndexOf(symbolName);
        if (index >= 0)
        {
            return symbolList[index];
        }
        return null;
    }
    public Bitmap getBitmap(string bitmapName)
    {
        int index = bitmapNames.IndexOf(bitmapName);
        if (index >= 0)
        {
            return bitmapList[index];
        }
        return null;
    }
}
