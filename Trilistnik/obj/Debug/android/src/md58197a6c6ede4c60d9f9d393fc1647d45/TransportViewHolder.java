package md58197a6c6ede4c60d9f9d393fc1647d45;


public class TransportViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Trilistnik.TransportViewHolder, Trilistnik, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", TransportViewHolder.class, __md_methods);
	}


	public TransportViewHolder (android.view.View p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == TransportViewHolder.class)
			mono.android.TypeManager.Activate ("Trilistnik.TransportViewHolder, Trilistnik, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Views.View, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
