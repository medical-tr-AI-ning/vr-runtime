using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using MedicalTraining.Dialogue;
using MedicalTraining.Dialogue.ActionHandlers;
using MedicalTraining.Utils;
using Runtime.Utils;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;


namespace MedicalTraining.Configuration.Scenarios
{
    class Scenario_SkinCancerScreening_Configurator : ScenarioConfigurator
    {
        [Header("Skin Cancer Screening Specific")]
        [SerializeField] private GameObject m_screen;
        [SerializeField] private PhotoCountDisplay m_photoCountDisplay;
        [SerializeField] private DialogueUISkinCancerScreeningActionHandler m_dialogueUISkinCancerScreeningActionHandler;
        [SerializeField] private int m_numLesions = 512;
        [SerializeField] private int m_lesionResolution = 454;

        public override void ConfigureScenarioSpecific()
        {
            // Apply skin textures to the agent
            Transform agentTransform = this.m_agent.transform;

            // Find the skin script in the agent
            Skin.Skin skin = agentTransform.GetComponentInChildren<Skin.Skin>();
            if (skin == null)
            {
                Debug.LogError("Skin component not found in agent! Place it on the main body component of the agent with the skin textures.");
                // For CC3 characters: Model/Body/CC_Base_Body
                // For MakeHuman characters: Model/base.objMesh
                return;
            }

            // Set character type
            // TODO: Better way to determine character model type
            if (this.gameObject.name.Contains("CC"))
            {
                skin.SetCharacterType(Skin.Skin.CharacterType.CC3);
            }
            else
            {
                skin.SetCharacterType(Skin.Skin.CharacterType.MAKEHUMAN);
            }

            // Apply skin type and textures to virtual agent
            skin.ReadMaterialsAndTextures();
            //skin.SkinTextures = this.m_config.AgentAssets;
            skin.ApplyTextures(this.m_config.Pathology.Pathology);

            // Set up post processing for dermatoscope
            // Find dermatoscope
            GameObject dermatoscope = this.ObjectsRoot.transform.Find("DigitalDermatoscopeRuntime").gameObject;

            // Add screen to take photo component
            TakePhoto takePhoto = dermatoscope.GetComponentInChildren<TakePhoto>();
            takePhoto.Initialize();
            takePhoto.SetScreen(this.m_screen);

            // Add TakePhoto component to dialogue action handler
            this.m_dialogueUISkinCancerScreeningActionHandler.SetDermatoscope(takePhoto);

            // Setup photo count display
            this.m_photoCountDisplay.SetupPhotoCount(takePhoto);

            // Find all relevant post processing objects within the dermatoscope
            GameObject[] DermatoscopePostProcessingObjects = dermatoscope.GetComponentsInChildren<CustomPassVolume>().Select(cp => cp.gameObject).ToArray();

            // Texture2DArray for lesion textures
            PathologyData pathologyData = this.m_config.Pathology.Pathology;
            LesionArrayMeta lesionArray = LoadLesionArray(pathologyData);

            // Apply generated assets to post processing objects
            foreach (GameObject postPro in DermatoscopePostProcessingObjects)
            {
                Debug.Log("Found post processing object: " + postPro.name);
                // Apply generated assets
                DrawRenderersCustomPass drawRenderersCustomPass = (DrawRenderersCustomPass)postPro.GetComponent<CustomPassVolume>().customPasses.ElementAt<CustomPass>(0);
                Material material = drawRenderersCustomPass.overrideMaterial;
                material.SetTexture("_Lesion_Positions", lesionArray.Meta);
                material.SetTexture("_Lesion_Array", lesionArray.Atlas);
                //material.SetTexture("_WhiteTexture", this.m_config.AgentAssets.WhiteTexture);
                material.SetTexture("_Texture_Head", skin.OriginalSkinTextures.GetTextureNotNull(Skin.Skin.TextureRegion.HEAD));
                material.SetTexture("_Texture_Body", skin.OriginalSkinTextures.GetTextureNotNull(Skin.Skin.TextureRegion.BODY));
                material.SetTexture("_Texture_Arms", skin.OriginalSkinTextures.GetTextureNotNull(Skin.Skin.TextureRegion.ARMS));
                material.SetTexture("_Texture_Legs", skin.OriginalSkinTextures.GetTextureNotNull(Skin.Skin.TextureRegion.LEGS));
                material.SetTexture("_Texture_Genitals", skin.OriginalSkinTextures.GetTextureNotNull(Skin.Skin.TextureRegion.GENITALS));
                material.SetTexture("_Texture_4", skin.OriginalSkinTextures.GetTextureNotNull(Skin.Skin.TextureRegion.FINGERNAILS));
                // TODO: Set import settings for these textures and load them from StreamingAssets
                //material.SetTexture("_SkinMask_All", this.m_config.AgentAssets.LowResolutionSkinmasks[SkinTextureCollection.GroupedSkinTextureRegion.TOGETHER]);
                //material.SetTexture("_SkinMask_Genitals", this.m_config.AgentAssets.LowResolutionSkinmasks[SkinTextureCollection.GroupedSkinTextureRegion.GENITALS]);
                //material.SetTexture("_Texture_2", this.m_config.AgentAssets.MeshedSkinPatches[SkinTextureCollection.SkinPatchTextureType.COLOR]);
                //material.SetTexture("_Normalmap", this.m_config.AgentAssets.MeshedSkinPatches[SkinTextureCollection.SkinPatchTextureType.NORMAL]);
                //material.SetTexture("_Texture_3", this.m_config.AgentAssets.MeshedSkinPatches[SkinTextureCollection.SkinPatchTextureType.HAIR]);
                //material.SetTexture("_Texture", this.m_config.AgentAssets.RidgedSkinPatches[SkinTextureCollection.SkinPatchTextureType.COLOR]);
                //material.SetTexture("_Normalmap_1", this.m_config.AgentAssets.RidgedSkinPatches[SkinTextureCollection.SkinPatchTextureType.NORMAL]);
                //material.SetTexture("_Texture_1", this.m_config.AgentAssets.MucosaSkinPatches[SkinTextureCollection.SkinPatchTextureType.DIFFUSE]);
                material.SetTexture("_Diffuse_Alpha_Height_Normal_Alpha_Head", ImageUtils.Texture2DFromRenderTexture(pathologyData.ModifiedHeight[0]));
                material.SetTexture("_Diffuse_Alpha_Height_Normal_Alpha_Body", ImageUtils.Texture2DFromRenderTexture(pathologyData.ModifiedHeight[0]));
                material.SetTexture("_Diffuse_Alpha_Height_Normal_Alpha_Arms", ImageUtils.Texture2DFromRenderTexture(pathologyData.ModifiedHeight[0]));
                material.SetTexture("_Diffuse_Alpha_Height_Normal_Alpha_Legs", ImageUtils.Texture2DFromRenderTexture(pathologyData.ModifiedHeight[0]));
            }
        }

