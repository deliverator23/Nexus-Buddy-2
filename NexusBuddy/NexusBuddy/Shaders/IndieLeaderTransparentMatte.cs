using Firaxis.Framework.Granny;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace NexusBuddy
{
	internal class IndieLeaderTransparentMatteShader : IndieMaterial
	{
        [Category("Leaderhead Materials"), DisplayName("Matte"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
        public string Matte
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderMatteTextures").GetParameterValue("Matte") as string);
			}
			set
			{
                base.GetMaterial().FindParameterSet("Civ5LeaderMatteTextures").SetParameterValue("Matte", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}

        [Category("Leaderhead Materials"), DisplayName("Transparency"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
        public string Transparency
        {
            get
            {
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("TransparencyMap").GetParameterValue("Transparency") as string);
            }
            set
            {
                base.GetMaterial().FindParameterSet("TransparencyMap").SetParameterValue("Transparency", value.Substring(value.LastIndexOf("\\") + 1));
            }
        }

        public IndieLeaderTransparentMatteShader(IGrannyMaterial material)
            : base(material)
		{
		}
		public void CopyTextures(string outputFolder)
		{
			base.CopyTextureIfExists(this.Matte, outputFolder);
            base.CopyTextureIfExists(this.Transparency, outputFolder);
		}
	}
}
