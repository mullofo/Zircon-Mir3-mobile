using Client.Envir;
using Client.Models;
using System.IO;

namespace Client.Scenes.Views
{
    public partial class HookHelper
    {
        public static void LoadPickFilterConfig(UserObject user)
        {
            var file = CEnvir.MobileClientPath + $@"Data/Saved/{user.Name}_Pick.cfg";
            var path = Path.GetDirectoryName(file);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //if (!File.Exists(file))
            //    File.WriteAllText(file, string.Empty);

            //GameScene.Game.BigPatchBox.AutoPick.ItemFilter.FileName = file;
            //GameScene.Game.BigPatchBox.AutoPick.Initialize();
        }
    }
}