        private struct LesionArrayMeta
        {
            public Texture2DArray Atlas;
            public Texture2D Meta;
        }

        private LesionArrayMeta LoadLesionArray(PathologyData pathologyData)
        {
            Texture2DArray atlas = new Texture2DArray(m_lesionResolution, m_lesionResolution, m_numLesions, TextureFormat.ARGB32, true);
            Texture2D meta = new Texture2D(4, m_numLesions, TextureFormat.RGB24, false);
            meta.filterMode = FilterMode.Point;
            meta.wrapMode = TextureWrapMode.Clamp;

            //SerializablePathologyVariant pathology = this.m_config.Scenario.Pathology;

            int idx = 1;

            Debug.Log("Loading lesion textures");

            // Load nevi textures
            if (pathologyData.NaeviEnabled)
            {
                Debug.Log("Loading nevi textures");
                int serializedRegionIdx = 0;
                foreach (var nevi in pathologyData.Naevis)
                {
                    Debug.Log("Loading nevi body region: " + serializedRegionIdx);
                    foreach (var highResNaevi in nevi.HighRes)
                    {
                        //Debug.Log("Loading nevi texture: " + idx);
                        string path = PathTools.CombinePaths(this.m_config.PathologyPath, highResNaevi.Texture);
                        Texture2D texture = ImageUtils.DeserializeImageToTexture2D(path);
                        // Fill texture with color for debugging
                        //Texture2D texture = this.GetDebugLesionTexture(m_lesionResolution, idx, bodyRegionIdx);
                        texture.Apply();
                        Graphics.CopyTexture(texture, 0, 0, atlas, idx - 1, 0);

                        // Set meta data
                        this.FillMetaTextureEntry(meta, idx, serializedRegionIdx, highResNaevi.Position, bodyTextureSize: 4096);

                        idx++;
                        if (idx > m_numLesions)
                            break;
                    }
                    serializedRegionIdx++;
                    if (idx > m_numLesions)
                        break;
                }
            }

            // Load melanoma textures
            if (pathologyData.MelanomaEnabled)
            {
                Debug.Log("Loading melanoma textures");
                foreach (var mel in pathologyData.Melanoma)
                {
                    Debug.Log("Loading melanoma: " + idx);
                    Texture2D texture = ImageUtils.Texture2DFromRenderTexture(mel.Shape);
                    texture.Apply();
                    Graphics.CopyTexture(texture, 0, 0, atlas, idx - 1, 0);

                    // Set meta data
                    // Identify pixel position from serialized data
                    // Convert bottom left origin to top left origin
                    Vector2 position = new Vector2(0, 0);  
                    position.x = (mel.TextureCoord.x % 1) * 4096;
                    position.y = ((1 - mel.TextureCoord.y) % 1) * 4096;
                    //Debug.Log("Position: " + position);
                    //Debug.Log("TextureCoord: " + mel.TextureCoord);
                    int serializedRegionIdx = (int)(mel.TextureCoord.x);
                    //Debug.Log("SerializedRegionIdx: " + serializedRegionIdx);
                    // Adjust position according to body region because of changed origin
                    // TODO: REQUIRES WAY MORE ROBUST SOLUTION!
                    if (serializedRegionIdx == 1)
                    {
                        // Size of melanoma patches on the body region (1) is 96x96 pixels
                        position.y -= 96;
                    }
                    else
                    {
                        // Size of melanoma patches on all remaining regions is 64x64 pixels
                        position.y -= 64;
                    }
                    this.FillMetaTextureEntry(meta, idx, serializedRegionIdx, position, bodyTextureSize: 4096);

                    idx++;
                    if (idx > m_numLesions)
                        break;
                }
            }

            // Fill remaining meta entries with black
            for (int i = idx; i <= m_numLesions; i++)
            {
                meta.SetPixel(0, m_numLesions - i, new Color(0, 0, 0));
                meta.SetPixel(1, m_numLesions - i, new Color(0, 0, 0));
                meta.SetPixel(2, m_numLesions - i, new Color(0, 0, 0));
                meta.SetPixel(3, m_numLesions - i, new Color(0, 0, 0));
            }

            atlas.Apply();
            meta.Apply();

            /*
            // Save meta texture to disk for debugging
            byte[] metaBytes = ImageUtils.SerializeImage(meta);
            string path2 = PathTools.CombinePaths(Application.streamingAssetsPath, "lesionmeta_tmp.png");
            File.WriteAllBytes(path2, metaBytes);

            // Save first atlas texture to disk for debugging
            Texture2D texture2D = new Texture2D(m_lesionResolution, m_lesionResolution, TextureFormat.ARGB32, false);
            texture2D.SetPixels(atlas.GetPixels(0));
            texture2D.Apply();
            byte[] atlasBytes = ImageUtils.SerializeImage(texture2D);
            string path3 = PathTools.CombinePaths(Application.streamingAssetsPath, "lesionatlas_tmp.png");
            File.WriteAllBytes(path3, atlasBytes);
            */

            LesionArrayMeta lesionArrayMeta = new LesionArrayMeta
            {
                Atlas = atlas,
                Meta = meta
            };
            return lesionArrayMeta;
        }

