using Firaxis.Framework;
using Firaxis.Framework.Granny;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using NexusBuddy.GrannyWrappers;
using NexusBuddy.GrannyInfos;
using NexusBuddy.Utils;
using NexusBuddy.FileOps;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;

namespace NexusBuddy
{ 

    public class NexusBuddyApplicationForm : Form
	{
		public static NexusBuddyApplicationForm form;
		public static IGrannyFile loadedFile;
        public static int major_version = 2;
        public static int minor_version = 3;
        public static int sub_minor_version = 3;

        private static string loadedStringDatabaseFilename;
        private string templateFilename;
        public string modelTemplateFilename;
        public string leaderTemplateFilename;
        public string sceneTemplateFilename;
        private string[] sourceTemplateFilenames = new string[] {"output_template.gr2"};
        public string sourceTemplatePath = "C:\\Civ5Mod\\IndieStoneDev\\gr2_templates\\";
        private HashSet<string> usedShaderSet;

		public string Civ5Location;
		private Random rand = new Random();
		public List<IGrannyFile> openTempFiles = new List<IGrannyFile>();
        private List<string> TempFiles = new List<string>();
        public List<IndieAnimDef> AnimDefList = new List<IndieAnimDef>();
		private IContainer components;
        private SplitContainer masterSplitContainer;
		private PropertyGrid properties;
		private ColumnHeader animation;
		private ColumnHeader duration;
		private ToolStripMenuItem buildingShaderToolStripMenuItem;
		private ToolStripMenuItem landmarkShaderToolStripMenuItem;
		private ToolStripMenuItem landmarkShaderStencilToolStripMenuItem;
		private ToolStripMenuItem unitShaderSkinnedToolStripMenuItem;
        private ToolStripMenuItem leaderShaderToolStripMenuItem;
        private ToolStripMenuItem leaderSkinShaderToolStripMenuItem;
        private ToolStripMenuItem leaderOpaqueClothShaderToolStripMenuItem;
        private ToolStripMenuItem leaderFurShaderToolStripMenuItem;
        private ToolStripMenuItem leaderTransparencyShaderToolStripMenuItem;

        private ToolStripMenuItem leaderHairShaderToolStripMenuItem;
        private ToolStripMenuItem leaderFurFinShaderToolStripMenuItem;
        private ToolStripMenuItem leaderGlassShaderToolStripMenuItem;
        private ToolStripMenuItem leaderOpaqueHairShaderToolStripMenuItem;

        private ToolStripMenuItem leaderVelvetShaderToolStripMenuItem;
        private ToolStripMenuItem leaderMaskedShaderToolStripMenuItem;
        private ToolStripMenuItem leaderMaskedHairShaderToolStripMenuItem;
        private ToolStripMenuItem leaderOpaqueMatteShaderToolStripMenuItem;
        private ToolStripMenuItem leaderTransparentMatteShaderToolStripMenuItem;

        private ToolStripMenuItem simpleShaderToolStripMenuItem;

		private ContextMenuStrip addMaterialList;
		private ColumnHeader materialTypeColumnHeader;
        private ColumnHeader materialNameColumnHeader;
        private GroupBox fileInfoGroupBox;
        private RichTextBox fileInfoTextBox;
        private Label headerFilenameLabel;
        private SplitContainer leftHandSplitContainer;
        private TabControl mainTabControl;
        private TabPage grannyFileTabPage;
        private SplitContainer grannyFileSplitContainerA;
        private Panel mainButtonPanel;
        private Button openButton;
        private Button viewButton;
        private Button saveButton;
        private Button saveAsButton;
        private Button saveAnimationButton;
        private ListViewWithComboBox meshList;
        private ColumnHeader MeshName;
        private ColumnHeader Material;
        private SplitContainer grannyFileTabContainer;
        private ListView materialList;
        private ColumnHeader materialNameHeader;
        private ColumnHeader materialTypeHeader;
        private Panel materialButtonsPanel;
        private Button addMaterial;
        private Button deleteMaterial;
        private TabControl animationsTabControl;
        private TabPage animationDefsTabPage;
        private ListView animDefs;
        private ColumnHeader animName;
        private ColumnHeader startFrame;
        private ColumnHeader endFrame;
        private ColumnHeader eventCodes;
        private Panel animationsButtonPanel;
        private Button deleteAnim;
        private Button addAnim;
        private TabPage grannyAnimsTabPage;
        private ListView animList;
        private ColumnHeader anim;
        private ColumnHeader timeSlice;
        private Panel panel1;
        private ToolTip viewButtonToolTip;
        private TabPage otherActionsTabPage;
        private CheckBox enableLoggingBox;
        private Button exportNB2Button;
        private Button exportNB2CurrentModelButton;
        private Button br2ImportButton;
        private Button openFBXButton;
        private Button overwriteBR2Button;
        private Button overwriteMeshesButton;
        private TabPage selectModelTabPage;
        private ListView modelList;
        private ColumnHeader modelName;
		private TextBox UnitExport_Description;
        private ListViewItem lastModelListItemChecked;
        private Button loadStringDatabaseButton;
        private Button resaveAllFilesInDirButton;
        private Int32 currentModelIndex;
        private RadioButton useSceneTemplateRadioButton;
        private RadioButton useLeaderTemplateRadioButton;
        private RadioButton useUnitTemplateRadioButton;
        private Label templateBR2OverwriteLabel;
        private Button removeAnimationsButton;
        private Button resaveAllFBXAsAnimsButton;
        private Button cleanFTSXMLButton;
        private Label rescaleFactorLabel;
        private Button insertAdjustmentBoneButton;
        private TextBox rescaleFactorTextBox;
        private ComboBox bonesComboBox;
        private Label rescaleBoneNameLabel;
        private Button rescaleNamedBoneButton;
        private Button exportNA2Button;
        private Label endTImeTextBoxLabel;
        private Label startTimeTextBoxLabel;
        private TextBox endTimeTextBox;
        private Label angleLabel;
        private Label axisLabel;
        private ComboBox axisComboBox;
        private TextBox angleTextBox;
        private Button concatenateNA2Button;
        private Label fpsLevel;
        private TextBox fpsTextBox;
        private Button openBR2Button;
        private TextBox startTimeTextBox;

		public NexusBuddyApplicationForm()
		{
			try
			{
				RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Firaxis\\Nexus\\Civ5Settings");
				this.Civ5Location = registryKey.GetValue("GameBuild").ToString();
				this.Civ5Location = this.Civ5Location.Substring(0, this.Civ5Location.LastIndexOf("\\")) + "\\";
			}
			catch (Exception)
			{
			}
			NexusBuddyApplicationForm.form = this;
            usedShaderSet = new HashSet<string>();
			this.InitializeComponent();
			Available.Startup("Civ5");
			Context.Add(new GrannyContext());
			Context.Add(new ProjectConfig());
			Context.Get<GrannyContext>();
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			try
			{
				Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("NexusBuddy.GrannyTemplates.output_template.gr2");
				this.templateFilename = Path.GetTempPath() + "template.temp.gr2";
				FileStream fileStream = File.Create(this.templateFilename);
				this.ReadWriteStream(manifestResourceStream, fileStream);
				fileStream.Close();
			}
			catch
			{
			}
            try
            {
                Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("NexusBuddy.GrannyTemplates.model_template.gr2");
                this.modelTemplateFilename = Path.GetTempPath() + "modelTemplate.temp.gr2";
                FileStream fileStream = File.Create(this.modelTemplateFilename);
                this.ReadWriteStream(manifestResourceStream, fileStream);
                fileStream.Close();
            }
            catch
            {
            }
            try
            {
                Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("NexusBuddy.GrannyTemplates.leader_template.gr2");
                this.leaderTemplateFilename = Path.GetTempPath() + "leaderTemplate.temp.gr2";
                FileStream fileStream = File.Create(this.leaderTemplateFilename);
                this.ReadWriteStream(manifestResourceStream, fileStream);
                fileStream.Close();
            }
            catch
            {
            }
            try
            {
                Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("NexusBuddy.GrannyTemplates.scene_template.gr2");
                this.sceneTemplateFilename = Path.GetTempPath() + "sceneTemplate.temp.gr2";
                FileStream fileStream = File.Create(this.sceneTemplateFilename);
                this.ReadWriteStream(manifestResourceStream, fileStream);
                fileStream.Close();
            }
            catch
            {
            }
		}

		private void ReadWriteStream(Stream readStream, Stream writeStream)
		{
			int num = 256;
			byte[] buffer = new byte[num];
			for (int i = readStream.Read(buffer, 0, num); i > 0; i = readStream.Read(buffer, 0, num))
			{
				writeStream.Write(buffer, 0, i);
			}
			readStream.Close();
			writeStream.Close();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
            cleardownAllData();
            base.OnClosing(e);
		}

        private void cleardownAllData()
        {
            NexusBuddyApplicationForm.loadedFile = null;
            foreach (string current in this.TempFiles)
            {
                try
                {
                    File.Delete(current);
                }
                catch (Exception e)
                {
                }
            }
        }

		private void AddAnimToListbox(IGrannyAnimation grannyAnimation)
		{
			this.animList.Items.Add(grannyAnimation.Name);
			this.animList.Items[this.animList.Items.Count - 1].SubItems.Add(grannyAnimation.Duration.ToString());
			this.animList.Items[this.animList.Items.Count - 1].SubItems.Add(grannyAnimation.TimeStep.ToString());
		}

		private void AddMeshToListbox(IGrannyMesh grannyMesh)
		{
			this.meshList.Items.Add(grannyMesh.Name);
			if (grannyMesh.MaterialBindings.Count.Equals(1))
			{
				this.meshList.Items[this.meshList.Items.Count - 1].SubItems.Add(grannyMesh.MaterialBindings[0].Name);
			}
			else
			{
				this.meshList.Items[this.meshList.Items.Count - 1].SubItems.Add("<unassigned>");
			}
			this.meshList.Items[this.meshList.Items.Count - 1].Tag = grannyMesh;
		}

