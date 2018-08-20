using Firaxis.Framework.Granny;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace NexusBuddy
{
	public class IndieMaterial
	{
		private IGrannyMaterial material;
		[Category("Material"), Description("Name of the material")]
		public string Name
		{
			get
			{
				return this.material.Name;
			}
			set
			{
				this.material.Name = value;
			}
		}
		protected IndieMaterial(IGrannyMaterial material)
		{
			this.material = material;
		}
		public virtual void AddToListView(ListView view)
		{
			view.Items.Add(this.material.Name);
			view.Items[view.Items.Count - 1].SubItems.Add(this.material.ShaderSet);
			view.Items[view.Items.Count - 1].Tag = this;
		}
		public IGrannyMaterial GetMaterial()
		{
			return this.material;
		}

        public virtual string SpecTextureMap
        {
            get
            {
                return "SpecTextureMap";
            }
            set
            {
                var z = 0;
            }
        }


        public virtual string BuildingSREF
        {
            get
            {
                return "BuildingSREF";
            }
            set
            {
                var z = 0;
            }
        }


        public virtual string Diffuse
        {
            get
            {
                return "Diffuse";
            }
            set
            {
                var z = 0;
            }
        }

        public virtual string BaseTextureMap
        {
            get
            {
                return "BaseTextureMap";
            }
            set
            {
                var z = 0;
            }
        }

        public virtual string SREFMap
        {
            get
            {
                return "SREFMap";
            }
            set
            {
                var z = 0;
            }
        }

        protected void CopyTextureIfExists(string file, string outputFolder)
		{
			if (string.IsNullOrEmpty(file))
			{
				return;
			}
			file = file.Trim();
			if (string.IsNullOrEmpty(file))
			{
				return;
			}
			string text = NexusBuddyApplicationForm.loadedFile.Filename.Substring(0, NexusBuddyApplicationForm.loadedFile.Filename.LastIndexOf("\\")) + "\\";
			text += file;
			if (File.Exists(text))
			{
				if (File.Exists(outputFolder + "\\" + file))
				{
					File.Delete(outputFolder + "\\" + file);
				}
				File.Copy(text, outputFolder + "\\" + file);
			}
		}
	}
}
