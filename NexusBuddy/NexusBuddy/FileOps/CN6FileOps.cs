using System.Collections.Generic;
using System.IO;
using Firaxis.Framework.Granny;
using NexusBuddy.GrannyInfos;
using NexusBuddy.GrannyWrappers;
using NexusBuddy.Utils;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Windows.Forms;
using System.Linq;

namespace NexusBuddy.FileOps
{
    class CN6FileOps
    {
        public static void batchExport(GrannyContext grannyContext, string unitListFile)
        {
            StreamReader streamReader = new StreamReader(unitListFile);

            string directory = Path.GetDirectoryName(unitListFile);

            string regexString = "(.+);(.*);(.*);(.*)";

            string outFilename = unitListFile.Replace(".dat", "_new.dat");
            StreamWriter outputWriter = new StreamWriter(new FileStream(outFilename, FileMode.Create));

            while (!streamReader.EndOfStream)
            {
                string currentLine = streamReader.ReadLine();

                Regex regex = new Regex(regexString);
                MatchCollection mc = regex.Matches(currentLine);
                foreach (Match m in mc)
                {
                    string gr2Filename = m.Groups[1].Value.Trim().ToLower();
                    IGrannyFile grannyFile = grannyContext.LoadGrannyFile(directory + "\\" + gr2Filename);
                    NexusBuddyApplicationForm.loadedFile = grannyFile;
                    NexusBuddyApplicationForm.form.refreshAppData();
                    List<string> fileTextureMaps = exportAllModelsToCN6(grannyFile, true);

                    string animationFilenames = m.Groups[2].Value.Trim();
                    string textureFilenames = m.Groups[3].Value.Trim();
                    string prettyName = m.Groups[4].Value.Trim();

                    var fileTextureSet = new HashSet<string>(fileTextureMaps);

                    if (String.IsNullOrEmpty(textureFilenames))
                    {
                        textureFilenames = string.Join(",", fileTextureSet);
                    }

                    string newline = gr2Filename + ";" + animationFilenames + ";" + textureFilenames + ";" + prettyName;

                    outputWriter.WriteLine(newline);
                }
            }
            streamReader.Close();
            outputWriter.Close();
            File.Delete(unitListFile);
            File.Move(outFilename, unitListFile);
        }

        public static List<string> exportAllModelsToCN6(IGrannyFile grannyFile, bool isBatch)
        {
            List<string> fileTextureMaps = new List<string>();

            for (int i = 0; i < grannyFile.Models.Count; i++)
            {
                List<string> textureMaps = cn6Export(grannyFile, i, isBatch);
                fileTextureMaps.AddRange(textureMaps);
            }

            return fileTextureMaps;
        }

        public static string GetSafeFilename(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
        }

        public static List<string> cn6Export(IGrannyFile grannyFile, int currentModelIndex, bool isBatch)
        {
            IGrannyModel model = grannyFile.Models[currentModelIndex];

            WriteModelToCN6Response response = WriteModelToCN6(grannyFile, isBatch, model, null);

            foreach (string decalMeshName in response.decalMeshNames)
            {
                WriteModelToCN6(grannyFile, isBatch, model, decalMeshName);
            }

            return response.textureMaps;
        }

