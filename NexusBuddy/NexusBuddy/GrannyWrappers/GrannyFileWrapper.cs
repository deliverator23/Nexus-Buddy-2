using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firaxis.Framework.Granny;
using System.Reflection;
using System.Runtime.InteropServices;
using NexusBuddy.Utils;
using NexusBuddy.GrannyInfos;

namespace NexusBuddy.GrannyWrappers
{
    public unsafe class GrannyFileWrapper
    {
        public IGrannyFile wrappedFile;
        public void* m_info;

        public GrannyFileWrapper(IGrannyFile inputFile)
        {
            wrappedFile = inputFile;
            Type myType = inputFile.GetType();
            FieldInfo fm_info = myType.GetField("m_info", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Pointer pm_info = (Pointer)fm_info.GetValue(inputFile);
            m_info = Pointer.Unbox(pm_info);
        }

        public IntPtr getArtToolInfoPtr()
        {
            return ((IntPtr)m_info + 0);
        }

        public unsafe void setFromArtToolInfo(string toolName, int majorNum, int minorNum)
        {
            setFromArtToolName(toolName);
            setArtToolRevisionNumbers(majorNum, minorNum);
        }

        public unsafe String getArtToolInfoName()
        {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(getArtToolInfoPtr())));
        }

        public unsafe void setFromArtToolName(string meshName)
        {
            sbyte* stringPointer = (sbyte*)Marshal.StringToHGlobalAnsi(meshName).ToPointer();
            IntPtr spointer = new IntPtr((void*)stringPointer);
            *(int*)*(int*)(getArtToolInfoPtr()) = (int)spointer;
        }

        public unsafe void setArtToolRevisionNumbers(int majorNum, int minorNum)
        {
            *(int*)(*(int*)(getArtToolInfoPtr()) + 4) = majorNum;
            *(int*)(*(int*)(getArtToolInfoPtr()) + 8) = minorNum;
        }

        public unsafe void setUnitsPerMeter(float unitsPerMeter)
        {
            *(float*)(*(int*)(getArtToolInfoPtr()) + 16) = unitsPerMeter;
        }

