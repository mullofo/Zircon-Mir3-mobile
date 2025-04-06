using Client.Models;
using System.IO;

namespace Client.Scenes.Views
{
    public partial class HookHelper
    {
        public static void LoadPickFilterConfig(UserObject user)
        {
            var file = $@".\Data\Saved\{user.Name}_Pick.cfg";

            if (!Directory.Exists(Path.GetDirectoryName(file)))
                Directory.CreateDirectory(Path.GetDirectoryName(file));

            //if (!File.Exists(file))
            //    File.WriteAllText(file, string.Empty);

            //GameScene.Game.BigPatchBox.AutoPick.ItemFilter.FileName = file;
            //GameScene.Game.BigPatchBox.AutoPick.Initialize();
        }
    }
}
