namespace Server.Models.EventManager
{
    public interface IEvent
    {
        /// <summary>
        /// Gets the type of the event.
        /// <para>
        /// For convenience, this is declared as an object, providing the freedom to use any
        /// type, that correctly overrides the <see cref="System.Object.Equals" /> and
        /// <see cref="System.Object.GetHashCode" />, as an event type. This includes
        /// <see cref="System.String" /> and <see cref="System.Int" />. However, for practical
        /// applications and a maintainable code base, it is highly recommended to use enums here.
        /// </para>
        /// <para>
        /// Please note, that this interface only requires a getter. This is intentional, as one
        /// usually creates a new event using a concrete class, implementing this interface. So
        /// during object creation, the type can be set. The event manager and event handlers on
        /// the other hand will use this interface, intentionally preventing them from modifying
        /// the type of the event.
        /// </para>
        /// </summary>
        object EventType { get; }

        /// <summary>
        /// Gets the data, to send with the event or null, if the event doesn't require any data.
        /// <para>
        /// This can be any kind of data, system types like <see cref="System.String" /> or
        /// <see cref="System.Int" />. However, experience shows that these data sets tend to
        /// grow. When building editors, they also need to be reflected on, for which simple
        /// types like the above usually need to be special cased. For that reason, it is a
        /// very good habbit to always create a special class, containing all the values you
        /// want to send with the event. When doing this, it is also very important to actually
        /// store the values in properties, whose name reflects the meaning of the value, so
        /// event queueing code and event handlers remain readable and maintainable.
        /// </para>
        /// </summary>
        object EventData { get; }
    }
}
