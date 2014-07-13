//C# Unity event manager that uses strings in a hashtable over delegates and events in order to
//allow use of events without knowing where and when they're declared/defined.
//by Billy Fletcher of Rubix Studios
using UnityEngine;
using System.Collections;

public interface IEventListener
{
    string name { get; }

    void HandleEvent(IEvent evt);
}

public class IEvent
{
    public bool consumed { get; set; }

    string name = null;

    public string GetName() 
    { 
        if ( name == null ) name = GetType().FullName;

        return name;
    }

    public void Consume()
    {
        consumed = true;
    }
}


public class DelegateEventListener<T> : IEventListener where T : IEvent
{

	public delegate void EventHandler( T evt );

    public string name 
    {
        get 
        {
            return m_handler.ToString();
        }
    }
		
	private EventHandler   m_handler;
	private string         m_eventName;
	
	public DelegateEventListener( EventHandler handler )
	{
		m_handler = handler;
		m_eventName = typeof(T).FullName;
		EventManager.instance.AddListener( this, m_eventName );
	}

    public void Attach()
    {
        EventManager.instance.AddListener( this, m_eventName );    
    }
	
	public void Detach()
	{
		EventManager em = EventManager.instance;
		if ( em )
		{
			em.DetachListener( this, m_eventName );
		}
	}
	
	public void HandleEvent( IEvent evt )
	{
		m_handler( (T)evt );
	}
}

public class EventManager : MonoBehaviour
{
    public static EventManager instance
    {
        get;
		
        private set;
    }
   
    public void Awake()
    {
    	instance = this;

        Resources.UnloadUnusedAssets();
    }

    private Hashtable m_listenerTable = new Hashtable();
    private Queue m_eventQueue = new Queue();


    //Add a listener to the event manager that will receive any events of the supplied event name.
    public bool AddListener(IEventListener listener, string eventName)
    {
        if (listener == null || eventName == null)
        {
            Debug.Log("Event Manager: AddListener failed due to no listener or event name specified.");
            return false;
        }
		
        if ( !m_listenerTable.ContainsKey(eventName) )
        {
            m_listenerTable.Add(eventName, new ArrayList());
        }

        ArrayList listenerList = m_listenerTable[eventName] as ArrayList;
        if ( listenerList.Contains(listener) )
        {
            // Debug.Log("Event Manager: " + listener.name + "("+ listener.GetType().ToString() + ") is already in list for event: " + eventName);
            return false; //listener already in list
        }

        listenerList.Add(listener);
        return true;
    }

    public bool AddListener<T>( IEventListener listener ) where T : IEvent
    {
        return AddListener( listener, typeof(T).FullName );
    }
    
    public DelegateEventListener<T> AddListener<T>( DelegateEventListener<T>.EventHandler handler ) where T : IEvent
    {
    	DelegateEventListener<T> listener = new DelegateEventListener<T>( handler );
    	return listener;
    }

    //Remove a listener from the subscribed to event.
    public bool DetachListener(IEventListener listener, string eventName)
    {
        if (!m_listenerTable.ContainsKey(eventName))
            return false;
		
        ArrayList listenerList = m_listenerTable[eventName] as ArrayList;
        if (!listenerList.Contains(listener))
            return false;
		
        listenerList.Remove(listener);
        return true;
    }

    public bool DetachListener<T>( IEventListener listener ) where T : IEvent 
    {
        return DetachListener( listener, typeof(T).FullName );
    }

    static int indent = -1;
    static bool profile = false;

    //Trigger the event instantly, this should only be used in specific circumstances,
    //the QueueEvent function is usually fast enough for the vast majority of uses.
    public bool TriggerEvent(IEvent evt)
    {
        string eventName = evt.GetName();
        if (!m_listenerTable.ContainsKey(eventName))
        {
            //Debug.Log("Event Manager: Event \"" + eventName + "\" triggered has no listeners!");
            return false; //No listeners for event so ignore it
        }

        indent += 1;

        // clean it up
        evt.consumed = false;

        ArrayList listenerList = m_listenerTable[eventName] as ArrayList;
        for( int li = 0; li < listenerList.Count; ++li )
        {
            IEventListener listener = listenerList[li] as IEventListener;

            // don't try to do shit with null listeners
            if ( listener == null )
            {
                Debug.LogWarning( "Found a null listener in the list for " + eventName + "! Try not to let this happen!" );
                listenerList.RemoveAt(li);
                --li;
                continue;
            }

            float startTime  = Time.realtimeSinceStartup;
            
/*
            if ( evt.GetName() != "SequenceNotePlayed" )
            {
                print( "Sending " + evt.GetName() + " to " + listener.name );
            }
*/

            listener.HandleEvent(evt);

            if ( profile )
            {
                float timeTook   = Time.realtimeSinceStartup - startTime;
                if ( timeTook > 0.001 )
                {
                    string tabs = "";
                    for( int i = 0; i < indent; ++i ) tabs += "\t";
                    Debug.LogWarning( tabs + listener.name + " took " + timeTook.ToString("0.000") + " seconds to handle " + eventName );
                }
            }

            if ( evt.consumed ) break;
        }

        indent -= 1;

        return true;
    }

    //Inserts the event into the current queue.
    public bool QueueEvent(IEvent evt)
    {
        if (!m_listenerTable.ContainsKey(evt.GetName()))
        {
            //Debug.Log("EventManager: QueueEvent failed due to no listeners for event: " + evt.GetName());
            return false;
        }

/*
        if ( evt.GetName() != "SequenceNotePlayed" )
        {
            print( "Queuing Event: " + evt.GetName() );
        }
*/
        m_eventQueue.Enqueue(evt);

        return true;
    }

    //Every update cycle the queue is processed, if the queue processing is limited,
    //a maximum processing time per update can be set after which the events will have
    //to be processed next update loop.
    void LateUpdate()
    {
        while (m_eventQueue.Count > 0)
        {
            IEvent evt = m_eventQueue.Dequeue() as IEvent;
            if ( TriggerEvent(evt) == false )
            {
                Debug.Log("Error when processing event: " + evt.GetName());
            }
        }
    }

    public void OnApplicationQuit()
    {
        m_listenerTable.Clear();
        m_eventQueue.Clear();
        instance = null;
    }
}