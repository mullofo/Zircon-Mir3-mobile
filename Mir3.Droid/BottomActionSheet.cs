using Android.Content;
using Android.Widget;
using Google.Android.Material.BottomSheet;

namespace Mir3.Droid
{
    public class BottomActionSheet : BottomSheetDialog
    {
        int height = 0;
        public BottomActionSheet(Context context) : base(context)
        {
        }
        public override void Show()
        {
            base.Show();
            if (height == 0)
            {
                height = 800;
                FrameLayout bottomSheet = (FrameLayout)FindViewById(Resource.Id.design_bottom_sheet);
                bottomSheet.Background = null;
                var behavior = BottomSheetBehavior.From(bottomSheet);
                behavior.PeekHeight = height;
            }
        }
    }
}