        private void AddMaterialToListbox(IGrannyMaterial mat)
		{
			string shaderSet;
			if ((shaderSet = mat.ShaderSet) != null)
			{
                //if (this.enableLoggingBox.Checked)
                //{
                //    log("Material name: " + mat.Name);
                //    log("Shader set: " + shaderSet);
                //    for (int i = 0; i < mat.GetParameterSetCount(); i++)
                //    {
                //        log("Parameter set " + i + ": " + mat.GetParameterSet(i).ParamBlock);
                //        for (int j = 0; j < mat.GetParameterSet(i).ParamCount; j++)
                //        {
                //            log("  Param Name: " + GetParameterName(mat, mat.GetParameterSet(i).ParamBlock, j));
                //            log("  Param Type: " + mat.GetParameterSet(i).GetParameterType(j));
                //            if (!shaderSet.Equals("SimpleShader"))
                //            {
                //                log("  Param Value: " + mat.GetParameterSet(i).GetParameterValue(j));
                //            }
                //        }
                //    }
                //    log("");
                //}

                usedShaderSet.Add(shaderSet);

                if (shaderSet == "SimpleShader")
                {
                    IndieSimpleShader indieSimpleShader = new IndieSimpleShader(mat);
                    indieSimpleShader.AddToListView(this.materialList);
                    return;
                }
				if (shaderSet == "BuildingShader")
				{
					IndieBuildingShader indieBuildingShader = new IndieBuildingShader(mat);
					indieBuildingShader.AddToListView(this.materialList);
					return;
				}
                if (shaderSet == "Leader")
                {
                    IndieLeaderShader indieLeaderShader = new IndieLeaderShader(mat);
                    indieLeaderShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Opaque_Cloth")
                {
                    IndieLeaderOpaqueClothShader indieLeaderOpaqueClothShader = new IndieLeaderOpaqueClothShader(mat);
                    indieLeaderOpaqueClothShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Skin")
                {
                    IndieLeaderSkinShader indieLeaderSkinShader = new IndieLeaderSkinShader(mat);
                    indieLeaderSkinShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Fur")
                {
                    IndieLeaderFurShader indieLeaderFurShader = new IndieLeaderFurShader(mat);
                    indieLeaderFurShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Transparency")
                {
                    IndieLeaderTransparencyShader indieLeaderTransparencyShader = new IndieLeaderTransparencyShader(mat);
                    indieLeaderTransparencyShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Hair")
                {
                    IndieLeaderHairShader indieLeaderHairShader = new IndieLeaderHairShader(mat);
                    indieLeaderHairShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Opaque_Hair")
                {
                    IndieLeaderOpaqueHairShader indieLeaderOpaqueHairShader = new IndieLeaderOpaqueHairShader(mat);
                    indieLeaderOpaqueHairShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Glass")
                {
                    IndieLeaderGlassShader indieLeaderGlassShader = new IndieLeaderGlassShader(mat);
                    indieLeaderGlassShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Fur_Fin")
                {
                    IndieLeaderFurFinShader indieLeaderFurFinShader = new IndieLeaderFurFinShader(mat);
                    indieLeaderFurFinShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Velvet")
                {
                    IndieLeaderVelvetShader indieLeaderVelvetShader = new IndieLeaderVelvetShader(mat);
                    indieLeaderVelvetShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Masked")
                {
                    IndieLeaderMaskedShader indieLeaderMaskedShader = new IndieLeaderMaskedShader(mat);
                    indieLeaderMaskedShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Masked_Hair")
                {
                    IndieLeaderMaskedHairShader indieLeaderMaskedHairShader = new IndieLeaderMaskedHairShader(mat);
                    indieLeaderMaskedHairShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Opaque_Matte")
                {
                    IndieLeaderOpaqueMatteShader indieLeaderOpaqueMatteShader = new IndieLeaderOpaqueMatteShader(mat);
                    indieLeaderOpaqueMatteShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "Leader_Transparent_Matte")
                {
                    IndieLeaderTransparentMatteShader indieLeaderTransparentMatteShader = new IndieLeaderTransparentMatteShader(mat);
                    indieLeaderTransparentMatteShader.AddToListView(this.materialList);
                    return;
                }
                if (shaderSet == "UnitShader_Skinned")
				{
					IndieUnitSkinnedShader indieUnitSkinnedShader = new IndieUnitSkinnedShader(mat);
                    //indieUnitSkinnedShader.SREFMap = "blacksref_sref.dds";
					indieUnitSkinnedShader.AddToListView(this.materialList);
					return;
				}
				if (shaderSet == "LandmarkShader_Stencil")
				{
					IndieLandmarkStencilShader indieLandmarkStencilShader = new IndieLandmarkStencilShader(mat);
					indieLandmarkStencilShader.AddToListView(this.materialList);
					return;
				}
				if (shaderSet != "LandmarkShader")
				{
					return;
				}
				IndieLandmarkStencilShader indieLandmarkStencilShader2 = new IndieLandmarkStencilShader(mat);
				indieLandmarkStencilShader2.AddToListView(this.materialList);
			}
		}

        private void refreshAppDataWithMessage(string message)
        {
            this.refreshAppData();
            this.fileInfoTextBox.Text = this.fileInfoTextBox.Text + message;
        }

		private void refreshAppData()
		{
            string fileInfo = "";
            Dictionary<string, int> materialNameCount = new Dictionary<string, int>();

            if (NexusBuddyApplicationForm.loadedFile != null) {

                if (currentModelIndex == -1)
                {
                    currentModelIndex = 0;
                }

			    this.AnimDefList.Clear();
			    this.materialList.Items.Clear();
			    this.meshList.Items.Clear();
			    this.animList.Items.Clear();
			    this.properties.SelectedObject = null;

                if (NexusBuddyApplicationForm.loadedFile.Models.Count > 0 && NexusBuddyApplicationForm.loadedFile.Models[currentModelIndex].Skeleton != null)
                {
                    IGrannySkeleton skeleton = NexusBuddyApplicationForm.loadedFile.Models[currentModelIndex].Skeleton;
                    bonesComboBox.Items.Clear();
                    foreach (IGrannyBone bone in skeleton.Bones)
                    {
                        bonesComboBox.Items.Add(bone.Name);
                    }
                }

                if (this.modelList.Items.Count != NexusBuddyApplicationForm.loadedFile.Models.Count)
                {
                    foreach (IGrannyModel currentModel in NexusBuddyApplicationForm.loadedFile.Models)
                    {
                        this.modelList.Items.Add(currentModel.Name);
                    }
                    modelList.Items[currentModelIndex].Checked = true;
                }
                if (NexusBuddyApplicationForm.loadedFile.Models.Count > 0)
                {
                    foreach (IGrannyMesh currentMesh in NexusBuddyApplicationForm.loadedFile.Models[currentModelIndex].MeshBindings)
                    {
                        this.AddMeshToListbox(currentMesh);
                    }
                }
			    foreach (IGrannyMaterial currentMaterial in NexusBuddyApplicationForm.loadedFile.Materials)
			    {
                    if (!materialNameCount.ContainsKey(currentMaterial.Name))
                    {
                        materialNameCount.Add(currentMaterial.Name, 1);
                    }
                    else
                    {
                        materialNameCount[currentMaterial.Name] = materialNameCount[currentMaterial.Name] + 1;
                        GrannyMaterialWrapper materialWrapper = new GrannyMaterialWrapper(currentMaterial);
                        String newName = currentMaterial.Name + "_" + materialNameCount[currentMaterial.Name];
                        materialWrapper.setName(newName);
                    }
                    this.AddMaterialToListbox(currentMaterial); 
			    }
                foreach (IGrannyAnimation currentAnimation in NexusBuddyApplicationForm.loadedFile.Animations)
			    {
                    this.AddAnimToListbox(currentAnimation);
			    }

                IGrannyFile file = NexusBuddyApplicationForm.loadedFile;
                string filename = file.Filename.Substring(file.Filename.LastIndexOf("\\") + 1);
                this.headerFilenameLabel.Text = filename;
                fileInfo += "Models: " + file.Models.Count + "     ";
                if (file.Models.Count > 0)
                {
                    fileInfo += "Current Model: " + file.Models[currentModelIndex].Name + " (Index:" + currentModelIndex + ")" + System.Environment.NewLine;
                    fileInfo += "Meshes (Current Model): " + file.Models[currentModelIndex].MeshBindings.Count + "     ";
                }
                fileInfo += "Meshes (Total): " + file.Meshes.Count + System.Environment.NewLine;
                fileInfo += "Materials: " + file.Materials.Count + System.Environment.NewLine;
                fileInfo += "Animations: " + file.Animations.Count + System.Environment.NewLine;
                fileInfo += "Shader Types Used: (" + usedShaderSet.Count + ") " + string.Join(", ", usedShaderSet) + System.Environment.NewLine;
                if (NexusBuddyApplicationForm.loadedStringDatabaseFilename != null) {
                    fileInfo += "Loaded String Database: " + NexusBuddyApplicationForm.loadedStringDatabaseFilename + System.Environment.NewLine;
                }   
            }

            this.fileInfoTextBox.Text = fileInfo;
		}

		private void materialList_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<object> list = new List<object>();
			if (this.materialList.SelectedItems.Count > 0)
			{
				foreach (object current in this.materialList.SelectedItems)
				{
					IndieMaterial indieMaterial = (current as ListViewItem).Tag as IndieMaterial;
					if (indieMaterial != null)
					{
						list.Add(indieMaterial);
					}
				}
				this.properties.SelectedObjects = list.ToArray();
			}
		}

        private unsafe void rescaleButtonClick(object sender, EventArgs e)
        {
            GrannySkeletonWrapper skeletonWrapper = new GrannySkeletonWrapper(loadedFile.Models[currentModelIndex].Skeleton);
            GrannySkeletonInfo skeletonInfo = skeletonWrapper.readSkeletonInfo();

            foreach (GrannyBoneInfo curBoneInfo in skeletonInfo.bones)
            {
                curBoneInfo.parentIndex = curBoneInfo.parentIndex + 1;
            }

            float scaleFactor = 1f;
            try
            {
                scaleFactor = Single.Parse(rescaleFactorTextBox.Text, CultureInfo.InvariantCulture);
            }
            catch (FormatException exception) {}

            Vector3D axisVector = new Vector3D(1.0d, 0.0d, 0.0d);
            if (axisComboBox.SelectedIndex == 1)
            {
                axisVector = new Vector3D(0.0d, 1.0d, 0.0d);
            }
            else if (axisComboBox.SelectedIndex == 2)
            {
                axisVector = new Vector3D(0.0d, 0.0d, 1.0d);
            }

            float angle = 1f;
            try
            {
                angle = Single.Parse(angleTextBox.Text, CultureInfo.InvariantCulture);
            }
            catch (FormatException exception) { }
            
            System.Windows.Media.Media3D.Quaternion quat = new System.Windows.Media.Media3D.Quaternion(axisVector, angle);

            GrannyBoneInfo boneInfo = new GrannyBoneInfo();

            int j = 1;
            string adjBoneNameRoot = "NB2_ADJUSTMENT_BONE";
            foreach (IGrannyBone bone in skeletonWrapper.wrappedSkeleton.Bones)
            {
                if (bone.Name.StartsWith(adjBoneNameRoot))
                {
                    j++;
                }
            }

            boneInfo.name = "NB2_ADJUSTMENT_BONE_" + j;
            //boneInfo.name = "NB2_ADJUSTMENT_BONE";

            boneInfo.parentIndex = -1;
            GrannyTransformInfo transformInfo = new GrannyTransformInfo();
            float[] position = { 0f, 0f, 0f };
            float[] orientation = { (float)quat.X, (float)quat.Y, (float)quat.Z, (float)quat.W };
            float[] scaleShear = { scaleFactor, 0f, 0f, 0f, scaleFactor, 0f, 0f, 0f, scaleFactor };
            float[] invWorldTransform = { 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f };
            transformInfo.position = position;
            transformInfo.orientation = orientation;
            transformInfo.scaleShear = scaleShear;
            transformInfo.flags = 7;
            boneInfo.localTransform = transformInfo;
            boneInfo.inverseWorldTransform = invWorldTransform;
            skeletonInfo.bones.Insert(0, boneInfo);

            skeletonWrapper.writeSkeletonInfo(skeletonInfo);
            saveAction();
            refreshAppDataWithMessage("ADJUSTMENT BONE INSERTED.");
        }

        private unsafe void rescaleNamedBoneButtonClick(object sender, EventArgs e)
        {
            GrannySkeletonWrapper skeletonWrapper = new GrannySkeletonWrapper(loadedFile.Models[currentModelIndex].Skeleton);
            GrannySkeletonInfo skeletonInfo = skeletonWrapper.readSkeletonInfo();

            float scaleFactor = Single.Parse(rescaleFactorTextBox.Text, CultureInfo.InvariantCulture);

            GrannyBoneInfo boneInfo = new GrannyBoneInfo();
            boneInfo.name = bonesComboBox.SelectedItem + "_FX";
            boneInfo.parentIndex = bonesComboBox.SelectedIndex;
            GrannyTransformInfo transformInfo = new GrannyTransformInfo();
            float[] position = { 0f, 0f, 0f };
            float[] orientation = { 0f, 0f, 0f, 1f };
            float[] scaleShear = { scaleFactor, 0f, 0f, 0f, scaleFactor, 0f, 0f, 0f, scaleFactor };
            float[] invWorldTransform = skeletonInfo.bones[bonesComboBox.SelectedIndex].inverseWorldTransform;
            transformInfo.position = position;
            transformInfo.orientation = orientation;
            transformInfo.scaleShear = scaleShear;
            transformInfo.flags = 4;
            boneInfo.localTransform = transformInfo;
            boneInfo.inverseWorldTransform = invWorldTransform;
            skeletonInfo.bones.Add(boneInfo);

            skeletonWrapper.writeSkeletonInfo(skeletonInfo);
            saveAction();
            refreshAppDataWithMessage("RESCALE BONE ADDED.");
        }

        private unsafe void exportNA2ButtonClick(object sender, EventArgs e)
        {
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                MessageBox.Show("Must have model file loaded to export animation.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (loadedFile != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "gr2 files (*.gr2) | *.gr2";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    IGrannyFile animationFile = openFileAsTempFileCopy(openFileDialog.FileName, "tempanimopen");
                    if (animationFile.Animations.Count == 1)
                    {
                        int retryLimit = 5;
                        Single startTime = -1;
                        try
                        {
                            startTime = Single.Parse(startTimeTextBox.Text, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException exception) { }

                        Single endTime = -1;
                        try
                        {
                            endTime = Single.Parse(endTimeTextBox.Text, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException exception) { }

                        Int32 fpsInput = 0;
                        try
                        {
                            fpsInput = Int32.Parse(fpsTextBox.Text, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException exception) { }

                        int retryCount = NB2Exporter.exportNA2(loadedFile.Models[currentModelIndex], animationFile.Animations[0], openFileDialog.FileName, startTime, endTime, fpsInput, retryLimit);

                        if (retryCount == retryLimit)
                        {
                            refreshAppDataWithMessage("ANIMATION EXPORT FAILED! Retry Count: " + retryCount);
                         }
                        else
                        {
                            refreshAppDataWithMessage("ANIMATION EXPORTED! Retry Count: " + retryCount);
                        }             
                    }
                }
                
            }
        }


        private void openButtonClick(object sender, EventArgs e)
        {
            openButtonDialogAction("gr2 files (*.gr2)|*.gr2|FBX files (*.fbx)|*.fbx|br2 files (*.br2)|*.br2|All files (*.*)|*.*");
        }

        private void openFBXButtonClick(object sender, EventArgs e)
        {
            openButtonDialogAction("FBX files (*.fbx)|*.fbx|All files (*.*)|*.*");
        }

        private void openBR2ButtonClick(object sender, EventArgs e)
        {
            openButtonDialogAction("br2 files (*.br2)|*.br2|All files (*.*)|*.*");       
        }

        private void openButtonDialogAction(string filterString)
        {
            usedShaderSet.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filterString;
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string openFilename = openFileDialog.FileName;

                string text = openFilename.Substring(openFilename.LastIndexOf("\\") + 1);
                text = text.ToLower().Replace(".gr2", "");
                text = text.ToLower().Replace(".br2", "");
                text = text.ToLower().Replace(".fbx", "");
                text = text.Replace(" ", "_");

                currentModelIndex = -1;
                modelList.Items.Clear();
                NexusBuddyApplicationForm.loadedFile = openFileAction(openFilename);
                refreshAppDataWithMessage("FILE OPENED.");
                this.addAnimation();  
            }
        }

        private void loadStringDatabaseButtonClick(object sender, EventArgs e)
        {
            openStringDatabase();
        }

        private void openStringDatabase()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GSD files (*.gsd)|*.gsd";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string openFilename = openFileDialog.FileName;
                GrannyContext grannyContext = Context.Get<GrannyContext>();
                grannyContext.LoadStringDatabase(openFilename);
                NexusBuddyApplicationForm.loadedStringDatabaseFilename = openFilename.Substring(openFilename.LastIndexOf("\\") + 1);
                refreshAppDataWithMessage("STRING DATABASE LOADED.");
            }
        }

        public IGrannyFile openFileAction(string openFilename)
        {
            IGrannyFile targetFile = openFileAsTempFileCopy(openFilename, "tempopen");

            if (openFilename.ToLower().Contains(".fbx"))
            {
                openFilename = openFilename.Replace(".fbx", ".gr2");
                openFilename = openFilename.Replace(".FBX", ".gr2");
            }
            else if (openFilename.ToLower().Contains(".br2"))
            {
                openFilename = openFilename.Replace(".br2", ".gr2");
                openFilename = openFilename.Replace(".BR2", ".gr2");
            }

            targetFile.Filename = openFilename;

            return targetFile;
        }

        public static string getVersionString()
        {
            return major_version + "." + minor_version + "." + sub_minor_version;
        }

        public static void setExporterInfo(GrannyFileWrapper fileWrapper)
        {
            fileWrapper.setExporterInfo("Nexus Buddy 2", major_version, minor_version, sub_minor_version, 0);
        }

        public IGrannyFile openFileAsTempFileCopy(string openFilename, string prefix)
        {
            GrannyContext grannyContext = Context.Get<GrannyContext>();
            string tempPath = Path.GetTempPath();
            string tempFilename = tempPath + prefix;
            if (openFilename.ToLower().Contains(".fbx"))
            {
                string tempFilename2 = tempFilename;
                tempFilename2 = appendRandomNumberAndGr2Extension(tempFilename2);

                FBXImporter.ImportFBXFile(openFilename, tempFilename2, null);

                IGrannyFile fbxgr2file = grannyContext.LoadGrannyFile(tempFilename2);
                GrannyFileWrapper fileWrapper = new GrannyFileWrapper(fbxgr2file);
                //fileWrapper.setFromArtToolInfo("Blender", 2, 0);
                float[] matrix = { 1f, 0f, 0f, 0f, 0f, 1f, 0f, -1f, 0f };
                fileWrapper.setMatrix(matrix);
                setExporterInfo(fileWrapper);
                fileWrapper.setFromFileName(openFilename);

                tempFilename = appendRandomNumberAndGr2Extension(tempFilename);

                saveAsAction(fbxgr2file, tempFilename, false);

                this.TempFiles.Add(tempFilename);
                this.TempFiles.Add(tempFilename2);
            }
            else if (openFilename.ToLower().Contains(".br2"))
            {
                tempFilename = appendRandomNumberAndGr2Extension(tempFilename);
                BR2Importer.importBR2(openFilename, tempFilename, grannyContext);
                this.TempFiles.Add(tempFilename);
            }
            else
            {
                tempFilename = appendRandomNumberAndGr2Extension(tempFilename);
                this.TempFiles.Add(tempFilename);
                try
                {
                    if (File.Exists(tempFilename))
                    {
                        File.Delete(tempFilename);
                    }
                }
                catch
                {
                }
                File.Copy(openFilename, tempFilename);
            }

            IGrannyFile targetFile = grannyContext.LoadGrannyFile(tempFilename);
            return targetFile;
        }

        private string appendRandomNumberAndGr2Extension(string tempFilename)
        {
            tempFilename += this.rand.Next(1000000).ToString();
            tempFilename += this.rand.Next(1000000).ToString();
            tempFilename += this.rand.Next(1000000).ToString();
            tempFilename += ".gr2";
            return tempFilename;
        }

        private void viewButtonClick(object sender, EventArgs e)
        {
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                MessageBox.Show("No file is currently loaded.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(NexusBuddyApplicationForm.loadedFile);

            if (!fileWrapper.getExporterName().Equals("Nexus Buddy 2"))
            {
                MessageBox.Show("You need to save this file from Nexus Buddy 2 before it can be viewed.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            this.viewAction();
        }

        private void viewAction()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "gr2_viewer.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            if (loadedFile != null)
            {
                startInfo.Arguments = "\"" + loadedFile.Filename + "\"";
                Process.Start(startInfo);
            }
        }

        //private void log(string messageLine) {
        //    //if (this.enableLoggingBox.Checked) {
        //    //    logFileWriter = new StreamWriter(new FileStream("nexusbuddy.log", FileMode.Append));
        //    //    logFileWriter.WriteLine(messageLine);
        //    //    logFileWriter.Close();
        //    //}
        //}
		
		private void saveAsButtonClick(object sender, EventArgs e)
		{
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                MessageBox.Show("No file is currently loaded.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "gr2 files (*.gr2)|*.gr2|All files (*.*)|*.*";
			saveFileDialog.FilterIndex = 0;
			saveFileDialog.RestoreDirectory = true;
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = saveFileDialog.FileName;
                saveAsAction(NexusBuddyApplicationForm.loadedFile, fileName, true);       
			}
		}

        private void saveButtonClick(object sender, EventArgs e)
        {
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                MessageBox.Show("No file is currently loaded.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            DialogResult dialogResult = DialogResult.Yes;
            if (File.Exists(NexusBuddyApplicationForm.loadedFile.Filename))
            {
                dialogResult = MessageBox.Show("Are you sure you want to overwrite this file?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            }
            if (dialogResult == DialogResult.Yes)
            {
                saveAction();
            }
        }

        private void saveAction()
        {
            string filename = NexusBuddyApplicationForm.loadedFile.Filename;
            saveAsAction(NexusBuddyApplicationForm.loadedFile, filename, true);      
        }

        public void saveAsAction(IGrannyFile fileToSave, string fileName, bool updateAppDataAndSaveMessage)
        {

            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(fileToSave);
            setExporterInfo(fileWrapper);

            string tempStagingFileName = fileName.Replace(".gr2", "_temp_" + this.rand.Next(1000000).ToString() + this.rand.Next(1000000).ToString() + ".gr2");
            fileToSave.Filename = tempStagingFileName;
            fileToSave.Source = "Tool";
            fileToSave.Save();
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            System.IO.File.Move(tempStagingFileName, fileName);
            fileToSave.Filename = fileName;

            this.openTempFiles.Clear();
            GrannyContext grannyContext = Context.Get<GrannyContext>();
            string tempPath = Path.GetTempPath();
            string tempFileName = tempPath + "tempopen";
            tempFileName += this.rand.Next(1000000).ToString();
            tempFileName += this.rand.Next(1000000).ToString();
            tempFileName += ".gr2";
            this.TempFiles.Add(tempFileName);
            File.Copy(fileName, tempFileName);
            IGrannyFile savedFile = grannyContext.LoadGrannyFile(tempFileName);

            savedFile.Filename = fileName;
            savedFile.Source = "Tool";

            NexusBuddyApplicationForm.loadedFile = savedFile;
            modelList.Items.Clear();

            if (updateAppDataAndSaveMessage)
            {
                refreshAppDataWithMessage("FILE SAVED.");
            }
        }

        private string getShortFilenameForLoadedFile()
        {
            return getShortFilename(loadedFile.Filename); 
        }

        private string getShortFilename(String filename)
        {
            return filename.Substring(filename.LastIndexOf("\\") + 1);
        }

        private void outputAllVisFXNames(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = Settings.Default.RecentFolder
            };
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                HashSet<string> visFxNamesSet = new HashSet<string>();
                Settings.Default.RecentFolder = folderBrowserDialog.SelectedPath;
                var files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(s => s.ToLower().EndsWith(".xml"));

                foreach (string filename in files)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(VisEffectArtInfos));
                    VisEffectArtInfos visEffectArtInfos = (VisEffectArtInfos)serializer.Deserialize(new XmlTextReader(filename));

                    foreach (VisEffectArtInfosVisEffectArtInfo visEffectArtInfo in visEffectArtInfos.Items)
                    {
                        visFxNamesSet.Add(visEffectArtInfo.Name);
                    }
                }

                foreach (string name in visFxNamesSet)
                {
                    MemoryUtil.memLogLine(name);
                }
            }
        }

        private void cleanFTSXMLButtonClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ftsxml files (*.ftsxml)|*.FTSXML";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Read Anim Code Names file
                Dictionary<int, string> animCodeNames = new Dictionary<int, string>();
                StreamReader animCodesStreamReader = new StreamReader("animCodes.txt");
                Regex regex = new Regex("([0-9]+)\\s+(\\w+)\\s*");
                while (animCodesStreamReader.Peek() >= 0)
                {
                    string currentAnimCodeLine = animCodesStreamReader.ReadLine();
                    MatchCollection mc = regex.Matches(currentAnimCodeLine);
                    foreach (Match m in mc)
                    {
                        int animCode = NumberUtils.parseInt(m.Groups[1].Value);
                        string animName = m.Groups[2].Value;
                        animCodeNames.Add(animCode, animName);
                    }
                }
                // Reorder triggers
                string filename = openFileDialog.FileName;

                XmlSerializer serializer = new XmlSerializer(typeof(trigger_system));
                trigger_system triggerSystem = (trigger_system)serializer.Deserialize(new XmlTextReader(filename));

                List<trigger_systemTriggersTrigger> triggersList = triggerSystem.triggers.ToList();

                triggersList.Sort((x, y) => x.CompareTo(y));

                triggerSystem.triggers = triggersList.ToArray();

                //int maxTriggerId = 0;
                HashSet<int> animCodesSet = new HashSet<int>();
                int currentId = 0;
                foreach (trigger_systemTriggersTrigger trigger in triggersList)
                {
                    // int triggerId = Int32.Parse(trigger.id);
                    // if (triggerId > maxTriggerId)
                    //{
                    //    maxTriggerId = triggerId;
                    //}
                    trigger.id = currentId.ToString();
                    if (trigger.type.Equals("FTimedTriggerTransfer"))
                    {
                        trigger.refid = (currentId - 1).ToString();
                    }
                    animCodesSet.Add(int.Parse(trigger.ec));
                    currentId++;
                }

                // Determine max trigger ID
                //int maxTriggerId = 0;
                //HashSet<int> animCodesSet = new HashSet<int>();
                //foreach (trigger_systemTriggersTrigger trigger in triggersList)
                //{
                //    int triggerId = Int32.Parse(trigger.id);
                //    if (triggerId > maxTriggerId)
                //    {
                //        maxTriggerId = triggerId;
                //    }
                //    animCodesSet.Add(int.Parse(trigger.ec));
                //}

                string newfilename = filename.ToLower().Replace(".ftsxml", "_.ftsxml");
                TextWriter writer = new StreamWriter(newfilename);
                serializer.Serialize(writer, triggerSystem);
                writer.Close();

                // Re-read file data
                string currentLine = "";
                StreamReader streamReader = new StreamReader(newfilename);
                List<string> eventTrackLines = new List<string>();
                List<string> triggerLines = new List<string>();
                while (!currentLine.Contains("<triggers>"))
                {
                    currentLine = streamReader.ReadLine();
                    if (!currentLine.Contains("xml version") && !currentLine.Contains("<trigger_system"))
                    {
                        eventTrackLines.Add(currentLine);
                    }   
                }
                while (!currentLine.Contains("</triggers>"))
                {
                    currentLine = streamReader.ReadLine();
                    if (currentLine.Trim().StartsWith("<trigger"))
                    {
                        triggerLines.Add(currentLine);
                    }                  
                }
                streamReader.Close();


                // Insert anim code name comments
                Boolean first = true;
                foreach (int animCode in animCodesSet)
                {
                    for (int i = 0; i < triggerLines.Count; i++)
                    {
                        if (triggerLines[i].Contains("ec=\"" + animCode + "\""))
                        {                   
                            triggerLines.Insert(i, "    <!-- ec=\"" + animCode + "\" " + animCodeNames[animCode] + " -->");
                            if (!first)
                            {
                                triggerLines.Insert(i, "");
                            }
                            first = false;
                            break;
                        }
                    }
                }

                // Write final output
                TextWriter writer2 = new StreamWriter(newfilename);
                writer2.Write("<trigger_system type=\"FGrannyTimedTriggerSystem\">\n");
                foreach (string line in eventTrackLines)
                {
                    writer2.Write(line + "\n");
                }
                foreach (string line in triggerLines)
                {
                    writer2.Write(line + "\n");
                }
                writer2.Write("  </triggers>\n");
                writer2.Write("</trigger_system>");
                writer2.Close();

                refreshAppDataWithMessage("FTSXML REORDERED.\n" );
            }  
        }

        private void saveAnimationClick(object sender, EventArgs e)
        {
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                MessageBox.Show("No file is currently loaded.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "gr2 files (*.gr2)|*.gr2|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = getShortFilenameForLoadedFile().Replace(".gr2", "_anim.gr2");
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                saveAnimationsAction(NexusBuddyApplicationForm.loadedFile, fileName);

                refreshAppDataWithMessage("ANIMATION FILE SAVED.");
            }
        }

        private static void saveAnimationsAction(IGrannyFile grannyFile, string filename)
        {
            GrannyContext grannyContext = Context.Get<GrannyContext>();
            int i = 0;
            foreach (IGrannyAnimation animation in grannyFile.Animations)
            {
                string extension_string = "";
                if (i > 0)
                {
                    extension_string = i + "";
                }
                IGrannyFile animationFile = grannyContext.CreateEmptyGrannyFile(filename.Replace(".gr2", extension_string + ".gr2"));
                animationFile.AddAnimationReference(animation);
                animationFile.AddArtToolAndExporterReference(grannyFile);
                animationFile.Save();
                i++;
            }
        }

        private void overwriteMeshesButtonClick(object sender, EventArgs e)
        {
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can overwrite meshes!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            GrannyContext grannyContext = Context.Get<GrannyContext>();
            usedShaderSet.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "br2 files (*.br2)|*.br2";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string meshBR2filename = openFileDialog.FileName;
                BR2Importer.overwriteMeshes(NexusBuddyApplicationForm.loadedFile, meshBR2filename, grannyContext, currentModelIndex);
                refreshAppDataWithMessage("MESHES OVERWRITTEN.");
            }
        }

        private unsafe void removeAnimationsButtonClick(object sender, EventArgs e)
        {
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                MessageBox.Show("No file is currently loaded.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(NexusBuddyApplicationForm.loadedFile);
            fileWrapper.setNumAnimations(0);
            fileWrapper.setNumTrackGroups(0);
            saveAction();
        }

        private void concatenateNA2ButtonClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = Settings.Default.RecentFolder
            };
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                String tempFilename = "combined" + this.rand.Next(100000).ToString() + ".na2";
                Settings.Default.RecentFolder = folderBrowserDialog.SelectedPath;
                var files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(s => s.ToLower().EndsWith(".na2"));

                TextWriter textWriter = new StreamWriter(tempFilename);
                textWriter.WriteLine("// Nexus Buddy Animation NA2 - Exported from Nexus Buddy 2");

                int frameSets = 0;
                foreach (string filename in files)
                {
                    if (filename.LastIndexOf("__") > 0)
                    {
                        frameSets++;
                    }
                }

                textWriter.WriteLine("FrameSets: " + frameSets);

                String outputFilename = null;

                foreach (string filename in files)
                {
                    if (filename.LastIndexOf("__") > 0 ) { 
                        outputFilename = filename.Substring(0, filename.LastIndexOf("__")) + ".na2";
                        StreamReader fileReader = new StreamReader(filename);
                        while (fileReader.Peek() >= 0)
                        {
                            string fileLine = fileReader.ReadLine();
                            if (!fileLine.StartsWith("//") && !fileLine.StartsWith("FrameSets"))
                            {
                                textWriter.Write(fileLine + "\n");
                            }
                        }
                        fileReader.Close();
                    }
                }
                textWriter.Close();

                if (File.Exists(outputFilename))
                {
                    File.Delete(outputFilename);
                }
                    if (File.Exists(tempFilename))
                {
                    File.Copy(tempFilename, outputFilename);
                    File.Delete(tempFilename);
                }
                refreshAppDataWithMessage("NA2 FILES CONCATENATED.");
            }
        }

        private void resaveAllFilesInDirButtonClick(object sender, EventArgs e)
        {
            resaveAllFilesInDir();
        }

        private void resaveAllFilesInDir()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = Settings.Default.RecentFolder
            };
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                Settings.Default.RecentFolder = folderBrowserDialog.SelectedPath;
                var files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(s => s.ToLower().EndsWith(".gr2"));

                Directory.CreateDirectory(folderBrowserDialog.SelectedPath + "\\resaveBatch");

                foreach (string filename in files)
                {
                    IGrannyFile grannyFile = openFileAction(filename);
                    saveAsAction(grannyFile, folderBrowserDialog.SelectedPath + "\\resaveBatch\\" + getShortFilename(filename), false);
                }
                cleardownAllData();
                refreshAppDataWithMessage("ALL FILES IN DIRECTORY RESAVED.");
            }
        }

        private void resaveAllFBXAsAnimsButtonClick(object sender, EventArgs e)
        {
            resaveAllFBXFilesInDirAsAnims();
        }

        private void resaveAllFBXFilesInDirAsAnims()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = Settings.Default.RecentFolder
            };
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                Settings.Default.RecentFolder = folderBrowserDialog.SelectedPath;
                var files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(s => s.ToLower().EndsWith(".fbx"));

                Directory.CreateDirectory(folderBrowserDialog.SelectedPath + "\\converted_gr2");

                foreach (string filename in files)
                {
                    IGrannyFile grannyFile = openFileAction(filename);
                    String savefilename = filename;
                    if (savefilename.ToLower().Contains(".fbx"))
                    {
                        savefilename = savefilename.Replace(".fbx", "_anim.gr2");
                        savefilename = savefilename.Replace(".FBX", "_anim.gr2");
                    }
                    saveAnimationsAction(grannyFile, folderBrowserDialog.SelectedPath + "\\converted_gr2\\" + getShortFilename(savefilename));
                }
                cleardownAllData();
                refreshAppDataWithMessage("ALL FBX FILES IN DIRECTORY SAVED AS GR2.");
            }
        }

