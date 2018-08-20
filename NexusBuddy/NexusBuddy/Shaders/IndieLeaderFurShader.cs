using Firaxis.Framework.Granny;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace NexusBuddy
{
	internal class IndieLeaderFurShader : IndieMaterial
	{
		[Category("Leaderhead Fur Materials"), DisplayName("Fur_BaseMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string Fur_BaseMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderFurTextures").GetParameterValue("Fur_BaseMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderFurTextures").SetParameterValue("Fur_BaseMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Leaderhead Fur Materials"), DisplayName("Fur_SREF"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string Fur_SREF
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderFurTextures").GetParameterValue("Fur_SREF") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderFurTextures").SetParameterValue("Fur_SREF", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Leaderhead Fur Materials"), DisplayName("Fur_Transparency"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string Fur_Transparency
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderFurTextures").GetParameterValue("Fur_Transparency") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderFurTextures").SetParameterValue("Fur_Transparency", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Leaderhead Fur Materials"), DisplayName("Fur_IrradianceMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string Fur_IrradianceMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderFurTextures").GetParameterValue("Fur_IrradianceMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderFurTextures").SetParameterValue("Fur_IrradianceMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		public IndieLeaderFurShader(IGrannyMaterial material) : base(material)
		{
		}
		public void CopyTextures(string outputFolder)
		{
			base.CopyTextureIfExists(this.Fur_BaseMap, outputFolder);
			base.CopyTextureIfExists(this.Fur_IrradianceMap, outputFolder);
			base.CopyTextureIfExists(this.Fur_SREF, outputFolder);
			base.CopyTextureIfExists(this.Fur_Transparency, outputFolder);
		}
	}
}
