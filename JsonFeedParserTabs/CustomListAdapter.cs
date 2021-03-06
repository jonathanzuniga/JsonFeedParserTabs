﻿using System;
using Android.Widget;
using Android.App;
using System.Collections.Generic;
using Android.Views;

namespace JsonFeedParserTabs
{
	public class CustomListAdapter : BaseAdapter<Post>
	{
		Activity context;
		List<Post> list;

		public CustomListAdapter (Activity _context, List<Post> _list) : base()
		{
			this.context = _context;
			this.list = _list;
		}

		public override int Count
		{
			get { return list.Count; }
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override Post this[int index]
		{
			get { return list [index]; }
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView;

			// Re-use an existing view, if one is available,
			// otherwise create a new one.
			if (view == null)
				view = context.LayoutInflater.Inflate (Resource.Layout.ListRowLayout, parent, false);

			Post item = this [position];

			view.FindViewById<TextView>(Resource.Id.title).Text = Android.Text.Html.FromHtml(item.title).ToString();
			view.FindViewById<TextView>(Resource.Id.description).Text = item.date;

			using (var imageView = view.FindViewById<ImageView> (Resource.Id.thumbnail)) {
				string url = Android.Text.Html.FromHtml (item.thumbnail).ToString ();

				// Download and display image.
				Koush.UrlImageViewHelper.SetUrlDrawable (imageView, url, Resource.Drawable.Placeholder);
			}

			return view;
		}
	}
}
