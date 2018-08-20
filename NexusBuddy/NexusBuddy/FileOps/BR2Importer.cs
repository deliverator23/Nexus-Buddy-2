using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using NexusBuddy.GrannyWrappers;
using NexusBuddy.GrannyInfos;
using NexusBuddy.Utils;
using Firaxis.Framework.Granny;
using System.Runtime.InteropServices;

namespace NexusBuddy.FileOps
{
    class BR2Importer
    {

        public unsafe static void importBR2(string sourceFilename, string outputFilename, GrannyContext grannyContext)
        {
            IGrannyFile file = cleardownTemplate(grannyContext);

            GrannyModelInfo modelInfo = loadModelInfo(sourceFilename);
            doBoneBindings(modelInfo);

            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(file);

            List<IGrannyFile> meshFileList = new List<IGrannyFile>();
            foreach (GrannyMeshInfo meshInfo in modelInfo.meshBindings) {
                meshFileList.Add(writeMesh(meshInfo));
            }

            IGrannyModel model = file.Models[0];
            GrannyModelWrapper modelWrapper = new GrannyModelWrapper(model);

            modelWrapper.setNumMeshBindings(0);
            model.MeshBindings.Clear();

            foreach (IGrannyFile meshFile in meshFileList)
            {
                doAppendMeshBinding(grannyContext, file, meshFile, 0);
            }

            fileWrapper.setNumMeshes(0);
            file.Meshes.Clear();

            foreach (IGrannyMesh mesh in file.Models[0].MeshBindings)
            {
                file.AddMeshReference(mesh);
            }

            GrannySkeletonWrapper skeletonWrapper = new GrannySkeletonWrapper(file.Models[0].Skeleton);
            skeletonWrapper.writeSkeletonInfo(modelInfo.skeleton);

            string worldBoneName = modelInfo.skeleton.bones[0].name;
            modelWrapper.setName(worldBoneName);
            skeletonWrapper.setName(worldBoneName);

            fileWrapper.setFromArtToolInfo("Blender", 2, 0);
            float[] matrix = {1f, 0f, 0f, 0f, 0f, 1f, 0f, -1f, 0f };
            fileWrapper.setMatrix(matrix);
            NexusBuddyApplicationForm.setExporterInfo(fileWrapper);
            fileWrapper.setFromFileName(sourceFilename);

            fileWrapper.addSkeletonPointer((int)skeletonWrapper.m_pkSkeleton);

            NexusBuddyApplicationForm.form.saveAsAction(file, outputFilename, false);
        }


        public unsafe static void overwriteMeshes(IGrannyFile file, string sourceFilename, GrannyContext grannyContext, Int32 currentModelIndex)
        {
            string filename = file.Filename;

            GrannyModelInfo modelInfo = loadModelInfo(sourceFilename);
            doBoneBindings(modelInfo);

            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(file);

            List<IGrannyFile> meshFileList = new List<IGrannyFile>();
            foreach (GrannyMeshInfo meshInfo in modelInfo.meshBindings)
            {
                meshFileList.Add(writeMesh(meshInfo));
            }

            IGrannyModel model = file.Models[currentModelIndex];
            GrannyModelWrapper modelWrapper = new GrannyModelWrapper(model);

            modelWrapper.setNumMeshBindings(0);
            model.MeshBindings.Clear();

            foreach (IGrannyFile meshFile in meshFileList)
            {
                doAppendMeshBinding(grannyContext, file, meshFile, currentModelIndex);
            }

            fileWrapper.setNumMeshes(0);
            fileWrapper.setNumTriTopologies(0);
            fileWrapper.setNumVertexDatas(0);
            file.Meshes.Clear();

            int meshesCount = 0;
            foreach (IGrannyModel loopModel in file.Models) {
                foreach (IGrannyMesh mesh in loopModel.MeshBindings)
                {
                    file.AddMeshReference(mesh);
                    meshesCount++;
                }
            }

            fileWrapper.setFromArtToolInfo("Blender", 2, 0);
            fileWrapper.setUnitsPerMeter(10.7f);
            NexusBuddyApplicationForm.setExporterInfo(fileWrapper);
            fileWrapper.setFromFileName(sourceFilename);

            fileWrapper.setNumMeshes(meshesCount);
            NexusBuddyApplicationForm.form.saveAsAction(file, filename, false);
        }