        public void outputSkeletonDataButtonClick(object sender, EventArgs e)
        {
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can log skeleton info!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            MemoryUtil.memLogClear();
            MemoryUtil.memLogLine(getShortFilenameForLoadedFile());
            MemoryUtil.memLogLine("-----------------");

            IGrannySkeleton skeleton = NexusBuddyApplicationForm.loadedFile.Models[currentModelIndex].Skeleton;
            foreach (IGrannyBone bone in skeleton.Bones)
            {

                float[] invWorldTransform = bone.InverseWorldTransform;
                MemoryUtil.memLogLine("Bone: " + bone.Name);

                IGrannyTransform localTransform = bone.LocalTransform;
                float[] position = localTransform.Position;
                float[] orientation = localTransform.Orientation;
                float[] scaleShear = localTransform.ScaleShear;

                MemoryUtil.memLog("Position: ");
                for (int i = 0; i < position.Length; i++)
                {
                    MemoryUtil.memLog(position[i].ToString("f6", CultureInfo.InvariantCulture) + " ");
                }
                MemoryUtil.memLog(System.Environment.NewLine);

                MemoryUtil.memLog("Orientation: ");
                for (int i = 0; i < orientation.Length; i++)
                {
                    MemoryUtil.memLog(orientation[i].ToString("f6", CultureInfo.InvariantCulture) + " ");
                }
                MemoryUtil.memLog(System.Environment.NewLine);

                MemoryUtil.memLogLine("Scale Shear:");
                for (int i = 0; i < scaleShear.Length; i++)
                {
                    MemoryUtil.memLog(scaleShear[i].ToString("f6", CultureInfo.InvariantCulture) + " ");
                    if (i == 2 || i == 5)
                    {
                        MemoryUtil.memLog(System.Environment.NewLine);
                    }
                }
                MemoryUtil.memLog(System.Environment.NewLine);

                MemoryUtil.memLogLine("Inverse World Matrix:");
                for (int i = 0; i < invWorldTransform.Length; i++)
                {
                    MemoryUtil.memLog(invWorldTransform[i].ToString("f6", CultureInfo.InvariantCulture) + " ");
                    if (i == 3 || i == 7 || i == 11)
                    {
                        MemoryUtil.memLog(System.Environment.NewLine);
                    }
                }
                MemoryUtil.memLog(System.Environment.NewLine);
                MemoryUtil.memLog(System.Environment.NewLine);
            }
        }

