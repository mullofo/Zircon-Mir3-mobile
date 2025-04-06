namespace Client.Controls
{
    public static class DXControlEx
    {
        public static void TryDispose(this DXControl control)
        {
            if (control != null)
            {
                if (!control.IsDisposed)
                    control.Dispose();

                control = null;
            }
        }
    }
}
