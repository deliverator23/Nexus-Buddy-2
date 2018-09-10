using Firaxis.Framework.Granny;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace NexusBuddy
{
	internal class IndieFXShader : IndieMaterial
	{
		[Category("FX Materials"), DisplayName("BaseTextureMap0"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public override string BaseTextureMap
        {
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("FXShaderTextures").GetParameterValue("BaseTextureMap0") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("FXShaderTextures").SetParameterValue("BaseTextureMap0", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("FX Materials"), DisplayName("UVScrollingMap0"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string UVScrollingMap0
        {
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("FXShaderTextures").GetParameterValue("UVScrollingMap0") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("FXShaderTextures").SetParameterValue("UVScrollingMap0", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		[Category("FX Materials"), DisplayName("AlphaLookupMap0"), Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
		public string AlphaLookupMap0
        {
			get
			{
                return ShaderUtils.trimPathFromFilename(base.GetMaterial().FindParameterSet("FXShaderTextures").GetParameterValue("AlphaLookupMap0") as string);
			}
			set
			{
				base.GetMaterial().FindParameterSet("FXShaderTextures").SetParameterValue("AlphaLookupMap0", value.Substring(value.LastIndexOf("\\") + 1));
			}
		}
		
		public IndieFXShader(IGrannyMaterial material) : base(material)
		{
		}
		public void CopyTextures(string outputFolder)
		{
			base.CopyTextureIfExists(this.BaseTextureMap, outputFolder);
			base.CopyTextureIfExists(this.UVScrollingMap0, outputFolder);
			base.CopyTextureIfExists(this.AlphaLookupMap0, outputFolder);
		}
	}
}
