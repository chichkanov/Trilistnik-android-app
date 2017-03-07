package md58197a6c6ede4c60d9f9d393fc1647d45;


public class JsonAsyncLoader
	extends android.content.AsyncTaskLoader
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onStartLoading:()V:GetOnStartLoadingHandler\n" +
			"n_loadInBackground:()Ljava/lang/Object;:GetLoadInBackgroundHandler\n" +
			"";
		mono.android.Runtime.register ("Trilistnik.JsonAsyncLoader, Trilistnik, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", JsonAsyncLoader.class, __md_methods);
	}


	public JsonAsyncLoader (android.content.Context p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == JsonAsyncLoader.class)
			mono.android.TypeManager.Activate ("Trilistnik.JsonAsyncLoader, Trilistnik, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public void onStartLoading ()
	{
		n_onStartLoading ();
	}

	private native void n_onStartLoading ();


	public java.lang.Object loadInBackground ()
	{
		return n_loadInBackground ();
	}

	private native java.lang.Object n_loadInBackground ();

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