        public unsafe static IGrannyFile writeMesh(GrannyMeshInfo meshInfo)
        {
            bool useLeaderTemplate = NexusBuddyApplicationForm.form.useLeaderTemplate();
            bool useSceneTemplate = NexusBuddyApplicationForm.form.useSceneTemplate();

            string templateFilename;
            if (useLeaderTemplate)
            {
                templateFilename = NexusBuddyApplicationForm.form.leaderTemplateFilename;
            }
            else if (useSceneTemplate) {
                templateFilename = NexusBuddyApplicationForm.form.sceneTemplateFilename;
            }
            else
            {
                templateFilename = NexusBuddyApplicationForm.form.modelTemplateFilename;
            }

            IGrannyFile appendFile = NexusBuddyApplicationForm.form.openFileAsTempFileCopy(templateFilename, "tempappend");
            appendFile.Meshes.RemoveRange(1, appendFile.Meshes.Count - 1);
            IGrannyMesh mesh = appendFile.Models[0].MeshBindings[0];

            GrannyMeshWrapper meshWrapper = new GrannyMeshWrapper(mesh);
            meshWrapper.writeMeshInfo(meshInfo, useLeaderTemplate, useSceneTemplate);

            return appendFile;
        }

        public unsafe static void doAppendMeshBinding(GrannyContext grannyContext, IGrannyFile inputFile, IGrannyFile appendFile, Int32 currentModelIndex)
        {
            // Get wrappers
            IGrannyModel inputModel = inputFile.Models[currentModelIndex];
            GrannyModelWrapper inputModelWrapper = new GrannyModelWrapper(inputModel);

            IGrannyModel appendModel = appendFile.Models[0];
            GrannyModelWrapper appendModelWrapper = new GrannyModelWrapper(appendModel);

            // Update model 
            inputModel.MeshBindings.Add(appendFile.Meshes[0]);

            int meshCountInput = inputModelWrapper.getNumMeshBindings();
            int newMeshBindingsCount = meshCountInput + 1;
            inputModelWrapper.setNumMeshBindings(newMeshBindingsCount);

            // Update model mesh bindings
            int oldMeshBindingsPtr = *(int*)inputModelWrapper.getMeshBindingsPtr();
            *(int*)inputModelWrapper.getMeshBindingsPtr() = (int)Marshal.AllocHGlobal(newMeshBindingsCount * 4);
            int newMeshBindingsPtr = *(int*)inputModelWrapper.getMeshBindingsPtr();

            int modelMeshBindingsPtrAppend = *(int*)appendModelWrapper.getMeshBindingsPtr();

            MemoryUtil.MemCpy((void*)newMeshBindingsPtr, (void*)oldMeshBindingsPtr, (uint)(meshCountInput * 4));
            MemoryUtil.MemCpy((void*)(newMeshBindingsPtr + meshCountInput * 4), (void*)modelMeshBindingsPtrAppend, 4);
        }

        //// todo - update to create new memory areas
        //public unsafe static void doAppendMeshBinding(GrannyContext grannyContext, IGrannyFile inputFile, IGrannyFile appendFile)
        //{
        //    // UNPACK CURRENT MODEL
        //    IGrannyModel inputModel = inputFile.Models[0];
        //    GrannyModelWrapper inputModelWrapper = new GrannyModelWrapper(inputModel);
        //    void* m_pkModel_input = inputModelWrapper.m_pkModel;

        //    // UNPACK APPEND MODEL
        //    IGrannyModel appendModel = appendFile.Models[0];
        //    GrannyModelWrapper appendModelWrapper = new GrannyModelWrapper(appendModel);
        //    void* m_pkModel_app = appendModelWrapper.m_pkModel;

        //    // UNPACK NEW MODEL
        //    IGrannyFile newModelFile = inputFile;
        //    IGrannyModel newModel = newModelFile.Models[0];
        //    GrannyModelWrapper newModelWrapper = new GrannyModelWrapper(newModel);
        //    void* m_pkModel_new = newModelWrapper.m_pkModel;

