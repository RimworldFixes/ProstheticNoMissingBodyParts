using System.Linq;
using Verse;

namespace ProstheticNoMissingBodyParts
{
    public static class BodyPartUtils
    {
        public static bool ExistsDeep(BodyPartRecord part, string groupDefName)
        {
            // check if correct part is founded
            if (part.groups.Exists((x) => x.defName.Equals(groupDefName))) return true;
            
            return part.parts != null &&
                   // recursive check for nested parts
                   Enumerable.Any(part.parts, nestedPart => ExistsDeep(nestedPart, groupDefName));
        }
    }
}