        private static WriteModelToCN6Response WriteModelToCN6(IGrannyFile grannyFile, bool isBatch, IGrannyModel model, string filterMeshName)
        {
            List<string> textureMaps = new List<string>();

            string fileExtension = ".cn6";
            string outputFilename = "";
            string numberFormat = "f8";
            List<string> decalMeshNames = new List<string>();

            IGrannySkeleton skeleton = model.Skeleton;

            string decalMeshName = "";
            if (filterMeshName != null)
            {
                decalMeshName = "_DCL_" + GetSafeFilename(filterMeshName);
            }
            
            if (grannyFile.Models.Count > 1)
            {
                string modelFilename = "__" + model.Name + decalMeshName + fileExtension;
                outputFilename = grannyFile.Filename.Replace(".gr2", modelFilename);
                outputFilename = outputFilename.Replace(".GR2", modelFilename);
            }
            else
            {
                outputFilename = grannyFile.Filename.Replace(".gr2", fileExtension);
                outputFilename = outputFilename.Replace(".GR2", fileExtension);
            }

            if (isBatch)
            {
                outputFilename = outputFilename.Replace(fileExtension, "_batch" + fileExtension);
            }

            string directory = Path.GetDirectoryName(outputFilename);
            string filename = Path.GetFileName(outputFilename);
            outputFilename = directory + "\\" + GetSafeFilename(filename);

            StreamWriter outputWriter = new StreamWriter(new FileStream(outputFilename, FileMode.Create));

            // Lookup so we can identify the meshes belonging the current model in the list of file meshes
            Dictionary<int, int> meshBindingToMesh = new Dictionary<int, int>();
            HashSet<string> distinctMeshNames = new HashSet<string>();
            for (int i = 0; i < model.MeshBindings.Count; i++)
            {
                for (int j = 0; j < grannyFile.Meshes.Count; j++)
                {
                    GrannyMeshWrapper modelMesh = new GrannyMeshWrapper(model.MeshBindings[i]);
                    IGrannyMesh fileMesh = grannyFile.Meshes[j];
                    if (modelMesh.meshEqual(fileMesh))
                    {
                        meshBindingToMesh.Add(i, j);
                    }
                }
                distinctMeshNames.Add(model.MeshBindings[i].Name);
            }

            // Used to give meshes distinct names where we have multiple meshes with the same name in our source gr2
            Dictionary<string, int> meshNameCount = new Dictionary<string, int>();
            foreach (string meshName in distinctMeshNames)
            {
                meshNameCount.Add(meshName, 0);
            }

            List<GrannyMeshInfo> grannyMeshInfos = new List<GrannyMeshInfo>();
            for (int i = 0; i < model.MeshBindings.Count; i++)
            {
                GrannyMeshWrapper meshWrapper = new GrannyMeshWrapper(model.MeshBindings[i]);
                grannyMeshInfos.Add(meshWrapper.getMeshInfo());
            }

            BiLookup<int, string> boneLookup = new BiLookup<int, string>();
            for (int i = 0; i < skeleton.Bones.Count; i++)
            {
                boneLookup.Add(i, skeleton.Bones[i].Name);
            }

            Dictionary<string, double[]> boneNameToPositionMap = new Dictionary<string, double[]>();
            foreach (IGrannyBone bone in skeleton.Bones)
            {
                if (!boneNameToPositionMap.ContainsKey(bone.Name))
                {
                    double[] bonePosition = NB2Exporter.getBoneWorldPosition(bone);
                    boneNameToPositionMap.Add(bone.Name, bonePosition);
                }
            }

            string headerLine = "// CivNexus6 CN6 - Exported from Nexus Buddy " + NexusBuddyApplicationForm.getVersionString();
            outputWriter.WriteLine(headerLine);
            outputWriter.WriteLine("skeleton");

            // Write Bones
            for (int boneIndex = 0; boneIndex < skeleton.Bones.Count; boneIndex++)
            {
                IGrannyBone bone = skeleton.Bones[boneIndex];
                string boneName = bone.Name;
                IGrannyTransform transform = bone.LocalTransform;
                float[] orientation = transform.Orientation;
                float[] position = transform.Position;
                float[] invWorldTransform = bone.InverseWorldTransform;

                StringBuilder boneStringBuilder = new StringBuilder(boneIndex + " \"" + boneName + "\" " + bone.ParentIndex + " " + position[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + position[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + position[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                                orientation[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + orientation[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + orientation[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + orientation[3].ToString(numberFormat, CultureInfo.InvariantCulture));

                foreach (float j in invWorldTransform)
                {
                    boneStringBuilder.Append(" " + j.ToString(numberFormat, CultureInfo.InvariantCulture));
                }

                outputWriter.WriteLine(boneStringBuilder);
            }


            List<IndieMaterial> modelMaterials = new List<IndieMaterial>();
            foreach (IGrannyMesh mesh in model.MeshBindings)
            {
                IGrannyMaterial material = mesh.MaterialBindings[0];
                foreach (ListViewItem materialListItem in NexusBuddyApplicationForm.form.materialList.Items)
                {
                    IndieMaterial shader = (IndieMaterial)materialListItem.Tag;
                    GrannyMaterialWrapper matWrap1 = new GrannyMaterialWrapper(shader.GetMaterial());
                    GrannyMaterialWrapper matWrap2 = new GrannyMaterialWrapper(material);
                    if (matWrap1.getName().Equals(matWrap2.getName()) && !modelMaterials.Contains(shader))
                    {
                        modelMaterials.Add(shader);
                    }
                }
            }

            int meshCount = 0;
            for (int mi = 0; mi < grannyMeshInfos.Count; mi++)
            {
                IGrannyMesh mesh = model.MeshBindings[mi];
                string meshName = model.MeshBindings[mi].Name;
                GrannyMeshWrapper meshWrapper = new GrannyMeshWrapper(mesh);
                GrannyMeshInfo meshInfo = meshWrapper.getMeshInfo();

                bool isFlatMesh = true;
                foreach (GrannyVertexInfo vertexInfo in meshInfo.vertices)
                {
                    float zPos = vertexInfo.position[2];
                    bool isZero = NumberUtils.almostEquals(zPos, 0.0f, 4);
                    if (!isZero)
                    {
                        isFlatMesh = false;
                        break;
                    }
                }

                if (filterMeshName == null)
                {
                    if (isFlatMesh)
                    {
                        decalMeshNames.Add(meshName);
                    }
                    else
                    {
                        meshCount++;
                    }
                }
                else
                {
                    meshCount = 1;
                }
            }

            if (meshCount == 0)
            {
                outputWriter.Close();
                File.Delete(outputFilename);
                return new WriteModelToCN6Response(decalMeshNames, textureMaps);
            }

            // Write Meshes
            outputWriter.WriteLine("meshes:" + meshCount);
            
            for (int mi = 0; mi < grannyMeshInfos.Count; mi++)
            {
                GrannyMeshInfo grannyMeshInfo = grannyMeshInfos[mi];
                string meshName = model.MeshBindings[mi].Name;

                if ((filterMeshName == null && !decalMeshNames.Contains(meshName)) || meshName.Equals(filterMeshName))
                {
                    meshNameCount[meshName]++;
                    if (meshNameCount[meshName] > 1)
                    {
                        meshName += meshNameCount[meshName];
                    }

                    outputWriter.WriteLine("mesh:\"" + meshName + "\"");

                    // Write Materials
                    outputWriter.WriteLine("materials");
                    foreach (IGrannyMaterial material in model.MeshBindings[mi].MaterialBindings)
                    {
                        IndieMaterial shader = NexusBuddyApplicationForm.GetIndieMaterialFromMaterial(material);

                        string baseMap;

                        if (shader.GetType() == typeof(IndieBuildingShader))
                        {
                            baseMap = shader.Diffuse;

                        }
                        else if (shader.GetType() == typeof(IndieLandmarkStencilShader))
                        {
                            baseMap = shader.BaseTextureMap;

                        }
                        else
                        {
                            baseMap = shader.BaseTextureMap;
                        }

                        if (!String.IsNullOrEmpty(baseMap))
                        {
                            outputWriter.WriteLine("\"" + baseMap + "\"");
                            textureMaps.Add(baseMap);
                        }
                        else
                        {
                            outputWriter.WriteLine("\"" + material.Name + "\"");
                        }
                    }

                    // Write Vertices
                    outputWriter.WriteLine("vertices");
                    for (int vi = 0; vi < grannyMeshInfo.vertices.Count; vi++)
                    {
                        GrannyVertexInfo vertex = grannyMeshInfo.vertices[vi];

                        string[] boneNames = new string[8];
                        float[] boneWeights = new float[8];
                        int[] boneIds = new int[8];

                        for (int z = 0; z < 8; z++)
                        {
                            if (z > 3)
                            {
                                //boneNames[z] = grannyMeshInfo.boneBindings[vertex.boneIndices[z]];
                                boneWeights[z] = 0f;
                                boneIds[z] = 0;
                            }
                            else
                            {
                                boneNames[z] = grannyMeshInfo.boneBindings[vertex.boneIndices[z]];
                                boneWeights[z] = (float)vertex.boneWeights[z] / 255;
                                boneIds[z] = NB2Exporter.getBoneIdForBoneName(boneLookup, boneNameToPositionMap, boneNames[z], boneWeights[z], vertex.position);
                            }
                        }

                        float[] tangents = new float[3];
                        if (vertex.tangent == null)
                        {
                            tangents[0] = vertex.normal[0];
                            tangents[1] = vertex.normal[1];
                            tangents[2] = vertex.normal[2];
                        }
                        else
                        {
                            tangents[0] = vertex.tangent[0];
                            tangents[1] = vertex.tangent[1];
                            tangents[2] = vertex.tangent[2];
                        }

                        float[] binormals = new float[3];
                        if (vertex.binormal == null)
                        {
                            binormals[0] = vertex.normal[0];
                            binormals[1] = vertex.normal[1];
                            binormals[2] = vertex.normal[2];
                        }
                        else
                        {
                            binormals[0] = vertex.binormal[0];
                            binormals[1] = vertex.binormal[1];
                            binormals[2] = vertex.binormal[2];
                        }

                        outputWriter.WriteLine(vertex.position[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.position[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.position[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                               vertex.normal[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.normal[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.normal[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                               tangents[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + tangents[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + tangents[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                               binormals[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + binormals[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + binormals[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                               vertex.uv[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.uv[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                               0.00000000f.ToString(numberFormat, CultureInfo.InvariantCulture) + " " + 0.00000000f.ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                               0.00000000f.ToString(numberFormat, CultureInfo.InvariantCulture) + " " + 0.00000000f.ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                               boneIds[0] + " " + boneIds[1] + " " + boneIds[2] + " " + boneIds[3] + " 0 0 0 0 " +
                                               vertex.boneWeights[0] + " " + vertex.boneWeights[1] + " " + vertex.boneWeights[2] + " " + vertex.boneWeights[3] + " 0 0 0 0 "
                                               );
                    }

                    // Write Triangles
                    outputWriter.WriteLine("triangles");
                    for (int ti = 0; ti < grannyMeshInfo.triangles.Count; ti++)
                    {
                        int[] triangle = grannyMeshInfo.triangles[ti];
                        outputWriter.WriteLine(triangle[0] + " " + triangle[1] + " " + triangle[2] + " 0");
                    }
                }
            }

            outputWriter.WriteLine("end");

            outputWriter.Close();

            WriteModelToCN6Response response = new WriteModelToCN6Response(decalMeshNames, textureMaps);

            return response;
        }
    }
}
