using Firaxis.Framework.Granny;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace NexusBuddy
{
	internal class IndieUnitSkinnedShader : IndieMaterial
	{
		[Category("Skinned Unit Materials"), DisplayName("BaseTextureMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public override string BaseTextureMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("UnitShaderTextures").GetParameterValue("BaseTextureMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("UnitShaderTextures").SetParameterValue("BaseTextureMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Skinned Unit Materials"), DisplayName("SREFMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string SREFMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("UnitShaderTextures").GetParameterValue("SREFMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("UnitShaderTextures").SetParameterValue("SREFMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Skinned Unit Materials"), DisplayName("IrradianceCubeMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string IrradianceCubeMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("UnitShaderTextures").GetParameterValue("IrradianceCubeMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("UnitShaderTextures").SetParameterValue("IrradianceCubeMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Skinned Unit Materials"), DisplayName("DullEnvironmentCubeMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string DullEnvironmentCubeMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("UnitShaderTextures").GetParameterValue("DullEnvironmentCubeMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("UnitShaderTextures").SetParameterValue("DullEnvironmentCubeMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Skinned Unit Materials"), DisplayName("EnvironmentCubeMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string EnvironmentCubeMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("UnitShaderTextures").GetParameterValue("EnvironmentCubeMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("UnitShaderTextures").SetParameterValue("EnvironmentCubeMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Skinned Unit Materials"), DisplayName("DistanceFogRamp"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string DistanceFogRamp
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("UnitShaderTextures").GetParameterValue("DistanceFogRamp") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("UnitShaderTextures").SetParameterValue("DistanceFogRamp", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		public IndieUnitSkinnedShader(IGrannyMaterial material) : base(material)
		{
            int z = 1;
		}
		public void CopyTextures(string outputFolder)
		{
			base.CopyTextureIfExists(this.BaseTextureMap, outputFolder);
			base.CopyTextureIfExists(this.SREFMap, outputFolder);
			base.CopyTextureIfExists(this.IrradianceCubeMap, outputFolder);
			base.CopyTextureIfExists(this.DullEnvironmentCubeMap, outputFolder);
			base.CopyTextureIfExists(this.EnvironmentCubeMap, outputFolder);
			base.CopyTextureIfExists(this.DistanceFogRamp, outputFolder);
		}
	}
}
