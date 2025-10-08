using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using Runtime.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;


namespace MedicalTraining.Skin
{
    public class Skin : MonoBehaviour
    {
        public enum TextureRegion
        {
            UNKNOWN,
            FULL,
            HEAD,
            BODY,
            ARMS,
            LEGS,
            GENITALS,
            FINGERNAILS
        }

        private Dictionary<string, TextureRegion> m_RegionMapping = new()
        {
            // CC3 characters
            { "Std_Skin_Head", TextureRegion.HEAD },
            { "Std_Skin_Body", TextureRegion.BODY },
            { "Std_Skin_Arm", TextureRegion.ARMS },
            { "Std_Skin_Leg", TextureRegion.LEGS },
            { "Std_Nails", TextureRegion.FINGERNAILS },
            // MakeHuman characters
            { "base.obj", TextureRegion.FULL }
        };

        private class MaterialProvider
        {
            private Dictionary<TextureRegion, Material> m_materials_dict;

            public MaterialProvider()
            {
                this.m_materials_dict = new Dictionary<TextureRegion, Material>();
            }

            public void AddMaterial(TextureRegion region, Material material)
            {
                this.m_materials_dict.Add(region, material);
            }

            public Material GetMaterial(TextureRegion region)
            {
                return this.m_materials_dict[region];
            }

            public Material GetMaterialByIndex(int index)
            {
                List<TextureRegion> orderedRegions = new List<TextureRegion>()
                { TextureRegion.HEAD, TextureRegion.BODY, TextureRegion.ARMS, TextureRegion.LEGS, TextureRegion.GENITALS, TextureRegion.FINGERNAILS };

                if (index < 0 || index >= orderedRegions.Count)
                {
                    return null;
                }
                else
                {
                    return this.m_materials_dict[orderedRegions[index]];
                }
            }
        }

        public class TextureProvider
        {
            private Dictionary<TextureRegion, Texture2D> m_textures;

            public TextureProvider()
            {
                this.m_textures = new Dictionary<TextureRegion, Texture2D>();
            }

            public void AddTexture(TextureRegion region, Texture2D texture)
            {
                this.m_textures.Add(region, texture);
            }

            public Texture2D GetTexture(TextureRegion region)
            {
                return this.m_textures[region];
            }

            public Texture2D GetTextureNotNull(TextureRegion region)
            {
                if (this.m_textures.ContainsKey(region))
                {
                    return this.m_textures[region];
                }
                else if (region == TextureRegion.GENITALS && this.m_textures.ContainsKey(TextureRegion.BODY))
                {
                    return this.m_textures[TextureRegion.BODY];
                }
                else if (this.m_textures.ContainsKey(TextureRegion.FULL))
                {
                    return this.m_textures[TextureRegion.FULL];
                }
                else if (this.m_textures.ContainsKey(TextureRegion.UNKNOWN))
                {
                    return this.m_textures[TextureRegion.UNKNOWN];
                }
                return null;
            }

            public void SetTexture(TextureRegion region, Texture2D texture)
            {
                this.m_textures[region] = texture;
            }
        }

        public enum CharacterType
        {
            CC3,
            MAKEHUMAN
        }

        private CharacterType m_characterType;
        private MaterialProvider m_materials = new();

        public TextureProvider OriginalSkinTextures = new();

        public void SetCharacterType(CharacterType characterType)
        {
            this.m_characterType = characterType;
        }

        public void ReadMaterialsAndTextures()
        {
            Material[] materials = this.GetComponent<Renderer>().materials;
            foreach (Material m in materials)
            {
                TextureRegion region = m_RegionMapping.TryGetValue(m.name, out region) ? region : TextureRegion.FULL;
                this.m_materials.AddMaterial(region, m);
                if (m_characterType == CharacterType.CC3)
                {
                    this.OriginalSkinTextures.AddTexture(region, m.GetTexture("_DiffuseMap") as Texture2D);
                }
                else if (m_characterType == CharacterType.MAKEHUMAN)
                {
                    this.OriginalSkinTextures.AddTexture(region, m.GetTexture("_BaseColorMap") as Texture2D);
                }
            }
        }

        public void ApplyTextures(PathologyData pathologyData)
        {
            // apply non-dermatoscopic diffuse textures
            for (int i = 0; i < pathologyData.Modified.Length; i++)
            {
                Material material;
                switch (m_characterType)
                {
                    case CharacterType.CC3:
                        material = this.m_materials.GetMaterialByIndex(i);
                        material.SetTexture("_DiffuseMap", ImageUtils.Texture2DFromRenderTexture(pathologyData.Modified[i]));
                        break;
                    case CharacterType.MAKEHUMAN:
                        material = this.m_materials.GetMaterial(TextureRegion.FULL);
                        material.SetTexture("_BaseColorMap", ImageUtils.Texture2DFromRenderTexture(pathologyData.Modified[i]));
                        break;
                    default:
                        Debug.LogError("Unknown character type");
                        break;
                }
            }

            // apply non-dermatoscopic height maps
            for (int i = 0; i < pathologyData.ModifiedHeight.Length; i++)
            {
                Material material;
                switch (m_characterType)
                {
                    case CharacterType.CC3:
                        material = this.m_materials.GetMaterialByIndex(i);
                        material.SetTexture("_ParallaxMapping_Heightmap", ImageUtils.Texture2DFromRenderTexture(pathologyData.ModifiedHeight[i]));
                        break;
                    case CharacterType.MAKEHUMAN:
                        material = this.m_materials.GetMaterial(TextureRegion.FULL);
                        material.SetTexture("_HeightMap", ImageUtils.Texture2DFromRenderTexture(pathologyData.ModifiedHeight[i]));
                        break;
                    default:
                        Debug.LogError("Unknown character type");
                        break;
                }
            }
        }
    }
}
