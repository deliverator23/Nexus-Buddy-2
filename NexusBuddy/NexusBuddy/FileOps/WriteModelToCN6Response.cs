using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexusBuddy.FileOps
{
    class WriteModelToCN6Response
    {
        public List<string> decalMeshNames;
        public List<string> textureMaps;

        public WriteModelToCN6Response(List<string> decalMeshNames, List<string> textureMaps)
        {
            this.decalMeshNames = decalMeshNames;
            this.textureMaps = textureMaps;
        }
    }
}