        public unsafe void setOrigin(float[] origin)
        {
            *(float*)(*(int*)(getArtToolInfoPtr()) + 20) = origin[0];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 24) = origin[1];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 28) = origin[2];
        }

        public unsafe void setMatrix(float[] matrix)
        {
            *(float*)(*(int*)(getArtToolInfoPtr()) + 32) = matrix[0];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 36) = matrix[1];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 40) = matrix[2];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 44) = matrix[3];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 48) = matrix[4];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 52) = matrix[5];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 56) = matrix[6];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 60) = matrix[7];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 64) = matrix[8];
        }

        public IntPtr getExporterInfoPtr()
        {
            return ((IntPtr)m_info + 4);
        }

        public unsafe void setExporterInfo(string name, int majorNum, int minorNum, int customization, int build)
        {
            setExporterName(name);
            setExporterNumbers(majorNum, minorNum, customization, build);
        }
        
        public unsafe string getExporterName() {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(getExporterInfoPtr())));
        }

        public unsafe void setExporterName(string meshName)
        {
            sbyte* stringPointer = (sbyte*)Marshal.StringToHGlobalAnsi(meshName).ToPointer();
            IntPtr spointer = new IntPtr((void*)stringPointer);
            *(int*)*(int*)(getExporterInfoPtr()) = (int)spointer;
        }

        public unsafe void setExporterNumbers(int majorNum, int minorNum, int customization, int build)
        {
            *(int*)(*(int*)(getExporterInfoPtr()) + 4) = majorNum;
            *(int*)(*(int*)(getExporterInfoPtr()) + 8) = minorNum;
            *(int*)(*(int*)(getExporterInfoPtr()) + 12) = customization;
            *(int*)(*(int*)(getExporterInfoPtr()) + 16) = build;
        }

        public unsafe void setFromFileName(string meshName)
        {
            sbyte* stringPointer = (sbyte*)Marshal.StringToHGlobalAnsi(meshName).ToPointer();
            IntPtr spointer = new IntPtr((void*)stringPointer);
            *(int*)((IntPtr)m_info + 8) = (int)spointer;
        }

        public int getNumMeshes()
        {
            return *(int*)((IntPtr)m_info + 52);
        }

        public void setNumMeshes(int numMeshes)
        {
            *(int*)((IntPtr)m_info + 52) = numMeshes;
        }

        public IntPtr getMeshesPtr()
        {
            return ((IntPtr)m_info + 56);
        }

        public int getNumMaterials()
        {
            return *(int*)((IntPtr)m_info + 20);
        }

        public void setNumMaterials(int num)
        {
            *(int*)((IntPtr)m_info + 20) = num;
        }

        public IntPtr getMaterialsPtr()
        {
            return ((IntPtr)m_info + 24);
        }

        public int getNumModels()
        {
            return *(int*)((IntPtr)m_info + 60);
        }

        public void setNumModels(int num)
        {
            *(int*)((IntPtr)m_info + 60) = num;
        }

        public IntPtr getModelsPtr()
        {
            return ((IntPtr)m_info + 64);
        }

        public int getNumTrackGroups()
        {
            return *(int*)((IntPtr)m_info + 68);
        }

        public void setNumTrackGroups(int num)
        {
            *(int*)((IntPtr)m_info + 68) = num;
        }

        public IntPtr getTrackGroupsPtr()
        {
            return ((IntPtr)m_info + 72);
        }

        public int getNumAnimations()
        {
            return *(int*)((IntPtr)m_info + 76);
        }

        public void setNumAnimations(int num)
        {
            *(int*)((IntPtr)m_info + 76) = num;
        }

        public IntPtr getAnimationsPtr()
        {
            return ((IntPtr)m_info + 80);
        }

        public void setAnimationsPtr(int num)
        {
            *(int*)((IntPtr)m_info + 80) = num;
        }


        public int getNumSkeletons()
        {
            return *(int*)((IntPtr)m_info + 28);
        }

        public void setNumSkeletons(int num)
        {
            *(int*)((IntPtr)m_info + 28) = num;
        }

        public IntPtr getSkeletonsPtr()
        {
            return ((IntPtr)m_info + 32);
        }

        public int getNumVertexDatas()
        {
            return *(int*)((IntPtr)m_info + 36);
        }

        public void setNumVertexDatas(int num)
        {
            *(int*)((IntPtr)m_info + 36) = num;
        }

        public IntPtr getVertexDatasPtr()
        {
            return ((IntPtr)m_info + 40);
        }

        public int getNumTriTopologies()
        {
            return *(int*)((IntPtr)m_info + 44);
        }

        public void setNumTriTopologies(int num)
        {
            *(int*)((IntPtr)m_info + 44) = num;
        }

        public IntPtr getTriTopologiesPtr()
        {
            return ((IntPtr)m_info + 48);
        }



        public void addSkeletonPointer(int skeletonPtr) {

            int oldNumSkeletons = getNumSkeletons();
            int newNumSkeletons = oldNumSkeletons + 1;

            int oldSkeletonsPtr = *(int*)getSkeletonsPtr();
            *(int*)(getSkeletonsPtr()) = (int)Marshal.AllocHGlobal(newNumSkeletons * 4);
            int newSkeletonsPtr = *(int*)getSkeletonsPtr();

            MemoryUtil.MemCpy((void*)newSkeletonsPtr, (void*)oldSkeletonsPtr, (uint) oldNumSkeletons * 4);

            *(int*)(newSkeletonsPtr + oldNumSkeletons * 4) = skeletonPtr;

            setNumSkeletons(newNumSkeletons);
        }

        public void addVertexDatasPointer(int skeletonPtr)
        {
            int oldNumVertexDatas = getNumVertexDatas();
            int newNumVertexDatas = oldNumVertexDatas + 1;

            int oldVertexDatasPtr = *(int*)getVertexDatasPtr();
            *(int*)(getVertexDatasPtr()) = (int)Marshal.AllocHGlobal(newNumVertexDatas * 4);
            int newVertexDatasPtr = *(int*)getVertexDatasPtr();

            MemoryUtil.MemCpy((void*)newVertexDatasPtr, (void*)oldVertexDatasPtr, (uint)oldNumVertexDatas * 4);

            *(int*)(newVertexDatasPtr + oldNumVertexDatas * 4) = skeletonPtr;

            setNumVertexDatas(newNumVertexDatas);
        }

        public void addTriTopologiesPointer(int skeletonPtr)
        {

            int oldNumTriTopologies = getNumTriTopologies();
            int newNumTriTopologies = oldNumTriTopologies + 1;

            int oldTriTopologiesPtr = *(int*)getTriTopologiesPtr();
            *(int*)(getTriTopologiesPtr()) = (int)Marshal.AllocHGlobal(newNumTriTopologies * 4);
            int newTriTopologiesPtr = *(int*)getTriTopologiesPtr();

            MemoryUtil.MemCpy((void*)newTriTopologiesPtr, (void*)oldTriTopologiesPtr, (uint)oldNumTriTopologies * 4);

            *(int*)(newTriTopologiesPtr + oldNumTriTopologies * 4) = skeletonPtr;

            setNumTriTopologies(newNumTriTopologies);
        }
    }
}
