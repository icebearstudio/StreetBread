using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class Outliner : MonoBehaviour
{
    [System.Serializable]
    public class OutlinedObject
    {
        public Renderer[] renderers;

        public OutlinedObject(Renderer[] renderers)
        {
            this.renderers = renderers;
        }
    }

    public List<Color> outlineColor = new List<Color>();
    [Range(0, 1)] public float occludedOutlineOpacity = 0.3f;
    [Range(0, 10)] public List<float> blurSize = new List<float>();
    [Range(0, 1)] public float alphaCutoff = 0.1f;
    public CameraEvent bufferDrawEvent = CameraEvent.BeforeImageEffects;

    public List<CommandBuffer> commandBuffer = new List<CommandBuffer>();
    public List<Material> outlineMaterial = new List<Material>();
    public Camera cam;
    public List<List<OutlinedObject>> outlinedObjects = new List<List<OutlinedObject>>();
    public List<Transform> parent;
    int offset = 4;

    void OnEnable()
    {
        Cleanup();
    }

    void OnDisable()
    {
        Cleanup();
    }

    private void Start()
    {
        //Invoke("init", 0.5f);
    }

    public void init()
    {
        ClearAllOutlinedObjects();
        outlinedObjects = new List<List<OutlinedObject>>();
        bool b = false;
        for (int k = 0; k < parent.Count; k++)
        {
            outlinedObjects.Add(new List<OutlinedObject>());
            if (parent[k] != null)
            {
                b = true;
                for (int i = 0; i < parent[k].childCount; i++)
                {
                    for (int j = 0; j < parent[k].GetChild(i).childCount; j++)
                    {
                        if (parent[k].GetChild(i).GetChild(j).gameObject.activeSelf)
                        {
                            Transform t = parent[k].GetChild(i).GetChild(j);
                            foreach (Transform tf in t)
                            {
                                if (tf.childCount == 0)
                                {
                                    if (tf.GetComponent<MeshRenderer>() != null)
                                        AddOutlinedObject(new OutlinedObject(new Renderer[] { tf.GetComponent<MeshRenderer>() }));
                                }
                                else
                                {
                                    foreach (Transform tf1 in tf)
                                    {
                                        if (tf1.GetComponent<MeshRenderer>() != null)
                                            AddOutlinedObject(new OutlinedObject(new Renderer[] { tf1.GetComponent<MeshRenderer>() }));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (b)
            CreateCommandBuffer();
    }

    void Cleanup()
    {
        if (commandBuffer == null) return;
        foreach (CommandBuffer c in commandBuffer)
            cam.RemoveCommandBuffer(bufferDrawEvent, c);
        commandBuffer = null;
    }


    public void AddOutlinedObject(OutlinedObject obj)
    {
        outlinedObjects[outlinedObjects.Count - 1].Add(obj);
    }


    public void RemoveOutlinedObject(OutlinedObject obj)
    {
        outlinedObjects[outlinedObjects.Count - 1].Remove(obj);
        CreateCommandBuffer();
    }


    public void ClearAllOutlinedObjects()
    {
        outlinedObjects.Clear();
        CreateCommandBuffer();
    }


    public void CreateCommandBuffer()
    {
        // nothing to outline? cleanup
        if (outlinedObjects.Count == 0)
        {
            Cleanup();
            return;
        }

        // camera
        if (cam == null)
        {
            cam = GetComponent<Camera>();
            cam.depthTextureMode = DepthTextureMode.Depth;
        }

        for (int k = 0; k < outlinedObjects.Count; k++)
        {
            // material
            if (outlineMaterial.Count - 1 < k)
            {
                outlineMaterial.Add(new Material(Shader.Find("Hidden/Outliner")));
            }

            if (commandBuffer == null) commandBuffer = new List<CommandBuffer>();
            // command buffer
            if (commandBuffer.Count - 1 < k)
            {
                commandBuffer.Add(new CommandBuffer());
                commandBuffer[commandBuffer.Count - 1].name = "Outliner Command Buffer " + k;
                cam.AddCommandBuffer(bufferDrawEvent, commandBuffer[commandBuffer.Count - 1]);
            }
            else
            {
                commandBuffer.Clear();
            }

            // initialization
            int width = (cam.targetTexture != null) ? cam.targetTexture.width : cam.pixelWidth;
            int height = (cam.targetTexture != null) ? cam.targetTexture.height : cam.pixelHeight;
#if !UNITY_EDITOR && UNITY_ANDROID
            width /= offset;
            height /= offset;
#endif
            int aTempID = Shader.PropertyToID("_aTemp");
            commandBuffer[k].GetTemporaryRT(aTempID, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            commandBuffer[k].SetRenderTarget(aTempID, BuiltinRenderTextureType.CurrentActive);
            commandBuffer[k].ClearRenderTarget(false, true, Color.clear);

            // render selected objects into a mask buffer, with different colors for visible vs occluded ones 
            float id = 0f;
            foreach (var collection in outlinedObjects[k])
            {
                commandBuffer[k].SetGlobalFloat("_ObjectId", id);
                foreach (var render in collection.renderers)
                {
                    Material mat = outlineMaterial[k];
                    if (render.material.mainTexture != null)
                    {
                        mat = new Material(outlineMaterial[k]);
                        mat.SetTexture("_MainTex", render.material.mainTexture);
                        mat.SetFloat("_Cutoff", alphaCutoff);
                        mat.SetFloat("_DoClip", 1);
                    }
                    commandBuffer[k].DrawRenderer(render, mat, 0, 1);
                    commandBuffer[k].DrawRenderer(render, mat, 0, 0);
                }
            }

            // object ID edge dectection pass
            int bTempID = Shader.PropertyToID("_bTemp");
            commandBuffer[k].GetTemporaryRT(bTempID, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            commandBuffer[k].Blit(aTempID, bTempID, outlineMaterial[k], 3);

            // Blur - adjusting blur size to appear the same size, no matter the resolution
            float proportionalBlurSize = ((float)width / 1080f) * blurSize[k];
#if !UNITY_EDITOR && UNITY_ANDROID
          proportionalBlurSize = ((float)width / (1080f * offset * 0.5f)) * blurSize[k];
#endif
            int cTempID = Shader.PropertyToID("_cTemp");
            commandBuffer[k].GetTemporaryRT(cTempID, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            commandBuffer[k].SetGlobalVector("_BlurDirection", new Vector2(proportionalBlurSize, 0));
            commandBuffer[k].Blit(bTempID, cTempID, outlineMaterial[k], 2);
            commandBuffer[k].SetGlobalVector("_BlurDirection", new Vector2(0, proportionalBlurSize));
            commandBuffer[k].Blit(cTempID, bTempID, outlineMaterial[k], 2);

            // final overlay
            commandBuffer[k].SetGlobalColor("_OutlineColor", outlineColor[k]);
            commandBuffer[k].SetGlobalFloat("_OutlineInFrontOpacity", occludedOutlineOpacity);
            commandBuffer[k].Blit(bTempID, BuiltinRenderTextureType.CameraTarget, outlineMaterial[k], 4);

            // release tempRTs
            commandBuffer[k].ReleaseTemporaryRT(aTempID);
            commandBuffer[k].ReleaseTemporaryRT(bTempID);
            commandBuffer[k].ReleaseTemporaryRT(cTempID);
        }
    }
}
