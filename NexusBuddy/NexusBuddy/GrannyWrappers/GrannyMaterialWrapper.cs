using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firaxis.Framework.Granny;
using System.Reflection;
using System.Runtime.InteropServices;
using NexusBuddy.Utils;

namespace NexusBuddy.GrannyWrappers
{
    public unsafe class GrannyMaterialWrapper
    {
        public IGrannyMaterial wrappedMaterial;
        public void* m_pkMaterial;

        public GrannyMaterialWrapper(IGrannyMaterial inputMaterial)
        {
            wrappedMaterial = inputMaterial;
            Type myType = inputMaterial.GetType();
            FieldInfo fm_pkMaterial = myType.GetField("m_pkMaterial", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Pointer pm_pkMaterial = (Pointer)fm_pkMaterial.GetValue(inputMaterial);
            m_pkMaterial = (void*)Pointer.Unbox(pm_pkMaterial);
        }

        public unsafe String getName()
        {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(m_pkMaterial));
        }

        public unsafe void setName(string name)
        {
            *(int*)(m_pkMaterial) = (int)MemoryUtil.getStringIntPtr(name);
        }

        public IntPtr get16Ptr()
        {
            return ((IntPtr)m_pkMaterial + 16);
        }

        public IntPtr getExtendedDataPtr()
        {
            return ((IntPtr)m_pkMaterial + 20);
        }

        public string getShaderSetName()
        {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)*(int*)(getExtendedDataPtr()));
        }
    }
}
