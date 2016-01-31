using display;
using UnityEngine;

namespace renderer
{
    [ExecuteInEditMode]
    public class GUIRenderer:MonoBehaviour
    {
        public XFLLibrary library;
        [HideInInspector]
        public string symbolName;
        bool isReady = false;
        Symbol activeSymbol;
        [SerializeField, HideInInspector]
        int maxFrames = 0;
        [SerializeField, HideInInspector]
        float currentFrame = 0;
        [SerializeField, HideInInspector]
        bool isPlaying = true;
        public void load()
        {
            maxFrames = 0;
            currentFrame = 0;
            isReady = false;
            activeSymbol = null;
            //Debug.Log("lib " + library != null);
            if (library != null)
            {
                //library.symbolLibrary.TryGetValue(symbolName, out activeSymbol);
                activeSymbol = library.getSymbol(symbolName);
                if (activeSymbol != null)
                {
                    
                    foreach (DOMTimelineData timeline in activeSymbol.Timeline)
                    {
                        
                        foreach (LayerData layer in timeline.layers)
                        {
                            FrameData lastFrame = layer.frames[layer.frames.Count - 1];
                            maxFrames = Mathf.Max(maxFrames, lastFrame.index + lastFrame.duration);
                        }
                    }
                    isReady = true;
                }
            }
            
        }

        void renderSymbol(Symbol symbol,float targetFrame,MatrixData parentMatrix=null)
        {
            
            foreach (DOMTimelineData timeline in symbol.Timeline)
            {
                for(int layerIndex = timeline.layers.Count-1; layerIndex >=0; --layerIndex)
                {
                    LayerData layer = timeline.layers[layerIndex];
                    for (int frameIndex=0;frameIndex<layer.frames.Count; frameIndex++)
                    {
                        FrameData frame = layer.frames[frameIndex];
                        
                        if (targetFrame >= frame.index)
                        {
                            if (targetFrame <= frame.index + frame.duration)
                            {
                                
                                for (int instanceIndex=0;instanceIndex<frame.instances.Count;instanceIndex++)
                                {
                                    InstanceData instance = frame.instances[instanceIndex];
                                    MatrixData mat = instance.matrix.matrix.clone();
                                    if (parentMatrix != null)
                                    {
                                        mat.tx += parentMatrix.tx;
                                        mat.ty += parentMatrix.ty;
                                        mat.a *= parentMatrix.a;
                                        mat.b *= parentMatrix.b;
                                        mat.c *= parentMatrix.c;
                                        mat.d *= parentMatrix.d;
                                    }
                                    if (frame.tweenType == "motion")
                                    {
                                        
                                        if (frameIndex < layer.frames.Count - 1)
                                        {
                                            
                                            
                                            FrameData nextFrame = layer.frames[frameIndex + 1];
                                            if (instanceIndex < nextFrame.instances.Count)
                                            {
                                                InstanceData nextFrameInstance = nextFrame.instances[instanceIndex];
                                                MatrixData nextFramePos = nextFrameInstance.matrix.matrix;
                                                float animProgress = (targetFrame - frame.index) / frame.duration;
                                                mat.tx = Mathf.Lerp(mat.tx, nextFramePos.tx, animProgress);
                                                mat.ty = Mathf.Lerp(mat.ty, nextFramePos.ty, animProgress);
                                                mat.a = Mathf.Lerp(mat.a, nextFramePos.a, animProgress);
                                                mat.b = Mathf.Lerp(mat.b, nextFramePos.b, animProgress);
                                                mat.c = Mathf.Lerp(mat.c, nextFramePos.c, animProgress);
                                                mat.d = Mathf.Lerp(mat.d, nextFramePos.d, animProgress);
                                            }

                                        }

                                    }
                                    if (instance.symbolType == "graphic")
                                    {
                                        Symbol graphic = library.getSymbol(instance.libraryItemName);
                                        if (graphic != null)
                                        {
                                            renderSymbol(graphic, 0,mat);
                                            
                                        }
                                    }
                                    else {
                                        Bitmap bitmap = library.getBitmap(instance.libraryItemName);

                                        if (bitmap == null)
                                        {

                                        }
                                        else
                                        {
                                            if (bitmap.texture != null)
                                            {



                                                float w = bitmap.texture.width;
                                                float h = bitmap.texture.height;
                                                //got the math stuffs from here: http://math.stackexchange.com/questions/13150/extracting-rotation-scale-values-from-2d-transformation-matrix
                                                Matrix4x4 mat4 = Matrix4x4.identity;
                                                float rotation = Mathf.Atan2(-mat.b, mat.a) * 180 / Mathf.PI;
                                                mat4.SetTRS(new Vector3(transform.position.x+w/2+mat.tx, transform.position.y+h/2+mat.ty, 0), Quaternion.Euler(new Vector3(0,0,rotation)), new Vector3(mat.a,mat.d,1));
                                                GUI.matrix = mat4;
                                                GUI.DrawTexture(new Rect(-w/2, -h/2, w, h), bitmap.texture);
                                                GUI.matrix = Matrix4x4.identity;
                                                /*mat4.m00 = mat.a;
                                                mat4.m10 = mat.b;
                                                mat4.m10 = mat.c;
                                                mat4.m11 = mat.d;
                                                mat4.m20 = mat.tx;
                                                mat4.m21 = mat.ty;
                                                GUI.matrix = mat4;*/
                                                //GUI.DrawTexture(new Rect(transform.position.x+ tx, transform.position.y + ty, w, h), bitmap.texture);

                                            }
                                        }
                                    }
                                    //
                                }
                                //break;
                            }
                        }
                    }
                }
            }
        }
        void OnGUI()
        {
            
            
            if (isReady)
            {
                if (maxFrames > 0)
                {
                    if (isPlaying)
                    {
                        currentFrame += Time.deltaTime * 30;
                        if (currentFrame >= maxFrames)
                        {
                            currentFrame = 0;
                        }
                    }
                }
                float targetFrame = currentFrame;
                GUILayout.Label(targetFrame + " " + maxFrames);
                renderSymbol(activeSymbol,targetFrame);
            }
        }
    }
}