        //    // UPDATE NEW MODEL          
        //    int meshCountInput = *(int*)((IntPtr)m_pkModel_input + 76);

        //    newModel.MeshBindings.Add(appendFile.Meshes[0]);
        //    // update Mesh Count
        //    *(int*)((IntPtr)m_pkModel_new + 76) = meshCountInput + 1;

        //    //update model mesh bindings
        //    int size = meshCountInput;
        //    void* modelMeshBindingsPtrInput = (void*)*(int*)((IntPtr)m_pkModel_input + 80);
        //    MemoryUtil.MemCpy((void*)*(int*)((IntPtr)m_pkModel_new + 80), (void*)modelMeshBindingsPtrInput, (uint)(size * 4));

        //    int writeOffset = meshCountInput;
        //    void* modelMeshBindingsPtrAppend = (void*)*(int*)((IntPtr)m_pkModel_app + 80);
        //    MemoryUtil.MemCpy((void*)(*(int*)((IntPtr)m_pkModel_new + 80) + writeOffset * 4), (void*)modelMeshBindingsPtrAppend, (uint)(1 * 4));

        //    GrannyFileWrapper newModelFileWrapper = new GrannyFileWrapper(newModelFile);
        //    void* m_info = newModelFileWrapper.m_info;
        //    void** grannyMeshPtr = (void**)*(int*)((IntPtr)m_info + 56);

        //    // Add all meshes from input file   
        //    *(int*)((IntPtr)m_info + 52) = 0; // Reset Mesh Count to 0
        //    for (int index = 0; index < inputFile.Meshes.Count; index++)
        //    {
        //        IGrannyMesh mesh = inputFile.Meshes[index];
        //        GrannyMeshWrapper meshWrapper = new GrannyMeshWrapper(mesh);
        //        void* m_pkMesh = meshWrapper.m_pkMesh;
        //        MemoryUtil.MemCpy((void*)*(int*)((IntPtr)m_info + 56), (void*)grannyMeshPtr, (uint)(*(int*)((IntPtr)m_info + 52) * 4));
        //        *(int*)(*(int*)((IntPtr)m_info + 56) + *(int*)((IntPtr)m_info + 52) * 4) = (int)m_pkMesh;
        //        *(int*)((IntPtr)m_info + 52) = *(int*)((IntPtr)m_info + 52) + 1;
        //    }

        //    // Add mesh from append file
        //    IGrannyMesh appendMesh = appendFile.Meshes[0];
        //    GrannyMeshWrapper appendMeshWrapper = new GrannyMeshWrapper(appendMesh);
        //    void* m_pkAppendMesh = appendMeshWrapper.m_pkMesh;
        //    MemoryUtil.MemCpy((void*)*(int*)((IntPtr)m_info + 56), (void*)grannyMeshPtr, (uint)(*(int*)((IntPtr)m_info + 52) * 4));
        //    *(int*)(*(int*)((IntPtr)m_info + 56) + *(int*)((IntPtr)m_info + 52) * 4) = (int)m_pkAppendMesh;
        //    *(int*)((IntPtr)m_info + 52) = *(int*)((IntPtr)m_info + 52) + 1;
        //}

     
        public static IGrannyFile cleardownTemplate(GrannyContext grannyContext)
        {
            //string filename = sourceTemplatePath + "model_template.gr2";

            IGrannyFile file = NexusBuddyApplicationForm.form.openFileAsTempFileCopy(NexusBuddyApplicationForm.form.modelTemplateFilename, "tempimport");

            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(file);

            //GrannyMeshWrapper meshWrapper = new GrannyMeshWrapper(file.Meshes[0]);
            //meshWrapper.setName("BLANK_MESH");
            //meshWrapper.setNumVertices(0);
            //meshWrapper.setNumBoneBindings(0);
            //meshWrapper.setNumIndices(0);
            //meshWrapper.setNumIndices16(0);
            //meshWrapper.setGroup0TriCount(0);

            ////memProbe(meshWrapper.getBoneBindingsPtr());

            //GrannyModelWrapper modelWrapper = new GrannyModelWrapper(file.Models[0]);
            //modelWrapper.setNumMeshBindings(1);
            //modelWrapper.setName("BLANK_MODEL");

            //GrannySkeletonWrapper skeletonWrapper = new GrannySkeletonWrapper(modelWrapper.wrappedModel.Skeleton);
            //skeletonWrapper.setNumBones(0);
            //skeletonWrapper.setName("BLANK_SKELETON");

            fileWrapper.setNumMeshes(0);
            //fileWrapper.setNumModels(0);
            fileWrapper.setNumVertexDatas(0);
            fileWrapper.setNumSkeletons(0);
            fileWrapper.setNumTriTopologies(0);
            fileWrapper.setNumMaterials(0);

            file.Meshes.Clear();

            //fileWrapper.setFromFileName("My lovely lovely filename");
            //fileWrapper.setFromArtToolName("Blender 2.49");
            //fileWrapper.setExporterName("Nexus Buddy 2.0 beta 6");

            return file;
        }


