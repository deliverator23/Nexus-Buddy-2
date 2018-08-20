using System;
using System.Collections.Generic;
using Firaxis.Framework.Granny;
using System.Reflection;
using System.Runtime.InteropServices;
using NexusBuddy.GrannyInfos;
using NexusBuddy.Utils;

namespace NexusBuddy.GrannyWrappers
{
    public unsafe class GrannySkeletonWrapper
    {
        private int boneSize = 152;

        public IGrannySkeleton wrappedSkeleton;
        public DummyGrannySkeleton* m_pkSkeleton;

        public GrannySkeletonWrapper(IGrannySkeleton inputSkeleton)
        {
            wrappedSkeleton = inputSkeleton;
            Type myType = inputSkeleton.GetType();
            FieldInfo fm_pkSkeleton = myType.GetField("m_pkSkeleton", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Pointer pm_pkSkeleton = (Pointer)fm_pkSkeleton.GetValue(inputSkeleton);
            m_pkSkeleton = (DummyGrannySkeleton*)Pointer.Unbox(pm_pkSkeleton);
        }

        public void setName(string name)
        {
            *(int*)(m_pkSkeleton) = (int)MemoryUtil.getStringIntPtr(name);
        }

        public int getNumBones()
        {
            return *(int*)((IntPtr)m_pkSkeleton + 4);
        }

        public void setNumBones(int num)
        {
            *(int*)((IntPtr)m_pkSkeleton + 4) = num;
        }

        public IntPtr getBonesPtr()
        {
            return ((IntPtr)m_pkSkeleton + 8);
        }

        public GrannySkeletonInfo readSkeletonInfo()
        {
            GrannySkeletonInfo skeletonInfo = new GrannySkeletonInfo();
            List<GrannyBoneInfo> boneInfos = new List<GrannyBoneInfo>();

            for (int bi = 0; bi < wrappedSkeleton.Bones.Count; bi++)
            {
                IGrannyBone bone = wrappedSkeleton.Bones[bi];
                GrannyBoneInfo boneInfo = new GrannyBoneInfo();
                boneInfo.name = bone.Name;
                boneInfo.parentIndex = bone.ParentIndex;
                GrannyTransformInfo boneTransformInfo = new GrannyTransformInfo();

                IGrannyTransform boneTransform = bone.LocalTransform;
                boneTransformInfo.flags = GrannyTransformInfo.getFlagsInt(boneTransform.Flags);

                GrannyBoneWrapper boneWrapper = new GrannyBoneWrapper(bone);
                boneTransformInfo.orientation = boneWrapper.getOrientation();
                boneTransformInfo.position = boneWrapper.getPosition();
                boneTransformInfo.scaleShear = boneWrapper.getScaleShear();      

                boneInfo.localTransform = boneTransformInfo;

                boneInfo.inverseWorldTransform = bone.InverseWorldTransform;
                boneInfo.LODError = bone.LODError;

                boneInfos.Add(boneInfo);
            }

            skeletonInfo.bones = boneInfos;
            return skeletonInfo;
        }

        public void writeSkeletonInfo(GrannySkeletonInfo skeletonInfo)
        {
            int bonesCount = skeletonInfo.bones.Count;
            setNumBones(bonesCount);

            int oldBonesPtr = *(int*)getBonesPtr();
            *(int*)(getBonesPtr()) = (int)Marshal.AllocHGlobal(bonesCount * boneSize);
            int newBonesPtr = *(int*)getBonesPtr();

            List<GrannyBoneInfo> boneInfos = skeletonInfo.bones;
            for (int i = 0; i < bonesCount; i++)
            {
                MemoryUtil.MemCpy((void*)(newBonesPtr + i * boneSize), (void*)oldBonesPtr, (uint)boneSize);

                GrannyBoneInfo currentBone = boneInfos[i];

                *(int*)(newBonesPtr + (i * boneSize) + 0) = (int)MemoryUtil.getStringIntPtr(currentBone.name);

                *(int*)(newBonesPtr + (i * boneSize) + 4) = currentBone.parentIndex;

                int flags = currentBone.localTransform.flags;

                *(int*)(newBonesPtr + (i * boneSize) + 8) = flags;           

                *(float*)(newBonesPtr + (i * boneSize) + 12) = currentBone.localTransform.position[0];
                *(float*)(newBonesPtr + (i * boneSize) + 16) = currentBone.localTransform.position[1];
                *(float*)(newBonesPtr + (i * boneSize) + 20) = currentBone.localTransform.position[2];

                *(float*)(newBonesPtr + (i * boneSize) + 24) = currentBone.localTransform.orientation[0];
                *(float*)(newBonesPtr + (i * boneSize) + 28) = currentBone.localTransform.orientation[1];
                *(float*)(newBonesPtr + (i * boneSize) + 32) = currentBone.localTransform.orientation[2];
                *(float*)(newBonesPtr + (i * boneSize) + 36) = currentBone.localTransform.orientation[3];

                for (int j = 0; j < 9; j++)
                {
                    *(float*)(newBonesPtr + (i * boneSize) + 40 + (j * 4)) = currentBone.localTransform.scaleShear[j];
                }

                for (int j = 0; j < 16; j++)
                {
                    *(float*)(newBonesPtr + (i * boneSize) + 76 + (j * 4)) = currentBone.inverseWorldTransform[j];
                }

                *(float*)(newBonesPtr + (i * boneSize) + 140) = 1.0f;

                // +140 LOD Error 
                // +144 Num Extended Datas
                // +148 Extended Datas Pointer
            }
        }
    }
}