﻿using ImageDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows;

namespace ImageDBTest
{
	public class DBTest : IDisposable
	{
		private DB db { get; set; }

		public DBTest()
		{
			db = new();
		}

		[StaFact]
		public void FillTagButtonTest()
		{
			db = new();
			var type = typeof(DB);
			var flag = BindingFlags.Instance | BindingFlags.NonPublic;
			var count = db.TagsGroup.Children.Count;
			var fill = type.GetMethod("FillMarkerButton", flag);

			Assert.NotEmpty(db.TagsGroup.Children);

			db.TagsGroup.Children.Clear();

			Assert.Empty(db.TagsGroup.Children);

			fill?.Invoke(db, parameters: [db.TagsGroup, "Tag"]);

			Assert.Equal(expected: count, db.TagsGroup.Children.Count);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
