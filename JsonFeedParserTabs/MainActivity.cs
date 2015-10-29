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
			SetContentView (Resource.Layout.FeedsList);

			// Initializing listView.
			listView = FindViewById<ListView> (Resource.Id.feedList);
			listView.ItemClick += OnListItemClick;

			progressBar = FindViewById<ProgressBar> (Resource.Id.progressBar);

			// Showing loading progressBar.
			progressBar.Visibility = ViewStates.Visible;

			// Download and display data in url.
			downloadJsonFeedAsync (url);
		}

		public async void downloadJsonFeedAsync(String url) {
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

		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e) {
			Post item = result.posts.ElementAt (e.Position);

			// Passing object form one activity to other.
			Intent i = new Intent(Application.Context, typeof(FeedDetailsActivity));
			i.PutExtra("item", JsonConvert.SerializeObject(item));
			StartActivity(i);
		}
	}
}
