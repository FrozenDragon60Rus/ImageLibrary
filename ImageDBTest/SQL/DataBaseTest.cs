using ImageDB.Table;
using ImageDB.SQL;
using Moq;
using System.Data;

namespace ImageDBTest.SQL
{
	public class ImageDataBaseTest : IDisposable
	{
		private Mock<ImageDataBase> MockTest { get; set; }

		private readonly string[] join = ["Tag", "Character", "Author"];
		private readonly string dbName = "ImageLibrary",
								tableName = "img";

		public ImageDataBaseTest()
		{
			MockTest = new(dbName, tableName, join);
			MockTest.Setup(d => d.Get<Image>()).Returns(GetData);
		}

		private IEnumerable<Image> GetData()
		{
			return
			[
				new Image
				(
					new Dictionary<string, object>()
					{
						{"Id", 1},
						{"Address", @"image_1.jpg"},
						{"Rating", 0},
						{"Tag", "android,angel,animal_ear"},
						{"Character", string.Empty},
						{"Author", string.Empty}
					}
				),
				new Image
				(
					new Dictionary<string, object>()
					{
						{"Id", 2},
						{"Address", @"image_2.jpg"},
						{"Rating", 0},
						{"Tag", "android"},
						{"Character", string.Empty},
						{"Author", string.Empty}
					}
				),
				new Image
				(
					new Dictionary<string, object>()
					{
						{"Id", 3},
						{"Address", @"image_3.jpg"},
						{"Rating", 0},
						{"Tag", "angel"},
						{"Character", string.Empty},
						{"Author", string.Empty}
					}
				)
			];
		}
		private void Reset()
		{
			MockTest.Reset();
			MockTest.Setup(d => d.Get<Image>()).Returns(GetData);
		}

		[Fact]
		public void AddTest()
		{
			var img = new Image(4, "image_4.jpg", 0);
			MockTest.Object.Add(img);

			var newData = GetData();
			newData = newData.Append(img);

			var image = MockTest.Object.Image;

			Assert.Equal(newData, image);

			Reset();
		}
		[Fact]
		public void AddListTest()
		{
			var newImage = GetData();
			MockTest.Object.Add(newImage);

			var newData = GetData();
			foreach(var img in newImage)
				newData = newData.Append(img);

			var image = MockTest.Object.Image;

			Assert.Equal(newData, image);

			Reset();
		}

		[Fact]
		public void GetImageTest()
		{
			var image = MockTest.Object.Image;

			Assert.Equal(GetData(), image);
		}
		[Fact]
		public void GetByIdImageTest()
		{
			var first = GetData().First();

			MockTest.Setup(d => d.Get<Image>(1)).Returns(first.Parameter);

			var image = MockTest.Object.Image;

			Assert.Contains(first, image);
		}

		[Fact]
		public void ClearTest()
		{
			MockTest.Object.Clear();

			MockTest.Verify(d => d.Clear(), Times.Exactly(1));

			Assert.Empty(MockTest.Object.Image);
		}

		[Fact]
		public void DeleteTest()
		{
			var first = GetData().First();

			var data = GetData().Where(d => d.Id != first.Id);
			MockTest.Object.Delete(first);

			var image = MockTest.Object.Image;

			Assert.Equal(data, image);

			Reset();
		}
		[Fact]
		public void DeleteListTest()
		{
			var first = GetData().First();

			var removeData = GetData().Where(d => d.Id != first.Id);
			var data = GetData().Where(d => d.Id == first.Id);
			MockTest.Object.Delete(removeData);

			var image = MockTest.Object.Image;

			Assert.Equal(data, image);

			Reset();
		}
		[Fact]
		public void DeleteByIdTest()
		{
			MockTest.Object.Delete(imageId: 1);

			var data = GetData().Where(d => d.Id != 1);

			var image = MockTest.Object.Image;

			Assert.Equal(data, image);

			Reset();
		}

		void IDisposable.Dispose() 
		{ 
			GC.SuppressFinalize(this);
		}
	}
}
