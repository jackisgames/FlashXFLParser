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

        void renderSymbol(Symbol symbol,float targetFrame, Vector2 parentPoint, MatrixData parentMat=null)
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
                                    InstanceBaseData instance = frame.instances[instanceIndex];
                                    MatrixData mat = instance.matrix.matrix.clone();
                                    Vector2 centerPoint = new Vector2(instance.centerPoint3DX,instance.centerPoint3DY);
                                    if (parentMat != null)
                                    {
                                        mat.tx += parentMat.tx;
                                        mat.ty += parentMat.ty;
                                        mat.a *= parentMat.a;
                                        mat.b *= parentMat.b;
                                        mat.c *= parentMat.c;
                                        mat.d *= parentMat.d;
                                        centerPoint += parentPoint;
                                    }
                                    if (frame.tweenType == "motion")
                                    {
                                        
                                        if (frameIndex < layer.frames.Count - 1)
                                        {
                                            

                                            FrameData nextFrame = layer.frames[frameIndex + 1];
                                            
                                            if (instanceIndex < nextFrame.instances.Count)
                                            {
                                                


                                                InstanceBaseData nextFrameInstance = nextFrame.instances[instanceIndex];
                                                MatrixData nextFramePos = nextFrameInstance.matrix.matrix.clone();
                                                Vector2 nextCenterPoint = new Vector2(nextFrameInstance.centerPoint3DX, nextFrameInstance.centerPoint3DY);
                                                if (parentMat != null)
                                                {
                                                    nextFramePos.tx += parentMat.tx;
                                                    nextFramePos.ty += parentMat.ty;
                                                    nextFramePos.a *= parentMat.a;
                                                    nextFramePos.b *= parentMat.b;
                                                    nextFramePos.c *= parentMat.c;
                                                    nextFramePos.d *= parentMat.d;
                                                    nextCenterPoint += parentPoint;
                                                }

                                                float animProgress = (targetFrame - frame.index) / frame.duration;
                                                mat.tx = Mathf.Lerp(mat.tx, nextFramePos.tx, animProgress);
                                                mat.ty = Mathf.Lerp(mat.ty, nextFramePos.ty, animProgress);
                                                mat.a = Mathf.Lerp(mat.a, nextFramePos.a, animProgress);
                                                mat.b = Mathf.Lerp(mat.b, nextFramePos.b, animProgress);
                                                mat.c = Mathf.Lerp(mat.c, nextFramePos.c, animProgress);
                                                mat.d = Mathf.Lerp(mat.d, nextFramePos.d, animProgress);
                                                centerPoint = Vector2.Lerp(centerPoint, nextCenterPoint, animProgress);
                                                if (instance.type == InstanceType.Bitmap)
                                                {
                                                    GUILayout.Label("final point");
                                                    GUILayout.Label("point " + centerPoint.x + " " + centerPoint.y);
                                                }
                                                /*GUILayout.Label("===============");
                                                GUILayout.Label(frame.index + " " + frame.duration);
                                                GUILayout.Label("center "+instance.centerPoint3DX + " " + instance.centerPoint3DY);
                                                GUILayout.Label("point " + instance.transformationPoint.Point.x + " " + instance.transformationPoint.Point.y);
                                                GUILayout.Label(nextFrame.index + " " + nextFrame.duration);
                                                GUILayout.Label("center " + nextFrameInstance.centerPoint3DX + " " + nextFrameInstance.centerPoint3DY);
                                                GUILayout.Label("point " + nextFrameInstance.transformationPoint.Point.x + " " + nextFrameInstance.transformationPoint.Point.y);*/

                                            }

                                        }

                                    }
                                    //GUILayout.Label("instance: " + instance.libraryItemName +" type"+ instance.type);
                                    if (instance.type == InstanceType.DisplayObject)
                                    {
                                        Symbol graphic = library.getSymbol(instance.libraryItemName);
                                        if (graphic != null)
                                        {
                                            renderSymbol(graphic, 0,centerPoint,mat);
                                            
                                        }
                                    }
                                    else {
                                        Bitmap bitmap = library.getBitmap(instance.libraryItemName);

                                        if (bitmap == null)
                                        {
                                            //GUILayout.Label("bitmap null, " +instance.libraryItemName);
                                        }
                                        else
                                        {
                                            if (bitmap.texture != null)
                                            {
                                                //GUILayout.Label("bitmap: " + bitmap.name);

                                                
                                                float w = bitmap.texture.width;
                                                float h = bitmap.texture.height;
                                                //got the math stuffs from here: http://math.stackexchange.com/questions/13150/extracting-rotation-scale-values-from-2d-transformation-matrix
                                                Matrix4x4 mat4 = Matrix4x4.identity;
                                                float rotation = Mathf.Atan2(-mat.b, mat.a) * 180 / Mathf.PI;
                                                mat4.SetTRS(new Vector3(transform.position.x+w/2+mat.tx, transform.position.y+h/2+mat.ty, 0), Quaternion.Euler(new Vector3(0,0,rotation)), new Vector3(mat.a,mat.d,1));
                                                GUI.matrix = mat4;
                                                GUI.DrawTexture(new Rect(centerPoint.x - w/2, centerPoint.y- h/2, w, h), bitmap.texture);
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
                renderSymbol(activeSymbol,targetFrame,Vector2.zero);
            }
        }
    }
}