        private void deleteMaterial_Click(object sender, EventArgs e)
		{
			List<ListViewItem> list = new List<ListViewItem>();
			if (this.materialList.SelectedItems.Count > 0 && MessageBox.Show("Are you sure you want to delete the selected materials? There is no undo.", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				foreach (object current in this.materialList.SelectedItems)
				{
					IndieMaterial indieMaterial = (current as ListViewItem).Tag as IndieMaterial;
					foreach (IGrannyMesh current2 in NexusBuddyApplicationForm.loadedFile.Meshes)
					{
						current2.RemoveMaterialBinding(indieMaterial.GetMaterial());
					}
					NexusBuddyApplicationForm.loadedFile.RemoveMaterial(indieMaterial.GetMaterial());
					NexusBuddyApplicationForm.loadedFile.Materials.Remove(indieMaterial.GetMaterial());
					list.Add(current as ListViewItem);
					foreach (object current3 in this.meshList.Items)
					{
						if ((current3 as ListViewItem).SubItems[1].Text == indieMaterial.Name)
						{
							(current3 as ListViewItem).SubItems[1].Text = "<unassigned>";
						}
					}
				}
				foreach (ListViewItem current4 in list)
				{
					this.materialList.Items.Remove(current4);
				}
				this.properties.SelectedObject = null;
			}
		}

		private void addMaterial_Click(object sender, EventArgs e)
		{
			this.addMaterialList.Show(Cursor.Position);
		}

		public IGrannyMaterial GetMaterialFromTemplateFile(string type)
		{
			if (NexusBuddyApplicationForm.loadedFile == null)
			{
				return null;
			}
			GrannyContext grannyContext = Context.Get<GrannyContext>();
			string tempPath = Path.GetTempPath();
			string text = tempPath + "temptemplate";
			text += this.rand.Next(10000000).ToString();
			text += this.rand.Next(10000000).ToString();
			text += this.rand.Next(10000000).ToString();
			text += this.rand.Next(10000000).ToString();
			text += ".gr2";
			this.TempFiles.Add(text);
            File.Copy(this.templateFilename, text);
			IGrannyFile grannyFile = grannyContext.LoadGrannyFile(text);
			this.openTempFiles.Add(grannyFile);
			foreach (IGrannyMaterial current in grannyFile.Materials)
			{
				if (current.ShaderSet == type)
				{
					return current;
				}
			}
			return null;
		}

		private string GetNewMaterialName(string buildingshader)
		{
			int num = 1;
			bool flag = false;
			string text = "";
			while (!flag)
			{
				flag = true;
				text = buildingshader + num.ToString();
				foreach (IGrannyMaterial current in NexusBuddyApplicationForm.loadedFile.Materials)
				{
					if (current.Name == text)
					{
						flag = false;
						num++;
					}
				}
			}
			return text;
		}
       

        private void exportNB2ButtonClick(object sender, EventArgs e)
        {
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can export to NB2!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            NB2Exporter.exportAllModelsToNB2(NexusBuddyApplicationForm.loadedFile);
            refreshAppDataWithMessage("EXPORT TO NB2 COMPLETE.");
        }

        private void exportNB2CurrentModelButtonClick(object sender, EventArgs e)
        {
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can export to NB2!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            NB2Exporter.exportNB2(NexusBuddyApplicationForm.loadedFile, currentModelIndex);
            refreshAppDataWithMessage("EXPORT TO NB2 COMPLETE.");
        }

        // For making material template .gr2
        private void makeTemplateButtonClick2(object sender, EventArgs e)
        {

            List<IGrannyFile> sourceTemplateFiles = new List<IGrannyFile>();
            GrannyContext grannyContext = Context.Get<GrannyContext>();

            for (int i = 0; i < sourceTemplateFilenames.Length; i++)
            {
                string filename = sourceTemplatePath + sourceTemplateFilenames[i];
                IGrannyFile file = grannyContext.LoadGrannyFile(filename);
                sourceTemplateFiles.Add(file);
            }

            IGrannyFile outputFile = grannyContext.CreateEmptyGrannyFile(sourceTemplatePath + "output_template2.gr2");
            HashSet<string> shadersAdded = new HashSet<string>();

            foreach (IGrannyFile currentFile in sourceTemplateFiles)
            {
                foreach (IGrannyMaterial currentMaterial in currentFile.Materials)
                {
                    if (!shadersAdded.Contains(currentMaterial.ShaderSet))
                    {
                        if (currentMaterial.ShaderSet.Equals("UnitShader_Skinned"))
                        {
                            IndieUnitSkinnedShader indieUnitSkinnedShader = new IndieUnitSkinnedShader(currentMaterial);
                            indieUnitSkinnedShader.SREFMap = "blacksref_sref.dds";
                        }                        
                        outputFile.AddMaterialReference(currentMaterial);
                    }
                    shadersAdded.Add(currentMaterial.ShaderSet);
                }
                if (currentFile.Animations.Count > 0 && outputFile.Animations.Count == 0)
                {
                    outputFile.AddAnimationReference(currentFile.Animations[0]);
                }
            }

            outputFile.Save();
        }

        // For making material template .gr2
        private void makeTemplateButtonClick(object sender, EventArgs e)
        {
            string filename = sourceTemplatePath + "oda_scene.gr2";

            IGrannyFile file = NexusBuddyApplicationForm.form.openFileAsTempFileCopy(filename, "tempimport");

            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(file);

            GrannyMeshWrapper meshWrapper = new GrannyMeshWrapper(file.Meshes[0]);
            meshWrapper.setName("BLANK_MESH");
            meshWrapper.setNumVertices(1);
            //meshWrapper.setNumBoneBindings(0);
            //meshWrapper.setNumIndices(0);
            meshWrapper.setNumIndices16(1);
            meshWrapper.setGroup0TriCount(1);
            meshWrapper.setNumMaterialBindings(0);

            GrannyModelWrapper modelWrapper = new GrannyModelWrapper(file.Models[0]);
            modelWrapper.setNumMeshBindings(1);
            modelWrapper.setName("BLANK_MODEL");

            GrannySkeletonWrapper skeletonWrapper = new GrannySkeletonWrapper(modelWrapper.wrappedModel.Skeleton);
            skeletonWrapper.setNumBones(0);
            skeletonWrapper.setName("BLANK_SKELETON");

            fileWrapper.setNumMeshes(1);
            //fileWrapper.setNumModels(0);
            fileWrapper.setNumVertexDatas(1);
            fileWrapper.setNumSkeletons(0);
            fileWrapper.setNumTriTopologies(1);
            fileWrapper.setNumMaterials(0);

            file.Meshes.Clear();

            saveAsAction(file, sourceTemplatePath + "scene_template.gr2", true);

            //List<IGrannyFile> sourceTemplateFiles = new List<IGrannyFile>();
            //GrannyContext grannyContext = Context.Get<GrannyContext>();

            //for (int i = 0; i < sourceTemplateFilenames.Length; i++)
            //{
            //    string filename = sourceTemplatePath + sourceTemplateFilenames[i];
            //    IGrannyFile file = grannyContext.LoadGrannyFile(filename);
            //    sourceTemplateFiles.Add(file);
            //}

            //IGrannyFile outputFile = grannyContext.CreateEmptyGrannyFile(sourceTemplatePath + "output_template2.gr2");
            //HashSet<string> shadersAdded = new HashSet<string>();

            //foreach (IGrannyFile currentFile in sourceTemplateFiles)
            //{
            //    foreach (IGrannyMaterial currentMaterial in currentFile.Materials)
            //    {
            //        if (!shadersAdded.Contains(currentMaterial.ShaderSet))
            //        {
            //            outputFile.AddMaterialReference(currentMaterial);
            //        }
            //        shadersAdded.Add(currentMaterial.ShaderSet);
            //    }
            //    if (currentFile.Animations.Count > 0 && outputFile.Animations.Count == 0)
            //    {
            //        outputFile.AddAnimationReference(currentFile.Animations[0]);
            //    }
            //}

            //outputFile.Save();
        }

        private void shaderToolStripMenuItem_Click(string templateShaderName, string materialName)
        {
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                return;
            }
            IGrannyMaterial materialFromTemplateFile = this.GetMaterialFromTemplateFile(templateShaderName);
            string newMaterialName = this.GetNewMaterialName(materialName);
            materialFromTemplateFile.Name = newMaterialName;
            NexusBuddyApplicationForm.loadedFile.AddMaterialReference(materialFromTemplateFile);
            this.AddMaterialToListbox(materialFromTemplateFile);
        }