        private Color FloatToColor(float value)
        {
            Debug.Assert(value >= 0 && value <= 2, "Value must be between 0 and 2");
            float r = (float)(int)(value * 100) / 255;
            float g = (float)(int)((value * 100 - (int)(value * 100)) * 100) / 255;
            float b = (float)(int)((value * 10000 - (int)(value * 10000)) * 100) / 255;
            //Debug.Log("Value: " + value + " R: " + r + " G: " + g + " B: " + b);
            return new Color(r, g, b).gamma;
        }

        private int GetBodyRegionIndex(int serializedRegionIdx)
        {
            // TODO: Align mapping in skin shader
            // 2 = head, 1 = body, 0 = arms, 3 = legs
            int bodyRegionIdx = 0;  
            if (serializedRegionIdx == 0)
                bodyRegionIdx = 2;
            else if (serializedRegionIdx == 1)
                bodyRegionIdx = 1;
            else if (serializedRegionIdx == 2)
                bodyRegionIdx = 0;
            else if (serializedRegionIdx == 3)
                bodyRegionIdx = 3;
            return bodyRegionIdx;
        }

        private void FillMetaTextureEntry(Texture2D meta, int idx, int serializedRegionIdx, Vector2 position, int bodyTextureSize)
        {
            int bodyRegionIdx = GetBodyRegionIndex(serializedRegionIdx);                      // Required for mapping in skin shader
            // Set meta data
            meta.SetPixel(0, m_numLesions - idx, new Color(1, 1, 1));                         // Flag to indicate filled entry
            meta.SetPixel(1, m_numLesions - idx, FloatToColor((float)(bodyRegionIdx) / 4));   // Index divided by the number of body regions (4)
            float x = position.x / bodyTextureSize;                                           // Normalize with resolution of the original texture
            if (bodyRegionIdx == 1 || bodyRegionIdx == 3)                                     // Encode texture shift to different body regions
                x = x + 1;
            meta.SetPixel(2, m_numLesions - idx, FloatToColor(x));
            float y = position.y / bodyTextureSize;
            if (bodyRegionIdx == 0 || bodyRegionIdx == 3)                                     // Encode texture shift to different body regions
                y = y + 1;
            meta.SetPixel(3, m_numLesions - idx, FloatToColor(y));
        }

        private Texture2D GetDebugLesionTexture(int size, int idx, int bodyRegionIdx)
        {
            // Fill texture with color for debugging
            Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, true);
            Color[] colors = new Color[size * size];
            Color cl = Color.red;
            if (bodyRegionIdx == 1)
                cl = Color.green;
            else if (bodyRegionIdx == 2)
                cl = Color.blue;
            else if (bodyRegionIdx == 3)
                cl = Color.cyan;
            for (int i = 0; i < size * size; i++)
            {
                if (i < (1+idx)*200)
                    colors[i] = cl;
                else
                    colors[i] = Color.black;
            }
            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }
    }
}
