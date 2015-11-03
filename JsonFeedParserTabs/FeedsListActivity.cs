using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace JsonFeedParserTabs
{
	[Activity (Label = "Feed Reader", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		ListView listView;
		ProgressBar progressBar;
		RootObject result;

		string url = "http://javatechig.com/api/get_category_posts/?dev=1&slug=android";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource.
			SetContentView (Resource.Layout.Main);

			this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

			AddTab (" Tab 1", new SampleTabFragment (this));
			AddTab (" Tab 2", new SampleTabFragment2 (this));

			if (bundle != null)
				this.ActionBar.SelectTab(this.ActionBar.GetTabAt(bundle.GetInt("tab")));
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutInt("tab", this.ActionBar.SelectedNavigationIndex);

			base.OnSaveInstanceState(outState);
		}

		void AddTab (string tabText, Fragment view)
		{
			var tab = this.ActionBar.NewTab ();

			tab.SetText (tabText);

			// Must set event handler before adding tab.
			tab.TabSelected += delegate(object sender, ActionBar.TabEventArgs e) {
				var fragment = this.FragmentManager.FindFragmentById(Resource.Id.fragmentContainer);

				if (fragment != null)
					e.FragmentTransaction.Remove(fragment);

				e.FragmentTransaction.Add (Resource.Id.fragmentContainer, view);
			};
			tab.TabUnselected += delegate(object sender, ActionBar.TabEventArgs e) {
				e.FragmentTransaction.Remove(view);
			};

			this.ActionBar.AddTab (tab);
		}

		class SampleTabFragment : Fragment
		{
			private MainActivity context;
			public SampleTabFragment(MainActivity _context) : base()
			{
				this.context = _context;
			}

			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				base.OnCreateView (inflater, container, savedInstanceState);

				var view = context.LayoutInflater.Inflate (Resource.Layout.Tab, container, false);

				// Initializing listView.
				context.listView = view.FindViewById<ListView> (Resource.Id.listView);
				context.listView.ItemClick += context.OnListItemClick;
	
				context.progressBar = view.FindViewById<ProgressBar> (Resource.Id.progressBar);
	
				// Showing loading progressBar.
				context.progressBar.Visibility = ViewStates.Visible;
	
				// Download and display data in url.
				context.downloadJsonFeedAsync (context.url);

				return view;
			}
		}

		class SampleTabFragment2 : Fragment
		{
			private MainActivity context;
			public SampleTabFragment2(MainActivity _context) : base()
			{
				this.context = _context;
			}

			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				base.OnCreateView (inflater, container, savedInstanceState);

				var view = context.LayoutInflater.Inflate (Resource.Layout.Tab, container, false);

				// Initializing listView.
				context.listView = view.FindViewById<ListView> (Resource.Id.listView);
				context.listView.ItemClick += context.OnListItemClick;

				context.progressBar = view.FindViewById<ProgressBar> (Resource.Id.progressBar);

				// Showing loading progressBar.
				context.progressBar.Visibility = ViewStates.Visible;

				// Download and display data in url.
				context.downloadJsonFeedAsync (context.url);

				return view;
			}
		}

		public async void downloadJsonFeedAsync(String url)
		{
			var httpClient = new HttpClient();
			Task<string> contentsTask = httpClient.GetStringAsync(url);

			// Await! control returns to the caller and the task continues to run on another thread.
			string content = await contentsTask;
			Console.Out.WriteLine("Response Body: \r\n {0}", content);

			// Convert string to JSON object.
			result = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject> (content);

			// Update listview.
			RunOnUiThread (() => {
				listView.Adapter = new CustomListAdapter(this, result.posts);
				progressBar.Visibility = ViewStates.Gone;
			});
		}

		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			Post item = result.posts.ElementAt (e.Position);

			// Passing object form one activity to other.
			Intent i = new Intent(Application.Context, typeof(FeedDetailsActivity));
			i.PutExtra("item", JsonConvert.SerializeObject(item));
			StartActivity(i);
		}
	}
}
