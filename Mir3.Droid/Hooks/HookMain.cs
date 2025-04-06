using Android.Views;
using Android.Widget;
using Google.Android.Material.TextView;
using System;

namespace Mir3.Droid.Hooks
{
    public class HookMain
    {
        public Action Cancel;

        public HookMain(View view)
        {
            var button = view.FindViewById<ImageButton>(Resource.Id.h_main_cancel);
            button.Click += Button_Click;

            var layout = view.FindViewById<LinearLayout>(Resource.Id.hook_weather);
            layout.Click += Layout_Click;
        }

        private void Layout_Click(object sender, EventArgs e)
        {
            var data = new string[] { "未选择", "晴", "雾", "雪", "雨" };
            MainActivity.Main.PickDatas = data;
            MainActivity.Main.PickerEvent = (index) =>
            {
                var item = data[index];
                var textView = (sender as LinearLayout).FindViewById<MaterialTextView>(Resource.Id.hook_weather_txt);
                textView.Text = item;
            };
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Cancel?.Invoke();
        }
    }
}

