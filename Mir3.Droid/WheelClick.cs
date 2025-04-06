using Com.Aigestudio.Wheelpicker;

namespace Mir3.Droid
{
    public class WheelClick : Java.Lang.Object, WheelPicker.IOnItemSelectedListener
    {

        public void OnItemSelected(WheelPicker p0, Java.Lang.Object p1, int p2)
        {
            MainActivity.Main.ShowPick = false;
            MainActivity.Main.PickerEvent?.Invoke(p2);
            MainActivity.Main.PickerEvent = null;
        }
    }
}
