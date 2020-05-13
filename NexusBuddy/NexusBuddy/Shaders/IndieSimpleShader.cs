using Firaxis.Framework.Granny;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace NexusBuddy
{
	internal class IndieSimpleShader : IndieMaterial
	{

		[Category("Simple Materials"), DisplayName("DiffuseMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public override string DiffuseMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(ShaderUtils.getStringValuebyParamSetAndName(base.GetMaterial(), "Civ5LeaderheadTextures", "DiffuseMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderheadTextures").SetParameterValue("DiffuseMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Simple Materials"), DisplayName("NormalMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string NormalMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(ShaderUtils.getStringValuebyParamSetAndName(base.GetMaterial(), "Civ5LeaderheadTextures", "NormalMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderheadTextures").SetParameterValue("NormalMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Simple Materials"), DisplayName("SREFMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public override string SREFMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(ShaderUtils.getStringValuebyParamSetAndName(base.GetMaterial(), "Civ5LeaderheadTextures", "SREFMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LeaderheadTextures").SetParameterValue("SREFMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Simple Materials"), DisplayName("IrradianceMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string IrradianceMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(ShaderUtils.getStringValuebyParamSetAndName(base.GetMaterial(), "Civ5LightingTextures", "IrradianceMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LightingTextures").SetParameterValue("IrradianceMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Simple Materials"), DisplayName("EnvironmentMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string EnvironmentMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(ShaderUtils.getStringValuebyParamSetAndName(base.GetMaterial(), "Civ5LightingTextures", "EnvironmentMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LightingTextures").SetParameterValue("EnvironmentMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Simple Materials"), DisplayName("SPECMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string SPECMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(ShaderUtils.getStringValuebyParamSetAndName(base.GetMaterial(), "Civ5LightingTextures", "SpecMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("Civ5LightingTextures").SetParameterValue("SpecMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Simple Materials"), DisplayName("BaseSampler"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string BaseSampler
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(ShaderUtils.getStringValuebyParamSetAndName(base.GetMaterial(), "SimpleTextures", "BaseSampler") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("SimpleTextures").SetParameterValue("BaseSampler", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Simple Materials"), DisplayName("EnvironmentMaskSampler"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string EnvironmentMaskSampler
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(ShaderUtils.getStringValuebyParamSetAndName(base.GetMaterial(), "SimpleTextures", "EnvironmentMaskSampler") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("SimpleTextures").SetParameterValue("EnvironmentMaskSampler", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Simple Materials"), DisplayName("LightCubeMapSampler"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string LightCubeMapSampler
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(ShaderUtils.getStringValuebyParamSetAndName(base.GetMaterial(), "SimpleTextures", "LightCubeMapSampler") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("SimpleTextures").SetParameterValue("LightCubeMapSampler", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Simple Materials"), DisplayName("EnvironmentMapSampler"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string EnvironmentMapSampler
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(ShaderUtils.getStringValuebyParamSetAndName(base.GetMaterial(), "SimpleTextures", "EnvironmentMapSampler") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("SimpleTextures").SetParameterValue("EnvironmentMapSampler", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
        [Category("Simple Materials"), DisplayName("TeamColorSampler"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
        public string TeamColorSampler
        {
            get
            {
                return ShaderUtils.trimPathFromFilename(ShaderUtils.getStringValuebyParamSetAndName(base.GetMaterial(), "SimpleTextures", "TeamColorSampler") as string);
            }
            set
            {
                base.GetMaterial().FindParameterSet("SimpleTextures").SetParameterValue("TeamColorSampler", value.Substring(value.LastIndexOf("\\") + 1));
            }
        }
		
		public IndieSimpleShader(IGrannyMaterial material) : base(material)
		{
		}
		public void CopyTextures(string outputFolder)
		{
			base.CopyTextureIfExists(this.DiffuseMap, outputFolder);
			base.CopyTextureIfExists(this.NormalMap, outputFolder);
			base.CopyTextureIfExists(this.SREFMap, outputFolder);
			base.CopyTextureIfExists(this.IrradianceMap, outputFolder);
			base.CopyTextureIfExists(this.EnvironmentMap, outputFolder);
			base.CopyTextureIfExists(this.SPECMap, outputFolder);
            base.CopyTextureIfExists(this.BaseSampler, outputFolder);
            base.CopyTextureIfExists(this.EnvironmentMapSampler, outputFolder);
            base.CopyTextureIfExists(this.EnvironmentMaskSampler, outputFolder);
            base.CopyTextureIfExists(this.LightCubeMapSampler, outputFolder);
            base.CopyTextureIfExists(this.TeamColorSampler, outputFolder);
		}
	}
}
