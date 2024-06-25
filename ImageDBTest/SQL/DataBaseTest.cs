using ImageDB.Table;
using ImageDB.SQL;
using Moq;
using System.Data;

namespace ImageDBTest.SQL
{
	public class ImageDataBaseTest : IDisposable
	{
		private Mock<ImageDataBase> Mock { get; set; }

		private readonly string[] join = ["Tag", "Character", "Author"];
		private readonly string dbName = "ImageLibrary",
								tableName = "img";

		public ImageDataBaseTest()
		{
			Mock = new(dbName, tableName, join);
			Mock.Setup(d => d.Get<Image>()).Returns(GetData);
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

		[Fact]
		public void GetImageTest()
		{
			var image = Mock.Object.Image;

			Assert.Equal(GetData(), image);
		}
		[Fact]
		public void AddImageTest()
		{
			var img = new Image(4, "image_4.jpg", 0);
			Mock.Object.Add(img);

			var newData = GetData();
			newData = newData.Append(img);

			var image = Mock.Object.Image;

			Assert.Equal(newData, image);
		}
		[Fact]
		public void GetByIdImageTest()
		{
			var first = GetData().First();

			Mock.Setup(d => d.Get<Image>(1)).Returns(first.Parameter);

			var image = Mock.Object.Image;

			Assert.Contains(first, image);
		}

		[Fact]
		public void ClearTest()
		{
			Mock.SetupAdd(d => d.Clear());

			Assert.Empty(Mock.Object.Image);

			Mock.SetupRemove(d => d.Clear());
		}

		void IDisposable.Dispose() 
		{ 
			GC.SuppressFinalize(this);
		}
	}
}