        private static GrannyModelInfo loadModelInfo(string filename)
        {
            string currentLine = "";
            string numberRegex = "([-+]?[0-9]+\\.?[0-9]*) ";

            StreamReader streamReader = new StreamReader(filename);

            while (!currentLine.StartsWith("skeleton"))
            {
                currentLine = streamReader.ReadLine();
            }

            GrannyModelInfo modelInfo = new GrannyModelInfo();

            GrannySkeletonInfo skeletonInfo = new GrannySkeletonInfo();
            List<GrannyBoneInfo> skeletonBones = new List<GrannyBoneInfo>();

            while (!currentLine.StartsWith("meshes"))
            {
                currentLine = streamReader.ReadLine();
                if (!currentLine.StartsWith("meshes"))
                {
                    string regexString = "([0-9]+) \"(.+)\" ";
                    for (int i = 0; i < 24; i++)
                    {
                        regexString = regexString + numberRegex;
                    }

                    Regex regex = new Regex(regexString.Trim());
                    MatchCollection mc = regex.Matches(currentLine);
                    foreach (Match m in mc)
                    {
                        GrannyBoneInfo boneInfo = new GrannyBoneInfo();
                        GrannyTransformInfo transformInfo = new GrannyTransformInfo();

                        //int boneindex = NumberUtils.parseInt(m.Groups[1].Value.Trim());
                        string boneName = m.Groups[2].Value;
                        int parentIndex = NumberUtils.parseInt(m.Groups[3].Value.Trim());

                        float[] position = new float[3];
                        position[0] = NumberUtils.parseFloat(m.Groups[4].Value.Trim());
                        position[1] = NumberUtils.parseFloat(m.Groups[5].Value.Trim());
                        position[2] = NumberUtils.parseFloat(m.Groups[6].Value.Trim());

                        float[] orientation = new float[4];
                        orientation[0] = NumberUtils.parseFloat(m.Groups[7].Value.Trim());
                        orientation[1] = NumberUtils.parseFloat(m.Groups[8].Value.Trim());
                        orientation[2] = NumberUtils.parseFloat(m.Groups[9].Value.Trim());
                        orientation[3] = NumberUtils.parseFloat(m.Groups[10].Value.Trim());

                        float[] scaleShear = new float[9];
                        scaleShear[0] = 1.0f;
                        scaleShear[1] = 0.0f;
                        scaleShear[2] = 0.0f;
                        scaleShear[3] = 0.0f;
                        scaleShear[4] = 1.0f;
                        scaleShear[5] = 0.0f;
                        scaleShear[6] = 0.0f;
                        scaleShear[7] = 0.0f;
                        scaleShear[8] = 1.0f;

                        float[] invWorld = new float[16];

                        for (int j = 0; j < 16; j++)
                        {
                            invWorld[j] = NumberUtils.parseFloat(m.Groups[j + 11].Value.Trim());
                        }


                        bool hasPosition = true;
                        bool hasOrientation = true;
                        int flags = 0;

                        if (NumberUtils.almostEquals(position[0], 0.0f, 4) && NumberUtils.almostEquals(position[1], 0.0f, 4) && NumberUtils.almostEquals(position[2], 0.0f, 4))
                        {
                            hasPosition = false;
                        }

                        if (NumberUtils.almostEquals(orientation[0], 0.0f, 5) && NumberUtils.almostEquals(orientation[1], 0.0f, 5) &&
                            NumberUtils.almostEquals(orientation[2], 0.0f, 5) && NumberUtils.almostEquals(orientation[3], 1.0f, 5))
                        {
                            hasOrientation = false;
                        }

                        if (hasPosition)
                        {
                            flags = flags + 1;
                        }

                        if (hasOrientation)
                        {
                            flags = flags + 2;
                        }

                        transformInfo.flags = flags;
                        transformInfo.position = position;
                        transformInfo.orientation = orientation;
                        transformInfo.scaleShear = scaleShear;

                        boneInfo.name = boneName;
                        boneInfo.parentIndex = parentIndex;
                        boneInfo.localTransform = transformInfo;
                        boneInfo.inverseWorldTransform = invWorld;
                        boneInfo.LODError = 0;

                        skeletonBones.Add(boneInfo);
                    }
                }
            }

            skeletonInfo.bones = skeletonBones;

            // Read Meshes
            int numMeshes = NumberUtils.parseInt(currentLine.Replace("meshes:", ""));
            List<GrannyMeshInfo> meshInfos = new List<GrannyMeshInfo>();
            for (int meshId = 0; meshId < numMeshes; meshId++)
            {
                string meshName = "";
                while (!currentLine.StartsWith("mesh:"))
                {
                    currentLine = streamReader.ReadLine();
                }

                string regexString = "\"(.+)\"";
                Regex regex = new Regex(regexString);
                MatchCollection mc = regex.Matches(currentLine);

                foreach (Match m in mc)
                {
                    meshName = m.Groups[1].Value;
                }

                // Read Vertices
                List<GrannyVertexInfo> vertexInfos = new List<GrannyVertexInfo>();
                while (!currentLine.StartsWith("triangles"))
                {
                    currentLine = streamReader.ReadLine();
                    if (!currentLine.StartsWith("vertices") && !currentLine.StartsWith("triangles"))
                    {
                        int spacesCount = 0;
                        foreach (char c in currentLine)
                            if (c == ' ') spacesCount++;
                        bool hasBinormalsAndTangents = spacesCount > 15;

                        regexString = "";
                        for (int i = 0; i < 16; i++)
                        {
                            regexString = regexString + numberRegex;
                        }

                        if (hasBinormalsAndTangents)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                regexString = regexString + numberRegex;
                            }
                        }

                        regex = new Regex(regexString.Trim());
                        mc = regex.Matches(currentLine);
                        foreach (Match m in mc)
                        {
                            GrannyVertexInfo vertexInfo = new GrannyVertexInfo();

                            float[] position = new float[3];
                            position[0] = NumberUtils.parseFloat(m.Groups[1].Value.Trim());
                            position[1] = NumberUtils.parseFloat(m.Groups[2].Value.Trim());
                            position[2] = NumberUtils.parseFloat(m.Groups[3].Value.Trim());

                            float[] normal = new float[3];
                            normal[0] = NumberUtils.parseFloat(m.Groups[4].Value.Trim());
                            normal[1] = NumberUtils.parseFloat(m.Groups[5].Value.Trim());
                            normal[2] = NumberUtils.parseFloat(m.Groups[6].Value.Trim());

                            float[] uv = new float[2];
                            uv[0] = NumberUtils.parseFloat(m.Groups[7].Value.Trim());
                            uv[1] = NumberUtils.parseFloat(m.Groups[8].Value.Trim());

                            int[] boneIndices = new int[4];
                            boneIndices[0] = NumberUtils.parseInt(m.Groups[9].Value.Trim());
                            boneIndices[1] = NumberUtils.parseInt(m.Groups[10].Value.Trim());
                            boneIndices[2] = NumberUtils.parseInt(m.Groups[11].Value.Trim());
                            boneIndices[3] = NumberUtils.parseInt(m.Groups[12].Value.Trim());

                            int[] boneWeights = new int[4];
                            boneWeights[0] = NumberUtils.parseInt(m.Groups[13].Value.Trim());
                            boneWeights[1] = NumberUtils.parseInt(m.Groups[14].Value.Trim());
                            boneWeights[2] = NumberUtils.parseInt(m.Groups[15].Value.Trim());
                            boneWeights[3] = NumberUtils.parseInt(m.Groups[16].Value.Trim());

                            if (hasBinormalsAndTangents)
                            {
                                float[] tangent = new float[3];
                                tangent[0] = NumberUtils.parseFloat(m.Groups[17].Value.Trim());
                                tangent[1] = NumberUtils.parseFloat(m.Groups[18].Value.Trim());
                                tangent[2] = NumberUtils.parseFloat(m.Groups[19].Value.Trim());

                                float[] binormal = new float[3];
                                binormal[0] = NumberUtils.parseFloat(m.Groups[20].Value.Trim());
                                binormal[1] = NumberUtils.parseFloat(m.Groups[21].Value.Trim());
                                binormal[2] = NumberUtils.parseFloat(m.Groups[22].Value.Trim());

                                vertexInfo.binormal = binormal;
                                vertexInfo.tangent = tangent;
                            }

                            vertexInfo.position = position;
                            vertexInfo.normal = normal;
                            vertexInfo.boneIndices = boneIndices;
                            vertexInfo.boneWeights = boneWeights;
                            vertexInfo.uv = uv;

                            vertexInfos.Add(vertexInfo);
                        }
                    }
                }

