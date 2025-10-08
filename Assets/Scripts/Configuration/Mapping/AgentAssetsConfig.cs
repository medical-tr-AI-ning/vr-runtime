using System.Collections.Generic;
// AssetsConfig myDeserializedClass = JsonConvert.DeserializeObject<AssetsConfig>(myJsonResponse);

namespace MedicalTraining.Configuration.Mapping
{
    public class Atlas
    {
        public string type { get; set; }
        public string quality { get; set; }
        public string texture { get; set; }
        public string textureMeta { get; set; }
    }

    public class BaseTextures
    {
        public string path { get; set; }
        public List<FourSkinTexture> FourSkinTextures { get; set; }
        public Skinmasks Skinmasks { get; set; }
        public Patches Patches { get; set; }
    }

    public class CaseTextures
    {
        public string path { get; set; }
        public List<FourSkinTexture> FourSkinTextures { get; set; }
        public List<Atlas> Atlantes { get; set; }
    }

    public class FourSkinTexture
    {
        public string type { get; set; }
        public string quality { get; set; }
        public string head { get; set; }
        public string body { get; set; }
        public string arms { get; set; }
        public string legs { get; set; }
        public string genitals { get; set; }
        public string fingernails { get; set; }
    }

    public class Skinmasks
    {
        public string type { get; set; }
        public string quality { get; set; }
        public string together { get; set; }
        public string genitals { get; set; }
    }

    public class Patches
    {
        public string type { get; set; }
        public string quality { get; set; }
        public string whiteColor { get; set; }
        public string meshedSkinColor { get; set; }
        public string meshedSkinNormal { get; set; }
        public string meshedSkinHair { get; set; }
        public string ridgedSkinColor { get; set; }
        public string ridgedSkinNormal { get; set; }
        public string mucosaDiffuse { get; set; }
    }

    public class AgentAssetsConfig
    {
        public string id { get; set; }
        public Textures textures { get; set; }
    }

    public class Textures
    {
        public string path { get; set; }
        public BaseTextures BaseTextures { get; set; }
        public CaseTextures CaseTextures { get; set; }
    }

}
