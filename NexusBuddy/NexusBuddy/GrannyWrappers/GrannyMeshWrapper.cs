using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firaxis.Framework.Granny;
using System.Reflection;
using System.Runtime.InteropServices;
using NexusBuddy.GrannyInfos;
using NexusBuddy.Utils;

namespace NexusBuddy.GrannyWrappers
{
    public unsafe class GrannyMeshWrapper
    {
        public IGrannyMesh wrappedMesh;
        public DummyGrannyMesh* m_pkMesh;

        public GrannyMeshWrapper(IGrannyMesh inputMesh)
        {
            wrappedMesh = inputMesh;
            Type myType = inputMesh.GetType();
            FieldInfo fm_pkMesh = myType.GetField("m_pkMesh", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Pointer pm_pkMesh = (Pointer)fm_pkMesh.GetValue(inputMesh);
            m_pkMesh = (DummyGrannyMesh*)Pointer.Unbox(pm_pkMesh);
        }

        public unsafe String getName()
        {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(m_pkMesh));
        }

        public unsafe void setName(string meshName)
        {
            *(int*)(m_pkMesh) = (int)MemoryUtil.getStringIntPtr(meshName);
        }

        public IntPtr getNamePtr()
        {
            return ((IntPtr)m_pkMesh);
        }

        public IntPtr getPrimaryVertexDataPtr()
        {
            return ((IntPtr)m_pkMesh + 4);
        }

        public int getNumMorphTargets()
        {
            return *(int*)((IntPtr)this.m_pkMesh + 8);
        }

        public IntPtr getMorphTargetsPtr()
        {
            return ((IntPtr)m_pkMesh + 12);
        }

        public IntPtr getPrimaryTopologyPtr()
        {
            return ((IntPtr)m_pkMesh + 16);
        }

        public void setNumMaterialBindings(int num)
        {
            *(int*)((IntPtr)m_pkMesh + 20) = num;
        }

        public int getNumMaterialBindings()
        {
            return *(int*)((IntPtr)this.m_pkMesh + 20);
        }

        public IntPtr getMaterialBindingsPtr()
        {
            return ((IntPtr)m_pkMesh + 24);
        }

        public int getNumBoneBindings()
        {
            return *(int*)((IntPtr)this.m_pkMesh + 28);
        }

        public void setNumBoneBindings(int num)
        {
            *(int*)((IntPtr)m_pkMesh + 28) = num;
        }

        public IntPtr getBoneBindingsPtr()
        {
            return ((IntPtr)m_pkMesh + 32);
        }

        public int getNumVertices()
        {
            return *(int*)(*(int*)(getPrimaryVertexDataPtr()) + 4);
        }

        public void setNumVertices(int num)
        {
            *(int*)(*(int*)(getPrimaryVertexDataPtr()) + 4) = num;
        }

        public IntPtr getVerticesPtr()
        {
            return (IntPtr)(*(int*)(getPrimaryVertexDataPtr()) + 8);
        }

        public int getNumIndices()
        {
            return *(int*)(*(int*)(getPrimaryTopologyPtr()) + 8);
        }

        public void setNumIndices(int num)
        {
            *(int*)(*(int*)(getPrimaryTopologyPtr()) + 8) = num;
        }

        public IntPtr getIndicesPtr()
        {
            return (IntPtr)(*(int*)(getPrimaryTopologyPtr()) + 12);
        }

        public int getNumIndices16()
        {
            return *(int*)(*(int*)(getPrimaryTopologyPtr()) + 16);
        }

        public void setNumIndices16(int num)
        {
            *(int*)(*(int*)(getPrimaryTopologyPtr()) + 16) = num;
        }

        public IntPtr getIndices16Ptr()
        {
            return (IntPtr)(*(int*)(getPrimaryTopologyPtr()) + 20);
        }

        public int getGroup0TriCount()
        {
            return *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + 8);
        }

