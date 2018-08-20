using System;
using Firaxis.Framework.Granny;

namespace NexusBuddy
{
    internal class ShaderUtils
    {
        public static string getStringValuebyParamSetAndName(IGrannyMaterial material, string paramSetName, string paramName)
        {
            IFGXParameterSet paramSet = material.FindParameterSet(paramSetName);
            if (paramSet != null)
            {
                return paramSet.GetParameterValue(paramName) as string;
            }
            return null;
        }

        public static string trimPathFromFilename(String filename)
        {
            if (filename != null)
            {
                return filename.Substring(filename.LastIndexOf("\\") + 1);
            }
            else
            {
                return "";
            }
            
        }
    }
}