                // Read Triangles
                List<int[]> triangles = new List<int[]>();
                while (!currentLine.StartsWith("mesh") && !currentLine.StartsWith("end"))
                {
                    currentLine = streamReader.ReadLine();
                    if (!currentLine.StartsWith("mesh") && !currentLine.StartsWith("end"))
                    {
                        regexString = "";
                        for (int i = 0; i < 3; i++)
                        {
                            regexString = regexString + "([-+]?[0-9]+\\.?[0-9]*) ";
                        }

                        regex = new Regex(regexString.Trim());
                        mc = regex.Matches(currentLine);
                        foreach (Match m in mc)
                        {
                            int[] triangle = new int[3];
                            triangle[0] = NumberUtils.parseInt(m.Groups[1].Value.Trim());
                            triangle[1] = NumberUtils.parseInt(m.Groups[2].Value.Trim());
                            triangle[2] = NumberUtils.parseInt(m.Groups[3].Value.Trim());

                            triangles.Add(triangle);
                        }
                    }
                }

                GrannyMeshInfo meshInfo = new GrannyMeshInfo();
                meshInfo.name = meshName;
                meshInfo.vertices = vertexInfos;
                meshInfo.triangles = triangles;

                meshInfos.Add(meshInfo);
            }

            modelInfo.skeleton = skeletonInfo;
            modelInfo.meshBindings = meshInfos;

