using Firaxis.Framework.Granny;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace NexusBuddy
{
	internal class IndieBuildingShader : IndieMaterial
	{
		[Category("Building Materials"), DisplayName(" Diffuse"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string Diffuse
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("BuildingTextures").GetParameterValue("Diffuse") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("BuildingTextures").SetParameterValue("Diffuse", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Building Materials"), DisplayName("Irradiance"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string Irradiance
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("BuildingTextures").GetParameterValue("Irradiance") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("BuildingTextures").SetParameterValue("Irradiance", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Building Materials"), DisplayName("BuildingSREF"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string BuildingSREF
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("BuildingTextures").GetParameterValue("BuildingSREF") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("BuildingTextures").SetParameterValue("BuildingSREF", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Building Materials"), DisplayName("SpecSharp"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string SpecSharp
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("BuildingTextures").GetParameterValue("SpecSharp") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("BuildingTextures").SetParameterValue("SpecSharp", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Building Materials"), DisplayName("SpecSoft"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string SpecSoft
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("BuildingTextures").GetParameterValue("SpecSoft") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("BuildingTextures").SetParameterValue("SpecSoft", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Building Materials"), DisplayName("DistanceFogRamp"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string DistanceFogRamp
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("BuildingTextures").GetParameterValue("DistanceFogRamp") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("BuildingTextures").SetParameterValue("DistanceFogRamp", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Building Materials"), DisplayName("ColorCorrection"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string ColorCorrection
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("BuildingTextures").GetParameterValue("ColorCorrection") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("BuildingTextures").SetParameterValue("ColorCorrection", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Building Materials"), DisplayName("FOWColorKey"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string FOWColorKey
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("BuildingTextures").GetParameterValue("FOWColorKey") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("BuildingTextures").SetParameterValue("FOWColorKey", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Building Materials"), DisplayName("FOWMask"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string FOWMask
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("BuildingTextures").GetParameterValue("FOWMask") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("BuildingTextures").SetParameterValue("FOWMask", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Building Materials"), DisplayName("FOWBaseLayer"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string FOWBaseLayer
		{
			get
			{
				return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("BuildingTextures").GetParameterValue("FOWBaseLayer") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("BuildingTextures").SetParameterValue("FOWBaseLayer", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		public IndieBuildingShader(IGrannyMaterial material) : base(material)
		{
		}
		public void CopyTextures(string outputFolder)
		{
			base.CopyTextureIfExists(this.Diffuse, outputFolder);
			base.CopyTextureIfExists(this.Irradiance, outputFolder);
			base.CopyTextureIfExists(this.BuildingSREF, outputFolder);
			base.CopyTextureIfExists(this.SpecSharp, outputFolder);
			base.CopyTextureIfExists(this.SpecSoft, outputFolder);
			base.CopyTextureIfExists(this.DistanceFogRamp, outputFolder);
			base.CopyTextureIfExists(this.ColorCorrection, outputFolder);
			base.CopyTextureIfExists(this.FOWColorKey, outputFolder);
			base.CopyTextureIfExists(this.FOWMask, outputFolder);
			base.CopyTextureIfExists(this.FOWBaseLayer, outputFolder);
		}
	}
}
