using System;

namespace GridCode.Entities.Model.Components
{
	public class Name : Component
	{
		private string nameString;

		public string NameString
		{
			get { return nameString; }
		}

		public Name(string name)
		{
			nameString = name;
		}
	}
}