            streamReader.Close();

            return modelInfo;
        }

        private static void doBoneBindings(GrannyModelInfo modelInfo)
        {
            GrannySkeletonInfo skeletonInfo = modelInfo.skeleton;

            foreach (GrannyMeshInfo meshInfo in modelInfo.meshBindings)
            {
                HashSet<int> distinctBoneIds = new HashSet<int>();

                foreach (GrannyVertexInfo vertexInfo in meshInfo.vertices)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        distinctBoneIds.Add(vertexInfo.boneIndices[i]);
                    }
                }

                Dictionary<int, int> globalToLocalBoneIdDict = new Dictionary<int, int>();
                List<int> distinctBoneIdList = distinctBoneIds.ToList();
                List<string> boneBindingNames = new List<string>();
                for (int i = 0; i < distinctBoneIdList.Count; i++)
                {
                    boneBindingNames.Add(skeletonInfo.bones[distinctBoneIdList[i]].name);
                    globalToLocalBoneIdDict.Add(distinctBoneIdList[i], i);
                }

                foreach (GrannyVertexInfo vertexInfo in meshInfo.vertices)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int localBoneId = globalToLocalBoneIdDict[vertexInfo.boneIndices[i]];
                        vertexInfo.boneIndices[i] = localBoneId;
                    }
                }

                meshInfo.boneBindings = boneBindingNames;
            }
        }
    }
}
