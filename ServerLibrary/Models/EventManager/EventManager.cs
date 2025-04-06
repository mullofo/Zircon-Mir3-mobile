using System.Collections.Generic;


/*
 * 事件管理器 v1.0 by 奥利奥
 * todo 支持python事件
 */
namespace Server.Models.EventManager
{
    /// <summary>
    /// 这个delegate必须被其他event handler所implement. 需要使用
    /// <see cref="EventManager.RegisterListener" /> 进行注册.
    /// </summary>
    /// <param name="evt">
    /// <see cref="IEvent"/> 事件管理器收到的事件
    /// </param>
    public delegate void EventHandlerDelegate(IEvent evt);

    /// <summary>
    /// EventManager, 负责触发事件的handler
    /// <para>
    /// 事件会被 <see cref="EventManager.QueueEvent" /> 加入队列. 仅当执行
    /// <see cref="EventManager.ProcessEvents" /> 时才会触发.
    /// 新的事件会被下一次 <see cref="EventManager.ProcessEvents" />所处理.
    /// </para>
    /// <para>
    /// 对于需要被尽快处理的事件,请使用 <see cref="EventManager.FireEvent" />. 但是请保守地使用它.
    /// </para>
    /// <para>
    /// 注意, 这里并不执行null check
    /// </para>
    /// </summary>
    public class EventManager
    {
        /// <summary>
        /// 维护一个字典 事件类型: 事件的delegate 
        /// 当处理事件的时候, 这个字典被用来获取所有特定类型的, 已注册的delegate
        /// Handlers 使用 <see cref="EventManager.RegisterListener" /> 进行注册
        /// 使用 <see cref="EventManager.RemoveListener" /> 进行移除.
        /// </summary>
        private Dictionary<object, EventHandlerDelegate> handlers = new Dictionary<object, EventHandlerDelegate>();

        /// <summary>
        /// 队列中的事件.
        /// </summary>
        private List<IEvent> queuedEvents = new List<IEvent>();

        /// <summary>
        /// 注册一个event handler.
        /// </summary>
        /// <param name="eventType">
        /// <see cref="System.Object"/> 事件类型
        /// 不可以是Null
        /// </param>
        /// <param name="eventHandler">
        /// <see cref="EventHandlerDelegate"/> 的delegate, 事件发生时call.
        /// 不可以是Null
        /// </param>
        public void RegisterListener(object eventType, EventHandlerDelegate eventHandler)
        {
            EventHandlerDelegate handler;
            if (this.handlers.TryGetValue(eventType, out handler))
            {
                // 首先移除handler, 避免handler被添加两次的情况
                handler -= eventHandler;
                handler += eventHandler;

                // Don't forget to re-assign the handler to, as delegates have overloaded
                // + and - operators, making them essentially behave like immutable objects.
                // They may also change state from being a Delegate object to becoming a
                // MulticastDelegate thingy, which requires those overloads.
                this.handlers[eventType] = handler;
            }
            else
            {
                // 这个类型的事件还没有 handler
                // 添加到字典
                handler = eventHandler;
                this.handlers.Add(eventType, handler);
            }
        }

        /// <summary>
        /// 移除已注册的event handler
        /// </summary>
        /// <param name="eventType">
        /// <see cref="System.Object"/> 事件类型
        /// 不可以是Null
        /// </param>
        /// <param name="eventHandler">
        /// <see cref="EventHandlerDelegate"/> 的delegate, 事件发生时call.
        /// 不可以是Null
        /// </param>
        public void RemoveListener(object eventType, EventHandlerDelegate eventHandler)
        {
            EventHandlerDelegate handler;
            if (this.handlers.TryGetValue(eventType, out handler))
            {
                handler -= eventHandler;
            }
        }

        /// <summary>
        /// 把一个待处理的事件加入队列. 仅当 <see cref="EventManager.ProcessEvents" />
        /// 被执行时, 此事件的handler才会被执行
        /// </summary>
        /// 下次 <see cref="EventManager.ProcessEvents" /> 执行时,
        /// <param name="evt">
        /// <see cref="IEvent"/> 事件被发送到所有对应类型的listener
        /// </param>
        public void QueueEvent(IEvent evt)
        {
            // todo 暂时停用
            //this.queuedEvents.Add(evt);
        }

        /// <summary>
        /// 立刻执行一个事件
        /// <see cref="EventManager.QueueEvent" /> 队列中的事件保持不变
        /// </summary>
        /// <param name="evt">
        /// <see cref="IEvent"/> 需要立刻被执行的事件
        /// </param>
        public void FireEvent(IEvent evt)
        {
            this.ProcessEvent(evt);
        }

        /// <summary>
        /// 处理队列中的所有事件. 在当前帧中只能执行一次.
        /// </summary>
        public void ProcessEvents()
        {
            // 用一下copy constructor, 新列表可以知道自己的长度
            List<IEvent> currentEvents = new List<IEvent>(this.queuedEvents);

            // 清空旧的队列
            this.queuedEvents.Clear();

            // 挨个处理
            foreach (IEvent evt in currentEvents)
            {
                this.ProcessEvent(evt);
            }
        }

        /// <summary>
        /// 处理一个给定的事件. 
        /// 供<see cref="EventManager.FireEvent" /> 和 <see cref="EventManager.ProcessEvents" />使用.
        /// 尝试拿到对应事件类型的handler, 然后执行handler
        /// </summary>
        /// <param name="evt">
        /// <see cref="IEvent"/> 事件
        /// </param>
        private void ProcessEvent(IEvent evt)
        {
            EventHandlerDelegate handler;
            if (this.handlers.TryGetValue(evt.EventType, out handler))
            {
                handler(evt);
            }
        }

        public void ClearQueue()
        {
            // 清空旧的队列
            this.queuedEvents.Clear();
        }
    }
}
