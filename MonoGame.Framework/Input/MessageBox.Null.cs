using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework.Input
{
    public partial class MessageBox
    {
        private static Task<int?> PlatformShow(string title, string description, List<string> buttons)
        {
            return null;
        }

        private static void PlatformCancel(int? result)
        {
        }
    }
}
