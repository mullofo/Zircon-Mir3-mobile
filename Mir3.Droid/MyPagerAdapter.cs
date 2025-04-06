using Android.Content;
using Android.Views;
using AndroidX.ViewPager.Widget;
using System;

namespace Mir3.Droid
{
    public class MyPagerAdapter : PagerAdapter
    {
        Context _context;
        public Action Close;
        public MyPagerAdapter(Context context)
        {
            _context = context;
        }
        public override int Count => 4;

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }
        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            var view = GetView(position);
            container.AddView(view);
            return view;
        }

        private void View_Click(object sender, EventArgs e)
        {
            Close?.Invoke();
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }

        private View GetView(int index)
        {
            var activity = _context as MainActivity;
            View view;
            switch (index)
            {
                case 0:
                    view = activity.LayoutInflater.Inflate(Resource.Layout.hook_main, null);
                    var hookMain = new Hooks.HookMain(view);
                    hookMain.Cancel = Close;
                    break;
                case 1:
                    view = activity.LayoutInflater.Inflate(Resource.Layout.hook_bh, null);
                    break;
                case 2:
                    view = activity.LayoutInflater.Inflate(Resource.Layout.hook_mf, null);
                    break;
                case 3:
                    view = activity.LayoutInflater.Inflate(Resource.Layout.hook_gw, null);
                    break;
                default:
                    view = activity.LayoutInflater.Inflate(Resource.Layout.hook_main, null);
                    break;
            }
            return view;
        }

    }
}
