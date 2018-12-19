using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class CommandQueue
    {
        public delegate void RaiseCommandHandler(CustomRoutedCommand command);

        RaiseCommandHandler handler;

        public CommandQueue(RaiseCommandHandler handler)
        {
            this.handler = handler;
        }

        class QueuedEvent
        {
            public DateTime Time { get; set; }
            public CustomRoutedCommand Command { get; set; }
        }

        LinkedList<QueuedEvent> eventQueue = new LinkedList<QueuedEvent>();

        public void Add(TimeSpan when, CustomRoutedCommand command, bool deleteExisting = true)
        {
            Add(DateTime.Now + when, command, deleteExisting);
        }

        public void Add(DateTime when, CustomRoutedCommand command, bool deleteExisting = true)
        {
            if (deleteExisting)
            {
                Remove(command);
            }

            var newEvent = new QueuedEvent() { Time = when, Command = command };

            if (eventQueue.Count == 0)
            {
                eventQueue.AddFirst(newEvent);
            }
            else
            {
                var item = eventQueue.First;
                while (item != null)
                {
                    if (when < item.Value.Time)
                    {
                        eventQueue.AddBefore(item, newEvent);
                        return;
                    }
                    item = item.Next;
                }

                eventQueue.AddAfter(eventQueue.Last, newEvent);
            }
        }

        public void Remove(CustomRoutedCommand command)
        {
            var itemsToDelete = (from i in eventQueue where command == i.Command select i).ToList();
            foreach (var item in itemsToDelete)
            {
                eventQueue.Remove(item);
            }
        }

        public void Raise(DateTime now)
        {
            var item = eventQueue.First;

            while (item != null && item.Value.Time < now)
            {
                handler?.Invoke(item.Value.Command);
                eventQueue.RemoveFirst();
                item = eventQueue.First;
            }
        }
    }
}
