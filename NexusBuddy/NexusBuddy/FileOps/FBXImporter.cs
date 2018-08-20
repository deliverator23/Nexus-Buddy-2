using Firaxis.Framework.Export;

namespace NexusBuddy.FileOps
{
    public class FBXImporter
    {
		public static bool ImportFBXFile(string inputFilename, string outputFilename, string template)
		{
			return GrannyExporterFBX.ExportFBXFile(inputFilename, outputFilename, template);
        }
    }
}
