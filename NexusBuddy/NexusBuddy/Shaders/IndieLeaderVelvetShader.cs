using Firaxis.Framework.Granny;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace NexusBuddy
{
	internal class IndieLeaderVelvetShader : IndieMaterial
	{
		[Category("Leaderhead Materials"), DisplayName("VelvetBaseMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string BaseMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderVelvetTextures").GetParameterValue("VelvetBaseMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderVelvetTextures").SetParameterValue("VelvetBaseMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Leaderhead Materials"), DisplayName("VelvetNormalMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string NormalMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderVelvetTextures").GetParameterValue("VelvetNormalMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderVelvetTextures").SetParameterValue("VelvetNormalMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Leaderhead Materials"), DisplayName("VelvetSheenMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string SheenMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderVelvetTextures").GetParameterValue("VelvetSheenMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderVelvetTextures").SetParameterValue("VelvetSheenMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Leaderhead Materials"), DisplayName("VelvetIrradianceMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string IrradianceMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("Civ5LeaderVelvetTextures").GetParameterValue("VelvetIrradianceMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderVelvetTextures").SetParameterValue("VelvetIrradianceMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		
		public IndieLeaderVelvetShader(IGrannyMaterial material) : base(material)
		{
		
		}
		public void CopyTextures(string outputFolder)
		{
			base.CopyTextureIfExists(this.BaseMap, outputFolder);
			base.CopyTextureIfExists(this.NormalMap, outputFolder);
			base.CopyTextureIfExists(this.SheenMap, outputFolder);
			base.CopyTextureIfExists(this.IrradianceMap, outputFolder);
		}
	}
}