        private void simpleShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("SimpleShader", "MaterialSimple");
        }
        private void buildingShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("BuildingShader", "MaterialBuilding");
        }
        private void landmarkShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("LandmarkShader", "MaterialLandmark");
        }
        private void landmarkShaderStencilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("LandmarkShader_Stencil", "MaterialLandmarkStencil");
        }
        private void unitShaderSkinnedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("UnitShader_Skinned", "MaterialUnitSkinned");
        }
        private void leaderShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader", "MaterialLeader");
        }
        private void leaderSkinShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Skin", "MaterialLeaderSkin");
        }
        private void leaderOpaqueClothShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Opaque_Cloth", "MaterialLeaderOpaqueCloth");
        }
        private void leaderFurShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Fur", "MaterialLeaderFur");
        }
        private void leaderTransparencyShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Transparency", "MaterialLeaderTransparency");
        }
        private void leaderGlassShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Glass", "MaterialLeaderGlass");
        }
        private void leaderFurFinShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Fur_Fin", "MaterialLeaderFurFin");
        }
        private void leaderHairShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Hair", "MaterialLeaderHair");
        }
        private void leaderOpaqueHairShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Opaque_Hair", "MaterialLeaderOpaqueHair");
        }
        private void leaderOpaqueMatteShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Opaque_Matte", "MaterialLeaderOpaqueMatte");
        }
        private void leaderTransparentMatteShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Transparent_Matte", "MaterialLeaderTransparentMatte");
        }
        private void leaderMaskedHairShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Masked_Hair", "MaterialLeaderMaskedHair");
        }
        private void leaderMaskedShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Masked", "MaterialLeaderMasked");
        }
        private void leaderVelvetShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.shaderToolStripMenuItem_Click("Leader_Velvet", "MaterialLeaderVelvet");
        }

		public void UpdateMaterialBinding(string material)
		{
			if (this.meshList.SelectedItems.Count == 0 || this.meshList.SelectedItems.Count > 1)
			{
				return;
			}
			IGrannyMesh grannyMesh = this.meshList.SelectedItems[0].Tag as IGrannyMesh;
			IGrannyMaterial[] array = grannyMesh.MaterialBindings.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				IGrannyMaterial kMaterial = array[i];
				grannyMesh.RemoveMaterialBinding(kMaterial);
			}
			grannyMesh.MaterialBindings.Clear();
			if (material == "<unassigned>")
			{
				return;
			}
			foreach (IGrannyMaterial current in NexusBuddyApplicationForm.loadedFile.Materials)
			{
				if (current.Name == material)
				{
					grannyMesh.AddMaterialBinding(current);
                    break;
				}
			}
		}

		private void properties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.Label == "Name" && this.properties.SelectedObject is IndieMaterial)
			{
				bool flag = true;
				foreach (ListViewItem listViewItem in this.materialList.Items)
				{
					if (listViewItem.Text == (string)e.ChangedItem.Value && this.properties.SelectedObject as IndieMaterial != listViewItem.Tag as IndieMaterial)
					{
						(this.properties.SelectedObject as IndieMaterial).Name = (e.OldValue as string);
						MessageBox.Show("A material of this name already exists.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						flag = false;
					}
				}
				if (flag)
				{
					foreach (ListViewItem listViewItem2 in this.materialList.Items)
					{
						if (listViewItem2.Text == (string)e.OldValue)
						{
							listViewItem2.Text = (e.ChangedItem.Value as string);
						}
					}
					foreach (ListViewItem listViewItem3 in this.meshList.Items)
					{
						if (listViewItem3.SubItems[1].Text == (string)e.OldValue)
						{
							listViewItem3.SubItems[1].Text = (e.ChangedItem.Value as string);
						}
					}
				}
			}
			if (e.ChangedItem.Label == "Animation Name" && this.properties.SelectedObject is IndieAnimDef)
			{
				bool flag2 = true;
				foreach (ListViewItem listViewItem4 in this.animDefs.Items)
				{
					if (listViewItem4.Text == (string)e.ChangedItem.Value && this.properties.SelectedObject as IndieAnimDef != listViewItem4.Tag as IndieAnimDef)
					{
						(this.properties.SelectedObject as IndieMaterial).Name = (e.OldValue as string);
						MessageBox.Show("An animation of this name already exists.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						flag2 = false;
					}
				}
				if (flag2)
				{
					foreach (object current in this.animDefs.Items)
					{
						if ((current as ListViewItem).Text == (string)e.OldValue)
						{
							(current as ListViewItem).SubItems[0].Text = (e.ChangedItem.Value as string);
						}
					}
				}
			}
			if (e.ChangedItem.Label == "Start Frame")
			{
				foreach (object current2 in this.animDefs.Items)
				{
					if ((current2 as ListViewItem).Text == (this.properties.SelectedObject as IndieAnimDef).Name)
					{
						(current2 as ListViewItem).SubItems[1].Text = e.ChangedItem.Value.ToString();
					}
				}
			}
			if (e.ChangedItem.Label == "End Frame")
			{
				foreach (object current3 in this.animDefs.Items)
				{
					if ((current3 as ListViewItem).Text == (this.properties.SelectedObject as IndieAnimDef).Name)
					{
						(current3 as ListViewItem).SubItems[2].Text = e.ChangedItem.Value.ToString();
					}
				}
			}
			if (e.ChangedItem.Label == "Event Codes")
			{
				foreach (object current4 in this.animDefs.Items)
				{
					if ((current4 as ListViewItem).Text == (this.properties.SelectedObject as IndieAnimDef).Name)
					{
						(current4 as ListViewItem).SubItems[3].Text = (e.ChangedItem.Value as string);
					}
				}
			}
		}

        public bool useLeaderTemplate()
        {
            return useLeaderTemplateRadioButton.Checked;
        }

        public bool useSceneTemplate()
        {
            return useSceneTemplateRadioButton.Checked;
        }

		private void meshList_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<object> list = new List<object>();
			if (this.meshList.SelectedItems.Count > 0)
			{
				foreach (object current in this.meshList.SelectedItems)
				{
					IGrannyMesh grannyMesh = (current as ListViewItem).Tag as IGrannyMesh;
					if (grannyMesh != null)
					{
						list.Add(grannyMesh);
					}
				}
				this.properties.SelectedObjects = list.ToArray();
			}
		}

        private void addAnimation()
        {
            if (NexusBuddyApplicationForm.loadedFile == null)
            {
                return;
            }
            IndieAnimDef indieAnimDef = new IndieAnimDef();
            indieAnimDef.Name = this.GetNewAnimName();
            indieAnimDef.EndFrame = "0";
            if (NexusBuddyApplicationForm.loadedFile.Animations.Count > 0)
            {
                indieAnimDef.EndFrame = (Math.Ceiling((double)(NexusBuddyApplicationForm.loadedFile.Animations[0].Duration / NexusBuddyApplicationForm.loadedFile.Animations[0].TimeStep)) / 2.0).ToString();
            }
            indieAnimDef.StartFrame = "0";
            indieAnimDef.EventCodes = "";
            if (this.AnimDefList.Count == 0)
            {
                indieAnimDef.EventCodes = "1000, 1020, 1040, 1100, 1120, 1140, 1160, 1180, 1200, 1220, 1280, 1285, 1290, 1400, 1440, 1450, 1500, 1520, 1540, 1560, 1580, 1600, 1620, 1640, 1800, 2000, 2020, 2040, 2100, 2200, 2220, 2440";
            }
            this.AnimDefList.Add(indieAnimDef);
            this.animDefs.Items.Add(indieAnimDef.Name);
            this.animDefs.Items[this.animDefs.Items.Count - 1].Tag = indieAnimDef;
            this.animDefs.Items[this.animDefs.Items.Count - 1].SubItems.Add("0");
            this.animDefs.Items[this.animDefs.Items.Count - 1].SubItems.Add(indieAnimDef.EndFrame.ToString());
            this.animDefs.Items[this.animDefs.Items.Count - 1].SubItems.Add(indieAnimDef.EventCodes);
            this.properties.SelectedObject = indieAnimDef;
        }

		private void addAnim_Click(object sender, EventArgs e)
		{
            this.addAnimation();
		}

		private void deleteAnim_Click(object sender, EventArgs e)
		{
			if (NexusBuddyApplicationForm.loadedFile == null)
			{
				return;
			}
			List<ListViewItem> list = new List<ListViewItem>();
			foreach (ListViewItem listViewItem in this.animDefs.SelectedItems)
			{
				IndieAnimDef item = listViewItem.Tag as IndieAnimDef;
				this.AnimDefList.Remove(item);
				list.Add(listViewItem);
			}
			foreach (ListViewItem current in list)
			{
				this.animDefs.Items.Remove(current);
			}
		}

		private string GetNewAnimName()
		{
			int num = 1;
			bool flag = false;
			string text = "";
			while (!flag)
			{
				flag = true;
				text = "NewAnimation" + num.ToString();
				foreach (object current in this.animDefs.Items)
				{
					if ((current as ListViewItem).Text == text)
					{
						flag = false;
						num++;
					}
				}
			}
			return text;
		}

		private void animDefs_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<object> list = new List<object>();
			if (this.animDefs.SelectedItems.Count > 0)
			{
				foreach (object current in this.animDefs.SelectedItems)
				{
					list.Add((current as ListViewItem).Tag);
				}
			}
			this.properties.SelectedObjects = list.ToArray();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NexusBuddyApplicationForm));
            this.masterSplitContainer = new System.Windows.Forms.SplitContainer();
            this.leftHandSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mainButtonPanel = new System.Windows.Forms.Panel();
            this.openFBXButton = new System.Windows.Forms.Button();
            this.overwriteBR2Button = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.saveAsButton = new System.Windows.Forms.Button();
            this.saveAnimationButton = new System.Windows.Forms.Button();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.grannyFileTabPage = new System.Windows.Forms.TabPage();
            this.grannyFileSplitContainerA = new System.Windows.Forms.SplitContainer();
            this.grannyFileTabContainer = new System.Windows.Forms.SplitContainer();
            this.materialList = new System.Windows.Forms.ListView();
            this.materialNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.materialTypeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.materialButtonsPanel = new System.Windows.Forms.Panel();
            this.addMaterial = new System.Windows.Forms.Button();
            this.deleteMaterial = new System.Windows.Forms.Button();
            this.animationsTabControl = new System.Windows.Forms.TabControl();
            this.grannyAnimsTabPage = new System.Windows.Forms.TabPage();
            this.animList = new System.Windows.Forms.ListView();
            this.anim = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timeSlice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.animationDefsTabPage = new System.Windows.Forms.TabPage();
            this.animDefs = new System.Windows.Forms.ListView();
            this.animName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.startFrame = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.endFrame = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.eventCodes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.animationsButtonPanel = new System.Windows.Forms.Panel();
            this.deleteAnim = new System.Windows.Forms.Button();
            this.addAnim = new System.Windows.Forms.Button();
            this.otherActionsTabPage = new System.Windows.Forms.TabPage();
            this.fpsLevel = new System.Windows.Forms.Label();
            this.fpsTextBox = new System.Windows.Forms.TextBox();
            this.concatenateNA2Button = new System.Windows.Forms.Button();
            this.angleLabel = new System.Windows.Forms.Label();
            this.axisLabel = new System.Windows.Forms.Label();
            this.axisComboBox = new System.Windows.Forms.ComboBox();
            this.endTImeTextBoxLabel = new System.Windows.Forms.Label();
            this.startTimeTextBoxLabel = new System.Windows.Forms.Label();
            this.endTimeTextBox = new System.Windows.Forms.TextBox();
            this.startTimeTextBox = new System.Windows.Forms.TextBox();
            this.exportNA2Button = new System.Windows.Forms.Button();
            this.rescaleBoneNameLabel = new System.Windows.Forms.Label();
            this.rescaleNamedBoneButton = new System.Windows.Forms.Button();
            this.bonesComboBox = new System.Windows.Forms.ComboBox();
            this.rescaleFactorLabel = new System.Windows.Forms.Label();
            this.insertAdjustmentBoneButton = new System.Windows.Forms.Button();
            this.angleTextBox = new System.Windows.Forms.TextBox();
            this.rescaleFactorTextBox = new System.Windows.Forms.TextBox();
            this.cleanFTSXMLButton = new System.Windows.Forms.Button();
            this.removeAnimationsButton = new System.Windows.Forms.Button();
            this.resaveAllFBXAsAnimsButton = new System.Windows.Forms.Button();
            this.templateBR2OverwriteLabel = new System.Windows.Forms.Label();
            this.useSceneTemplateRadioButton = new System.Windows.Forms.RadioButton();
            this.useLeaderTemplateRadioButton = new System.Windows.Forms.RadioButton();
            this.useUnitTemplateRadioButton = new System.Windows.Forms.RadioButton();
            this.resaveAllFilesInDirButton = new System.Windows.Forms.Button();
            this.loadStringDatabaseButton = new System.Windows.Forms.Button();
            this.exportNB2CurrentModelButton = new System.Windows.Forms.Button();
            this.overwriteMeshesButton = new System.Windows.Forms.Button();
            this.exportNB2Button = new System.Windows.Forms.Button();
            this.selectModelTabPage = new System.Windows.Forms.TabPage();
            this.modelList = new System.Windows.Forms.ListView();
            this.modelName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.headerFilenameLabel = new System.Windows.Forms.Label();
            this.viewButton = new System.Windows.Forms.Button();
            this.fileInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.fileInfoTextBox = new System.Windows.Forms.RichTextBox();
            this.properties = new System.Windows.Forms.PropertyGrid();
            this.enableLoggingBox = new System.Windows.Forms.CheckBox();
            this.animation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.duration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.simpleShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildingShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.landmarkShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.landmarkShaderStencilToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unitShaderSkinnedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderFurShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderOpaqueClothShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderSkinShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderTransparencyShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderHairShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderOpaqueHairShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderFurFinShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderGlassShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderMaskedShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderMaskedHairShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderVelvetShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderOpaqueMatteShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaderTransparentMatteShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMaterialList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.materialTypeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.materialNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.viewButtonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.br2ImportButton = new System.Windows.Forms.Button();
            this.meshList = new NexusBuddy.ListViewWithComboBox();
            this.MeshName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Material = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.openBR2Button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.masterSplitContainer)).BeginInit();
            this.masterSplitContainer.Panel1.SuspendLayout();
            this.masterSplitContainer.Panel2.SuspendLayout();
            this.masterSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftHandSplitContainer)).BeginInit();
            this.leftHandSplitContainer.Panel1.SuspendLayout();
            this.leftHandSplitContainer.Panel2.SuspendLayout();
            this.leftHandSplitContainer.SuspendLayout();
            this.mainButtonPanel.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.grannyFileTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grannyFileSplitContainerA)).BeginInit();
            this.grannyFileSplitContainerA.Panel1.SuspendLayout();
            this.grannyFileSplitContainerA.Panel2.SuspendLayout();
            this.grannyFileSplitContainerA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grannyFileTabContainer)).BeginInit();
            this.grannyFileTabContainer.Panel1.SuspendLayout();
            this.grannyFileTabContainer.Panel2.SuspendLayout();
            this.grannyFileTabContainer.SuspendLayout();
            this.materialButtonsPanel.SuspendLayout();
            this.animationsTabControl.SuspendLayout();
            this.grannyAnimsTabPage.SuspendLayout();
            this.animationDefsTabPage.SuspendLayout();
            this.animationsButtonPanel.SuspendLayout();
            this.otherActionsTabPage.SuspendLayout();
            this.selectModelTabPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.fileInfoGroupBox.SuspendLayout();
            this.addMaterialList.SuspendLayout();
            this.SuspendLayout();
            // 
            // masterSplitContainer
            // 
            this.masterSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.masterSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.masterSplitContainer.Name = "masterSplitContainer";
            // 
            // masterSplitContainer.Panel1
            // 
            this.masterSplitContainer.Panel1.Controls.Add(this.leftHandSplitContainer);
            // 
            // masterSplitContainer.Panel2
            // 
            this.masterSplitContainer.Panel2.Controls.Add(this.panel1);
            this.masterSplitContainer.Panel2.Controls.Add(this.fileInfoGroupBox);
            this.masterSplitContainer.Panel2.Controls.Add(this.properties);
            this.masterSplitContainer.Size = new System.Drawing.Size(1008, 618);
            this.masterSplitContainer.SplitterDistance = 492;
            this.masterSplitContainer.TabIndex = 1;
            // 
            // leftHandSplitContainer
            // 
            this.leftHandSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftHandSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.leftHandSplitContainer.Name = "leftHandSplitContainer";
            this.leftHandSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // leftHandSplitContainer.Panel1
            // 
            this.leftHandSplitContainer.Panel1.Controls.Add(this.mainButtonPanel);
            // 
            // leftHandSplitContainer.Panel2
            // 
            this.leftHandSplitContainer.Panel2.Controls.Add(this.mainTabControl);
            this.leftHandSplitContainer.Size = new System.Drawing.Size(492, 618);
            this.leftHandSplitContainer.SplitterDistance = 42;
            this.leftHandSplitContainer.TabIndex = 2;
            // 
            // mainButtonPanel
            // 
            this.mainButtonPanel.Controls.Add(this.openFBXButton);
            this.mainButtonPanel.Controls.Add(this.overwriteBR2Button);
            this.mainButtonPanel.Controls.Add(this.openButton);
            this.mainButtonPanel.Controls.Add(this.saveButton);
            this.mainButtonPanel.Controls.Add(this.saveAsButton);
            this.mainButtonPanel.Controls.Add(this.saveAnimationButton);
            this.mainButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.mainButtonPanel.Name = "mainButtonPanel";
            this.mainButtonPanel.Size = new System.Drawing.Size(492, 42);
            this.mainButtonPanel.TabIndex = 9;
            // 
            // openFBXButton
            // 
            this.openFBXButton.Location = new System.Drawing.Point(155, 5);
            this.openFBXButton.Name = "openFBXButton";
            this.openFBXButton.Size = new System.Drawing.Size(87, 33);
            this.openFBXButton.TabIndex = 7;
            this.openFBXButton.Text = "Open FBX";
            this.openFBXButton.UseVisualStyleBackColor = true;
            this.openFBXButton.Click += new System.EventHandler(this.openFBXButtonClick);
            // 
            // overwriteBR2Button
            // 
            this.overwriteBR2Button.Location = new System.Drawing.Point(67, 5);
            this.overwriteBR2Button.Name = "overwriteBR2Button";
            this.overwriteBR2Button.Size = new System.Drawing.Size(82, 33);
            this.overwriteBR2Button.TabIndex = 6;
            this.overwriteBR2Button.Text = "Ovwr. BR2";
            this.overwriteBR2Button.UseVisualStyleBackColor = true;
            this.overwriteBR2Button.Click += new System.EventHandler(this.overwriteMeshesButtonClick);
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(4, 5);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(57, 33);
            this.openButton.TabIndex = 2;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButtonClick);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(248, 5);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(66, 33);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButtonClick);
            // 
            // saveAsButton
            // 
            this.saveAsButton.Location = new System.Drawing.Point(320, 5);
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.Size = new System.Drawing.Size(70, 33);
            this.saveAsButton.TabIndex = 4;
            this.saveAsButton.Text = "Save As";
            this.saveAsButton.UseVisualStyleBackColor = true;
            this.saveAsButton.Click += new System.EventHandler(this.saveAsButtonClick);
            // 
            // saveAnimationButton
            // 
            this.saveAnimationButton.Location = new System.Drawing.Point(396, 5);
            this.saveAnimationButton.Name = "saveAnimationButton";
            this.saveAnimationButton.Size = new System.Drawing.Size(90, 33);
            this.saveAnimationButton.TabIndex = 5;
            this.saveAnimationButton.Text = "Save Anim.";
            this.saveAnimationButton.UseVisualStyleBackColor = true;
            this.saveAnimationButton.Click += new System.EventHandler(this.saveAnimationClick);
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.grannyFileTabPage);
            this.mainTabControl.Controls.Add(this.otherActionsTabPage);
            this.mainTabControl.Controls.Add(this.selectModelTabPage);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(492, 572);
            this.mainTabControl.TabIndex = 2;
            // 
            // grannyFileTabPage
            // 
            this.grannyFileTabPage.Controls.Add(this.grannyFileSplitContainerA);
            this.grannyFileTabPage.Location = new System.Drawing.Point(4, 25);
            this.grannyFileTabPage.Name = "grannyFileTabPage";
            this.grannyFileTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.grannyFileTabPage.Size = new System.Drawing.Size(484, 543);
            this.grannyFileTabPage.TabIndex = 0;
            this.grannyFileTabPage.Text = "Edit Model";
            this.grannyFileTabPage.UseVisualStyleBackColor = true;
            // 
            // grannyFileSplitContainerA
            // 
            this.grannyFileSplitContainerA.Dock = System.Windows.Forms.DockStyle.Left;
            this.grannyFileSplitContainerA.Location = new System.Drawing.Point(3, 3);
            this.grannyFileSplitContainerA.Name = "grannyFileSplitContainerA";
            this.grannyFileSplitContainerA.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // grannyFileSplitContainerA.Panel1
            // 
            this.grannyFileSplitContainerA.Panel1.Controls.Add(this.meshList);
            // 
            // grannyFileSplitContainerA.Panel2
            // 
            this.grannyFileSplitContainerA.Panel2.Controls.Add(this.grannyFileTabContainer);
            this.grannyFileSplitContainerA.Size = new System.Drawing.Size(480, 537);
            this.grannyFileSplitContainerA.SplitterDistance = 195;
            this.grannyFileSplitContainerA.TabIndex = 0;
            // 
            // grannyFileTabContainer
            // 
            this.grannyFileTabContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grannyFileTabContainer.Location = new System.Drawing.Point(0, 0);
            this.grannyFileTabContainer.Name = "grannyFileTabContainer";
            this.grannyFileTabContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // grannyFileTabContainer.Panel1
            // 
            this.grannyFileTabContainer.Panel1.Controls.Add(this.materialList);
            this.grannyFileTabContainer.Panel1.Controls.Add(this.materialButtonsPanel);
            // 
            // grannyFileTabContainer.Panel2
            // 
            this.grannyFileTabContainer.Panel2.Controls.Add(this.animationsTabControl);
            this.grannyFileTabContainer.Size = new System.Drawing.Size(480, 338);
            this.grannyFileTabContainer.SplitterDistance = 162;
            this.grannyFileTabContainer.TabIndex = 5;
            // 
            // materialList
            // 
            this.materialList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.materialNameHeader,
            this.materialTypeHeader});
            this.materialList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialList.FullRowSelect = true;
            this.materialList.GridLines = true;
            this.materialList.Location = new System.Drawing.Point(0, 0);
            this.materialList.Name = "materialList";
            this.materialList.Size = new System.Drawing.Size(480, 131);
            this.materialList.TabIndex = 6;
            this.materialList.UseCompatibleStateImageBehavior = false;
            this.materialList.View = System.Windows.Forms.View.Details;
            this.materialList.SelectedIndexChanged += new System.EventHandler(this.materialList_SelectedIndexChanged);
            // 
            // materialNameHeader
            // 
            this.materialNameHeader.Text = "Material";
            this.materialNameHeader.Width = 180;
            // 
            // materialTypeHeader
            // 
            this.materialTypeHeader.Text = "Type";
            this.materialTypeHeader.Width = 200;
            // 
            // materialButtonsPanel
            // 
            this.materialButtonsPanel.Controls.Add(this.addMaterial);
            this.materialButtonsPanel.Controls.Add(this.deleteMaterial);
            this.materialButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.materialButtonsPanel.Location = new System.Drawing.Point(0, 131);
            this.materialButtonsPanel.Name = "materialButtonsPanel";
            this.materialButtonsPanel.Size = new System.Drawing.Size(480, 31);
            this.materialButtonsPanel.TabIndex = 7;
            // 
            // addMaterial
            // 
            this.addMaterial.Location = new System.Drawing.Point(12, 5);
            this.addMaterial.Name = "addMaterial";
            this.addMaterial.Size = new System.Drawing.Size(75, 23);
            this.addMaterial.TabIndex = 1;
            this.addMaterial.Text = "Add";
            this.addMaterial.UseVisualStyleBackColor = true;
            this.addMaterial.Click += new System.EventHandler(this.addMaterial_Click);
            // 
            // deleteMaterial
            // 
            this.deleteMaterial.Location = new System.Drawing.Point(95, 5);
            this.deleteMaterial.Name = "deleteMaterial";
            this.deleteMaterial.Size = new System.Drawing.Size(75, 23);
            this.deleteMaterial.TabIndex = 0;
            this.deleteMaterial.Text = "Delete";
            this.deleteMaterial.UseVisualStyleBackColor = true;
            this.deleteMaterial.Click += new System.EventHandler(this.deleteMaterial_Click);
            // 
            // animationsTabControl
            // 
            this.animationsTabControl.Controls.Add(this.grannyAnimsTabPage);
            this.animationsTabControl.Controls.Add(this.animationDefsTabPage);
            this.animationsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.animationsTabControl.Location = new System.Drawing.Point(0, 0);
            this.animationsTabControl.Name = "animationsTabControl";
            this.animationsTabControl.SelectedIndex = 0;
            this.animationsTabControl.Size = new System.Drawing.Size(480, 172);
            this.animationsTabControl.TabIndex = 1;
            // 
            // grannyAnimsTabPage
            // 
            this.grannyAnimsTabPage.Controls.Add(this.animList);
            this.grannyAnimsTabPage.Location = new System.Drawing.Point(4, 25);
            this.grannyAnimsTabPage.Name = "grannyAnimsTabPage";
            this.grannyAnimsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.grannyAnimsTabPage.Size = new System.Drawing.Size(472, 143);
            this.grannyAnimsTabPage.TabIndex = 0;
            this.grannyAnimsTabPage.Text = "Granny Anims";
            this.grannyAnimsTabPage.UseVisualStyleBackColor = true;
            // 
            // animList
            // 
            this.animList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.anim,
            this.timeSlice});
            this.animList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.animList.FullRowSelect = true;
            this.animList.GridLines = true;
            this.animList.Location = new System.Drawing.Point(3, 3);
            this.animList.Name = "animList";
            this.animList.Size = new System.Drawing.Size(466, 137);
            this.animList.TabIndex = 10;
            this.animList.UseCompatibleStateImageBehavior = false;
            this.animList.View = System.Windows.Forms.View.Details;
            // 
            // anim
            // 
            this.anim.Text = "Animation";
            this.anim.Width = 100;
            // 
            // timeSlice
            // 
            this.timeSlice.Text = "Time Slice";
            this.timeSlice.Width = 98;
            // 
            // animationDefsTabPage
            // 
            this.animationDefsTabPage.Controls.Add(this.animDefs);
            this.animationDefsTabPage.Controls.Add(this.animationsButtonPanel);
            this.animationDefsTabPage.Location = new System.Drawing.Point(4, 25);
            this.animationDefsTabPage.Name = "animationDefsTabPage";
            this.animationDefsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.animationDefsTabPage.Size = new System.Drawing.Size(472, 143);
            this.animationDefsTabPage.TabIndex = 1;
            this.animationDefsTabPage.Text = "Animation Definitions";
            this.animationDefsTabPage.UseVisualStyleBackColor = true;
            // 
            // animDefs
            // 
            this.animDefs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.animName,
            this.startFrame,
            this.endFrame,
            this.eventCodes});
            this.animDefs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.animDefs.FullRowSelect = true;
            this.animDefs.GridLines = true;
            this.animDefs.Location = new System.Drawing.Point(3, 3);
            this.animDefs.Name = "animDefs";
            this.animDefs.Size = new System.Drawing.Size(466, 106);
            this.animDefs.TabIndex = 11;
            this.animDefs.UseCompatibleStateImageBehavior = false;
            this.animDefs.View = System.Windows.Forms.View.Details;
            this.animDefs.SelectedIndexChanged += new System.EventHandler(this.animDefs_SelectedIndexChanged);
            // 
            // animName
            // 
            this.animName.Text = "Animation";
            this.animName.Width = 100;
            // 
            // startFrame
            // 
            this.startFrame.Text = "Start Frame";
            this.startFrame.Width = 98;
            // 
            // endFrame
            // 
            this.endFrame.Text = "End Frame";
            this.endFrame.Width = 92;
            // 
            // eventCodes
            // 
            this.eventCodes.Text = "Event Codes";
            this.eventCodes.Width = 172;
            // 
            // animationsButtonPanel
            // 
            this.animationsButtonPanel.Controls.Add(this.deleteAnim);
            this.animationsButtonPanel.Controls.Add(this.addAnim);
            this.animationsButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.animationsButtonPanel.Location = new System.Drawing.Point(3, 109);
            this.animationsButtonPanel.Name = "animationsButtonPanel";
            this.animationsButtonPanel.Size = new System.Drawing.Size(466, 31);
            this.animationsButtonPanel.TabIndex = 12;
            // 
            // deleteAnim
            // 
            this.deleteAnim.Location = new System.Drawing.Point(88, 5);
            this.deleteAnim.Name = "deleteAnim";
            this.deleteAnim.Size = new System.Drawing.Size(75, 23);
            this.deleteAnim.TabIndex = 3;
            this.deleteAnim.Text = "Delete";
            this.deleteAnim.UseVisualStyleBackColor = true;
            this.deleteAnim.Click += new System.EventHandler(this.deleteAnim_Click);
            // 
            // addAnim
            // 
            this.addAnim.Location = new System.Drawing.Point(5, 5);
            this.addAnim.Name = "addAnim";
            this.addAnim.Size = new System.Drawing.Size(75, 23);
            this.addAnim.TabIndex = 2;
            this.addAnim.Text = "Add";
            this.addAnim.UseVisualStyleBackColor = true;
            this.addAnim.Click += new System.EventHandler(this.addAnim_Click);
            // 
            // otherActionsTabPage
            // 
            this.otherActionsTabPage.Controls.Add(this.openBR2Button);
            this.otherActionsTabPage.Controls.Add(this.fpsLevel);
            this.otherActionsTabPage.Controls.Add(this.fpsTextBox);
            this.otherActionsTabPage.Controls.Add(this.concatenateNA2Button);
            this.otherActionsTabPage.Controls.Add(this.angleLabel);
            this.otherActionsTabPage.Controls.Add(this.axisLabel);
            this.otherActionsTabPage.Controls.Add(this.axisComboBox);
            this.otherActionsTabPage.Controls.Add(this.endTImeTextBoxLabel);
            this.otherActionsTabPage.Controls.Add(this.startTimeTextBoxLabel);
            this.otherActionsTabPage.Controls.Add(this.endTimeTextBox);
            this.otherActionsTabPage.Controls.Add(this.startTimeTextBox);
            this.otherActionsTabPage.Controls.Add(this.exportNA2Button);
            this.otherActionsTabPage.Controls.Add(this.rescaleBoneNameLabel);
            this.otherActionsTabPage.Controls.Add(this.rescaleNamedBoneButton);
            this.otherActionsTabPage.Controls.Add(this.bonesComboBox);
            this.otherActionsTabPage.Controls.Add(this.rescaleFactorLabel);
            this.otherActionsTabPage.Controls.Add(this.insertAdjustmentBoneButton);
            this.otherActionsTabPage.Controls.Add(this.angleTextBox);
            this.otherActionsTabPage.Controls.Add(this.rescaleFactorTextBox);
            this.otherActionsTabPage.Controls.Add(this.cleanFTSXMLButton);
            this.otherActionsTabPage.Controls.Add(this.removeAnimationsButton);
            this.otherActionsTabPage.Controls.Add(this.resaveAllFBXAsAnimsButton);
            this.otherActionsTabPage.Controls.Add(this.templateBR2OverwriteLabel);
            this.otherActionsTabPage.Controls.Add(this.useSceneTemplateRadioButton);
            this.otherActionsTabPage.Controls.Add(this.useLeaderTemplateRadioButton);
            this.otherActionsTabPage.Controls.Add(this.useUnitTemplateRadioButton);
            this.otherActionsTabPage.Controls.Add(this.resaveAllFilesInDirButton);
            this.otherActionsTabPage.Controls.Add(this.loadStringDatabaseButton);
            this.otherActionsTabPage.Controls.Add(this.exportNB2CurrentModelButton);
            this.otherActionsTabPage.Controls.Add(this.overwriteMeshesButton);
            this.otherActionsTabPage.Controls.Add(this.exportNB2Button);
            this.otherActionsTabPage.Location = new System.Drawing.Point(4, 25);
            this.otherActionsTabPage.Name = "otherActionsTabPage";
            this.otherActionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.otherActionsTabPage.Size = new System.Drawing.Size(484, 543);
            this.otherActionsTabPage.TabIndex = 1;
            this.otherActionsTabPage.Text = "Additional Actions";
            this.otherActionsTabPage.UseVisualStyleBackColor = true;
            // 
            // fpsLevel
            // 
            this.fpsLevel.AutoSize = true;
            this.fpsLevel.Location = new System.Drawing.Point(13, 197);
            this.fpsLevel.Name = "fpsLevel";
            this.fpsLevel.Size = new System.Drawing.Size(89, 16);
            this.fpsLevel.TabIndex = 44;
            this.fpsLevel.Text = "FPS Override";
            // 
            // fpsTextBox
            // 
            this.fpsTextBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.fpsTextBox.Location = new System.Drawing.Point(117, 194);
            this.fpsTextBox.Name = "fpsTextBox";
            this.fpsTextBox.Size = new System.Drawing.Size(139, 22);
            this.fpsTextBox.TabIndex = 43;
            this.fpsTextBox.Text = "60";
            // 
            // concatenateNA2Button
            // 
            this.concatenateNA2Button.Location = new System.Drawing.Point(6, 228);
            this.concatenateNA2Button.Name = "concatenateNA2Button";
            this.concatenateNA2Button.Size = new System.Drawing.Size(250, 40);
            this.concatenateNA2Button.TabIndex = 42;
            this.concatenateNA2Button.Text = "Concatenate All NA2 Files in Directory";
            this.concatenateNA2Button.UseVisualStyleBackColor = true;
            this.concatenateNA2Button.Click += new System.EventHandler(this.concatenateNA2ButtonClick);
            // 
            // angleLabel
            // 
            this.angleLabel.AutoSize = true;
            this.angleLabel.Location = new System.Drawing.Point(260, 387);
            this.angleLabel.Name = "angleLabel";
            this.angleLabel.Size = new System.Drawing.Size(96, 16);
            this.angleLabel.TabIndex = 41;
            this.angleLabel.Text = "Rotation Angle";
            // 
            // axisLabel
            // 
            this.axisLabel.AutoSize = true;
            this.axisLabel.Location = new System.Drawing.Point(260, 357);
            this.axisLabel.Name = "axisLabel";
            this.axisLabel.Size = new System.Drawing.Size(33, 16);
            this.axisLabel.TabIndex = 41;
            this.axisLabel.Text = "Axis";
            // 
            // axisComboBox
            // 
            this.axisComboBox.FormattingEnabled = true;
            this.axisComboBox.Items.AddRange(new object[] {
            "X (Left-Right)",
            "Y (Forward-Back)",
            "Z (Up-Down)"});
            this.axisComboBox.Location = new System.Drawing.Point(316, 354);
            this.axisComboBox.Name = "axisComboBox";
            this.axisComboBox.Size = new System.Drawing.Size(159, 24);
            this.axisComboBox.TabIndex = 40;
            this.axisComboBox.Text = "X (Left-Right)";
            // 
            // endTImeTextBoxLabel
            // 
            this.endTImeTextBoxLabel.AutoSize = true;
            this.endTImeTextBoxLabel.Location = new System.Drawing.Point(13, 170);
            this.endTImeTextBoxLabel.Name = "endTImeTextBoxLabel";
            this.endTImeTextBoxLabel.Size = new System.Drawing.Size(95, 16);
            this.endTImeTextBoxLabel.TabIndex = 39;
            this.endTImeTextBoxLabel.Text = "NA2 End Time";
            this.endTImeTextBoxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // startTimeTextBoxLabel
            // 
            this.startTimeTextBoxLabel.AutoSize = true;
            this.startTimeTextBoxLabel.Location = new System.Drawing.Point(13, 143);
            this.startTimeTextBoxLabel.Name = "startTimeTextBoxLabel";
            this.startTimeTextBoxLabel.Size = new System.Drawing.Size(98, 16);
            this.startTimeTextBoxLabel.TabIndex = 38;
            this.startTimeTextBoxLabel.Text = "NA2 Start Time";
            this.startTimeTextBoxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // endTimeTextBox
            // 
            this.endTimeTextBox.Location = new System.Drawing.Point(117, 167);
            this.endTimeTextBox.Name = "endTimeTextBox";
            this.endTimeTextBox.Size = new System.Drawing.Size(139, 22);
            this.endTimeTextBox.TabIndex = 37;
            // 
            // startTimeTextBox
            // 
            this.startTimeTextBox.Location = new System.Drawing.Point(117, 140);
            this.startTimeTextBox.Name = "startTimeTextBox";
            this.startTimeTextBox.Size = new System.Drawing.Size(139, 22);
            this.startTimeTextBox.TabIndex = 37;
            // 
            // exportNA2Button
            // 
            this.exportNA2Button.Location = new System.Drawing.Point(6, 94);
            this.exportNA2Button.Name = "exportNA2Button";
            this.exportNA2Button.Size = new System.Drawing.Size(250, 40);
            this.exportNA2Button.TabIndex = 36;
            this.exportNA2Button.Text = "Export Animation to NA2";
            this.exportNA2Button.UseVisualStyleBackColor = true;
            this.exportNA2Button.Click += new System.EventHandler(this.exportNA2ButtonClick);
            // 
            // rescaleBoneNameLabel
            // 
            this.rescaleBoneNameLabel.AutoSize = true;
            this.rescaleBoneNameLabel.Location = new System.Drawing.Point(174, 505);
            this.rescaleBoneNameLabel.Name = "rescaleBoneNameLabel";
            this.rescaleBoneNameLabel.Size = new System.Drawing.Size(82, 16);
            this.rescaleBoneNameLabel.TabIndex = 35;
            this.rescaleBoneNameLabel.Text = "Parent Bone";
            // 
            // rescaleNamedBoneButton
            // 
            this.rescaleNamedBoneButton.Location = new System.Drawing.Point(262, 458);
            this.rescaleNamedBoneButton.Name = "rescaleNamedBoneButton";
            this.rescaleNamedBoneButton.Size = new System.Drawing.Size(213, 38);
            this.rescaleNamedBoneButton.TabIndex = 34;
            this.rescaleNamedBoneButton.Text = "Add Scaled FX Bone and Save";
            this.rescaleNamedBoneButton.UseVisualStyleBackColor = true;
            this.rescaleNamedBoneButton.Click += new System.EventHandler(this.rescaleNamedBoneButtonClick);
            // 
            // bonesComboBox
            // 
            this.bonesComboBox.FormattingEnabled = true;
            this.bonesComboBox.Location = new System.Drawing.Point(262, 502);
            this.bonesComboBox.Name = "bonesComboBox";
            this.bonesComboBox.Size = new System.Drawing.Size(213, 24);
            this.bonesComboBox.TabIndex = 33;
            // 
            // rescaleFactorLabel
            // 
            this.rescaleFactorLabel.AutoSize = true;
            this.rescaleFactorLabel.Location = new System.Drawing.Point(260, 328);
            this.rescaleFactorLabel.Name = "rescaleFactorLabel";
            this.rescaleFactorLabel.Size = new System.Drawing.Size(100, 16);
            this.rescaleFactorLabel.TabIndex = 32;
            this.rescaleFactorLabel.Text = "Rescale Factor";
            // 
            // insertAdjustmentBoneButton
            // 
            this.insertAdjustmentBoneButton.Location = new System.Drawing.Point(262, 274);
            this.insertAdjustmentBoneButton.Name = "insertAdjustmentBoneButton";
            this.insertAdjustmentBoneButton.Size = new System.Drawing.Size(213, 40);
            this.insertAdjustmentBoneButton.TabIndex = 31;
            this.insertAdjustmentBoneButton.Text = "Insert Adjustment Bone and Save";
            this.insertAdjustmentBoneButton.UseVisualStyleBackColor = true;
            this.insertAdjustmentBoneButton.Click += new System.EventHandler(this.rescaleButtonClick);
            // 
            // angleTextBox
            // 
            this.angleTextBox.Location = new System.Drawing.Point(366, 384);
            this.angleTextBox.Name = "angleTextBox";
            this.angleTextBox.Size = new System.Drawing.Size(109, 22);
            this.angleTextBox.TabIndex = 30;
            this.angleTextBox.Text = "0";
            // 
            // rescaleFactorTextBox
            // 
            this.rescaleFactorTextBox.Location = new System.Drawing.Point(366, 325);
            this.rescaleFactorTextBox.Name = "rescaleFactorTextBox";
            this.rescaleFactorTextBox.Size = new System.Drawing.Size(109, 22);
            this.rescaleFactorTextBox.TabIndex = 30;
            this.rescaleFactorTextBox.Text = "1";
            // 
            // cleanFTSXMLButton
            // 
            this.cleanFTSXMLButton.Location = new System.Drawing.Point(262, 186);
            this.cleanFTSXMLButton.Name = "cleanFTSXMLButton";
            this.cleanFTSXMLButton.Size = new System.Drawing.Size(213, 38);
            this.cleanFTSXMLButton.TabIndex = 29;
            this.cleanFTSXMLButton.Text = "Reorder FTSXML Triggers";
            this.cleanFTSXMLButton.UseVisualStyleBackColor = true;
            this.cleanFTSXMLButton.Click += new System.EventHandler(this.cleanFTSXMLButtonClick);
            // 
            // removeAnimationsButton
            // 
            this.removeAnimationsButton.Location = new System.Drawing.Point(262, 140);
            this.removeAnimationsButton.Name = "removeAnimationsButton";
            this.removeAnimationsButton.Size = new System.Drawing.Size(213, 40);
            this.removeAnimationsButton.TabIndex = 28;
            this.removeAnimationsButton.Text = "Remove Animations and Save";
            this.removeAnimationsButton.UseVisualStyleBackColor = true;
            this.removeAnimationsButton.Click += new System.EventHandler(this.removeAnimationsButtonClick);
            // 
            // resaveAllFBXAsAnimsButton
            // 
            this.resaveAllFBXAsAnimsButton.Location = new System.Drawing.Point(262, 94);
            this.resaveAllFBXAsAnimsButton.Name = "resaveAllFBXAsAnimsButton";
            this.resaveAllFBXAsAnimsButton.Size = new System.Drawing.Size(213, 40);
            this.resaveAllFBXAsAnimsButton.TabIndex = 27;
            this.resaveAllFBXAsAnimsButton.Text = "Convert all FBX Files in Directory to GR2 Animations";
            this.resaveAllFBXAsAnimsButton.UseVisualStyleBackColor = true;
            this.resaveAllFBXAsAnimsButton.Click += new System.EventHandler(this.resaveAllFBXAsAnimsButtonClick);
            // 
            // templateBR2OverwriteLabel
            // 
            this.templateBR2OverwriteLabel.AutoSize = true;
            this.templateBR2OverwriteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.3125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.templateBR2OverwriteLabel.Location = new System.Drawing.Point(8, 374);
            this.templateBR2OverwriteLabel.Name = "templateBR2OverwriteLabel";
            this.templateBR2OverwriteLabel.Size = new System.Drawing.Size(198, 16);
            this.templateBR2OverwriteLabel.TabIndex = 24;
            this.templateBR2OverwriteLabel.Text = "Template for BR2 Overwrite";
            // 
            // useSceneTemplateRadioButton
            // 
            this.useSceneTemplateRadioButton.AutoSize = true;
            this.useSceneTemplateRadioButton.Location = new System.Drawing.Point(29, 452);
            this.useSceneTemplateRadioButton.Name = "useSceneTemplateRadioButton";
            this.useSceneTemplateRadioButton.Size = new System.Drawing.Size(157, 20);
            this.useSceneTemplateRadioButton.TabIndex = 23;
            this.useSceneTemplateRadioButton.TabStop = true;
            this.useSceneTemplateRadioButton.Text = "Use Scene Template";
            this.useSceneTemplateRadioButton.UseVisualStyleBackColor = true;
            // 
            // useLeaderTemplateRadioButton
            // 
            this.useLeaderTemplateRadioButton.AutoSize = true;
            this.useLeaderTemplateRadioButton.Location = new System.Drawing.Point(29, 426);
            this.useLeaderTemplateRadioButton.Name = "useLeaderTemplateRadioButton";
            this.useLeaderTemplateRadioButton.Size = new System.Drawing.Size(161, 20);
            this.useLeaderTemplateRadioButton.TabIndex = 22;
            this.useLeaderTemplateRadioButton.TabStop = true;
            this.useLeaderTemplateRadioButton.Text = "Use Leader Template";
            this.useLeaderTemplateRadioButton.UseVisualStyleBackColor = true;
            // 
            // useUnitTemplateRadioButton
            // 
            this.useUnitTemplateRadioButton.AutoSize = true;
            this.useUnitTemplateRadioButton.Checked = true;
            this.useUnitTemplateRadioButton.Location = new System.Drawing.Point(29, 400);
            this.useUnitTemplateRadioButton.Name = "useUnitTemplateRadioButton";
            this.useUnitTemplateRadioButton.Size = new System.Drawing.Size(141, 20);
            this.useUnitTemplateRadioButton.TabIndex = 21;
            this.useUnitTemplateRadioButton.TabStop = true;
            this.useUnitTemplateRadioButton.Text = "Use Unit Template";
            this.useUnitTemplateRadioButton.UseVisualStyleBackColor = true;
            // 
            // resaveAllFilesInDirButton
            // 
            this.resaveAllFilesInDirButton.Location = new System.Drawing.Point(262, 50);
            this.resaveAllFilesInDirButton.Name = "resaveAllFilesInDirButton";
            this.resaveAllFilesInDirButton.Size = new System.Drawing.Size(213, 40);
            this.resaveAllFilesInDirButton.TabIndex = 18;
            this.resaveAllFilesInDirButton.Text = "Resave All GR2 Files in Directory";
            this.resaveAllFilesInDirButton.UseVisualStyleBackColor = true;
            this.resaveAllFilesInDirButton.Click += new System.EventHandler(this.resaveAllFilesInDirButtonClick);
            // 
            // loadStringDatabaseButton
            // 
            this.loadStringDatabaseButton.Location = new System.Drawing.Point(262, 6);
            this.loadStringDatabaseButton.Name = "loadStringDatabaseButton";
            this.loadStringDatabaseButton.Size = new System.Drawing.Size(213, 40);
            this.loadStringDatabaseButton.TabIndex = 17;
            this.loadStringDatabaseButton.Text = "Load String Database";
            this.loadStringDatabaseButton.UseVisualStyleBackColor = true;
            this.loadStringDatabaseButton.Click += new System.EventHandler(this.loadStringDatabaseButtonClick);
            // 
            // exportNB2CurrentModelButton
            // 
            this.exportNB2CurrentModelButton.Location = new System.Drawing.Point(6, 50);
            this.exportNB2CurrentModelButton.Name = "exportNB2CurrentModelButton";
            this.exportNB2CurrentModelButton.Size = new System.Drawing.Size(250, 40);
            this.exportNB2CurrentModelButton.TabIndex = 15;
            this.exportNB2CurrentModelButton.Text = "Export to NB2 (Current Model)";
            this.exportNB2CurrentModelButton.UseVisualStyleBackColor = true;
            this.exportNB2CurrentModelButton.Click += new System.EventHandler(this.exportNB2CurrentModelButtonClick);
            // 
            // overwriteMeshesButton
            // 
            this.overwriteMeshesButton.Location = new System.Drawing.Point(6, 320);
            this.overwriteMeshesButton.Name = "overwriteMeshesButton";
            this.overwriteMeshesButton.Size = new System.Drawing.Size(250, 40);
            this.overwriteMeshesButton.TabIndex = 14;
            this.overwriteMeshesButton.Text = "Overwrite Meshes From BR2 (Current Model)";
            this.overwriteMeshesButton.UseVisualStyleBackColor = true;
            this.overwriteMeshesButton.Click += new System.EventHandler(this.overwriteMeshesButtonClick);
            // 
            // exportNB2Button
            // 
            this.exportNB2Button.Location = new System.Drawing.Point(6, 6);
            this.exportNB2Button.Name = "exportNB2Button";
            this.exportNB2Button.Size = new System.Drawing.Size(250, 40);
            this.exportNB2Button.TabIndex = 11;
            this.exportNB2Button.Text = "Export to NB2 (All Models)";
            this.exportNB2Button.UseVisualStyleBackColor = true;
            this.exportNB2Button.Click += new System.EventHandler(this.exportNB2ButtonClick);
            // 
            // selectModelTabPage
            // 
            this.selectModelTabPage.Controls.Add(this.modelList);
            this.selectModelTabPage.Location = new System.Drawing.Point(4, 25);
            this.selectModelTabPage.Name = "selectModelTabPage";
            this.selectModelTabPage.Size = new System.Drawing.Size(484, 543);
            this.selectModelTabPage.TabIndex = 2;
            this.selectModelTabPage.Text = "Select Model";
            this.selectModelTabPage.UseVisualStyleBackColor = true;
            // 
            // modelList
            // 
            this.modelList.CheckBoxes = true;
            this.modelList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.modelName});
            this.modelList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelList.FullRowSelect = true;
            this.modelList.GridLines = true;
            this.modelList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.modelList.Location = new System.Drawing.Point(0, 0);
            this.modelList.MultiSelect = false;
            this.modelList.Name = "modelList";
            this.modelList.Size = new System.Drawing.Size(484, 543);
            this.modelList.TabIndex = 0;
            this.modelList.UseCompatibleStateImageBehavior = false;
            this.modelList.View = System.Windows.Forms.View.Details;
            this.modelList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.modelListCheckChanged);
            // 
            // modelName
            // 
            this.modelName.Text = "Model Name";
            this.modelName.Width = 300;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.headerFilenameLabel);
            this.panel1.Controls.Add(this.viewButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 42);
            this.panel1.TabIndex = 8;
            // 
            // headerFilenameLabel
            // 
            this.headerFilenameLabel.AutoSize = true;
            this.headerFilenameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerFilenameLabel.Location = new System.Drawing.Point(3, 12);
            this.headerFilenameLabel.Name = "headerFilenameLabel";
            this.headerFilenameLabel.Size = new System.Drawing.Size(118, 22);
            this.headerFilenameLabel.TabIndex = 3;
            this.headerFilenameLabel.Text = "No file open";
            // 
            // viewButton
            // 
            this.viewButton.Location = new System.Drawing.Point(412, 3);
            this.viewButton.Name = "viewButton";
            this.viewButton.Size = new System.Drawing.Size(97, 35);
            this.viewButton.TabIndex = 6;
            this.viewButton.Text = "View";
            this.viewButtonToolTip.SetToolTip(this.viewButton, "View .gr2 in Granny Viewer");
            this.viewButton.UseVisualStyleBackColor = true;
            this.viewButton.Click += new System.EventHandler(this.viewButtonClick);
            // 
            // fileInfoGroupBox
            // 
            this.fileInfoGroupBox.Controls.Add(this.fileInfoTextBox);
            this.fileInfoGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.fileInfoGroupBox.Location = new System.Drawing.Point(0, 46);
            this.fileInfoGroupBox.Name = "fileInfoGroupBox";
            this.fileInfoGroupBox.Size = new System.Drawing.Size(512, 159);
            this.fileInfoGroupBox.TabIndex = 2;
            this.fileInfoGroupBox.TabStop = false;
            this.fileInfoGroupBox.Text = "Current File Info";
            // 
            // fileInfoTextBox
            // 
            this.fileInfoTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileInfoTextBox.Enabled = false;
            this.fileInfoTextBox.Location = new System.Drawing.Point(3, 18);
            this.fileInfoTextBox.Name = "fileInfoTextBox";
            this.fileInfoTextBox.Size = new System.Drawing.Size(506, 138);
            this.fileInfoTextBox.TabIndex = 0;
            this.fileInfoTextBox.Text = "";
            // 
            // properties
            // 
            this.properties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.properties.Location = new System.Drawing.Point(0, 205);
            this.properties.Name = "properties";
            this.properties.Size = new System.Drawing.Size(512, 413);
            this.properties.TabIndex = 0;
            this.properties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.properties_PropertyValueChanged);
            // 
            // openBR2Button
            // 
            this.openBR2Button.Location = new System.Drawing.Point(6, 274);
            this.openBR2Button.Name = "openBR2Button";
            this.openBR2Button.Size = new System.Drawing.Size(250, 40);
            this.openBR2Button.TabIndex = 45;
            this.openBR2Button.Text = "Open BR2";
            this.openBR2Button.UseVisualStyleBackColor = true;
            this.openBR2Button.Click += new System.EventHandler(this.openBR2ButtonClick);
            // 
            // animation
            // 
            this.animation.Tag = "";
            this.animation.Text = "Animation";
            this.animation.Width = 149;
            // 
            // duration
            // 
            this.duration.Text = "Duration";
            this.duration.Width = 94;
            // 
            // simpleShaderToolStripMenuItem
            // 
            this.simpleShaderToolStripMenuItem.Name = "simpleShaderToolStripMenuItem";
            this.simpleShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.simpleShaderToolStripMenuItem.Text = "SimpleShader";
            this.simpleShaderToolStripMenuItem.Click += new System.EventHandler(this.simpleShaderToolStripMenuItem_Click);
            // 
            // buildingShaderToolStripMenuItem
            // 
            this.buildingShaderToolStripMenuItem.Name = "buildingShaderToolStripMenuItem";
            this.buildingShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.buildingShaderToolStripMenuItem.Text = "BuildingShader";
            this.buildingShaderToolStripMenuItem.Click += new System.EventHandler(this.buildingShaderToolStripMenuItem_Click);
            // 
            // landmarkShaderToolStripMenuItem
            // 
            this.landmarkShaderToolStripMenuItem.Name = "landmarkShaderToolStripMenuItem";
            this.landmarkShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.landmarkShaderToolStripMenuItem.Text = "LandmarkShader";
            this.landmarkShaderToolStripMenuItem.Click += new System.EventHandler(this.landmarkShaderToolStripMenuItem_Click);
            // 
            // landmarkShaderStencilToolStripMenuItem
            // 
            this.landmarkShaderStencilToolStripMenuItem.Name = "landmarkShaderStencilToolStripMenuItem";
            this.landmarkShaderStencilToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.landmarkShaderStencilToolStripMenuItem.Text = "LandmarkShader_Stencil";
            this.landmarkShaderStencilToolStripMenuItem.Click += new System.EventHandler(this.landmarkShaderStencilToolStripMenuItem_Click);
            // 
            // unitShaderSkinnedToolStripMenuItem
            // 
            this.unitShaderSkinnedToolStripMenuItem.Name = "unitShaderSkinnedToolStripMenuItem";
            this.unitShaderSkinnedToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.unitShaderSkinnedToolStripMenuItem.Text = "UnitShader_Skinned";
            this.unitShaderSkinnedToolStripMenuItem.Click += new System.EventHandler(this.unitShaderSkinnedToolStripMenuItem_Click);
            // 
            // leaderShaderToolStripMenuItem
            // 
            this.leaderShaderToolStripMenuItem.Name = "leaderShaderToolStripMenuItem";
            this.leaderShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderShaderToolStripMenuItem.Text = "Leader";
            this.leaderShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderShaderToolStripMenuItem_Click);
            // 
            // leaderFurShaderToolStripMenuItem
            // 
            this.leaderFurShaderToolStripMenuItem.Name = "leaderFurShaderToolStripMenuItem";
            this.leaderFurShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderFurShaderToolStripMenuItem.Text = "Leader_Fur";
            this.leaderFurShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderFurShaderToolStripMenuItem_Click);
            // 
            // leaderOpaqueClothShaderToolStripMenuItem
            // 
            this.leaderOpaqueClothShaderToolStripMenuItem.Name = "leaderOpaqueClothShaderToolStripMenuItem";
            this.leaderOpaqueClothShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderOpaqueClothShaderToolStripMenuItem.Text = "Leader_Opaque_Cloth";
            this.leaderOpaqueClothShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderOpaqueClothShaderToolStripMenuItem_Click);
            // 
            // leaderSkinShaderToolStripMenuItem
            // 
            this.leaderSkinShaderToolStripMenuItem.Name = "leaderSkinShaderToolStripMenuItem";
            this.leaderSkinShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderSkinShaderToolStripMenuItem.Text = "Leader_Skin";
            this.leaderSkinShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderSkinShaderToolStripMenuItem_Click);
            // 
            // leaderTransparencyShaderToolStripMenuItem
            // 
            this.leaderTransparencyShaderToolStripMenuItem.Name = "leaderTransparencyShaderToolStripMenuItem";
            this.leaderTransparencyShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderTransparencyShaderToolStripMenuItem.Text = "Leader_Transparency";
            this.leaderTransparencyShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderTransparencyShaderToolStripMenuItem_Click);
            // 
            // leaderHairShaderToolStripMenuItem
            // 
            this.leaderHairShaderToolStripMenuItem.Name = "leaderHairShaderToolStripMenuItem";
            this.leaderHairShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderHairShaderToolStripMenuItem.Text = "Leader_Hair";
            this.leaderHairShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderHairShaderToolStripMenuItem_Click);
            // 
            // leaderOpaqueHairShaderToolStripMenuItem
            // 
            this.leaderOpaqueHairShaderToolStripMenuItem.Name = "leaderOpaqueHairShaderToolStripMenuItem";
            this.leaderOpaqueHairShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderOpaqueHairShaderToolStripMenuItem.Text = "Leader_Opaque_Hair";
            this.leaderOpaqueHairShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderOpaqueHairShaderToolStripMenuItem_Click);
            // 
            // leaderFurFinShaderToolStripMenuItem
            // 
            this.leaderFurFinShaderToolStripMenuItem.Name = "leaderFurFinShaderToolStripMenuItem";
            this.leaderFurFinShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderFurFinShaderToolStripMenuItem.Text = "Leader_Fur_Fin";
            this.leaderFurFinShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderFurFinShaderToolStripMenuItem_Click);
            // 
            // leaderGlassShaderToolStripMenuItem
            // 
            this.leaderGlassShaderToolStripMenuItem.Name = "leaderGlassShaderToolStripMenuItem";
            this.leaderGlassShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderGlassShaderToolStripMenuItem.Text = "Leader_Glass";
            this.leaderGlassShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderGlassShaderToolStripMenuItem_Click);
            // 
            // leaderMaskedShaderToolStripMenuItem
            // 
            this.leaderMaskedShaderToolStripMenuItem.Name = "leaderMaskedShaderToolStripMenuItem";
            this.leaderMaskedShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderMaskedShaderToolStripMenuItem.Text = "Leader_Masked";
            this.leaderMaskedShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderMaskedShaderToolStripMenuItem_Click);
            // 
            // leaderMaskedHairShaderToolStripMenuItem
            // 
            this.leaderMaskedHairShaderToolStripMenuItem.Name = "leaderMaskedHairShaderToolStripMenuItem";
            this.leaderMaskedHairShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderMaskedHairShaderToolStripMenuItem.Text = "Leader_Masked_Hair";
            this.leaderMaskedHairShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderMaskedHairShaderToolStripMenuItem_Click);
            // 
            // leaderVelvetShaderToolStripMenuItem
            // 
            this.leaderVelvetShaderToolStripMenuItem.Name = "leaderVelvetShaderToolStripMenuItem";
            this.leaderVelvetShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderVelvetShaderToolStripMenuItem.Text = "Leader_Velvet";
            this.leaderVelvetShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderVelvetShaderToolStripMenuItem_Click);
            // 
            // leaderOpaqueMatteShaderToolStripMenuItem
            // 
            this.leaderOpaqueMatteShaderToolStripMenuItem.Name = "leaderOpaqueMatteShaderToolStripMenuItem";
            this.leaderOpaqueMatteShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderOpaqueMatteShaderToolStripMenuItem.Text = "Leader_Opaque_Matte";
            this.leaderOpaqueMatteShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderOpaqueMatteShaderToolStripMenuItem_Click);
            // 
            // leaderTransparentMatteShaderToolStripMenuItem
            // 
            this.leaderTransparentMatteShaderToolStripMenuItem.Name = "leaderTransparentMatteShaderToolStripMenuItem";
            this.leaderTransparentMatteShaderToolStripMenuItem.Size = new System.Drawing.Size(271, 28);
            this.leaderTransparentMatteShaderToolStripMenuItem.Text = "Leader_Transparent_Matte";
            this.leaderTransparentMatteShaderToolStripMenuItem.Click += new System.EventHandler(this.leaderTransparentMatteShaderToolStripMenuItem_Click);
            // 
            // addMaterialList
            // 
            this.addMaterialList.ImageScalingSize = new System.Drawing.Size(21, 21);
            this.addMaterialList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildingShaderToolStripMenuItem,
            this.landmarkShaderToolStripMenuItem,
            this.landmarkShaderStencilToolStripMenuItem,
            this.simpleShaderToolStripMenuItem,
            this.unitShaderSkinnedToolStripMenuItem,
            this.leaderShaderToolStripMenuItem,
            this.leaderSkinShaderToolStripMenuItem,
            this.leaderHairShaderToolStripMenuItem,
            this.leaderOpaqueHairShaderToolStripMenuItem,
            this.leaderFurShaderToolStripMenuItem,
            this.leaderFurFinShaderToolStripMenuItem,
            this.leaderOpaqueClothShaderToolStripMenuItem,
            this.leaderGlassShaderToolStripMenuItem,
            this.leaderTransparencyShaderToolStripMenuItem,
            this.leaderVelvetShaderToolStripMenuItem,
            this.leaderMaskedShaderToolStripMenuItem,
            this.leaderMaskedHairShaderToolStripMenuItem,
            this.leaderOpaqueMatteShaderToolStripMenuItem,
            this.leaderTransparentMatteShaderToolStripMenuItem});
            this.addMaterialList.Name = "addMaterialList";
            this.addMaterialList.Size = new System.Drawing.Size(272, 536);
            // 
            // materialTypeColumnHeader
            // 
            this.materialTypeColumnHeader.Text = "Material Type";
            this.materialTypeColumnHeader.Width = 138;
            // 
            // materialNameColumnHeader
            // 
            this.materialNameColumnHeader.Tag = "";
            this.materialNameColumnHeader.Text = "Material Name";
            this.materialNameColumnHeader.Width = 149;
            // 
            // br2ImportButton
            // 
            this.br2ImportButton.Location = new System.Drawing.Point(0, 0);
            this.br2ImportButton.Name = "br2ImportButton";
            this.br2ImportButton.Size = new System.Drawing.Size(75, 23);
            this.br2ImportButton.TabIndex = 0;
            // 
            // meshList
            // 
            this.meshList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.MeshName,
            this.Material});
            this.meshList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meshList.FullRowSelect = true;
            this.meshList.GridLines = true;
            this.meshList.Location = new System.Drawing.Point(0, 0);
            this.meshList.MultiSelect = false;
            this.meshList.Name = "meshList";
            this.meshList.Size = new System.Drawing.Size(480, 195);
            this.meshList.TabIndex = 2;
            this.meshList.UseCompatibleStateImageBehavior = false;
            this.meshList.View = System.Windows.Forms.View.Details;
            this.meshList.SelectedIndexChanged += new System.EventHandler(this.meshList_SelectedIndexChanged);
            // 
            // MeshName
            // 
            this.MeshName.Tag = "Mesh Name";
            this.MeshName.Text = "Mesh Name";
            this.MeshName.Width = 180;
            // 
            // Material
            // 
            this.Material.Text = "Material";
            this.Material.Width = 200;
            // 
            // NexusBuddyApplicationForm
            // 
            this.ClientSize = new System.Drawing.Size(1008, 618);
            this.Controls.Add(this.masterSplitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NexusBuddyApplicationForm";
            this.Text = "Nexus Buddy " + getVersionString() + " - Granny Editor for Civilization 5 and Beyond Earth";
            this.masterSplitContainer.Panel1.ResumeLayout(false);
            this.masterSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.masterSplitContainer)).EndInit();
            this.masterSplitContainer.ResumeLayout(false);
            this.leftHandSplitContainer.Panel1.ResumeLayout(false);
            this.leftHandSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.leftHandSplitContainer)).EndInit();
            this.leftHandSplitContainer.ResumeLayout(false);
            this.mainButtonPanel.ResumeLayout(false);
            this.mainTabControl.ResumeLayout(false);
            this.grannyFileTabPage.ResumeLayout(false);
            this.grannyFileSplitContainerA.Panel1.ResumeLayout(false);
            this.grannyFileSplitContainerA.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grannyFileSplitContainerA)).EndInit();
            this.grannyFileSplitContainerA.ResumeLayout(false);
            this.grannyFileTabContainer.Panel1.ResumeLayout(false);
            this.grannyFileTabContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grannyFileTabContainer)).EndInit();
            this.grannyFileTabContainer.ResumeLayout(false);
            this.materialButtonsPanel.ResumeLayout(false);
            this.animationsTabControl.ResumeLayout(false);
            this.grannyAnimsTabPage.ResumeLayout(false);
            this.animationDefsTabPage.ResumeLayout(false);
            this.animationsButtonPanel.ResumeLayout(false);
            this.otherActionsTabPage.ResumeLayout(false);
            this.otherActionsTabPage.PerformLayout();
            this.selectModelTabPage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.fileInfoGroupBox.ResumeLayout(false);
            this.addMaterialList.ResumeLayout(false);
            this.ResumeLayout(false);

		}

        private void modelListCheckChanged(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            int preCurrentModelIndex = currentModelIndex;

            // if we have the lastItem set as checked, and it is different
            // item than the one that fired the event, uncheck it
            if (lastModelListItemChecked != null && lastModelListItemChecked.Checked
                && lastModelListItemChecked != modelList.Items[e.Index])
            {
                // uncheck the last item and store the new one
                lastModelListItemChecked.Checked = false;
            }

            // store current item
            lastModelListItemChecked = modelList.Items[e.Index];

            currentModelIndex = lastModelListItemChecked.Index;

            if (preCurrentModelIndex != currentModelIndex)
            {
                this.refreshAppData();
            }
        }
        
    }
}