        public void setGroup0TriCount(int num)
        {
            *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + 8) = num;
        }
        
        public unsafe GrannyMeshInfo getMeshInfo()
        {
            GrannyMeshInfo meshInfo = new GrannyMeshInfo();
            meshInfo.name = Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(m_pkMesh));

            List<string> boneBindings = new List<string>();
            for (int i = 0; i < getNumBoneBindings(); i++)
            {
                boneBindings.Add(Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(getBoneBindingsPtr()) + i * 36)));
            }
            meshInfo.boneBindings = boneBindings;

            meshInfo.setVertexStructInfos(getVertexStructInfos());

            int vertexCount = getNumVertices();
            IntPtr vertexPtr = getVerticesPtr();

            int vertexSize = meshInfo.bytesPerVertex;

            List<GrannyVertexInfo> vertexInfos = new List<GrannyVertexInfo>();
            for (int i = 0; i < vertexCount; i++)
            {
                GrannyVertexInfo currentVertex = new GrannyVertexInfo();

                GrannyMeshVertexStructInfo structInfo = meshInfo.getVertexStructInfoByName("Position");
                if (structInfo != null)
                {
                    currentVertex.position = new float[3];
                    currentVertex.position[0] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.position[1] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                    currentVertex.position[2] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 2));
                }

                structInfo = meshInfo.getVertexStructInfoByName("BoneWeights");
                if (structInfo != null)
                {
                    currentVertex.boneWeights = new int[4];
                    currentVertex.boneWeights[0] = (byte)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.boneWeights[1] = (byte)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                    currentVertex.boneWeights[2] = (byte)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 2));
                    currentVertex.boneWeights[3] = (byte)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 3));
                }
                else
                {
                    currentVertex.boneWeights = new int[4];
                    currentVertex.boneWeights[0] = 255;
                    currentVertex.boneWeights[1] = 0;
                    currentVertex.boneWeights[2] = 0;
                    currentVertex.boneWeights[3] = 0;
                }

                structInfo = meshInfo.getVertexStructInfoByName("BoneIndices");
                if (structInfo != null)
                {
                    currentVertex.boneIndices = new int[4];
                    currentVertex.boneIndices[0] = (byte)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.boneIndices[1] = (byte)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                    currentVertex.boneIndices[2] = (byte)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 2));
                    currentVertex.boneIndices[3] = (byte)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 3));
                }
                else
                {
                    currentVertex.boneIndices = new int[4];
                    currentVertex.boneIndices[0] = 0;
                    currentVertex.boneIndices[1] = 0;
                    currentVertex.boneIndices[2] = 0;
                    currentVertex.boneIndices[3] = 0;
                }

                structInfo = meshInfo.getVertexStructInfoByName("Normal");
                if (structInfo != null)
                {
                    currentVertex.normal = new float[3];
                    currentVertex.normal[0] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.normal[1] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                    currentVertex.normal[2] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 2));
                }

                structInfo = meshInfo.getVertexStructInfoByName("TextureCoordinates0");
                if (structInfo != null)
                {
                    currentVertex.uv = new float[2];
                    currentVertex.uv[0] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.uv[1] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                }

                vertexInfos.Add(currentVertex);
            }

            meshInfo.vertices = vertexInfos;

            if (*(int*)(getMaterialBindingsPtr()) != 0)
            {
                string materialName = Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)*(int*)*(int*)(getMaterialBindingsPtr()));
                meshInfo.materialName = materialName;
            }

            int numTriangleGroups = *(int*)*(int*)getPrimaryTopologyPtr();

            int group0MaterialIndex = *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + 0);
            int group0TriFirst = *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + 4);
            int group0TriCount = *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + 8);

            int numIndices16 = getNumIndices16();
            int numIndices = getNumIndices();

            List<int> indices = new List<int>();
            if (numIndices16 > 0)
            {
                for (int i = 0; i < numIndices16; i++)
                {
                    indices.Add((int)*(ushort*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 20) + i * 2));
                }
            }
            else if (numIndices > 0)
            {
                for (int i = 0; i < numIndices; i++)
                {
                    indices.Add(*(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 12) + i * 4));
                }
            }

            if (indices.Count > 0)
            {
                List<int[]> triangles = new List<int[]>();
                for (int iTriangle = 0; iTriangle < group0TriCount; iTriangle++)
                {
                    int index0 = iTriangle * 3;
                    int index1 = index0 + 1;
                    int index2 = index0 + 2;

                    int[] triangle = new int[3];
                    triangle[0] = indices[index0];
                    triangle[1] = indices[index1];
                    triangle[2] = indices[index2];

                    triangles.Add(triangle);
                }
                meshInfo.triangles = triangles;
            }

            return meshInfo;
        }

        unsafe private List<GrannyMeshVertexStructInfo> getVertexStructInfos()
        {
            IntPtr vertexInfoPtr = (IntPtr)(*(int*)(getPrimaryVertexDataPtr()));

            List<GrannyMeshVertexStructInfo> vertexStructInfos = new List<GrannyMeshVertexStructInfo>();
            int z = 0;
            while (true)
            {
                int type = *(int*)(*(int*)(vertexInfoPtr) + (z * 32) + 0);
                if (type != 0)
                {
                    string name = Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(vertexInfoPtr) + (z * 32) + 4));
                    int count = *(int*)(*(int*)(vertexInfoPtr) + (z * 32) + 12);
                    GrannyMeshVertexStructInfo info = new GrannyMeshVertexStructInfo(type, name, count);
                    vertexStructInfos.Add(info);
                    z++;
                }
                else
                {
                    break;
                }
            }
            return vertexStructInfos;
        }

        public unsafe void writeMeshInfo(GrannyMeshInfo meshInfo, bool isLeaderFormat, bool isSceneFormat)
        {
            meshInfo.setVertexStructInfos(getVertexStructInfos());
            setName(meshInfo.name);
            writeVertices(meshInfo, isLeaderFormat, isSceneFormat);
            writeTriangles(meshInfo.triangles, isLeaderFormat || isSceneFormat);
            writeBoneBindings(meshInfo.boneBindings);
        }

        unsafe private void writeVertices(GrannyMeshInfo meshInfo, bool isLeaderFormat, bool isSceneFormat)
        {
            int vertexCount = meshInfo.vertices.Count;
            setNumVertices(vertexCount);

            int vertexSize = meshInfo.bytesPerVertex;

            int oldVerticesPtr = *(int*)getVerticesPtr();
            *(int*)(getVerticesPtr()) = (int)Marshal.AllocHGlobal(vertexCount * vertexSize);
            int newVerticesPtr = *(int*)getVerticesPtr();

            List<GrannyVertexInfo> vertexInfos = meshInfo.vertices;
            for (int i = 0; i < vertexCount; i++)
            {
                MemoryUtil.MemCpy((void*)(newVerticesPtr + i * vertexSize), (void*)oldVerticesPtr, (uint)vertexSize);

                GrannyVertexInfo currentVertex = vertexInfos[i];

                GrannyMeshVertexStructInfo structInfo = meshInfo.getVertexStructInfoByName("Position");
                if (structInfo != null)
                {
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = currentVertex.position[0];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = currentVertex.position[1];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 2)) = currentVertex.position[2];
                }

                if (!isSceneFormat) { 
                    structInfo = meshInfo.getVertexStructInfoByName("BoneWeights");
                    if (structInfo != null)
                    {
                        *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = (byte)currentVertex.boneWeights[0];
                        *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = (byte)currentVertex.boneWeights[1];
                        *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 2)) = (byte)currentVertex.boneWeights[2];
                        *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 3)) = (byte)currentVertex.boneWeights[3];
                    }

                    structInfo = meshInfo.getVertexStructInfoByName("BoneIndices");
                    if (structInfo != null)
                    {
                        *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = (byte)currentVertex.boneIndices[0];
                        *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = (byte)currentVertex.boneIndices[1];
                        *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 2)) = (byte)currentVertex.boneIndices[2];
                        *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 3)) = (byte)currentVertex.boneIndices[3];
                    }
                }

                if (isLeaderFormat || isSceneFormat)
                {
                    structInfo = meshInfo.getVertexStructInfoByName("Normal");
                    if (structInfo != null)
                    {
                        // granny_real16 
                        *(ushort*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = NumberUtils.floatToHalf(currentVertex.normal[0]);
                        *(ushort*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = NumberUtils.floatToHalf(currentVertex.normal[1]);
                        *(ushort*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 2)) = NumberUtils.floatToHalf(currentVertex.normal[2]);
                    }

                    structInfo = meshInfo.getVertexStructInfoByName("Binormal");
                    if (structInfo != null && currentVertex.binormal != null)
                    {
                        // granny_real16 
                        *(ushort*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = NumberUtils.floatToHalf(currentVertex.binormal[0]);
                        *(ushort*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = NumberUtils.floatToHalf(currentVertex.binormal[1]);
                        *(ushort*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 2)) = NumberUtils.floatToHalf(currentVertex.binormal[2]);
                    }

                    structInfo = meshInfo.getVertexStructInfoByName("Tangent");
                    if (structInfo != null && currentVertex.tangent != null)
                    {
                        // granny_real16 
                        *(ushort*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = NumberUtils.floatToHalf(currentVertex.tangent[0]);
                        *(ushort*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = NumberUtils.floatToHalf(currentVertex.tangent[1]);
                        *(ushort*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 2)) = NumberUtils.floatToHalf(currentVertex.tangent[2]);
                    }
                }
                else
                {
                    structInfo = meshInfo.getVertexStructInfoByName("Normal");
                    if (structInfo != null)
                    {
                        // granny_real32 
                        *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = currentVertex.normal[0];
                        *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = currentVertex.normal[1];
                        *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 2)) = currentVertex.normal[2];
                    }
                }

                if (isLeaderFormat || isSceneFormat)
                {
                    structInfo = meshInfo.getVertexStructInfoByName("TextureCoordinates0");
                    if (structInfo != null)
                    {
                        // granny_real16
                        *(ushort*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = NumberUtils.floatToHalf(currentVertex.uv[0]);
                        *(ushort*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = NumberUtils.floatToHalf(currentVertex.uv[1]);
                    }
                }
                else 
                {
                    structInfo = meshInfo.getVertexStructInfoByName("TextureCoordinates0");
                    if (structInfo != null)
                    {
                        // granny_real32 
                        *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = currentVertex.uv[0];
                        *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = currentVertex.uv[1];
                    } 
                }
            }
        }

        unsafe private void writeTriangles(List<int[]> triangles, bool isLeaderOrSceneFormat)
        {
            int group0TriCount = triangles.Count;
            setGroup0TriCount(group0TriCount);

            if (isLeaderOrSceneFormat)
            {
                // Assumes using 2 byte indices (Indices16 - not Indices)
                int indexSize = 2;
                int numIndicies = group0TriCount * 3;
                setNumIndices16(numIndicies);

                int oldIndicesPtr = *(int*)getIndices16Ptr();
                *(int*)(getIndices16Ptr()) = (int)Marshal.AllocHGlobal(numIndicies * indexSize);
                int newIndicesPtr = *(int*)getIndices16Ptr();

                for (int iTriangle = 0; iTriangle < group0TriCount; iTriangle++)
                {
                    int index0 = iTriangle * 3;
                    int index1 = index0 + 1;
                    int index2 = index0 + 2;

                    MemoryUtil.MemCpy((void*)(newIndicesPtr + index0 * indexSize), (void*)oldIndicesPtr, (uint)indexSize * 3);

                    int[] triangle = triangles[iTriangle];
                    *(ushort*)(newIndicesPtr + index0 * indexSize) = (ushort)triangle[0];
                    *(ushort*)(newIndicesPtr + index1 * indexSize) = (ushort)triangle[1];
                    *(ushort*)(newIndicesPtr + index2 * indexSize) = (ushort)triangle[2];
                }
            }
            else
            {
                // Assumes using 4 byte indices (Indices - not Indices16)
                int indexSize = 4;
                int numIndicies = group0TriCount * 3;
                setNumIndices(numIndicies);

                int oldIndicesPtr = *(int*)getIndicesPtr();
                *(int*)(getIndicesPtr()) = (int)Marshal.AllocHGlobal(numIndicies * indexSize);
                int newIndicesPtr = *(int*)getIndicesPtr();

                for (int iTriangle = 0; iTriangle < group0TriCount; iTriangle++)
                {
                    int index0 = iTriangle * 3;
                    int index1 = index0 + 1;
                    int index2 = index0 + 2;

                    MemoryUtil.MemCpy((void*)(newIndicesPtr + index0 * indexSize), (void*)oldIndicesPtr, (uint)indexSize * 3);

                    int[] triangle = triangles[iTriangle];
                    *(int*)(newIndicesPtr + index0 * indexSize) = triangle[0];
                    *(int*)(newIndicesPtr + index1 * indexSize) = triangle[1];
                    *(int*)(newIndicesPtr + index2 * indexSize) = triangle[2];
                }
            }
        }

        unsafe private void writeBoneBindings(List<string> boneBindings)
        {
            setNumBoneBindings(boneBindings.Count);

            int oldBBPointer = *(int*)getBoneBindingsPtr();
            // Blank out OBBMin/Max fields
            *(float*)(oldBBPointer + 4) = 0.0f;
            *(float*)(oldBBPointer + 8) = 0.0f;
            *(float*)(oldBBPointer + 12) = 0.0f;
            *(float*)(oldBBPointer + 16) = 0.0f;
            *(float*)(oldBBPointer + 20) = 0.0f;
            *(float*)(oldBBPointer + 24) = 0.0f;

            *(int*)(getBoneBindingsPtr()) = (int)Marshal.AllocHGlobal(boneBindings.Count * 36);
            int newBBPointer = *(int*)getBoneBindingsPtr();

            for (int i = 0; i < boneBindings.Count; i++)
            {
                MemoryUtil.MemCpy((void*)(newBBPointer + i * 36), (void*)oldBBPointer, 36); //copy first old bone binding to ith slot
                *(int*)(newBBPointer + i * 36) = (int)MemoryUtil.getStringIntPtr(boneBindings[i]); // overwrite bone binding string
            }
        }

        public bool meshEqual(IGrannyMesh otherMesh)
        {
            if (wrappedMesh.Name == otherMesh.Name &&
                wrappedMesh.IndexCount == otherMesh.IndexCount &&
                wrappedMesh.VertexCount == otherMesh.VertexCount &&
                wrappedMesh.BoneBindings.Count == otherMesh.BoneBindings.Count &&
                wrappedMesh.MaterialBindings.Count == otherMesh.MaterialBindings.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}

// getPrimaryTopologyPtr
// + 0 = Num Groups
// + 4 = Group Data
// + 8 = Num Indicies
// + 12 = Indicies
// + 16 = Num Indicies16
// + 20 = Indicies 16

//log("*(int*)(*(int*)((IntPtr)m_pkMesh + 0))): " + *(int*)((IntPtr)m_pkMesh + 0)); // Name - Done
//log("*(int*)(*(int*)((IntPtr)m_pkMesh + 4))): " + *(int*)((IntPtr)m_pkMesh + 4)); // PrimaryVertexData - Done
//log("*(int*)(*(int*)((IntPtr)m_pkMesh + 8))): " + *(int*)((IntPtr)m_pkMesh + 8)); // nMorphTargets
//log("*(int*)(*(int*)((IntPtr)m_pkMesh + 12))): " + *(int*)((IntPtr)m_pkMesh + 12)); // MorphTargets
//log("*(int*)(*(int*)((IntPtr)m_pkMesh + 16))): " + *(int*)((IntPtr)m_pkMesh + 16)); // PrimaryTopology - ToDo
//log("*(int*)(*(int*)((IntPtr)m_pkMesh + 20))): " + *(int*)((IntPtr)m_pkMesh + 20)); // number of material bindings - ToDo
//log("*(int*)(*(int*)((IntPtr)m_pkMesh + 24))): " + *(int*)((IntPtr)m_pkMesh + 24)); // material bindings - ToDo
//log("*(int*)(*(int*)((IntPtr)m_pkMesh + 28))): " + *(int*)((IntPtr)m_pkMesh + 28)); // number of bone bindings - ToDo
//log("*(int*)(*(int*)((IntPtr)m_pkMesh + 32))): " + *(int*)((IntPtr)m_pkMesh + 32)); // bone bindings - ToDo
//log("*(int*)(*(int*)((IntPtr)m_pkMesh + 36))): " + *(int*)((IntPtr)m_pkMesh + 36)); // num Extended Datas
//log("*(int*)(*(int*)((IntPtr)m_pkMesh + 40))): " + *(int*)((IntPtr)m_pkMesh + 40)); // Extended Data
// size = 44,   11 pointers

//int vertexComponentCount = *(int*)(*(int*)(meshWrapper.getPrimaryVertexDataPtr()) + 12);
//IntPtr vertexComponentPtr = (IntPtr)(*(int*)(meshWrapper.getPrimaryVertexDataPtr()) + 16);
//List<string> vertexComponentNames = new List<string>();
//for (int i = 0; i < vertexComponentCount; i++)
//{
//    vertexComponentNames.Add(Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(vertexComponentPtr) + i * 4)));
//}

//granny_real32 : float
//granny_uint8  : byte
//granny_real16 : UInt16


//Addition Vertex Components:
// Tangent 
// Binormal 
// DiffuseColor
// TextureCoords1 