using Firaxis.Framework.Granny;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace NexusBuddy
{
	internal class IndieLeaderMaskedHairShader : IndieMaterial
	{
		[Category("Leaderhead Materials"), DisplayName("DiffuseMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public override string DiffuseMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderheadTextures").GetParameterValue("DiffuseMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderheadTextures").SetParameterValue("DiffuseMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Leaderhead Materials"), DisplayName("NormalMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string NormalMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderheadTextures").GetParameterValue("NormalMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderheadTextures").SetParameterValue("NormalMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Leaderhead Materials"), DisplayName("SREFMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public override string SREFMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderheadTextures").GetParameterValue("SREFMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderheadTextures").SetParameterValue("SREFMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Leaderhead Materials"), DisplayName("IrradianceMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string IrradianceMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LightingTextures").GetParameterValue("IrradianceMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LightingTextures").SetParameterValue("IrradianceMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Leaderhead Materials"), DisplayName("EnvironmentMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string EnvironmentMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LightingTextures").GetParameterValue("EnvironmentMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LightingTextures").SetParameterValue("EnvironmentMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
        [Category("Leaderhead Materials"), DisplayName("TangentMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
        public string TangentMap
        {
            get
            {
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderTangentMap").GetParameterValue("TangentMap") as string);
            }
            set
            {
                base.GetMaterial().FindParameterSet("Civ5LeaderTangentMap").SetParameterValue("TangentMap", value.Substring(value.LastIndexOf("\\") + 1));
            }
        }
        [Category("Leaderhead Materials"), DisplayName("MaskMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
        public string MaskMap
        {
            get
            {
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("MaskMap").GetParameterValue("Mask") as string);
            }
            set
            {
                base.GetMaterial().FindParameterSet("MaskMap").SetParameterValue("Mask", value.Substring(value.LastIndexOf("\\") + 1));
            }
        }
		public IndieLeaderMaskedHairShader(IGrannyMaterial material) : base(material)
		{
		}
		public void CopyTextures(string outputFolder)
		{
			base.CopyTextureIfExists(this.DiffuseMap, outputFolder);
			base.CopyTextureIfExists(this.NormalMap, outputFolder);
			base.CopyTextureIfExists(this.SREFMap, outputFolder);
			base.CopyTextureIfExists(this.IrradianceMap, outputFolder);
			base.CopyTextureIfExists(this.EnvironmentMap, outputFolder);
            base.CopyTextureIfExists(this.TangentMap, outputFolder);
            base.CopyTextureIfExists(this.MaskMap, outputFolder);
		}
	}
}
