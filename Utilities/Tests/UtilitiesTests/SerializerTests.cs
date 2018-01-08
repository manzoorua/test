using System;
using System.Drawing;
using Cravens.Utilities.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace UtilitiesTests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class SerializerTests
	{
		[Serializable]
		public class Person
		{
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public DateTime Birthdate { get; set;}
			public Image Image { get; set; }
			public Rectangle Bounds { get; set; }
			public int Position1 { get; set; }
			public double Position2 { get; set; }
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

        [TestMethod]
		public void ConvertToByteArray_Is_Reversible_ConvertToObject()
		{
			Person before = TestPerson();

			byte[] bytes = Serializer.ConvertToByteArray(before);

			Person after = Serializer.ConvertToObject<Person>(bytes);

			Assert.IsTrue(after.FirstName == before.FirstName);
			Assert.IsTrue(after.LastName == before.LastName);
			Assert.IsTrue(after.Birthdate == before.Birthdate);
        	Assert.IsTrue(after.Bounds == before.Bounds);
        	Assert.IsTrue(after.Position1 == before.Position1);
        	Assert.IsTrue(after.Position2 == before.Position2);
        	Assert.IsTrue(after.Image.Width == before.Image.Width);
			Assert.IsTrue(after.Image.Height == before.Image.Height);
        	Bitmap bmp1 = (Bitmap)before.Image;
        	Bitmap bmp2 = (Bitmap) after.Image;

			for(int x=0;x<bmp1.Width;x++)
			{
				for(int y=0;y<bmp1.Height;y++)
				{
					Assert.IsTrue(bmp1.GetPixel(x, y) == bmp2.GetPixel(x, y));	
				}
			}
		}

		private static Person TestPerson()
		{
			Person person = new Person
			                	{
			                		FirstName = "Bob",
			                		LastName = "Cravens",
			                		Birthdate = DateTime.Now,
			                		Bounds = new Rectangle(10, 40, 100, 202),
			                		Position1 = 25,
			                		Position2 = 3.45672345
			                	};
			Rectangle screenSize = Screen.PrimaryScreen.Bounds;
			person.Image = new Bitmap(screenSize.Width, screenSize.Height);
			using(Graphics g=Graphics.FromImage(person.Image))
			{
				g.CopyFromScreen(0, 0, 0, 0, new Size(screenSize.Width, screenSize.Height));
			}

			return person;
		}
	}
}
