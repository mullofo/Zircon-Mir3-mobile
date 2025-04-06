using CoreFoundation;
using SystemConfiguration;
using System.Net;
using System;

namespace Mir3.iOS
{
    public static class Reachability
    {
        private static NetworkReachability _defaultRouteReachability;

        /// <summary>
        /// 网络变化
        /// </summary>
        public static Action<NetworkReachabilityFlags> ReachabilityChanged;

        /// <summary>
        /// 初始监听
        /// </summary>
        public static void Init()
        {
            if (_defaultRouteReachability == null)
            {
                _defaultRouteReachability = new NetworkReachability(new IPAddress(0));
                _defaultRouteReachability.SetNotification(OnChange);
                _defaultRouteReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }
        }

        public static bool IsNetworkAvailable()
        {
            NetworkReachabilityFlags flags;

            return _defaultRouteReachability.TryGetFlags(out flags) &&
                IsReachableWithoutRequiringConnection(flags);
        }

        private static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
        {
            // 当前网络配置是否可访问?
            bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

            // 需要连接吗?
            bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0;

            // 由于网络堆栈将自动尝试接通WAN
            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
                noConnectionRequired = true;

            return isReachable && noConnectionRequired;
        }

        private static void OnChange(NetworkReachabilityFlags flags)
        {
            ReachabilityChanged?.Invoke(flags);
        }
    }
}
