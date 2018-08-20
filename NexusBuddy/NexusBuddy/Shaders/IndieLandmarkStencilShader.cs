using Firaxis.Framework.Granny;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace NexusBuddy
{
	internal class IndieLandmarkStencilShader : IndieMaterial
	{
		[Category("Landmark Stencil Materials"), DisplayName("BaseTextureMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string BaseTextureMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("LandmarkShaderTextures").GetParameterValue("BaseTextureMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("LandmarkShaderTextures").SetParameterValue("BaseTextureMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Landmark Stencil Materials"), DisplayName("HeightTextureMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string HeightTextureMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("LandmarkShaderTextures").GetParameterValue("HeightTextureMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("LandmarkShaderTextures").SetParameterValue("HeightTextureMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("Landmark Stencil Materials"), DisplayName("SpecTextureMap"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string SpecTextureMap
		{
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("LandmarkShaderTextures").GetParameterValue("SpecTextureMap") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("LandmarkShaderTextures").SetParameterValue("SpecTextureMap", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		public IndieLandmarkStencilShader(IGrannyMaterial material) : base(material)
		{
		}
		public void CopyTextures(string outputFolder)
		{
			base.CopyTextureIfExists(this.BaseTextureMap, outputFolder);
			base.CopyTextureIfExists(this.HeightTextureMap, outputFolder);
			base.CopyTextureIfExists(this.SpecTextureMap, outputFolder);
		}
	}
}
